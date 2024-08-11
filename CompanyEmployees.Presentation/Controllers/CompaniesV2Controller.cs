using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.RequestFeatures;

namespace CompanyEmployees.Presentation.Controllers
{
    /// <summary>
    /// Class for testing versioning
    /// </summary>
    [ApiVersion("2.0")]
    [Route("api/companies")]
    [ApiController]
    public class CompaniesV2Controller: ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public CompaniesV2Controller(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        public async Task<IActionResult> GetCompanies([FromQuery] CompanyParameters companyParameters)
        {
            var companiesWithMetaData = await _serviceManager.CompanyService.GetAllCompaniesAsync(companyParameters, false);
            
            var companiesV2 = companiesWithMetaData.companies.Select(x => $"{x.Name} V2");

            return Ok(companiesV2);
        }
    }
}
