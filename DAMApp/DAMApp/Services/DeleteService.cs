using DAMApp.Models;

namespace DAMApp.Services;

public class DeleteService
{
	public async Task DeleteTag(Tag tag)
	{
		try
		{
			// Make a new HttpClient
			using HttpClientHandler handler = new HttpClientHandler();
			using HttpClient Http = new HttpClient(handler)
			{
				BaseAddress = new Uri("http://localhost:5115/") // Base URL
			};
        
			// Send DELETE request to the backend API
			var response = await Http.DeleteAsync($"api/v1/tag/tagId={tag.UUID}");
        
			if (response.IsSuccessStatusCode)
			{
				Console.WriteLine("Tag removed from image successfully.");
			}
			else
			{
				var error = await response.Content.ReadAsStringAsync();
				Console.WriteLine($"Error: {response.StatusCode} - {error}");
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Exception removing tag from image: {ex.Message}");
		}
	}
	
	public async Task RemoveTagFromImage(string imageId, string tagUUID)
	{
		try
		{
			// Make a new HttpClient
			using HttpClientHandler handler = new HttpClientHandler();
			using HttpClient Http = new HttpClient(handler)
			{
				BaseAddress = new Uri("http://localhost:5115/") // Base URL
			};
        
			// Send DELETE request to the backend API
			var response = await Http.DeleteAsync($"api/v1/tag/{imageId}?tagId={tagUUID}");
        
			if (response.IsSuccessStatusCode)
			{
				Console.WriteLine("Tag removed from image successfully.");
			}
			else
			{
				var error = await response.Content.ReadAsStringAsync();
				Console.WriteLine($"Error: {response.StatusCode} - {error}");
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Exception removing tag from image: {ex.Message}");
		}
	}
	
	public class RemoveProductImageRequest
	{
		public string ImageId { get; set; }
	}
	
	public async Task RemoveImageFromProduct(string productId, string imageId)
	{
		var payload = new RemoveProductImageRequest()
		{
			ImageId = imageId,
		};

		// Make a new HttpClient
		using HttpClientHandler handler = new HttpClientHandler();
		using HttpClient Http = new HttpClient(handler)
		{
			BaseAddress = new Uri("http://localhost:5115/") // Replace with your API's base URL
		};
		// Post to the backend via HTTP
		var response = await Http.PostAsJsonAsync($"api/v1/assets/{productId}/remove", payload);

		if (response.IsSuccessStatusCode) {
			Console.WriteLine("Image removed from product successfully.");
		} 
		else {
			var error = await response.Content.ReadAsStringAsync();
			Console.WriteLine($"Error: {response.StatusCode} - {error}");
		}
	}
}