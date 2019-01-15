using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Results;

namespace PrivateCert.Lib.Interfaces
{
    public interface IValidationResult
    {
        ValidationResult ValidationResult { get; set; }
    }
}
