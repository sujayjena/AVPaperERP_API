using AVPaperERP.Application.Enums;
using AVPaperERP.Application.Helpers;
using AVPaperERP.Application.Interfaces;
using AVPaperERP.Application.Models;
using AVPaperERP.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Text;

namespace AVPaperERP.API.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : CustomBaseController
    {
        private ResponseModel _response;
        private readonly IUserRepository _userRepository;
        private readonly ICompanyRepository _companyRepository;
        private IFileManager _fileManager;

        public UserController(IUserRepository userRepository, ICompanyRepository companyRepository, IFileManager fileManager)
        {
            _userRepository = userRepository;
            _companyRepository = companyRepository;
            _fileManager = fileManager;

            _response = new ResponseModel();
            _response.IsSuccess = true;
        }

        #region User 

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> SaveUser(User_Request parameters)
        {
            // Aadhar Card Upload
            if (parameters! != null && !string.IsNullOrWhiteSpace(parameters.AadharImage_Base64))
            {
                var vUploadFile = _fileManager.UploadDocumentsBase64ToFile(parameters.AadharImage_Base64, "\\Uploads\\Employee\\", parameters.AadharOriginalFileName);

                if (!string.IsNullOrWhiteSpace(vUploadFile))
                {
                    parameters.AadharImage = vUploadFile;
                }
            }

            // Pan Card Upload
            if (parameters != null && !string.IsNullOrWhiteSpace(parameters.PanCardImage_Base64))
            {
                var vUploadFile = _fileManager.UploadDocumentsBase64ToFile(parameters.PanCardImage_Base64, "\\Uploads\\Employee\\", parameters.PanCardOriginalFileName);

                if (!string.IsNullOrWhiteSpace(vUploadFile))
                {
                    parameters.PanCardImage = vUploadFile;
                }
            }

            // Profile Upload
            if (parameters != null && !string.IsNullOrWhiteSpace(parameters.ProfileImage_Base64))
            {
                var vUploadFile = _fileManager.UploadDocumentsBase64ToFile(parameters.ProfileImage_Base64, "\\Uploads\\Employee\\", parameters.ProfileOriginalFileName);

                if (!string.IsNullOrWhiteSpace(vUploadFile))
                {
                    parameters.ProfileImage = vUploadFile;
                }
            }

            int result = await _userRepository.SaveUser(parameters);

            if (result == (int)SaveOperationEnums.NoRecordExists)
            {
                _response.Message = "No record exists";
            }
            else if (result == (int)SaveOperationEnums.ReocrdExists)
            {
                _response.Message = "Record already exists";
            }
            else if (result == -3)
            {
                _response.Message = "Email already exists";
            }
            else if (result == -4)
            {
                _response.Message = "Mobile already exists";
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
        public async Task<ResponseModel> GetUserList(BaseSearchEntity parameters)
        {
            IEnumerable<User_Response> lstUsers = await _userRepository.GetUserList(parameters);
            _response.Data = lstUsers.ToList();
            _response.Total = parameters.Total;
            return _response;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> GetUserById(long Id)
        {
            if (Id <= 0)
            {
                _response.Message = "Id is required";
            }
            else
            {
                var vResultObj = await _userRepository.GetUserById(Id);
                _response.Data = vResultObj;
            }
            return _response;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> GetUserLisByRoleIdOrRoleName(UserListByRole_Search parameters)
        {
            IEnumerable<UserListByRole_Response> lstUsers = await _userRepository.GetUserLisByRoleIdOrRoleName(parameters);
            _response.Data = lstUsers.ToList();
            return _response;
        }


        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> DownloadUserTemplate()
        {
            byte[]? formatFile = await Task.Run(() => _fileManager.GetFormatFileFromPath("Template_User.xlsx"));

            if (formatFile != null)
            {
                _response.Data = formatFile;
            }

            return _response;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> ImportUser([FromQuery] ImportRequest request)
        {
            _response.IsSuccess = false;

            ExcelWorksheets currentSheet;
            ExcelWorksheet workSheet;
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            int noOfCol, noOfRow;

            List<string[]> data = new List<string[]>();
            List<User_ImportData> lstUser_ImportData = new List<User_ImportData>();
            IEnumerable<User_ImportDataValidation> lstUser_ImportDataValidation;

            if (request.FileUpload == null || request.FileUpload.Length == 0)
            {
                _response.Message = "Please upload an excel file";
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

                if (!string.Equals(workSheet.Cells[1, 1].Value.ToString(), "UserCode", StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(workSheet.Cells[1, 2].Value.ToString(), "UserName", StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(workSheet.Cells[1, 3].Value.ToString(), "MobileNumber", StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(workSheet.Cells[1, 4].Value.ToString(), "EmailId", StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(workSheet.Cells[1, 5].Value.ToString(), "Password", StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(workSheet.Cells[1, 6].Value.ToString(), "Role", StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(workSheet.Cells[1, 7].Value.ToString(), "ReportingTo", StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(workSheet.Cells[1, 8].Value.ToString(), "Department", StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(workSheet.Cells[1, 9].Value.ToString(), "Company", StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(workSheet.Cells[1, 10].Value.ToString(), "Address", StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(workSheet.Cells[1, 11].Value.ToString(), "State", StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(workSheet.Cells[1, 12].Value.ToString(), "District", StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(workSheet.Cells[1, 13].Value.ToString(), "City", StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(workSheet.Cells[1, 14].Value.ToString(), "Pincode", StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(workSheet.Cells[1, 15].Value.ToString(), "DateOfBirth", StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(workSheet.Cells[1, 16].Value.ToString(), "DateOfJoining", StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(workSheet.Cells[1, 17].Value.ToString(), "EmergencyContactNumber", StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(workSheet.Cells[1, 18].Value.ToString(), "BloodGroup", StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(workSheet.Cells[1, 19].Value.ToString(), "AadharNumber", StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(workSheet.Cells[1, 20].Value.ToString(), "PanNumber", StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(workSheet.Cells[1, 21].Value.ToString(), "MobileUniqueId", StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(workSheet.Cells[1, 22].Value.ToString(), "IsMobileUser", StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(workSheet.Cells[1, 23].Value.ToString(), "IsWebUser", StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(workSheet.Cells[1, 24].Value.ToString(), "IsActive", StringComparison.OrdinalIgnoreCase))
                {
                    _response.IsSuccess = false;
                    _response.Message = "Please upload a valid excel file";
                    return _response;
                }

                for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                {
                    if (!string.IsNullOrWhiteSpace(workSheet.Cells[rowIterator, 2].Value?.ToString()) && !string.IsNullOrWhiteSpace(workSheet.Cells[rowIterator, 3].Value?.ToString()))
                    {
                        lstUser_ImportData.Add(new User_ImportData()
                        {
                            UserCode = workSheet.Cells[rowIterator, 1].Value?.ToString(),
                            UserName = workSheet.Cells[rowIterator, 2].Value?.ToString(),
                            MobileNumber = workSheet.Cells[rowIterator, 3].Value?.ToString(),
                            EmailId = workSheet.Cells[rowIterator, 4].Value?.ToString(),
                            Password = !string.IsNullOrWhiteSpace(workSheet.Cells[rowIterator, 5].Value?.ToString()) ? EncryptDecryptHelper.EncryptString(workSheet.Cells[rowIterator, 5].Value?.ToString()) : string.Empty,
                            Role = workSheet.Cells[rowIterator, 6].Value?.ToString(),
                            ReportingTo = workSheet.Cells[rowIterator, 7].Value?.ToString(),
                            Department = workSheet.Cells[rowIterator, 8].Value?.ToString(),
                            Company = workSheet.Cells[rowIterator, 9].Value?.ToString(),
                            Address = workSheet.Cells[rowIterator, 10].Value?.ToString(),
                            State = workSheet.Cells[rowIterator, 11].Value?.ToString(),
                            District = workSheet.Cells[rowIterator, 12].Value?.ToString(),
                            City = workSheet.Cells[rowIterator, 13].Value?.ToString(),
                            Pincode = workSheet.Cells[rowIterator, 14].Value?.ToString(),
                            DateOfBirth = !string.IsNullOrWhiteSpace(workSheet.Cells[rowIterator, 15].Value?.ToString()) ? DateTime.ParseExact(workSheet.Cells[rowIterator, 16].Value?.ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.CurrentUICulture.DateTimeFormat) : null,
                            DateOfJoining = !string.IsNullOrWhiteSpace(workSheet.Cells[rowIterator, 16].Value?.ToString()) ? DateTime.ParseExact(workSheet.Cells[rowIterator, 17].Value?.ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.CurrentUICulture.DateTimeFormat) : null,
                            EmergencyContactNumber = workSheet.Cells[rowIterator, 17].Value?.ToString(),
                            BloodGroup = workSheet.Cells[rowIterator, 18].Value?.ToString(),
                            AadharNumber = workSheet.Cells[rowIterator, 19].Value?.ToString(),
                            PanNumber = workSheet.Cells[rowIterator, 20].Value?.ToString(),
                            MobileUniqueId = workSheet.Cells[rowIterator, 21].Value?.ToString(),
                            IsMobileUser = workSheet.Cells[rowIterator, 22].Value?.ToString(),
                            IsWebUser = workSheet.Cells[rowIterator, 23].Value?.ToString(),
                            IsActive = workSheet.Cells[rowIterator, 24].Value?.ToString()
                        });
                    }
                }
            }

            if (lstUser_ImportData.Count == 0)
            {
                _response.Message = "File does not contains any record(s)";
                return _response;
            }

            lstUser_ImportDataValidation = await _userRepository.ImportUser(lstUser_ImportData);

            _response.IsSuccess = true;
            _response.Message = "Record imported successfully";

            #region Generate Excel file for Invalid Data

            if (lstUser_ImportDataValidation.ToList().Count > 0)
            {
                _response.Message = "Uploaded file contains invalid records, please check downloaded file for more details";
                _response.Data = GenerateInvalidImportDataFile(lstUser_ImportDataValidation);

            }

            #endregion

            return _response;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> ExportUserData(bool IsActive = true)
        {
            _response.IsSuccess = false;
            byte[] result;
            int recordIndex;
            ExcelWorksheet WorkSheet1;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var request = new BaseSearchEntity();
            request.IsActive = IsActive;

            IEnumerable<User_Response> lstSizeObj = await _userRepository.GetUserList(request);

            using (MemoryStream msExportDataFile = new MemoryStream())
            {
                using (ExcelPackage excelExportData = new ExcelPackage())
                {
                    WorkSheet1 = excelExportData.Workbook.Worksheets.Add("Employee");
                    WorkSheet1.TabColor = System.Drawing.Color.Black;
                    WorkSheet1.DefaultRowHeight = 12;

                    //Header of table
                    WorkSheet1.Row(1).Height = 20;
                    WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    WorkSheet1.Row(1).Style.Font.Bold = true;

                    WorkSheet1.Cells[1, 1].Value = "User Code";
                    WorkSheet1.Cells[1, 2].Value = "User Name";
                    WorkSheet1.Cells[1, 3].Value = "Mobile";
                    WorkSheet1.Cells[1, 4].Value = "EmailId";
                    WorkSheet1.Cells[1, 5].Value = "Role";
                    WorkSheet1.Cells[1, 6].Value = "ReportingTo";
                    //WorkSheet1.Cells[1, 7].Value = "Department";
                    WorkSheet1.Cells[1, 7].Value = "Company";
                    WorkSheet1.Cells[1, 8].Value = "Address";
                    WorkSheet1.Cells[1, 9].Value = "State";
                    WorkSheet1.Cells[1, 10].Value = "District";
                    WorkSheet1.Cells[1, 11].Value = "City";
                    WorkSheet1.Cells[1, 12].Value = "Pincode";
                    WorkSheet1.Cells[1, 13].Value = "DateOfBirth";
                    WorkSheet1.Cells[1, 14].Value = "Date Of Joining";
                    WorkSheet1.Cells[1, 15].Value = "Emergency Contact Number";
                    WorkSheet1.Cells[1, 16].Value = "Blood Group";
                    WorkSheet1.Cells[1, 17].Value = "Aadhar Number";
                    WorkSheet1.Cells[1, 18].Value = "Pan Number";
                    WorkSheet1.Cells[1, 19].Value = "Mobile UniqueId";
                    WorkSheet1.Cells[1, 20].Value = "IsMobileUser";
                    WorkSheet1.Cells[1, 21].Value = "IsWebUser";
                    WorkSheet1.Cells[1, 22].Value = "IsActive";


                    recordIndex = 2;

                    foreach (var items in lstSizeObj)
                    {
                        WorkSheet1.Cells[recordIndex, 1].Value = items.UserCode;
                        WorkSheet1.Cells[recordIndex, 2].Value = items.UserName;
                        WorkSheet1.Cells[recordIndex, 3].Value = items.MobileNumber;
                        WorkSheet1.Cells[recordIndex, 4].Value = items.EmailId;
                        WorkSheet1.Cells[recordIndex, 5].Value = items.RoleName;
                        WorkSheet1.Cells[recordIndex, 6].Value = items.ReportingToName;
                        //WorkSheet1.Cells[recordIndex, 7].Value = items.DepartmentName;
                        WorkSheet1.Cells[recordIndex, 7].Value = items.CompanyName;

                        WorkSheet1.Cells[recordIndex, 8].Value = items.AddressLine;
                        WorkSheet1.Cells[recordIndex, 9].Value = items.StateName;
                        WorkSheet1.Cells[recordIndex, 10].Value = items.DistrictName;
                        WorkSheet1.Cells[recordIndex, 11].Value = items.CityName;
                        WorkSheet1.Cells[recordIndex, 12].Value = items.Pincode;
                        WorkSheet1.Cells[recordIndex, 13].Value = items.DateOfBirth.HasValue ? items.DateOfBirth.Value.ToString("dd/MM/yyyy") : string.Empty;
                        WorkSheet1.Cells[recordIndex, 14].Value = items.DateOfJoining.HasValue ? items.DateOfJoining.Value.ToString("dd/MM/yyyy") : string.Empty;
                        WorkSheet1.Cells[recordIndex, 15].Value = items.EmergencyContactNumber;
                        WorkSheet1.Cells[recordIndex, 16].Value = items.BloodGroupName;
                        WorkSheet1.Cells[recordIndex, 17].Value = items.AadharNumber;
                        WorkSheet1.Cells[recordIndex, 18].Value = items.PanNumber;
                        WorkSheet1.Cells[recordIndex, 19].Value = items.MobileUniqueId;
                        WorkSheet1.Cells[recordIndex, 20].Value = items.IsMobileUser;
                        WorkSheet1.Cells[recordIndex, 21].Value = items.IsWebUser;
                        WorkSheet1.Cells[recordIndex, 22].Value = items.IsActive == true ? "Active" : "Inactive";

                        recordIndex += 1;
                    }

                    WorkSheet1.Column(1).AutoFit();
                    WorkSheet1.Column(2).AutoFit();
                    WorkSheet1.Column(3).AutoFit();
                    WorkSheet1.Column(4).AutoFit();
                    WorkSheet1.Column(5).AutoFit();
                    WorkSheet1.Column(6).AutoFit();
                    WorkSheet1.Column(7).AutoFit();
                    WorkSheet1.Column(8).AutoFit();
                    WorkSheet1.Column(9).AutoFit();
                    WorkSheet1.Column(10).AutoFit();
                    WorkSheet1.Column(11).AutoFit();
                    WorkSheet1.Column(12).AutoFit();
                    WorkSheet1.Column(13).AutoFit();
                    WorkSheet1.Column(14).AutoFit();
                    WorkSheet1.Column(15).AutoFit();
                    WorkSheet1.Column(16).AutoFit();
                    WorkSheet1.Column(17).AutoFit();
                    WorkSheet1.Column(18).AutoFit();
                    WorkSheet1.Column(19).AutoFit();
                    WorkSheet1.Column(20).AutoFit();
                    WorkSheet1.Column(21).AutoFit();
                    WorkSheet1.Column(22).AutoFit();

                    excelExportData.SaveAs(msExportDataFile);
                    msExportDataFile.Position = 0;
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

        private byte[] GenerateInvalidImportDataFile(IEnumerable<User_ImportDataValidation> lstUser_ImportDataValidation)
        {
            byte[] result;
            int recordIndex;
            ExcelWorksheet WorkSheet1;

            using (MemoryStream msInvalidDataFile = new MemoryStream())
            {
                using (ExcelPackage excelInvalidData = new ExcelPackage())
                {
                    WorkSheet1 = excelInvalidData.Workbook.Worksheets.Add("Invalid_Records");
                    WorkSheet1.TabColor = System.Drawing.Color.Black;
                    WorkSheet1.DefaultRowHeight = 12;

                    //Header of table
                    WorkSheet1.Row(1).Height = 20;
                    WorkSheet1.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    WorkSheet1.Row(1).Style.Font.Bold = true;

                    WorkSheet1.Cells[1, 1].Value = "UserCode";
                    WorkSheet1.Cells[1, 2].Value = "UserName";
                    WorkSheet1.Cells[1, 3].Value = "MobileNumber";
                    WorkSheet1.Cells[1, 4].Value = "EmailId";
                    WorkSheet1.Cells[1, 5].Value = "Password";
                    WorkSheet1.Cells[1, 6].Value = "Role";
                    WorkSheet1.Cells[1, 7].Value = "ReportingTo";
                    WorkSheet1.Cells[1, 8].Value = "Department";
                    WorkSheet1.Cells[1, 9].Value = "Company";
                    WorkSheet1.Cells[1, 10].Value = "Address";
                    WorkSheet1.Cells[1, 11].Value = "State";
                    WorkSheet1.Cells[1, 12].Value = "District";
                    WorkSheet1.Cells[1, 13].Value = "City";
                    WorkSheet1.Cells[1, 14].Value = "Pincode";
                    WorkSheet1.Cells[1, 15].Value = "DateOfBirth";
                    WorkSheet1.Cells[1, 16].Value = "DateOfJoining";
                    WorkSheet1.Cells[1, 17].Value = "EmergencyContactNumber";
                    WorkSheet1.Cells[1, 18].Value = "BloodGroup";
                    WorkSheet1.Cells[1, 19].Value = "AadharNumber";
                    WorkSheet1.Cells[1, 20].Value = "PanNumber";
                    WorkSheet1.Cells[1, 21].Value = "MobileUniqueId";
                    WorkSheet1.Cells[1, 22].Value = "IsMobileUser";
                    WorkSheet1.Cells[1, 23].Value = "IsWebUser";
                    WorkSheet1.Cells[1, 24].Value = "ErrorMessage";

                    recordIndex = 2;

                    foreach (User_ImportDataValidation record in lstUser_ImportDataValidation)
                    {
                        WorkSheet1.Cells[recordIndex, 1].Value = record.UserCode;
                        WorkSheet1.Cells[recordIndex, 2].Value = record.UserName;
                        WorkSheet1.Cells[recordIndex, 3].Value = record.MobileNumber;
                        WorkSheet1.Cells[recordIndex, 4].Value = record.EmailId;
                        WorkSheet1.Cells[recordIndex, 5].Value = record.Password;
                        WorkSheet1.Cells[recordIndex, 6].Value = record.Role;
                        WorkSheet1.Cells[recordIndex, 7].Value = record.ReportingTo;
                        WorkSheet1.Cells[recordIndex, 8].Value = record.Department;
                        WorkSheet1.Cells[recordIndex, 9].Value = record.Company;
                        WorkSheet1.Cells[recordIndex, 10].Value = record.Address;
                        WorkSheet1.Cells[recordIndex, 11].Value = record.State;
                        WorkSheet1.Cells[recordIndex, 12].Value = record.District;
                        WorkSheet1.Cells[recordIndex, 13].Value = record.City;
                        WorkSheet1.Cells[recordIndex, 14].Value = record.Pincode;
                        WorkSheet1.Cells[recordIndex, 15].Value = record.DateOfBirth;
                        WorkSheet1.Cells[recordIndex, 16].Value = record.DateOfJoining;
                        WorkSheet1.Cells[recordIndex, 17].Value = record.EmergencyContactNumber;
                        WorkSheet1.Cells[recordIndex, 18].Value = record.BloodGroup;
                        WorkSheet1.Cells[recordIndex, 19].Value = record.AadharNumber;
                        WorkSheet1.Cells[recordIndex, 20].Value = record.PanNumber;
                        WorkSheet1.Cells[recordIndex, 21].Value = record.MobileUniqueId;
                        WorkSheet1.Cells[recordIndex, 22].Value = record.IsMobileUser;
                        WorkSheet1.Cells[recordIndex, 23].Value = record.IsWebUser;
                        WorkSheet1.Cells[recordIndex, 24].Value = record.ValidationMessage;

                        recordIndex += 1;
                    }

                    WorkSheet1.Columns.AutoFit();

                    excelInvalidData.SaveAs(msInvalidDataFile);
                    msInvalidDataFile.Position = 0;
                    result = msInvalidDataFile.ToArray();
                }
            }

            return result;
        }

        #endregion
    }
}
