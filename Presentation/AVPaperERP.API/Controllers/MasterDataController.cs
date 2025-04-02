using AVPaperERP.Application.Interfaces;
using AVPaperERP.Application.Models;
using AVPaperERP.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AVPaperERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterDataController : CustomBaseController
    {
        private ResponseModel _response;
        private readonly IMasterDataRepository _masterDataRepository;

        public MasterDataController(IMasterDataRepository masterDataRepository)
        {
            _masterDataRepository = masterDataRepository;
            _response = new ResponseModel();
            _response.IsSuccess = true;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> GetReportingToEmpListForSelectList(ReportingToEmpListParameters parameters)
        {
            IEnumerable<SelectListResponse> lstResponse = await _masterDataRepository.GetReportingToEmployeeForSelectList(parameters);
            _response.Data = lstResponse.ToList();
            return _response;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> GetEmployeesListByReportingTo(int EmployeeId)
        {
            IEnumerable<EmployeesListByReportingTo_Response> lstResponse = await _masterDataRepository.GetEmployeesListByReportingTo(EmployeeId);
            _response.Data = lstResponse.ToList();
            return _response;
        }
    }
}
