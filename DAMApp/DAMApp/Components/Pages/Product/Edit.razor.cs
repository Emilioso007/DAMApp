using DAMApp.Services;
using Microsoft.AspNetCore.Components;

namespace DAMApp.Components.Pages.Product;

public partial class Edit : ComponentBase
{

	[Inject] private NavigationManager Navigation { get; set; }
	
	private string _productId = "";
    private string _productName = "";
    private int _pageNumber = 1;
    private string _searchText = "";

    private List<Models.Image> _productImages = [];
    private List<Models.Image> _gallery = [];
        
    protected override async Task OnInitializedAsync ()
    {
        var uri = new Uri(Navigation.Uri);
        var queryParams = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);

        if (queryParams.TryGetValue("productId", out var id))
            _productId = id;

        _productName = await ReadService.GetProductName(new Guid(_productId));
        
        List<string> imageIds = await ReadService.GetImagesByProduct(_productId); 
        foreach (string imgId in imageIds)
        {
	        _productImages.Add(new Models.Image
	        {
		        ImageId = imgId
	        });
        }

        List<string> galleryImageIds = await ReadService.GetAllImageUUIDs();
        foreach (string galleryImageId in galleryImageIds)
        {
	        _gallery.Add(new Models.Image()
	        {
		        ImageId = galleryImageId
	        });
        }
    }
    
    private async Task SearchButton()
    {
        
    }
    
    private async Task ProductImageRemove((int oldIndex, int newIndex) indices)
    {
        // get the item at the old index in list 1
        var item = _productImages[indices.oldIndex];

        // add it to the new index in list 2
        _gallery.Insert(indices.newIndex, item);

        await DeleteService.RemoveImageFromProduct(_productId, _productImages[indices.oldIndex].ImageId);
        
        // remove the item from the old index in list 1
        _productImages.Remove(_productImages[indices.oldIndex]);
    }

    private async Task GalleryRemove((int oldIndex, int newIndex) indices)
    {
        // get the item at the old index in list 2
        var item = _gallery[indices.oldIndex];

        // add it to the new index in list 1
        _productImages.Insert(indices.newIndex, item);
        
        await CreateService.AddImageToProduct(_productId, item.ImageId, indices.newIndex.ToString());
       // remove the item from the old index in list 2
        _gallery.Remove(_gallery[indices.oldIndex]);
    }
    
    private async Task ProductImageReorder((int oldIndex, int newIndex) indices)
    {
        // Get the item being moved
        var item = _productImages[indices.oldIndex];
    
        // Remove from old position
        _productImages.RemoveAt(indices.oldIndex);
    
        // Insert at new position
        _productImages.Insert(indices.newIndex, item);

        await UpdateService.UpdatePriority(_productId, item.ImageId, indices.newIndex);
    }

    private void GalleryReorder((int oldIndex, int newIndex) indices)
    {
        // Get the item being moved
        var item = _gallery[indices.oldIndex];
    
        // Remove from old position
        _gallery.RemoveAt(indices.oldIndex);
        
        // Insert at new position
        _gallery.Insert(indices.newIndex, item);
    }
}
