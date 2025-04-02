using AVPaperERP.Application.Enums;
using AVPaperERP.Application.Helpers;
using AVPaperERP.Application.Interfaces;
using AVPaperERP.Application.Models;
using AVPaperERP.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AVPaperERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : CustomBaseController
    {
        private ResponseModel _response;
        private IFileManager _fileManager;

        private readonly IDashboardRepository _dashboardRepository;

        public DashboardController(IFileManager fileManager, IDashboardRepository dashboardRepository)
        {
            _fileManager = fileManager;

            _response = new ResponseModel();
            _response.IsSuccess = true;
            _dashboardRepository = dashboardRepository;
        }
    }
}
