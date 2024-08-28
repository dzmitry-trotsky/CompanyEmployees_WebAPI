using Application.Queries.Company;
using AutoMapper;
using Contracts;
using MediatR;
using Shared.DTO;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.Company
{
    public class GetCompaniesHandler : IRequestHandler<GetCompaniesQuery, (IEnumerable<CompanyDto> companies, MetaData metaData)>
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;

        public GetCompaniesHandler(IRepositoryManager repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<(IEnumerable<CompanyDto> companies, MetaData metaData)> Handle(GetCompaniesQuery request, CancellationToken cancellationToken)
        {
            var companies = await _repository.Company.GetAllCompaniesAsync(request.CompanyParams, request.TrackChanges);

            var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);

            return (companiesDto, metaData: companies.MetaData);
        }
    }
}
