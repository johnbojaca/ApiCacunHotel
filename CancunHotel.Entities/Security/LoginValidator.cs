using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CancunHotel.Entities.Security
{
    public class LoginValidator : AbstractValidator<LoginEntity>
    {
        public LoginValidator()
        {
            RuleFor(model => model.Username)
                .NotNull().WithMessage("Username is Null")
                .NotEmpty().WithMessage("Please specify a Username");

            RuleFor(model => model.Password)
                .NotNull().WithMessage("Password is Null")
                .NotEmpty().WithMessage("Please specify a Password");
        }
    }
}
