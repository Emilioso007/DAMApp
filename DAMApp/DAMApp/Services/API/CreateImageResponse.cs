using DAMApp.Models;

namespace DAMApp.Services.API;

public class CreateImageResponse
{
    public string ImageId { get; set; }

    public CreateImageResponse(Image image)
    {
        ImageId = image.UUID.ToString();
    }
}