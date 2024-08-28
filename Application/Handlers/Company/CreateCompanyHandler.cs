using Application.Command.Company;
using AutoMapper;
using Contracts;
using MediatR;
using Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.Company
{
    public class CreateCompanyHandler: IRequestHandler<CreateCompanyCommand, CompanyDto>
    {
        public readonly IRepositoryManager _repository;
        public readonly IMapper _mapper;

        public CreateCompanyHandler(IRepositoryManager manager, IMapper mapper)
        {
            _repository = manager;
            _mapper = mapper;
        }

        public async Task<CompanyDto> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
        {
            var companyEntity = _mapper.Map<Entities.Models.Company>(request.Company);

            _repository.Company.CreateCompany(companyEntity);

            await _repository.SaveAsync();

            var companyToReturn = _mapper.Map<CompanyDto>(companyEntity);

            return companyToReturn;
        }
    }
}
