using EntityFrameworkExercise.Services;
using Microsoft.AspNetCore.Mvc;

namespace EntityFrameworkExercise.Requests;

public class CustomerQueryRequest
{
    [FromQuery(Name = "_page")]
    public int Page { get; set; } = 1;

    [FromQuery(Name = "_size")]
    public int Size { get; set; } = 5;

    [FromQuery(Name = "_search")]
    public string? SearchTerm { get; set; } = null;

    public static implicit operator CustomerSearch(CustomerQueryRequest request)
        => new()
        {
            Page = request.Page,
            Size = request.Size,
            Term = request.SearchTerm,
        };
}