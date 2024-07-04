﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Exceptions
{
    public sealed class CompanyCollectionBadRequestException : Exception
    {
        public CompanyCollectionBadRequestException(): base("Company collection sent from a client is null") { }
    }
}
