using Contracts;
using Entities.Exceptions;
using Entities.Models;
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

        public IEnumerable<Company> GetAllCompanies(bool trackChanges)
        {
            return GetAll(trackChanges).OrderBy(_ => _.Name).ToList();
        }

        public Company GetCompany(Guid companyId, bool trackChanges)
        {
            var company = GetByCondition(c => c.Id.Equals(companyId), trackChanges).SingleOrDefault();
            

            if (company is null) { throw new CompanyNotFoundException(companyId); }

            return company;
        }
    }
}
