using Asp.Versioning;
using CompanyEmployees.Presentation.ActionFilters;
using CompanyEmployees.Presentation.ModelBinders;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service.Contracts;
using Shared.DTO;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CompanyEmployees.Presentation.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/companies")]
    [ApiController]
    public class CompanyController: ControllerBase
    {
        private readonly IServiceManager _service;

        public CompanyController(IServiceManager service)
        {
            _service = service;
        }

        [HttpGet(Name = "GetCompanies")]
        public async Task<IActionResult> GetCompanies([FromQuery] CompanyParameters companyParameters)
        {
            var pagedResult = await _service.CompanyService.GetAllCompaniesAsync(companyParameters, trackChanges: false);

            Response.Headers.Add("X-Pagination",
            JsonSerializer.Serialize(pagedResult.metaData));

            return Ok(pagedResult.companies);
        }

        [HttpGet("{id:guid}", Name = "CompanyById")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]
        [HttpCacheValidation(MustRevalidate = false)]
        public async Task<IActionResult> GetCompany(Guid id)
        {
            var company = await _service.CompanyService.GetCompanyAsync(id, false);

            return Ok(company);
        }

        [HttpPost(Name = "CreateCompany")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyForCreationDto company)
        {
            var createdCompany = await _service.CompanyService.CreateCompanyAsync(company);

            return CreatedAtRoute("CompanyById", new {id = createdCompany.Id}, createdCompany);
        }

        [HttpGet("collection/({ids})", Name = "CompanyCollection")]
        public async Task<IActionResult> GetCompanyCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))]
                                                        IEnumerable<Guid> ids, [FromQuery] CompanyParameters companyParameters) 
        {
            var pagedResult = await _service.CompanyService.GetByIdsAsync(ids, companyParameters, false);

            Response.Headers.Add("X-Pagination",
            JsonSerializer.Serialize(pagedResult.metaData));

            return Ok(pagedResult.companies);
        
        }

        [HttpPost("collection")]
        public async Task<IActionResult> CreateCompanyCollection([FromBody] IEnumerable<CompanyForCreationDto> companyCollection) 
        {
            var result = await _service.CompanyService.CreateCompanyCollectionAsync(companyCollection);

            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            return CreatedAtRoute("CompanyCollection", new { result.ids }, result.companies);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCompany (Guid id)
        {
            await _service.CompanyService.DeleteCompanyAsync(id, false);

            return NoContent();
        }

        [HttpPut("{id:guid}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] CompanyForUpdateDto company)
        {
            await _service.CompanyService.UpdateCompanyAsync(id, company, true);

            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetCompaniesOptions()
        {
            Response.Headers.Add("Allow", "GET, OPTIONS, POST");

            return Ok();
        }
    }
}
