using Application.Queries.Company;
using AutoMapper;
using Contracts;
using Entities.Exceptions;
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
    public class GetCompanyCollectionQueryHandler : IRequestHandler<GetCompanyCollectionQuery, (IEnumerable<CompanyDto> companies, MetaData metaData)>
    {
        public readonly IRepositoryManager _repository;
        public readonly IMapper _mapper;

        public GetCompanyCollectionQueryHandler(IRepositoryManager repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<(IEnumerable<CompanyDto> companies, MetaData metaData)> Handle(GetCompanyCollectionQuery request, CancellationToken cancellationToken)
        {

            if (request.Ids is null)
                throw new IdParametersBadRequestException();

            var companyEnitiesWithMetadata = 
                await _repository.Company.GetByIdsAsync(request.Ids, request.companyParameters, request.TrackChanges);

            if (request.Ids.Count() != companyEnitiesWithMetadata.Count())
                throw new CollectionByIdsBadRequestException();

            var companiesToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEnitiesWithMetadata);

            return (companiesToReturn, metaData: companyEnitiesWithMetadata.MetaData);
        }
    }
}
