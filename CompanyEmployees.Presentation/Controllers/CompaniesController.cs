using Application.Command.Company;
using Application.Queries.Company;
using Asp.Versioning;
using CompanyEmployees.Presentation.ActionFilters;
using CompanyEmployees.Presentation.ModelBinders;
using Marvin.Cache.Headers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
    [ApiExplorerSettings(GroupName = "v1")]
    [Authorize(Roles = "Administrator")]
    public class CompaniesController: ControllerBase
    {
        private readonly ISender _sender;

        public CompaniesController(ISender sender)
        {
            _sender = sender;
        }

        ///<summary>
        ///Gets the list of all comanies
        ///</summary>
        ///<param name="companyParameters"></param>
        ///<returns>Companies list</returns>
        ///<response code="200">Returns companies list</response>
        ///<response code="400">If companies is null</response>
        ///<response code="401">If not authorized</response>
        ///<response code="422">If the model is invalid</response>

        [HttpGet(Name = "GetCompanies")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> GetCompanies([FromQuery] CompanyParameters companyParameters)
        {
            var pagedResult = await _sender.Send(new GetCompaniesQuery(companyParameters, TrackChanges: false));

            Response.Headers.Add("X-Pagination",
            JsonSerializer.Serialize(pagedResult.metaData));

            return Ok(pagedResult.companies);
        }


        ///<summary>
        /// Gets the company by id
        ///</summary>
        /// <param name="id"></param>
        ///<returns>Company</returns>
        ///<response code="200">Returns company</response>
        /// <response code="400">If company is null</response>
        ///<response code="401">If not authorized</response>
        /// <response code="422">If the model is invalid</response>
        [HttpGet("{id:guid}", Name = "CompanyById")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]
        [HttpCacheValidation(MustRevalidate = false)]
        public async Task<IActionResult> GetCompany(Guid id)
        {
            var company = await _sender.Send(new GetCompanyQuery(id, TrackChanges: false));

            return Ok(company);
        }


        ///<summary>
        /// Creates the new company
        ///</summary>
        /// <param name="company"></param>
        ///<returns>Just created company</returns>
        ///<response code="201">Returns the newly created company</response>
        /// <response code="400">If company is null</response>
        ///<response code="401">If not authorized</response>
        /// <response code="422">If the model is invalid</response>
        [HttpPost(Name = "CreateCompany")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]

        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyForCreationDto company)
        {
            if (company is null)
                return BadRequest("CompanyForCreationDto object is null");

            var createdCompany = await _sender.Send(new CreateCompanyCommand(company));

            return CreatedAtRoute("CompanyById", new {id = createdCompany.Id}, createdCompany);
        }

        ///<summary>
        /// Gets the list of companies by ids
        ///</summary>
        /// <param name="ids" type="string"></param>
        /// <param name="companyParameters"></param>
        ///<returns>Companies list</returns>
        ///<response code="200">Returns companies</response>
        /// <response code="400">If companies is null</response>
        ///<response code="401">If not authorized</response>
        /// <response code="422">If the model is invalid</response>
        [HttpGet("collection/({ids})", Name = "CompanyCollection")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> GetCompanyCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))]
                                                        IEnumerable<Guid> ids, [FromQuery] CompanyParameters companyParameters) 
        {
            var pagedResult = await _sender.Send(new GetCompanyCollectionQuery(ids, companyParameters, TrackChanges: false));

            Response.Headers.Add("X-Pagination",
            JsonSerializer.Serialize(pagedResult.metaData));

            return Ok(pagedResult.companies);
        
        }

        ///<summary>
        /// Creates the new companies
        ///</summary>
        /// <param name="companyCollection"></param>
        ///<returns>Just created companies list</returns>
        ///<response code="201">Returns the newly created companies</response>
        /// <response code="400">If companies is null</response>
        ///<response code="401">If not authorized</response>
        /// <response code="422">If the model is invalid</response>
        [HttpPost("collection")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> CreateCompanyCollection([FromBody] IEnumerable<CompanyForCreationDto> companyCollection) 
        {
            var result = await _sender.Send(new CreateCompanyCollectionCommand(companyCollection));

            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            return CreatedAtRoute("CompanyCollection", new { result.ids }, result.companies);
        }

        ///<summary>
        /// Removes company by id
        ///</summary>
        /// <param name="id"></param>
        ///<returns>No content if company removes successfuly</returns>
        ///<response code="204">If company successfuly removed</response>
        /// <response code="400">If companies is null</response>
        ///<response code="401">If not authorized</response>
        /// <response code="422">If the model is invalid</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> DeleteCompany (Guid id)
        {
            await _sender.Send(new DeleteCompanyCommand(id, TrackChanges: false));

            return NoContent();
        }

        ///<summary>
        /// Updates the company by id
        ///</summary>
        /// <param name="company"></param>
        ///<returns>No content if company updates sucessfuly</returns>
        ///<response code="204">If company successfuly updated</response>
        /// <response code="400">If companies is null</response>
        ///<response code="401">If not authorized</response>
        /// <response code="422">If the model is invalid</response>
        [HttpPut("{id:guid}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] CompanyForUpdateDto company)
        {
            await _sender.Send(new UpdateCompanyCommand(id, company, TrackChanges: true));

            return NoContent();
        }

        ///<summary>
        /// Info about actions wich can be used with companies
        ///</summary>
        ///<returns>Header with actions wich can be used with companies</returns>
        ///<response code="200">Returns header with actions wich can be used with companies</response>
        ///<response code="401">If not authorized</response>
        [HttpOptions]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public IActionResult GetCompaniesOptions()
        {
            Response.Headers.Add("Allow", "GET, OPTIONS, POST");

            return Ok();
        }
    }
}
