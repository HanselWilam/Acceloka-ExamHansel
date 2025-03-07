using System;
using Microsoft.AspNetCore.Mvc;

public class ProblemDetailsException : Exception
{
    public ProblemDetails ProblemDetails { get; }

    public ProblemDetailsException(int statusCode, string title, string detail)
        : base(detail)
    {
        ProblemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Type = $"https://httpstatuses.com/{statusCode}"
        };
    }
}