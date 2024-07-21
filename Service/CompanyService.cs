using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DTO;
using Shared.RequestFeatures;
using System.ComponentModel.Design;

namespace Service
{
    internal sealed class CompanyService : ICompanyService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public CompanyService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<(IEnumerable<CompanyDto> companies, MetaData metaData)> GetAllCompaniesAsync(CompanyParameters companyParameters, bool trackChanges)
        {
            var companiesWithMetadata = await _repository.Company.GetAllCompaniesAsync(companyParameters, trackChanges);
            var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companiesWithMetadata);

            return (companiesDto, metaData: companiesWithMetadata.MetaData);
        }

        public async Task<CompanyDto> GetCompanyAsync(Guid id, bool trackChanges)
        {
            var company = await GetCompanyAndCheckIfItExists(id, trackChanges);

            var companyDto = _mapper.Map<CompanyDto>(company);

            return companyDto;
        }

        public async Task<CompanyDto> CreateCompanyAsync(CompanyForCreationDto company)
        {
            var companyEntity = _mapper.Map<Company>(company);

            _repository.Company.CreateCompany(companyEntity);

            await _repository.SaveAsync();

            var companyToReturn = _mapper.Map<CompanyDto>(companyEntity);

            return companyToReturn;
        }

        public async Task<(IEnumerable<CompanyDto> companies, MetaData metaData)> GetByIdsAsync(IEnumerable<Guid> ids, CompanyParameters companyParameters, bool trackChanges)
        {
            if(ids is null)
                throw new IdParametersBadRequestException();

            var companyEnitiesWithMetadata = await _repository.Company.GetByIdsAsync(ids, companyParameters, trackChanges);

            if(ids.Count() != companyEnitiesWithMetadata.Count())
                throw new CollectionByIdsBadRequestException();

            var companiesToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEnitiesWithMetadata);

            return (companiesToReturn, metaData: companyEnitiesWithMetadata.MetaData);
        }

        public async Task<(IEnumerable<CompanyDto> companies, string ids)> CreateCompanyCollectionAsync(IEnumerable<CompanyForCreationDto> companyCollection)
        {
            if (companyCollection is null) 
                throw new CompanyCollectionBadRequestException();

            var companyEnities = _mapper.Map<IEnumerable<Company>>(companyCollection);

            foreach (var company in companyEnities)
                _repository.Company.CreateCompany(company);

            await _repository.SaveAsync();

            var companyCollectionToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEnities);
            var ids = string.Join(',', companyCollectionToReturn.Select(c => c.Id));

            return (companyCollectionToReturn, ids);
        }

        public async Task DeleteCompanyAsync(Guid id, bool trackChanges)
        {
            var company = await GetCompanyAndCheckIfItExists(id, trackChanges);

            _repository.Company.DeleteCompany(company);
            await _repository.SaveAsync();
        }

        public async Task UpdateCompanyAsync(Guid companyId, CompanyForUpdateDto companyForUpdate, bool trackChanges)
        {
            var company = await GetCompanyAndCheckIfItExists(companyId, trackChanges);

            _mapper.Map(companyForUpdate, company);
            await _repository.SaveAsync();
        }

        private async Task<Company> GetCompanyAndCheckIfItExists(Guid id, bool trackChanges)
        {
            var company = await _repository.Company.GetCompanyAsync(id, trackChanges);

            if (company is null)
                throw new CompanyNotFoundException(id);

            return company;
        }
    }
}
