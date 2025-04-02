using AVPaperERP.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVPaperERP.Application.Models
{
    public class UserModel
    {
    }

    public class User_Request : BaseEntity
    {
        [DefaultValue(1)]
        public int? CompanyId { get; set; }

        [DefaultValue(1)]
        public int? DepartmentId { get; set; }

        public string? UserCode { get; set; }

        public string? UserName { get; set; }

        public string? MobileNumber { get; set; }

        public string? EmailId { get; set; }

        public string? Password { get; set; }

        public string? UserType { get; set; }

        public int? RoleId { get; set; }

        public int? ReportingTo { get; set; }

        public string? AddressLine { get; set; }

        public int? StateId { get; set; }

        public int? DistrictId { get; set; }

        public int? CityId { get; set; }

        public int? Pincode { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime? DateOfJoining { get; set; }

        public string? EmergencyContactNumber { get; set; }

        public int? BloodGroupId { get; set; }

        public string? MobileUniqueId { get; set; }

        public string? AadharNumber { get; set; }

        public string? AadharImage { get; set; }

        public string? AadharImage_Base64 { get; set; }

        public string? AadharOriginalFileName { get; set; }

        public string? PanNumber { get; set; }

        public string? PanCardImage { get; set; }

        public string? PanCardImage_Base64 { get; set; }

        public string? PanCardOriginalFileName { get; set; }

        public string? ProfileImage { get; set; }

        public string? ProfileImage_Base64 { get; set; }

        public string? ProfileOriginalFileName { get; set; }

        public bool? IsMobileUser { get; set; }

        public bool? IsWebUser { get; set; }

        public bool? IsActive { get; set; }
    }

    public class User_Response : BaseResponseEntity
    {
        public string? UserCode { get; set; }

        public string? UserName { get; set; }

        public string? MobileNumber { get; set; }

        public string? EmailId { get; set; }

        public string? Password { get; set; }

        public string? UserType { get; set; }

        public int? RoleId { get; set; }

        public string? RoleName { get; set; }

        public int? EmployeeLevelId { get; set; }
        public string EmployeeLevel { get; set; }

        public int? ReportingTo { get; set; }

        public string? ReportingToName { get; set; }

        public string? ReportingToMobileNo { get; set; }

        [DefaultValue(1)]
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }

        [DefaultValue(1)]
        public int? CompanyId { get; set; }
        public string? CompanyName { get; set; }

        public string? AddressLine { get; set; }

        public int? StateId { get; set; }

        public string? StateName { get; set; }

        public int? DistrictId { get; set; }

        public string? DistrictName { get; set; }

        public int? CityId { get; set; }

        public string? CityName { get; set; }

        public int? Pincode { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime? DateOfJoining { get; set; }

        public string? EmergencyContactNumber { get; set; }

        public int? BloodGroupId { get; set; }
        public string? BloodGroupName { get; set; }
        public string? MobileUniqueId { get; set; }

        public string? AadharNumber { get; set; }

        public string? AadharImage { get; set; }

        public string? AadharOriginalFileName { get; set; }

        public string? AadharImageURL { get; set; }

        public string? PanNumber { get; set; }

        public string? PanCardImage { get; set; }

        public string? PanCardOriginalFileName { get; set; }

        public string? PanCardImageURL { get; set; }

        public string? ProfileImage { get; set; }

        public string? ProfileOriginalFileName { get; set; }

        public string? ProfileImageURL { get; set; }

        public bool? IsMobileUser { get; set; }

        public bool? IsWebUser { get; set; }

        public bool? IsActive { get; set; }
    }

    public class UserListByRole_Search
    {
        public int? CompanyId { get; set; }

        public string? RoleId { get; set; }
        public string? RoleName { get; set; }

        [DefaultValue(null)]
        public bool? IsActive { get; set; }
    }

    public class UserListByRole_Response
    {
        public int? UserId { get; set; }

        public string? UserName { get; set; }
    }

    public class ImportRequest
    {
        public IFormFile FileUpload { get; set; }
    }

    #region Import and Download

    public class User_ImportRequest
    {
        public IFormFile FileUpload { get; set; }
    }

    public class User_ImportData
    {
        public string? UserCode { get; set; }

        public string? UserName { get; set; }

        public string? MobileNumber { get; set; }

        public string? EmailId { get; set; }

        public string? Password { get; set; }

        public string? Role { get; set; }

        public string? ReportingTo { get; set; }

        public string? Department { get; set; }

        public string? Company { get; set; }

        public string? Address { get; set; }

        public string? State { get; set; }

        public string? District { get; set; }

        public string? City { get; set; }

        public string? Pincode { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime? DateOfJoining { get; set; }

        public string? EmergencyContactNumber { get; set; }

        public string? BloodGroup { get; set; }

        public string? AadharNumber { get; set; }

        public string? PanNumber { get; set; }

        public string? MobileUniqueId { get; set; }

        public string? IsMobileUser { get; set; }

        public string? IsWebUser { get; set; }

        public string? IsActive { get; set; }
    }

    public class User_ImportDataValidation
    {
        public string? UserCode { get; set; }

        public string? UserName { get; set; }

        public string? MobileNumber { get; set; }

        public string? EmailId { get; set; }

        public string? Password { get; set; }

        public string? Role { get; set; }

        public string? ReportingTo { get; set; }

        public string? Department { get; set; }

        public string? Company { get; set; }

        public string? Address { get; set; }

        public string? State { get; set; }

        public string? District { get; set; }

        public string? City { get; set; }

        public string? Pincode { get; set; }

        public string? DateOfBirth { get; set; }

        public string? DateOfJoining { get; set; }

        public string? EmergencyContactNumber { get; set; }

        public string? BloodGroup { get; set; }

        public string? AadharNumber { get; set; }

        public string? PanNumber { get; set; }

        public string? MobileUniqueId { get; set; }

        public string? IsMobileUser { get; set; }

        public string? IsWebUser { get; set; }

        public string? IsActive { get; set; }

        public string ValidationMessage { get; set; }
    }

    #endregion
}
