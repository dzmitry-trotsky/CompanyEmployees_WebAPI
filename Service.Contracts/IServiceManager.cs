using Entities.ConfigurationModels;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface IServiceManager
    {
        IEmployeeService EmployeeService { get; }
        IAuthenticationService AuthenticationService { get; }
    }
}
