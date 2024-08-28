using Application.Queries.Company;
using AutoMapper;
using Contracts;
using Entities.Exceptions;
using MediatR;
using Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.Company
{
    public class GetCompanyHandler: IRequestHandler<GetCompanyQuery, CompanyDto>
    {
        public readonly IRepositoryManager _repository;
        public readonly IMapper _mapper;

        public GetCompanyHandler(IRepositoryManager repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<CompanyDto> Handle(GetCompanyQuery request, CancellationToken cancellationToken)
        {
            var company = await _repository.Company.GetCompanyAsync(request.Id, request.TrackChanges);

            if (company is null)
                throw new CompanyNotFoundException(request.Id);

            var companyDto = _mapper.Map<CompanyDto>(company);

            return companyDto;
        }
    }
}
