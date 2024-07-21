using Entities.Models;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICompanyRepository
    {
        Task<PagedList<Company>> GetAllCompaniesAsync(CompanyParameters companyParameters,  bool trackChanges);

        Task<Company> GetCompanyAsync(Guid companyId, bool trackChanges);

        Task<PagedList<Company>> GetByIdsAsync(IEnumerable<Guid> companyIds, CompanyParameters companyParameters, bool trackChanges);

        void CreateCompany(Company company);

        void DeleteCompany(Company company);
    }
}
