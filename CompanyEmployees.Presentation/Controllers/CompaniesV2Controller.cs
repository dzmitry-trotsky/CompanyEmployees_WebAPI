using Application.Queries.Company;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.RequestFeatures;
using System.Text.Json;

namespace CompanyEmployees.Presentation.Controllers
{
    /// <summary>
    /// Class for testing versioning
    /// </summary>
    [ApiVersion("2.0")]
    [Route("api/companies")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v2")]
    public class CompaniesV2Controller: ControllerBase
    {
        private readonly ISender _sender;

        public CompaniesV2Controller(ISender sender)
        {
            _sender = sender;
        }

        public async Task<IActionResult> GetCompanies([FromQuery] CompanyParameters companyParameters)
        {
            var pagedResult = await _sender.Send(new GetCompaniesQuery(companyParameters, TrackChanges: false));

            Response.Headers.Add("X-Pagination",
            JsonSerializer.Serialize(pagedResult.metaData));

            return Ok(pagedResult.companies);
        }
    }
}
