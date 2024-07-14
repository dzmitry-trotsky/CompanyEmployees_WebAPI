﻿using Entities.Models;
using Shared.DTO;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface IEmployeeService
    {
        Task<(IEnumerable<EmployeeDto> employees, MetaData metaData)> GetAllAsync(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges);

        Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges);

        Task<EmployeeDto> CreateEmployeeForCompanyAsync(Guid companyId, EmployeeForCreationDto employeeForCreation, 
                                                                                bool trackChanges);

        Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid id, bool trackChanges);

        Task UpdateEmployeeForCompanyAsync(Guid companyId, Guid id, EmployeeForUpdateDto employeeForUpdate, 
                                         bool compTrackChanges, bool empTrackChanges);

        Task<(EmployeeForUpdateDto employeePatch, Employee employeeEntity)> GetEmployeeForPatchAsync
                    (Guid companuId, Guid id, bool compTrackChanges, bool empTrackChanges);

        Task SaveChangesForPatchAsync(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity);
    }
}
