using DAMApp.Models;

namespace DAMApp.Services.API;

public class CreateMockProductResponse
{
    public string ProductID { get; set; }

    public CreateMockProductResponse(Product product)
    {
        ProductID = product.UUID.ToString();
    }
}