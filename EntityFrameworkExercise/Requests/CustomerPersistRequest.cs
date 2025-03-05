using System.Text.Json.Serialization;
using EntityFrameworkExercise.Models;

namespace EntityFrameworkExercise.Requests;

public class CustomerPersistRequest
{
    [JsonIgnore]
    public int Id { get; set; } = 0;

    public string Name { get; set; } = string.Empty;

    public static implicit operator Customer(CustomerPersistRequest request)
           => new()
           {
               Id = request.Id,
               Name = request.Name,
           };
}