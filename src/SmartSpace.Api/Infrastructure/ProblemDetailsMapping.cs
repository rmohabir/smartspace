using Microsoft.AspNetCore.Mvc;
using SmartSpace.Api.Contracts;

namespace SmartSpace.Api.Infrastructure;

public static class ProblemDetailsMapping
{
    public static void Configure(ProblemDetailsOptions options)
    {
        options.CustomizeProblemDetails = context =>
        {
            context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
        };
    }

    public static ProblemDetails Create(int status, string title, string code, string detail) =>
        new()
        {
            Status = status,
            Title = title,
            Detail = detail,
            Extensions = { ["code"] = code }
        };
}
