using MediatR;
using Shared.DTO;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Company
{
    public sealed record GetCompanyCollectionQuery(IEnumerable<Guid> Ids, CompanyParameters companyParameters, bool TrackChanges)
        :IRequest<(IEnumerable<CompanyDto> companies, MetaData metaData)>;
}
