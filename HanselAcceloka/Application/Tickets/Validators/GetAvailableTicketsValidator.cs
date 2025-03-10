using FluentValidation;
using HanselAcceloka.Application.Tickets.Queries;

namespace HanselAcceloka.Application.Tickets.Validators
{
    public class GetAvailableTicketsValidator : AbstractValidator<GetAvailableTicketsQuery>
    {
        public GetAvailableTicketsValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThan(0);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
            RuleFor(x => x.OrderBy)
                .Must(order => new[] { "ticket_code", "ticket_name", "category_name", "price", "event_date", "quota" }
                .Contains(order.ToLower()))
                .WithMessage("Invalid orderBy column.");
        }
    }
}