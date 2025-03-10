using HanselAcceloka.Models;
using MediatR;
using System;
using System.Collections.Generic;

namespace HanselAcceloka.Application.Tickets.Queries
{
    public class GetAvailableTicketsQuery : IRequest<PagedResult<TicketModel>>
    {
        public string? TicketCode { get; set; }
        public string? TicketName { get; set; }
        public string? CategoryName { get; set; }
        public int? MaxPrice { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string OrderBy { get; set; } = "ticket_code";
        public string OrderState { get; set; } = "asc";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}