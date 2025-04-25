using System.Text.Json.Nodes;
using DAMApp.Components.Pages;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.VisualBasic;
using DAMApp.Models;

namespace DAMApp.Services;

public class ImageService
{
	public async Task UploadImage(InputFileChangeEventArgs e)
	{
		// Select a file 
		IBrowserFile file = e.File;

		// Finding the datatype of the file
		var datatype = file.ContentType;
		using var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024); // 10MB limit
		using var ms = new MemoryStream();
		await stream.CopyToAsync(ms);
		var bytes = ms.ToArray();
		string imageString = Convert.ToBase64String(bytes);
		string dataUrl = $"data:{datatype};base64,{imageString}";
		//Console.WriteLine(dataUrl);

		// Make payload for uploading an image to the backend
		var payload = new ImageUpload()
		{
			Content = dataUrl
		};

		// Make a new HttpClient
		using HttpClientHandler handler = new HttpClientHandler();
		using HttpClient Http = new HttpClient(handler)
		{
			BaseAddress = new Uri("http://localhost:5115/") // Replace with your API's base URL
		};
		// Post to the backend via HTTP
		var response = await Http.PostAsJsonAsync("api/v1/assets", payload);

		if (response.IsSuccessStatusCode)
		{
			Console.WriteLine("Image uploaded successfully.");
		}
		else
		{
			var error = await response.Content.ReadAsStringAsync();
			Console.WriteLine($"Error: {response.StatusCode} - {error}");
		}

	}

	public async Task<List<string>> GetImageIds(int size, int pageNumber, string searchText)
	{
		List<string> imageIds = new List<string>();

		try
		{
			HttpClientHandler handler = new HttpClientHandler();
			HttpClient Http = new HttpClient(handler)
			{
				BaseAddress = new Uri("http://localhost:5115/") // Replace with your API's base URL
			};

			string apiUrl = $"api/v1/assets/imageIdPile?size={size}&offset={pageNumber}";

			if (!string.IsNullOrEmpty(searchText))
			{
				apiUrl =
					$"api/v1/assets/imageIdPileFromSearch?size={size}&offset={pageNumber}&searchquery={searchText}";
			}
			
			imageIds = await Http.GetFromJsonAsync<List<string>>(apiUrl);

		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error: {ex.Message}");
		}

		foreach (string uuid in imageIds)
		{
			Console.WriteLine(uuid);
		}

		return imageIds;
	}

	public async Task<List<string>> GetImageSource(int size, int pageNumber, string searchText)
	{
		List<string> imageSources = new List<string>();

		foreach (string id in await GetImageIds(size, pageNumber, searchText))
		{
			imageSources.Add("http://localhost:5115/api/v1/assets/GetImageByUUID?uuid="+id);
		}
		return imageSources;
	}
	
	public async Task<string> GetImageSourceById(string id)
	{
		string imageSources = imageSources = ("http://localhost:5115/api/v1/assets/GetImageByUUID?uuid="+id);
		return imageSources;
	}

	public async Task<List<string>> GetImagesByProduct(string productId)
	{
		List<string> ImagesByProduct = new List<string>();

		try
		{
			HttpClientHandler handler = new HttpClientHandler();
			HttpClient Http = new HttpClient(handler)
			{
				BaseAddress = new Uri("http://localhost:5115/") // Replace with your API's base URL
			};

			string apiUrl = $"api/v1/assets/{productId}/all";

			GetProductAssetsIdsResponse response = await Http.GetFromJsonAsync<GetProductAssetsIdsResponse>(apiUrl);
			foreach (Guid guid in response.ImageIds)
			{
				ImagesByProduct.Add(guid.ToString());
			}

		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error: {ex.Message}");
		}
		
		return ImagesByProduct;
	}

	public async Task<List<string>> GetAllImageUUIDs ()
	{
		List<string> uuids = [];

		try
		{
			HttpClientHandler handler = new HttpClientHandler();
			HttpClient Http = new HttpClient(handler)
			{
				BaseAddress = new Uri("http://localhost:5115/")
			};

			string apiUrl = "api/v1/assets/get-all";
			var guids = await Http.GetFromJsonAsync<List<Guid>>(apiUrl);

			foreach (var guid in guids)
			{
				uuids.Add(guid.ToString());
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error: {ex.Message}");
		}

		return uuids;
	}
	
	/// <summary>
	/// Returns all tags in database
	/// </summary>
	/// <returns></returns>
	public async Task<List<Tag>> GetTags()
	{
		List<Tag> tags = new List<Tag>();

		try
		{
			HttpClientHandler handler = new HttpClientHandler();
			HttpClient Http = new HttpClient(handler)
			{
				BaseAddress = new Uri("http://localhost:5115/")
			};

			string apiUrl = "api/v1/assets/tags";
			tags = await Http.GetFromJsonAsync<List<Tag>>(apiUrl);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error: {ex.Message}");
		}

		return tags;
	}

	
	/// <summary>
	/// returns images by their tags
	/// </summary>
	/// <param name="selectedTags"></param>
	/// <returns></returns>
	public static async Task<List<string>> GetImagesByTags(List<string> selectedTags)
	{
		List<string> imageSources = new List<string>();

		try
		{
			HttpClientHandler handler = new HttpClientHandler();
			HttpClient Http = new HttpClient(handler)
			{
				BaseAddress = new Uri("http://localhost:5115/") 
			};

			string tagQuery = string.Join(",", selectedTags);
			string apiUrl = $"api/v1/assets/byTags?tags={tagQuery}";

			List<string> imageIds = await Http.GetFromJsonAsync<List<string>>(apiUrl);

			foreach (string id in imageIds)
			{
				imageSources.Add($"http://localhost:5115/api/v1/assets/GetImageByUUID?uuid={id}");
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error loading images by tags: {ex.Message}");
		}

		return imageSources;
	}
	

	class UpdateImageRequest
	{
		public string op { get; set; }
		public string path { get; set; }
		public int priority { get; set; }
		
	}
	
	public async Task UpdatePriority(string productId, string imageId, int newPriority)
	{
		var patchDoc = new[]
		{
			new
			{
				op = "replace",
				path = "/priority",
				value = newPriority
			}
		};

		// Make a new HttpClient
		using HttpClientHandler handler = new HttpClientHandler();
		using HttpClient Http = new HttpClient(handler)
		{
			BaseAddress = new Uri("http://localhost:5115/") // Replace with your API's base URL
		};
		// Post to the backend via HTTP
		var response = await Http.PatchAsJsonAsync($"api/v1/assets/{productId}/{imageId}", patchDoc);

		if (response.IsSuccessStatusCode) {
			Console.WriteLine("Image uploaded successfully.");
		} 
		else {
			var error = await response.Content.ReadAsStringAsync();
			Console.WriteLine($"Error: {response.StatusCode} - {error}");
		}
	}

	class GetProductAssetsIdsResponse {

		public List<Guid> ImageIds { get; set; }

		public GetProductAssetsIdsResponse(List<Guid> imageIds)
		{
			ImageIds = imageIds;
		}
	}
	
	public class AddProductImageRequest
	{
		public string ImageId { get; set; }
		public string Priority { get; set; }
	}
	
	public async Task AddImageToProduct(string productId, string imageId, string newPriority)
	{
		var payload = new AddProductImageRequest()
		{
			ImageId = imageId,
			Priority = newPriority
		};

		// Make a new HttpClient
		using HttpClientHandler handler = new HttpClientHandler();
		using HttpClient Http = new HttpClient(handler)
		{
			BaseAddress = new Uri("http://localhost:5115/") // Replace with your API's base URL
		};
		// Post to the backend via HTTP
		var response = await Http.PostAsJsonAsync($"api/v1/assets/{productId}/add", payload);

		if (response.IsSuccessStatusCode) {
			Console.WriteLine("Image uploaded to product successfully.");
		} 
		else {
			var error = await response.Content.ReadAsStringAsync();
			Console.WriteLine($"Error: {response.StatusCode} - {error}");
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
	
	public class GetProductResponse {
		public string Name { get; set; }
		public Guid UUID { get; set; }
	}

	public async Task<string> GetProductName(Guid productUUID)
	{
		try
		{
			string apiUrl = $"api/v1/assets/getProduct?productId={productUUID}";
        
			HttpClientHandler handler = new HttpClientHandler();
			HttpClient Http = new HttpClient(handler)
			{
				BaseAddress = new Uri("http://localhost:5115/")
			};
			
			var response = await Http.GetFromJsonAsync<GetProductResponse>(apiUrl);
        
			if (response != null)
			{
				return response.Name;
			}
        
			return "name not found";
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error in GetProductName: {ex.GetType().Name}: {ex.Message}");
			return "name not found";
		}
	}

	public async Task<List<Tag>> GetTagsByImage (Guid imageUUID)
	{
		try
		{
			string apiUrl = $"api/v1/tag?imageId={imageUUID}";
        
			HttpClientHandler handler = new HttpClientHandler();
			HttpClient Http = new HttpClient(handler)
			{
				BaseAddress = new Uri("http://localhost:5115/")
			};
			
			var response = await Http.GetFromJsonAsync<List<Tag>>(apiUrl);
        
			if (response != null)
			{
				return response;
			}
        
			return [];
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error in GetTagsByImage: {ex.GetType().Name}: {ex.Message}");
			return [];
		}
	}
	
	public async Task<List<Tag>> GetTagsNotOnImage (Guid imageUUID)
	{
		try
		{
			string apiUrl = $"api/v1/tag/exclude?imageId={imageUUID}";
        
			HttpClientHandler handler = new HttpClientHandler();
			HttpClient Http = new HttpClient(handler)
			{
				BaseAddress = new Uri("http://localhost:5115/")
			};
			
			var response = await Http.GetFromJsonAsync<List<Tag>>(apiUrl);
        
			if (response != null)
			{
				return response;
			}
        
			return [];
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error in GetTagsNotOnImage: {ex.GetType().Name}: {ex.Message}");
			return [];
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

	public async Task AddTagToImage(string imageId, Tag tag)
	{
		var payload = tag;

		// Make a new HttpClient
		using HttpClientHandler handler = new HttpClientHandler();
		using HttpClient Http = new HttpClient(handler)
		{
			BaseAddress = new Uri("http://localhost:5115/") // Replace with your API's base URL
		};
		// Post to the backend via HTTP
		var response = await Http.PostAsJsonAsync($"api/v1/tag/{imageId}", payload);

		if (response.IsSuccessStatusCode) {
			Console.WriteLine("Tag uploaded to image successfully.");
		} 
		else {
			var error = await response.Content.ReadAsStringAsync();
			Console.WriteLine($"Error: {response.StatusCode} - {error}");
		}
	}

	public async Task UploadTag(Tag tag)
	{
		var payload = tag;
		
		// Make a new HttpClient
		using HttpClientHandler handler = new HttpClientHandler();
		using HttpClient Http = new HttpClient(handler)
		{
			BaseAddress = new Uri("http://localhost:5115/") // Replace with your API's base URL
		};
		// Post to the backend via HTTP
		var response = await Http.PostAsJsonAsync($"api/v1/tag", payload);
		
		if (response.IsSuccessStatusCode)
		{
			Console.WriteLine("Image uploaded successfully.");
		}
		else
		{
			var error = await response.Content.ReadAsStringAsync();
			Console.WriteLine($"Error: {response.StatusCode} - {error}");
		}
	}

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
}