using FluentValidation.Results;

namespace PrivateCert.LibCore.Interfaces
{
    public interface IValidationResult
    {
        ValidationResult ValidationResult { get; set; }
    }
}
