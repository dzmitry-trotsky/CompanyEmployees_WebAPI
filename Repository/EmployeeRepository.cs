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
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(RepositoryContext context) : base(context)
        {
        }

        public Employee GetEmployee(Guid companyId, Guid id, bool trackChanges)
        {
            var employee = GetByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(id), trackChanges).SingleOrDefault();

            if(employee == null) { throw new EmployeeNotFoundException(id); }
            
            return employee;
        }

        public IEnumerable<Employee> GetEmployees(Guid companyId, bool trackChanges)
        {
            return GetByCondition(e => e.CompanyId.Equals(companyId), trackChanges)
                        .OrderBy(e => e.Name).ToList();
        }
    }
}
