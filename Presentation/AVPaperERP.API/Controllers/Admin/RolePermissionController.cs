using AVPaperERP.Application.Enums;
using AVPaperERP.Application.Interfaces;
using AVPaperERP.Application.Models;
using AVPaperERP.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;

namespace AVPaperERP.API.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolePermissionController : CustomBaseController
    {
        private ResponseModel _response;
        private readonly IRolePermissionRepository _rolePermissionRepository;

        public RolePermissionController(IRolePermissionRepository rolePermissionRepository)
        {
            _rolePermissionRepository = rolePermissionRepository;
            _response = new ResponseModel();
            _response.IsSuccess = true;
        }

        #region Module Master 

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> GetModuleMasterList(BaseSearchEntity parameters)
        {
            IEnumerable<ModuleMaster_Response> lstModuleMaster = await _rolePermissionRepository.GetModuleMasterList(parameters);
            _response.Data = lstModuleMaster.ToList();
            _response.Total = parameters.Total;
            return _response;
        }

        #endregion


        #region RoleMasterPermission 

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> SaveRoleMasterPermission(RolePermission_Request parameters)
        {
            int result = await _rolePermissionRepository.SaveRoleMasterPermission(parameters);

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
        public async Task<ResponseModel> GetRoleMasterPermissionList(RolePermission_Search parameters)
        {
            IEnumerable<RolePermission_Response> lstRoleMasterPermissions = await _rolePermissionRepository.GetRoleMasterPermissionList(parameters);
            _response.Data = lstRoleMasterPermissions.ToList();
            _response.Total = parameters.Total;
            return _response;
        }

        //[Route("[action]")]
        //[HttpPost]
        //public async Task<ResponseModel> GetRoleMasterPermissionById(long Id)
        //{
        //    if (Id <= 0)
        //    {
        //        _response.Message = "Id is required";
        //    }
        //    else
        //    {
        //        var vResultObj = await _rolePermissionRepository.GetRoleMasterPermissionById(Id);
        //        _response.Data = vResultObj;
        //    }
        //    return _response;
        //}

        #endregion


        #region Role Master Employee Permission 

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> SaveRoleMasterEmployeePermission(RoleMaster_Employee_Permission_Request parameters)
        {
            if (parameters.EmployeeId <= 0)
            {
                _response.Message = "EmployeeId is required";
            }
            else
            {
                int result = await _rolePermissionRepository.SaveRoleMasterEmployeePermission(parameters);

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
            }

            return _response;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> GetRoleMasterEmployeePermissionList(RoleMaster_Employee_Search_Request parameters)
        {
            if (parameters.EmployeeId <= 0)
            {
                _response.Message = "EmployeeId is required";
            }
            else
            {
                IEnumerable<RoleMaster_Employee_Permission_Response> lstRoleMasterEmployeePermissions = await _rolePermissionRepository.GetRoleMasterEmployeePermissionList(parameters);
                _response.Data = lstRoleMasterEmployeePermissions.ToList();
                _response.Total = parameters.Total;
            }
            return _response;
        }

        //[Route("[action]")]
        //[HttpPost]
        //public async Task<ResponseModel> GetRoleMasterEmployeePermissionById(long employeeid)
        //{
        //    if (employeeid <= 0)
        //    {
        //        _response.Message = "EmployeeId is required";
        //    }
        //    else
        //    {
        //        var vResultObj = await _rolePermissionRepository.GetRoleMasterEmployeePermissionById(employeeid);
        //        _response.Data = vResultObj;
        //    }
        //    return _response;
        //}

        #endregion
    }
}
