using Application.Command.Company;
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
    public class CreateCompanyCollectionCommandHandler: IRequestHandler<CreateCompanyCollectionCommand, (IEnumerable<CompanyDto> companies, string ids)>
    {
        public readonly IRepositoryManager _repository;
        public readonly IMapper _mapper;

        public CreateCompanyCollectionCommandHandler(IRepositoryManager repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<(IEnumerable<CompanyDto> companies, string ids)> Handle(CreateCompanyCollectionCommand request, CancellationToken cancellationToken)
        {
            if (request.CompanyCollection is null)
                throw new CompanyCollectionBadRequestException();

            var companyEnities = _mapper.Map<IEnumerable<Entities.Models.Company>>(request.CompanyCollection);

            foreach (var company in companyEnities)
                _repository.Company.CreateCompany(company);

            await _repository.SaveAsync();

            var companyCollectionToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEnities);

            var ids = string.Join(',', companyCollectionToReturn.Select(c => c.Id));

            return (companyCollectionToReturn, ids);
        }
    }
}
