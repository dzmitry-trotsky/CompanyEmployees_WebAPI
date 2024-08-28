using Application.Command.Company;
using AutoMapper;
using Contracts;
using Entities.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.Company
{
    public class UpdateCompanyCommandHandler: IRequestHandler<UpdateCompanyCommand, Unit>
    {
        public readonly IRepositoryManager _repository;
        public readonly IMapper _mapper;

        public UpdateCompanyCommandHandler(IRepositoryManager repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
        {
            var companyEntity = await _repository.Company.GetCompanyAsync(request.Id, request.TrackChanges) 
                ?? throw new CompanyNotFoundException(request.Id);

            _mapper.Map(request.Company, companyEntity);
            await _repository.SaveAsync();

            return Unit.Value;
        }
    }
}
