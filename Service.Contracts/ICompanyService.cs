﻿using Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface ICompanyService
    {
        IEnumerable<CompanyDto> GetAllCompanies(bool trackChanges);

        CompanyDto GetCompany(Guid id, bool trackChanges);

        CompanyDto CreateCompany(CompanyForCreationDto company);

        IEnumerable<CompanyDto> GetByIds(IEnumerable<Guid> ids, bool trackChanges);

        (IEnumerable<CompanyDto> companies, string ids) CreateCompanyCollection 
            (IEnumerable<CompanyForCreationDto> companyCollection);

        void DeleteCompany(Guid id, bool trackChanges);
    }
}
