using FluentValidation;

namespace CancunHotel.Entities.Guests
{
    public class GuestValidator : AbstractValidator<Guest>
    {
        public GuestValidator()
        {
            RuleFor(model => model.Names)
                .NotNull().WithMessage("Names are Null")
                .NotEmpty().WithMessage("Please specify Names");

            RuleFor(model => model.Surnames)
                .NotNull().WithMessage("Surnames are Null")
                .NotEmpty().WithMessage("Please specify Surnames");

            RuleFor(model => model.IdType.Id)
                .GreaterThan(p => 0).WithMessage("IdType great that 0");

            RuleFor(model => model.Identification)
                .NotNull().WithMessage("Identification is Null")
                .NotEmpty().WithMessage("Please specify Identification");

            RuleFor(model => model.Age)
                .GreaterThanOrEqualTo(p => 0).WithMessage("Age great or equal that 0");

            RuleFor(model => model.gender.IdGender)
                .GreaterThan(p => 0).WithMessage("IdGender great that 0");
        }
    }
}
