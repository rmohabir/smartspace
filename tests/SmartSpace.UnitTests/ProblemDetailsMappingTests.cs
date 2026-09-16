using Microsoft.AspNetCore.Http;
using SmartSpace.Api.Contracts;
using SmartSpace.Api.Infrastructure;

namespace SmartSpace.UnitTests;

public sealed class ProblemDetailsMappingTests
{
    [Fact]
    public void Creates_stable_problem_code_without_internal_details()
    {
        var problem = ProblemDetailsMapping.Create(
            StatusCodes.Status409Conflict,
            "Reservation conflict",
            ApiErrorCodes.BookingOverlap,
            "The requested interval is not available.");

        Assert.Equal(StatusCodes.Status409Conflict, problem.Status);
        Assert.Equal(ApiErrorCodes.BookingOverlap, problem.Extensions["code"]);
        Assert.DoesNotContain("StackTrace", problem.Detail ?? string.Empty);
    }
}
