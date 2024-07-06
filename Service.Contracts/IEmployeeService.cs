using Entities.Models;
using Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface IEmployeeService
    {
        IEnumerable<EmployeeDto> GetAll(Guid companyId, bool trackChanges);

        EmployeeDto GetEmployee(Guid companyId, Guid id, bool trackChanges);

        EmployeeDto CreateEmployeeForCompany(Guid companyId, EmployeeForCreationDto employeeForCreation, bool trackChanges);

        void DeleteEmployeeForCompany(Guid companyId, Guid id, bool trackChanges);

        void UpdateEmployeeForCompany(Guid companyId, Guid id, EmployeeForUpdateDto employeeForUpdate, 
                                         bool compTrackChanges, bool empTrackChanges);

        (EmployeeForUpdateDto employeePatch, Employee employeeEntity) GetEmployeeForPatch
                    (Guid companuId, Guid id, bool compTrackChanges, bool empTrackChanges);

        void SaveChangesForPatch(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity);
    }
}
