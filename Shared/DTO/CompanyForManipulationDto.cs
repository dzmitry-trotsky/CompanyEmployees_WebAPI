using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO
{
    public abstract record CompanyForManipulationDto
    {
        [Required(ErrorMessage = "Company name is required field")]
        [MaxLength(30, ErrorMessage = "Max length for company name is 30 chars")]
        public string? Name { get; init; }

        [Required(ErrorMessage = "Company address is required field")]
        [MaxLength(100, ErrorMessage = "Max length for company address is 100 chars")]
        public string? Address { get; init; }

        [Required(ErrorMessage = "Company country is required field")]
        [MaxLength(30, ErrorMessage = "Max length for company country is 30 chars")]
        public string? Country { get; init; }

        public IEnumerable<EmployeeForCreationDto>? Employees { get; init; }

    }
}
