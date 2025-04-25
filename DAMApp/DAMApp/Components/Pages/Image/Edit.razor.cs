using DAMApp.Services;
using Microsoft.AspNetCore.Components;

namespace DAMApp.Components.Pages.Image;

public partial class Edit : ComponentBase
{

	[Inject] private NavigationManager Navigation { get; set; }
	
	private string _imageId = "";
    private string _searchText = "";
    
    private List<Models.Tag> _imageTags = [];
    private List<Models.Tag> _list = [];
        
    protected override async Task OnInitializedAsync ()
    {
        var uri = new Uri(Navigation.Uri);
        var queryParams = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);

        if (queryParams.TryGetValue("imageId", out var id))
            _imageId = id;

        _imageTags = await ReadService.GetTagsByImage(new Guid(_imageId));

        _list = await ReadService.GetTagsNotOnImage(new Guid(_imageId));
        
    }
    
    private async Task SearchButton()
    {
        
    }
    
    private async Task ImageTagsRemove((int oldIndex, int newIndex) indices)
    {
        // get the item at the old index in list 1
        var item = _imageTags[indices.oldIndex];

        // add it to the new index in list 2
        _list.Insert(indices.newIndex, item);

        await DeleteService.RemoveTagFromImage(_imageId, _imageTags[indices.oldIndex].UUID.ToString());
        
        // remove the item from the old index in list 1
        _imageTags.Remove(_imageTags[indices.oldIndex]);
    }

    private async Task ListRemove((int oldIndex, int newIndex) indices)
    {
        // get the item at the old index in list 2
        var item = _list[indices.oldIndex];

        // add it to the new index in list 1
        _imageTags.Insert(indices.newIndex, item);
        
        await CreateService.AddTagToImage(_imageId, item);
       // remove the item from the old index in list 2
       _list.Remove(_list[indices.oldIndex]);
    }
    
    private async Task ImageTagsReorder((int oldIndex, int newIndex) indices)
    {
        // Get the item being moved
        var item = _imageTags[indices.oldIndex];
    
        // Remove from old position
        _imageTags.RemoveAt(indices.oldIndex);
    
        // Insert at new position
        _imageTags.Insert(indices.newIndex, item);
    }

    private void ListReorder((int oldIndex, int newIndex) indices)
    {
        // Get the item being moved
        var item = _list[indices.oldIndex];
    
        // Remove from old position
        _list.RemoveAt(indices.oldIndex);
        
        // Insert at new position
        _list.Insert(indices.newIndex, item);
    }
}
