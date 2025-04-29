using System.ComponentModel.DataAnnotations;

namespace DAMApp.Services.API;

public class CreateMockProductRequest
{
    [Required] public string Name { get; set; }
}