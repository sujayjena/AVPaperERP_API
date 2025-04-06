using AVPaperERP.Application.Enums;
using AVPaperERP.Application.Helpers;
using AVPaperERP.Application.Interfaces;
using AVPaperERP.Application.Models;
using AVPaperERP.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Globalization;
using System;

namespace AVPaperERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : CustomBaseController
    {
        private ResponseModel _response;
        private readonly INotificationRepository _notificationRepository;
        private IFileManager _fileManager;

        public NotificationController(INotificationRepository NotificationRepository, IFileManager fileManager)
        {
            _notificationRepository = NotificationRepository;
            _fileManager = fileManager;

            _response = new ResponseModel();
            _response.IsSuccess = true;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> SaveNotification(Notification_Request parameters)
        {
            int result = await _notificationRepository.SaveNotification(parameters);

            _response.Id = result;
            return _response;

        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> GetNotificationList(Notification_Search parameters)
        {
            var objList = await _notificationRepository.GetNotificationList(parameters);
            _response.Data = objList.ToList();
            _response.Total = parameters.Total;
            return _response;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> GetNotificationPopupList(Notification_Search parameters)
        {
            var vNotificationPopup_ResponseObj = new NotificationPopup_Response();

            _response.Data = vNotificationPopup_ResponseObj;
            _response.Total = parameters.Total;
            return _response;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> GetNotificationById(int Id)
        {
            if (Id <= 0)
            {
                _response.Message = "Id is required";
            }
            else
            {
                var vResultObj = await _notificationRepository.GetNotificationById(Id);

                _response.Data = vResultObj;
            }
            return _response;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> ExportNotificationData()
        {
            _response.IsSuccess = false;
            byte[] result;
            int recordIndex;
            ExcelWorksheet WorkSheet1;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var searchRequest = new Notification_Search();

            var lstObj = await _notificationRepository.GetNotificationList(searchRequest);

            using (MemoryStream msExportDataFile = new MemoryStream())
            {
                using (ExcelPackage excelExportData = new ExcelPackage())
                {
                    WorkSheet1 = excelExportData.Workbook.Worksheets.Add("Notification");
                    WorkSheet1.TabColor = System.Drawing.Color.Black;
                    WorkSheet1.DefaultRowHeight = 12;

                    //Header of table
                    WorkSheet1.Row(1).Height = 20;
                    WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    WorkSheet1.Row(1).Style.Font.Bold = true;

                    WorkSheet1.Cells[1, 1].Value = "No";
                    WorkSheet1.Cells[1, 2].Value = "Time Ago";
                    W
                                else
                                {
                                    vTimeAgo = vdiff.Days + " Days and " + vdiff.Hours + " Hr Ago";
                                }
                            }
                        }
                        else
                        {
                            if (vdiff.Hours == 0)
                            {
                                vTimeAgo = "Just Now";
                            }
                            else
                            {
                                vTimeAgo = vdiff.Hours + " Hr Ago";
                            }
                        }

                        WorkSheet1.Cells[recordIndex, 2].Value = vTimeAgo;
                        WorkSheet1.Cells[recordIndex, 3].Value = items.Message;

                    result = msExportDataFile.ToArray();
                }
            }

            if (result != null)
            {
                _response.Data = result;
                _response.IsSuccess = true;
                _response.Message = "Exported successfully";
            }

            return _response;
        }
    }
}
