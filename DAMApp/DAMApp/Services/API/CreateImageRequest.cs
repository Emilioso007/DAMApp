using System.ComponentModel.DataAnnotations;

namespace DAMApp.Services.API;

public class CreateImageRequest
{
    [Required] public string Content { get; set; }
}