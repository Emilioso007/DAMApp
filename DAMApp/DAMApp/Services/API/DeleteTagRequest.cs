using System.ComponentModel.DataAnnotations;

namespace DAMApp.Services.API;

public class DeleteTagRequest
{
    [Required] public string TagUUID { get; set; }
}