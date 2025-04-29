using System.ComponentModel.DataAnnotations;

namespace DAMApp.Services.API;

public class CreateTagRequest
{
    [Required] public string Name { get; set; }
}