using MediatR;
using Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.Company
{
    public sealed record UpdateCompanyCommand(Guid Id, CompanyForUpdateDto Company, bool TrackChanges): IRequest<Unit>;
}
