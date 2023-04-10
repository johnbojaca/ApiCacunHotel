using CancunHotel.Entities.Guests;
using FluentValidation;

namespace CancunHotel.Entities.Bookings
{
    public class BookingValidator : AbstractValidator<Booking>
    {
        public BookingValidator()
        {
            int MaxDaysInterval = 3;
            int MaxDaysBooking = 30;

            DateTime now = DateTime.Now;
            DateTime minStartDateLimit = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0).AddDays(1);
            DateTime maxStartDateLimit = new DateTime(minStartDateLimit.Year, minStartDateLimit.Month, minStartDateLimit.Day, 23, 59, 59).AddDays(MaxDaysBooking);

            RuleFor(model => model.StartDate)
                .NotEmpty().WithMessage("Please specify Start Date")
                .GreaterThan(p => minStartDateLimit).WithMessage($"StartDate greater that {minStartDateLimit}")
                .LessThan(p => maxStartDateLimit).WithMessage($"StartDate less that {maxStartDateLimit}");

            RuleFor(model => model.FinalDate)
                .NotEmpty().WithMessage("Please specify Start Date")
                .GreaterThan(p => new DateTime(p.StartDate.Year, p.StartDate.Month, p.StartDate.Day, 0, 0, 0)).WithMessage("FinalDate greater or equal that StartDate")
                .LessThan(p => new DateTime(p.StartDate.Year, p.StartDate.Month, p.StartDate.Day, 23, 59, 59).AddDays(MaxDaysInterval-1)).WithMessage($"FinalDate less {MaxDaysInterval} days of StartDate");

            RuleFor(model => model.room.RoomNumber)
                .GreaterThan(p => 0).WithMessage("RoomNumber greater than 0");

            RuleFor(model => model.guests)
                .NotNull().WithMessage("Guest list is Null")
                .NotEmpty().WithMessage("Guest list is Empty");
            RuleForEach(x => x.guests).SetValidator(new GuestValidator());
        }
    }
}
