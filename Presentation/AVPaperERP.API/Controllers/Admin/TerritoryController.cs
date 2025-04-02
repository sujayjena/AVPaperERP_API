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

namespace AVPaperERP.API.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class TerritoryController : CustomBaseController
    {
        private ResponseModel _response;
        private readonly ITerritoryRepository _territoryRepository;
        private IFileManager _fileManager;

        public TerritoryController(ITerritoryRepository territoryRepository, IFileManager fileManager)
        {
            _territoryRepository = territoryRepository;
            _response = new ResponseModel();
            _response.IsSuccess = true;
            _fileManager = fileManager;
        }

        #region State 

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> SaveState(State_Request parameters)
        {
            int result = await _territoryRepository.SaveState(parameters);

            if (result == (int)SaveOperationEnums.NoRecordExists)
            {
                _response.Message = "No record exists";
            }
            else if (result == (int)SaveOperationEnums.ReocrdExists)
            {
                _response.Message = "Record already exists";
            }
            else if (result == (int)SaveOperationEnums.NoResult)
            {
                _response.Message = "Something went wrong, please try again";
            }
            else
            {
                _response.Message = "Record details saved sucessfully";
            }

            _response.Id = result;
            return _response;
        }


        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> GetStateList(BaseSearchEntity parameters)
        {
            IEnumerable<State_Response> lstStates = await _territoryRepository.GetStateList(parameters);
            _response.Data = lstStates.ToList();
            _response.Total = parameters.Total;
            return _response;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> GetStateById(long Id)
        {
            if (Id <= 0)
            {
                _response.Message = "Id is required";
            }
            else
            {
                var vResultObj = await _territoryRepository.GetStateById(Id);
                _response.Data = vResultObj;
            }
            return _response;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> DownloadStateTemplate()
        {
            byte[]? formatFile = await Task.Run(() => _fileManager.GetFormatFileFromPath("Template_State.xlsx"));

            if (formatFile != null)
            {
                _response.Data = formatFile;
            }

            return _response;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> ImportState([FromQuery] ImportRequest request)
        {
            _response.IsSuccess = false;
            List<string[]> data = new List<string[]>();
            ExcelWorksheets currentSheet;
            ExcelWorksheet workSheet;
            List<ImportedState> lstImportedState = new List<ImportedState>();
            IEnumerable<StateDataValidationErrors> lstStateFailedToImport;
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            int noOfCol, noOfRow;

            if (request.FileUpload == null || request.FileUpload.Length == 0)
            {
                _response.Message = "Please upload an excel file to import State data";
                return _response;
            }

            using (MemoryStream stream = new MemoryStream())
            {
                request.FileUpload.CopyTo(stream);
                using ExcelPackage package = new ExcelPackage(stream);
                currentSheet = package.Workbook.Worksheets;
                workSheet = currentSheet.First();
                noOfCol = workSheet.Dimension.End.Column;
                noOfRow = workSheet.Dimension.End.Row;

                if (!string.Equals(workSheet.Cells[1, 1].Value.ToString(), "StateName", StringComparison.OrdinalIgnoreCase) ||
                   !string.Equals(workSheet.Cells[1, 2].Value.ToString(), "IsActive", StringComparison.OrdinalIgnoreCase))
                {
                    _response.IsSuccess = false;
                    _response.Message = "Please upload a valid excel file. Please Download Format file for reference";
                    return _response;
                }

                for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                {
                    lstImportedState.Add(new ImportedState()
                    {
                        StateName = workSheet.Cells[rowIterator, 1].Value?.ToString(),
                        IsActive = workSheet.Cells[rowIterator, 2].Value?.ToString()
                    });
                }
            }

            if (lstImportedState.Count == 0)
            {
                _response.Message = "File does not contains any record(s)";
                return _response;
            }

            lstStateFailedToImport = await _territoryRepository.ImportState(lstImportedState);

            _response.IsSuccess = true;
            _response.Message = "State list imported successfully";

            #region Generate Excel file for Invalid Data
            if (lstStateFailedToImport.ToList().Count > 0)
            {
                _response.Message = "Uploaded file contains invalid records, please check downloaded file for more details";
                _response.Data = GenerateInvalidStateDataFile(lstStateFailedToImport);

            }
            #endregion

            return _response;
        }

        private byte[] GenerateInvalidStateDataFile(IEnumerable<StateDataValidationErrors> lstStateFailedToImport)
        {
            byte[] result;
            int recordIndex;
            ExcelWorksheet WorkSheet1;

            using (MemoryStream msInvalidDataFile = new MemoryStream())
            {
                using (ExcelPackage excelInvalidData = new ExcelPackage())
                {
                    WorkSheet1 = excelInvalidData.Workbook.Worksheets.Add("Invalid_State_Records");
                    WorkSheet1.TabColor = System.Drawing.Color.Black;
                    WorkSheet1.DefaultRowHeight = 12;

                    //Header of table
                    WorkSheet1.Row(1).Height = 20;
                    WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    WorkSheet1.Row(1).Style.Font.Bold = true;

                    WorkSheet1.Cells[1, 1].Value = "StateName";
                    WorkSheet1.Cells[1, 2].Value = "IsActive";
                    WorkSheet1.Cells[1, 3].Value = "ValidationMessage";

                    recordIndex = 2;

                    foreach (StateDataValidationErrors record in lstStateFailedToImport)
                    {
                        WorkSheet1.Cells[recordIndex, 1].Value = record.StateName;
                        WorkSheet1.Cells[recordIndex, 2].Value = record.IsActive;
                        WorkSheet1.Cells[recordIndex, 3].Value = record.ValidationMessage;

                        recordIndex += 1;
                    }

                    WorkSheet1.Column(1).AutoFit();
                    WorkSheet1.Column(2).AutoFit();
                    WorkSheet1.Column(3).AutoFit();

                    excelInvalidData.SaveAs(msInvalidDataFile);
                    msInvalidDataFile.Position = 0;
                    result = msInvalidDataFile.ToArray();
                }
            }

            return result;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> ExportStateData()
        {
            _response.IsSuccess = false;
            byte[] result;
            int recordIndex;
            ExcelWorksheet WorkSheet1;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var request = new BaseSearchEntity();

            IEnumerable<State_Response> lstSizeObj = await _territoryRepository.GetStateList(request);

            using (MemoryStream msExportDataFile = new MemoryStream())
            {
                using (ExcelPackage excelExportData = new ExcelPackage())
                {
                    WorkSheet1 = excelExportData.Workbook.Worksheets.Add("State");
                    WorkSheet1.TabColor = System.Drawing.Color.Black;
                    WorkSheet1.DefaultRowHeight = 12;

                    //Header of table
                    WorkSheet1.Row(1).Height = 20;
                    WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    WorkSheet1.Row(1).Style.Font.Bold = true;

                    WorkSheet1.Cells[1, 1].Value = "State";
                    WorkSheet1.Cells[1, 2].Value = "Status";

                    WorkSheet1.Cells[1, 3].Value = "CreatedDate";
                    WorkSheet1.Cells[1, 4].Value = "CreatedBy";
                   

                    recordIndex = 2;

                    foreach (var items in lstSizeObj)
                    {
                        WorkSheet1.Cells[recordIndex, 1].Value = items.StateName;
                        WorkSheet1.Cells[recordIndex, 2].Value = items.IsActive == true ? "Active" : "Inactive";

                        WorkSheet1.Cells[recordIndex, 3].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                        WorkSheet1.Cells[recordIndex, 3].Value = items.CreatedDate;
                        WorkSheet1.Cells[recordIndex, 4].Value = items.CreatorName;

                        recordIndex += 1;
                    }

                    WorkSheet1.Column(1).AutoFit();
                    WorkSheet1.Column(2).AutoFit();
                    WorkSheet1.Column(3).AutoFit();
                    WorkSheet1.Column(4).AutoFit();

                    excelExportData.SaveAs(msExportDataFile);
                    msExportDataFile.Position = 0;
                    result = msExportDataFile.ToArray();
                }
            }

            if (result != null)
            {
                _response.Data = result;
                _response.IsSuccess = true;
                _response.Message = "State list Exported successfully";
            }

            return _response;
        }

        #endregion

        #region District 

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> SaveDistrict(District_Request parameters)
        {
            int result = await _territoryRepository.SaveDistrict(parameters);

            if (result == (int)SaveOperationEnums.NoRecordExists)
            {
                _response.Message = "No record exists";
            }
            else if (result == (int)SaveOperationEnums.ReocrdExists)
            {
                _response.Message = "Record already exists";
            }
            else if (result == (int)SaveOperationEnums.NoResult)
            {
                _response.Message = "Something went wrong, please try again";
            }
            else
            {
                _response.Message = "Record details saved sucessfully";
            }

            _response.Id = result;
            return _response;
        }


        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> GetDistrictList(BaseSearchEntity parameters)
        {
            IEnumerable<District_Response> lstDistricts = await _territoryRepository.GetDistrictList(parameters);
            _response.Data = lstDistricts.ToList();
            _response.Total = parameters.Total;
            return _response;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> GetDistrictById(long Id)
        {
            if (Id <= 0)
            {
                _response.Message = "Id is required";
            }
            else
            {
                var vResultObj = await _territoryRepository.GetDistrictById(Id);
                _response.Data = vResultObj;
            }
            return _response;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> DownloadDistrictTemplate()
        {
            byte[]? formatFile = await Task.Run(() => _fileManager.GetFormatFileFromPath("Template_District.xlsx"));

            if (formatFile != null)
            {
                _response.Data = formatFile;
            }

            return _response;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> ImportDistrict([FromQuery] ImportRequest request)
        {
            _response.IsSuccess = false;
            List<string[]> data = new List<string[]>();
            ExcelWorksheets currentSheet;
            ExcelWorksheet workSheet;
            List<ImportedDistrict> lstImportedDistrict = new List<ImportedDistrict>();
            IEnumerable<DistrictDataValidationErrors> lstDistrictFailedToImport;
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            int noOfCol, noOfRow;

            if (request.FileUpload == null || request.FileUpload.Length == 0)
            {
                _response.Message = "Please upload an excel file to import District data";
                return _response;
            }

            using (MemoryStream stream = new MemoryStream())
            {
                request.FileUpload.CopyTo(stream);
                using ExcelPackage package = new ExcelPackage(stream);
                currentSheet = package.Workbook.Worksheets;
                workSheet = currentSheet.First();
                noOfCol = workSheet.Dimension.End.Column;
                noOfRow = workSheet.Dimension.End.Row;

                if (!string.Equals(workSheet.Cells[1, 1].Value.ToString(), "DistrictName", StringComparison.OrdinalIgnoreCase) ||
                   !string.Equals(workSheet.Cells[1, 2].Value.ToString(), "IsActive", StringComparison.OrdinalIgnoreCase))
                {
                    _response.IsSuccess = false;
                    _response.Message = "Please upload a valid excel file. Please Download Format file for reference";
                    return _response;
                }

                for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                {
                    lstImportedDistrict.Add(new ImportedDistrict()
                    {
                        DistrictName = workSheet.Cells[rowIterator, 1].Value?.ToString(),
                        IsActive = workSheet.Cells[rowIterator, 2].Value?.ToString()
                    });
                }
            }

            if (lstImportedDistrict.Count == 0)
            {
                _response.Message = "File does not contains any record(s)";
                return _response;
            }

            lstDistrictFailedToImport = await _territoryRepository.ImportDistrict(lstImportedDistrict);

            _response.IsSuccess = true;
            _response.Message = "District list imported successfully";

            #region Generate Excel file for Invalid Data
            if (lstDistrictFailedToImport.ToList().Count > 0)
            {
                _response.Message = "Uploaded file contains invalid records, please check downloaded file for more details";
                _response.Data = GenerateInvalidDistrictDataFile(lstDistrictFailedToImport);

            }
            #endregion

            return _response;
        }

        private byte[] GenerateInvalidDistrictDataFile(IEnumerable<DistrictDataValidationErrors> lstDistrictFailedToImport)
        {
            byte[] result;
            int recordIndex;
            ExcelWorksheet WorkSheet1;

            using (MemoryStream msInvalidDataFile = new MemoryStream())
            {
                using (ExcelPackage excelInvalidData = new ExcelPackage())
                {
                    WorkSheet1 = excelInvalidData.Workbook.Worksheets.Add("Invalid_District_Records");
                    WorkSheet1.TabColor = System.Drawing.Color.Black;
                    WorkSheet1.DefaultRowHeight = 12;

                    //Header of table
                    WorkSheet1.Row(1).Height = 20;
                    WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    WorkSheet1.Row(1).Style.Font.Bold = true;

                    WorkSheet1.Cells[1, 1].Value = "DistrictName";
                    WorkSheet1.Cells[1, 2].Value = "IsActive";
                    WorkSheet1.Cells[1, 3].Value = "ValidationMessage";

                    recordIndex = 2;

                    foreach (DistrictDataValidationErrors record in lstDistrictFailedToImport)
                    {
                        WorkSheet1.Cells[recordIndex, 1].Value = record.DistrictName;
                        WorkSheet1.Cells[recordIndex, 2].Value = record.IsActive;
                        WorkSheet1.Cells[recordIndex, 3].Value = record.ValidationMessage;

                        recordIndex += 1;
                    }

                    WorkSheet1.Column(1).AutoFit();
                    WorkSheet1.Column(2).AutoFit();
                    WorkSheet1.Column(3).AutoFit();

                    excelInvalidData.SaveAs(msInvalidDataFile);
                    msInvalidDataFile.Position = 0;
                    result = msInvalidDataFile.ToArray();
                }
            }

            return result;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> ExportDistrictData()
        {
            _response.IsSuccess = false;
            byte[] result;
            int recordIndex;
            ExcelWorksheet WorkSheet1;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var request = new BaseSearchEntity();

            IEnumerable<District_Response> lstObj = await _territoryRepository.GetDistrictList(request);

            using (MemoryStream msExportDataFile = new MemoryStream())
            {
                using (ExcelPackage excelExportData = new ExcelPackage())
                {
                    WorkSheet1 = excelExportData.Workbook.Worksheets.Add("District");
                    WorkSheet1.TabColor = System.Drawing.Color.Black;
                    WorkSheet1.DefaultRowHeight = 12;

                    //Header of table
                    WorkSheet1.Row(1).Height = 20;
                    WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    WorkSheet1.Row(1).Style.Font.Bold = true;

                    WorkSheet1.Cells[1, 1].Value = "District";
                    WorkSheet1.Cells[1, 2].Value = "Status";

                    WorkSheet1.Cells[1, 3].Value = "CreatedDate";
                    WorkSheet1.Cells[1, 4].Value = "CreatedBy";


                    recordIndex = 2;

                    foreach (var items in lstObj)
                    {
                        WorkSheet1.Cells[recordIndex, 1].Value = items.DistrictName;
                        WorkSheet1.Cells[recordIndex, 2].Value = items.IsActive == true ? "Active" : "Inactive";

                        WorkSheet1.Cells[recordIndex, 3].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                        WorkSheet1.Cells[recordIndex, 3].Value = items.CreatedDate;
                        WorkSheet1.Cells[recordIndex, 4].Value = items.CreatorName;

                        recordIndex += 1;
                    }

                    WorkSheet1.Column(1).AutoFit();
                    WorkSheet1.Column(2).AutoFit();
                    WorkSheet1.Column(3).AutoFit();
                    WorkSheet1.Column(4).AutoFit();

                    excelExportData.SaveAs(msExportDataFile);
                    msExportDataFile.Position = 0;
                    result = msExportDataFile.ToArray();
                }
            }

            if (result != null)
            {
                _response.Data = result;
                _response.IsSuccess = true;
                _response.Message = "District list Exported successfully";
            }

            return _response;
        }

        #endregion

        #region City 

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> SaveCity(City_Request parameters)
        {
            int result = await _territoryRepository.SaveCity(parameters);

            if (result == (int)SaveOperationEnums.NoRecordExists)
            {
                _response.Message = "No record exists";
            }
            else if (result == (int)SaveOperationEnums.ReocrdExists)
            {
                _response.Message = "Record already exists";
            }
            else if (result == (int)SaveOperationEnums.NoResult)
            {
                _response.Message = "Something went wrong, please try again";
            }
            else
            {
                _response.Message = "Record details saved sucessfully";
            }

            _response.Id = result;
            return _response;
        }


        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> GetCityList(BaseSearchEntity parameters)
        {
            IEnumerable<City_Response> lstCitys = await _territoryRepository.GetCityList(parameters);
            _response.Data = lstCitys.ToList();
            _response.Total = parameters.Total;
            return _response;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> GetCityById(long Id)
        {
            if (Id <= 0)
            {
                _response.Message = "Id is required";
            }
            else
            {
                var vResultObj = await _territoryRepository.GetCityById(Id);
                _response.Data = vResultObj;
            }
            return _response;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> DownloadCityTemplate()
        {
            byte[]? formatFile = await Task.Run(() => _fileManager.GetFormatFileFromPath("Template_City.xlsx"));

            if (formatFile != null)
            {
                _response.Data = formatFile;
            }

            return _response;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> ImportCity([FromQuery] ImportRequest request)
        {
            _response.IsSuccess = false;
            List<string[]> data = new List<string[]>();
            ExcelWorksheets currentSheet;
            ExcelWorksheet workSheet;
            List<ImportedCity> lstImportedCity = new List<ImportedCity>();
            IEnumerable<CityDataValidationErrors> lstCityFailedToImport;
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            int noOfCol, noOfRow;

            if (request.FileUpload == null || request.FileUpload.Length == 0)
            {
                _response.Message = "Please upload an excel file to import City data";
                return _response;
            }

            using (MemoryStream stream = new MemoryStream())
            {
                request.FileUpload.CopyTo(stream);
                using ExcelPackage package = new ExcelPackage(stream);
                currentSheet = package.Workbook.Worksheets;
                workSheet = currentSheet.First();
                noOfCol = workSheet.Dimension.End.Column;
                noOfRow = workSheet.Dimension.End.Row;

                if (!string.Equals(workSheet.Cells[1, 1].Value.ToString(), "CityName", StringComparison.OrdinalIgnoreCase) ||
                   !string.Equals(workSheet.Cells[1, 2].Value.ToString(), "IsActive", StringComparison.OrdinalIgnoreCase))
                {
                    _response.IsSuccess = false;
                    _response.Message = "Please upload a valid excel file. Please Download Format file for reference";
                    return _response;
                }

                for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                {
                    lstImportedCity.Add(new ImportedCity()
                    {
                        CityName = workSheet.Cells[rowIterator, 1].Value?.ToString(),
                        IsActive = workSheet.Cells[rowIterator, 2].Value?.ToString()
                    });
                }
            }

            if (lstImportedCity.Count == 0)
            {
                _response.Message = "File does not contains any record(s)";
                return _response;
            }

            lstCityFailedToImport = await _territoryRepository.ImportCity(lstImportedCity);

            _response.IsSuccess = true;
            _response.Message = "City list imported successfully";

            #region Generate Excel file for Invalid Data
            if (lstCityFailedToImport.ToList().Count > 0)
            {
                _response.Message = "Uploaded file contains invalid records, please check downloaded file for more details";
                _response.Data = GenerateInvalidCityDataFile(lstCityFailedToImport);

            }
            #endregion

            return _response;
        }

        private byte[] GenerateInvalidCityDataFile(IEnumerable<CityDataValidationErrors> lstCityFailedToImport)
        {
            byte[] result;
            int recordIndex;
            ExcelWorksheet WorkSheet1;

            using (MemoryStream msInvalidDataFile = new MemoryStream())
            {
                using (ExcelPackage excelInvalidData = new ExcelPackage())
                {
                    WorkSheet1 = excelInvalidData.Workbook.Worksheets.Add("Invalid_City_Records");
                    WorkSheet1.TabColor = System.Drawing.Color.Black;
                    WorkSheet1.DefaultRowHeight = 12;

                    //Header of table
                    WorkSheet1.Row(1).Height = 20;
                    WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    WorkSheet1.Row(1).Style.Font.Bold = true;

                    WorkSheet1.Cells[1, 1].Value = "CityName";
                    WorkSheet1.Cells[1, 2].Value = "IsActive";
                    WorkSheet1.Cells[1, 3].Value = "ValidationMessage";

                    recordIndex = 2;

                    foreach (CityDataValidationErrors record in lstCityFailedToImport)
                    {
                        WorkSheet1.Cells[recordIndex, 1].Value = record.CityName;
                        WorkSheet1.Cells[recordIndex, 2].Value = record.IsActive;
                        WorkSheet1.Cells[recordIndex, 3].Value = record.ValidationMessage;

                        recordIndex += 1;
                    }

                    WorkSheet1.Column(1).AutoFit();
                    WorkSheet1.Column(2).AutoFit();
                    WorkSheet1.Column(3).AutoFit();

                    excelInvalidData.SaveAs(msInvalidDataFile);
                    msInvalidDataFile.Position = 0;
                    result = msInvalidDataFile.ToArray();
                }
            }

            return result;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> ExportCityData()
        {
            _response.IsSuccess = false;
            byte[] result;
            int recordIndex;
            ExcelWorksheet WorkSheet1;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var request = new BaseSearchEntity();

            IEnumerable<City_Response> lstObj = await _territoryRepository.GetCityList(request);

            using (MemoryStream msExportDataFile = new MemoryStream())
            {
                using (ExcelPackage excelExportData = new ExcelPackage())
                {
                    WorkSheet1 = excelExportData.Workbook.Worksheets.Add("City");
                    WorkSheet1.TabColor = System.Drawing.Color.Black;
                    WorkSheet1.DefaultRowHeight = 12;

                    //Header of table
                    WorkSheet1.Row(1).Height = 20;
                    WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    WorkSheet1.Row(1).Style.Font.Bold = true;

                    WorkSheet1.Cells[1, 1].Value = "City";
                    WorkSheet1.Cells[1, 2].Value = "Status";

                    WorkSheet1.Cells[1, 3].Value = "CreatedDate";
                    WorkSheet1.Cells[1, 4].Value = "CreatedBy";


                    recordIndex = 2;

                    foreach (var items in lstObj)
                    {
                        WorkSheet1.Cells[recordIndex, 1].Value = items.CityName;
                        WorkSheet1.Cells[recordIndex, 2].Value = items.IsActive == true ? "Active" : "Inactive";

                        WorkSheet1.Cells[recordIndex, 3].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                        WorkSheet1.Cells[recordIndex, 3].Value = items.CreatedDate;
                        WorkSheet1.Cells[recordIndex, 4].Value = items.CreatorName;

                        recordIndex += 1;
                    }

                    WorkSheet1.Column(1).AutoFit();
                    WorkSheet1.Column(2).AutoFit();
                    WorkSheet1.Column(3).AutoFit();
                    WorkSheet1.Column(4).AutoFit();

                    excelExportData.SaveAs(msExportDataFile);
                    msExportDataFile.Position = 0;
                    result = msExportDataFile.ToArray();
                }
            }

            if (result != null)
            {
                _response.Data = result;
                _response.IsSuccess = true;
                _response.Message = "City list Exported successfully";
            }

            return _response;
        }

        #endregion

        #region Territories 

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> SaveTerritories(Territories_Request parameters)
        {
            int result = await _territoryRepository.SaveTerritories(parameters);

            if (result == (int)SaveOperationEnums.NoRecordExists)
            {
                _response.Message = "No record exists";
            }
            else if (result == (int)SaveOperationEnums.ReocrdExists)
            {
                _response.Message = "Record already exists";
            }
            else if (result == (int)SaveOperationEnums.NoResult)
            {
                _response.Message = "Something went wrong, please try again";
            }
            else
            {
                _response.Message = "Record details saved sucessfully";
            }

            _response.Id = result;
            return _response;
        }


        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> GetTerritoriesList(BaseSearchEntity parameters)
        {
            IEnumerable<Territories_Response> lstTerritoriess = await _territoryRepository.GetTerritoriesList(parameters);
            _response.Data = lstTerritoriess.ToList();
            _response.Total = parameters.Total;
            return _response;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> GetTerritoriesById(long Id)
        {
            if (Id <= 0)
            {
                _response.Message = "Id is required";
            }
            else
            {
                var vResultObj = await _territoryRepository.GetTerritoriesById(Id);
                _response.Data = vResultObj;
            }
            return _response;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> GetTerritories_State_Dist_City_List_ById(Territories_State_Dist_City_Search parameters)
        {
            var vResultObj = await _territoryRepository.GetTerritories_State_Dist_City_List_ById(parameters);
            _response.Data = vResultObj;

            return _response;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> DownloadTerritoriesTemplate()
        {
            byte[]? formatFile = await Task.Run(() => _fileManager.GetFormatFileFromPath("Template_Territories.xlsx"));

            if (formatFile != null)
            {
                _response.Data = formatFile;
            }

            return _response;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> ImportTerritories([FromQuery] ImportRequest request)
        {
            _response.IsSuccess = false;
            List<string[]> data = new List<string[]>();
            ExcelWorksheets currentSheet;
            ExcelWorksheet workSheet;
            List<ImportedTerritories> lstImportedTerritories = new List<ImportedTerritories>();
            IEnumerable<TerritoriesDataValidationErrors> lstTerritoriesFailedToImport;
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            int noOfCol, noOfRow;

            if (request.FileUpload == null || request.FileUpload.Length == 0)
            {
                _response.Message = "Please upload an excel file to import Territories data";
                return _response;
            }

            using (MemoryStream stream = new MemoryStream())
            {
                request.FileUpload.CopyTo(stream);
                using ExcelPackage package = new ExcelPackage(stream);
                currentSheet = package.Workbook.Worksheets;
                workSheet = currentSheet.First();
                noOfCol = workSheet.Dimension.End.Column;
                noOfRow = workSheet.Dimension.End.Row;

                if (!string.Equals(workSheet.Cells[1, 1].Value.ToString(), "StateName", StringComparison.OrdinalIgnoreCase) ||
                   !string.Equals(workSheet.Cells[1, 2].Value.ToString(), "DistrictName", StringComparison.OrdinalIgnoreCase) ||
                   !string.Equals(workSheet.Cells[1, 3].Value.ToString(), "CityName", StringComparison.OrdinalIgnoreCase) ||
                   !string.Equals(workSheet.Cells[1, 4].Value.ToString(), "IsActive", StringComparison.OrdinalIgnoreCase))
                {
                    _response.IsSuccess = false;
                    _response.Message = "Please upload a valid excel file. Please Download Format file for reference";
                    return _response;
                }

                for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                {
                    lstImportedTerritories.Add(new ImportedTerritories()
                    {
                        StateName = workSheet.Cells[rowIterator, 1].Value?.ToString(),
                        DistrictName = workSheet.Cells[rowIterator, 2].Value?.ToString(),
                        CityName = workSheet.Cells[rowIterator, 3].Value?.ToString(),
                        IsActive = workSheet.Cells[rowIterator, 4].Value?.ToString()
                    });
                }
            }

            if (lstImportedTerritories.Count == 0)
            {
                _response.Message = "File does not contains any record(s)";
                return _response;
            }

            lstTerritoriesFailedToImport = await _territoryRepository.ImportTerritories(lstImportedTerritories);

            _response.IsSuccess = true;
            _response.Message = "Territories list imported successfully";

            #region Generate Excel file for Invalid Data
            if (lstTerritoriesFailedToImport.ToList().Count > 0)
            {
                _response.Message = "Uploaded file contains invalid records, please check downloaded file for more details";
                _response.Data = GenerateInvalidTerritoriesDataFile(lstTerritoriesFailedToImport);

            }
            #endregion

            return _response;
        }

        private byte[] GenerateInvalidTerritoriesDataFile(IEnumerable<TerritoriesDataValidationErrors> lstTerritoriesFailedToImport)
        {
            byte[] result;
            int recordIndex;
            ExcelWorksheet WorkSheet1;

            using (MemoryStream msInvalidDataFile = new MemoryStream())
            {
                using (ExcelPackage excelInvalidData = new ExcelPackage())
                {
                    WorkSheet1 = excelInvalidData.Workbook.Worksheets.Add("Invalid_Territories_Records");
                    WorkSheet1.TabColor = System.Drawing.Color.Black;
                    WorkSheet1.DefaultRowHeight = 12;

                    //Header of table
                    WorkSheet1.Row(1).Height = 20;
                    WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    WorkSheet1.Row(1).Style.Font.Bold = true;

                    WorkSheet1.Cells[1, 1].Value = "StateName";
                    WorkSheet1.Cells[1, 2].Value = "DistrictName";
                    WorkSheet1.Cells[1, 3].Value = "CityName";;
                    WorkSheet1.Cells[1, 4].Value = "IsActive";
                    WorkSheet1.Cells[1, 5].Value = "ValidationMessage";

                    recordIndex = 2;

                    foreach (TerritoriesDataValidationErrors record in lstTerritoriesFailedToImport)
                    {
                        WorkSheet1.Cells[recordIndex, 1].Value = record.StateName;
                        WorkSheet1.Cells[recordIndex, 2].Value = record.DistrictName;
                        WorkSheet1.Cells[recordIndex, 3].Value = record.CityName;
                        WorkSheet1.Cells[recordIndex, 4].Value = record.IsActive;
                        WorkSheet1.Cells[recordIndex, 5].Value = record.ValidationMessage;

                        recordIndex += 1;
                    }

                    WorkSheet1.Column(1).AutoFit();
                    WorkSheet1.Column(2).AutoFit();
                    WorkSheet1.Column(3).AutoFit();
                    WorkSheet1.Column(4).AutoFit();
                    WorkSheet1.Column(5).AutoFit();

                    excelInvalidData.SaveAs(msInvalidDataFile);
                    msInvalidDataFile.Position = 0;
                    result = msInvalidDataFile.ToArray();
                }
            }

            return result;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> ExportTerritoriesData()
        {
            _response.IsSuccess = false;
            byte[] result;
            int recordIndex;
            ExcelWorksheet WorkSheet1;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var request = new BaseSearchEntity();

            IEnumerable<Territories_Response> lstSizeObj = await _territoryRepository.GetTerritoriesList(request);

            using (MemoryStream msExportDataFile = new MemoryStream())
            {
                using (ExcelPackage excelExportData = new ExcelPackage())
                {
                    WorkSheet1 = excelExportData.Workbook.Worksheets.Add("Territories");
                    WorkSheet1.TabColor = System.Drawing.Color.Black;
                    WorkSheet1.DefaultRowHeight = 12;

                    //Header of table
                    WorkSheet1.Row(1).Height = 20;
                    WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    WorkSheet1.Row(1).Style.Font.Bold = true;

                    WorkSheet1.Cells[1, 1].Value = "State";
                    WorkSheet1.Cells[1, 2].Value = "District";
                    WorkSheet1.Cells[1, 3].Value = "City";
                    WorkSheet1.Cells[1, 4].Value = "Status";

                    WorkSheet1.Cells[1, 5].Value = "CreatedDate";
                    WorkSheet1.Cells[1, 6].Value = "CreatedBy";


                    recordIndex = 2;

                    foreach (var items in lstSizeObj)
                    {
                        WorkSheet1.Cells[recordIndex, 1].Value = items.StateName;
                        WorkSheet1.Cells[recordIndex, 2].Value = items.DistrictName;
                        WorkSheet1.Cells[recordIndex, 3].Value = items.CityName;
                        WorkSheet1.Cells[recordIndex, 4].Value = items.IsActive == true ? "Active" : "Inactive";

                        WorkSheet1.Cells[recordIndex, 5].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                        WorkSheet1.Cells[recordIndex, 5].Value = items.CreatedDate;
                        WorkSheet1.Cells[recordIndex, 6].Value = items.CreatorName;

                        recordIndex += 1;
                    }

                    WorkSheet1.Column(1).AutoFit();
                    WorkSheet1.Column(2).AutoFit();
                    WorkSheet1.Column(3).AutoFit();
                    WorkSheet1.Column(4).AutoFit();
                    WorkSheet1.Column(5).AutoFit();
                    WorkSheet1.Column(6).AutoFit();

                    excelExportData.SaveAs(msExportDataFile);
                    msExportDataFile.Position = 0;
                    result = msExportDataFile.ToArray();
                }
            }

            if (result != null)
            {
                _response.Data = result;
                _response.IsSuccess = true;
                _response.Message = "Territories list Exported successfully";
            }

            return _response;
        }

        #endregion
    }
}