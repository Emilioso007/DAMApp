using DAMApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace DAMApp.Components.Pages.Product;

public partial class Products : ComponentBase
{
	
	
	[Inject] private NavigationManager Navigation { get; set; }
	[Inject] private CreateService CreateService { get; set; }
	[Inject] private ReadService ReadService { get; set; }
	[Inject] private UpdateService UpdateService { get; set; }
	[Inject] private DeleteService DeleteService { get; set; }
	
	private string searchProduct = ""; //variable that holds text in searchbar

    private bool _isLoaded = false;
    private string searchText = ""; // Holds the search text
    List<Models.Product> products = [];

    private string _newProductName, _newProductUuid;

    private async Task AddNewProduct ()
    {
	    try
	    {
		    Models.Product product = new Models.Product()
		    {
			    Name = _newProductName,
			    UUID = new Guid(_newProductUuid)
		    };
		    await CreateService.UploadProductWithUUID(product);
	    }
	    catch (Exception e)
	    {
		    Console.WriteLine(e.Message);
	    }
    }
    
    protected override void OnInitialized ()
    {
        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await Task.Delay(100);
            _isLoaded = true;
            StateHasChanged();
        }
    }

    public void NavigateToHome ()
    {
        Navigation.NavigateTo("/dam");
    }

    public void OnImageClickNavigateTo(string productId)
    {
        Navigation.NavigateTo($"/dam/products/edit?productId={productId}");
    }
    
    private IEnumerable<Models.Product> FilteredProducts()
    {
        if (string.IsNullOrWhiteSpace(searchProduct)) //if there is nothing in search product it searches for all the products
            return products;

        return products.Where(products =>
            (products.Name != null && products.Name.Contains(searchProduct, StringComparison.OrdinalIgnoreCase)) || //returns based on product name and product id
            (products.UUID != null && products.UUID.ToString().Contains(searchProduct, StringComparison.OrdinalIgnoreCase)));
    }
}