using FluentValidation;

namespace CancunHotel.Entities.Guests
{
    public class ListGuestValidator : AbstractValidator<ListGuest>
    {
        public ListGuestValidator()
        {
            RuleFor(model => model.Guests)
                .NotNull().WithMessage("Guest list is Null")
                .NotEmpty().WithMessage("Guest list is Empty");
            RuleForEach(x => x.Guests).SetValidator(new GuestValidator());
        }
    }
}
