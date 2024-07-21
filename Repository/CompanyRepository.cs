using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
    {
        public CompanyRepository(RepositoryContext context) : base(context)
        {
        }

        public async Task<PagedList<Company>> GetAllCompaniesAsync(CompanyParameters companyParameters, bool trackChanges)
        {
            var companies =  await GetAll(trackChanges).OrderBy(_ => _.Name).ToListAsync();

            return PagedList<Company>.ToPagedList(companies, companyParameters.PageNumber, companyParameters.PageSize);
        }

        public async Task<Company> GetCompanyAsync(Guid companyId, bool trackChanges)
        {
            var company = await GetByCondition(c => c.Id.Equals(companyId), trackChanges).SingleOrDefaultAsync();
            

            if (company is null) { throw new CompanyNotFoundException(companyId); }

            return company;
        }

        public void CreateCompany(Company company)
        {
            Create(company);
        }

        public async Task<PagedList<Company>> GetByIdsAsync(IEnumerable<Guid> companyIds, CompanyParameters companyParameters, bool trackChanges)
        {
            var companies = await GetByCondition(c => companyIds.Contains(c.Id), trackChanges).ToListAsync();

            return PagedList<Company>.ToPagedList(companies, companyParameters.PageNumber, companyParameters.PageSize);
        }

        public void DeleteCompany(Company company)
        {
            Delete(company);
        }
    }
}
