using System.ComponentModel.DataAnnotations;

namespace DAMApp.Services.API;

public class CreateProductRequest
{
	[Required] public string Name { get; set; }
	[Required] public Guid UUID { get; set; }
}