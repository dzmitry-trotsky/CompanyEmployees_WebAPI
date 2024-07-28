using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.LinkModels;
using Entities.Models;
using Service.Contracts;
using Shared.DTO;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    internal sealed class EmployeeService: IEmployeeService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly IEmployeeLinks _employeeLinks;

        public EmployeeService(IRepositoryManager repository, ILoggerManager logger, 
            IMapper mapper, IEmployeeLinks employeeLinks)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _employeeLinks = employeeLinks;
        }

        public async Task<(LinkResponse linkResponse, MetaData metaData)> 
            GetAllAsync(Guid companyId, LinkParameters linkParameters, bool trackChanges)
        {
            if (!linkParameters.EmployeeParameters.ValidAgeRange)
                throw new MaxAgeRangeBadRequestException();

            await CheckIfCompanyExists(companyId, trackChanges);

            var employeesWithMetaData = await _repository.Employee.
                    GetEmployeesAsync(companyId, linkParameters.EmployeeParameters, trackChanges);

            var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesWithMetaData);

            var links = _employeeLinks.TryGenerateLinks(employeesDto, linkParameters.EmployeeParameters.Fields, 
                            companyId, linkParameters.Context);
            
            return (linkResponse: links, metaData: employeesWithMetaData.MetaData);

        }

        public async Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges)
        {
            await CheckIfCompanyExists(companyId, trackChanges);
            var employee = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id, trackChanges);

            var employeeDto = _mapper.Map<EmployeeDto>(employee);

            return employeeDto;
        }

        public async Task<EmployeeDto> CreateEmployeeForCompanyAsync(Guid companyId, EmployeeForCreationDto employeeForCreation, bool trackChanges)
        {
            await CheckIfCompanyExists(companyId, trackChanges);

            var employeeEntity = _mapper.Map<Employee>(employeeForCreation);

            _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
            await _repository.SaveAsync();

            var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);

            return employeeToReturn;
        }

        public async Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid id, bool trackChanges)
        {
            await CheckIfCompanyExists(companyId, trackChanges);

            var employeeEntity = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id, trackChanges);

            _repository.Employee.DeleteEmployee(employeeEntity);
            await _repository.SaveAsync();
        }

        public async Task UpdateEmployeeForCompanyAsync(Guid companyId, Guid id, EmployeeForUpdateDto employeeForUpdate, bool compTrackChanges, bool empTrackChanges)
        {
            await CheckIfCompanyExists(companyId, compTrackChanges);

            var employeeEntity = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id, empTrackChanges);
            _mapper.Map(employeeForUpdate, employeeEntity);
            await _repository.SaveAsync();
        }

        public async Task<(EmployeeForUpdateDto employeePatch, Employee employeeEntity)> GetEmployeeForPatchAsync
                    (Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges)
        {
            await CheckIfCompanyExists(companyId, compTrackChanges);

            var employee = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id, empTrackChanges);

            var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employee);

            return (employeeToPatch, employee);

        }

        public async Task SaveChangesForPatchAsync(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)
        {
            _mapper.Map(employeeToPatch, employeeEntity);
            await _repository.SaveAsync();
        }

        private async Task CheckIfCompanyExists(Guid id, bool trackChanges)
        {
            var company = await _repository.Company.GetCompanyAsync(id, trackChanges);

            if (company is null)
                throw new CompanyNotFoundException(id);
        }

        private async Task<Employee> GetEmployeeForCompanyAndCheckIfItExists(Guid companyId, Guid id, bool trackChanges)
        {
            var employee = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges);

            if (employee is null)
                throw new EmployeeNotFoundException(id);

            return employee;
        }
    }
}
