using CustomerApi.DTOs;
using FluentValidation;

namespace CustomerApi.Validations;

public class CreateCustomerRequestValidator :AbstractValidator<CreateCustomerRequest>
{
    public CreateCustomerRequestValidator()
    {
        RuleFor(c=>c.Name).NotEmpty().WithMessage("Name is required");

        RuleFor(c => c.Email).NotEmpty().WithMessage("Email is required")
                            .EmailAddress().WithMessage("Email Format is not correct");

    }
}
