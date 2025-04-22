using DAMApp.Components.Pages;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.VisualBasic;

namespace DAMApp.Services;

public static class ImageService
{
	public static async Task UploadImage(InputFileChangeEventArgs e)
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
		var payload = new Assets.ImageUploadWithoutProduct
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
		var response = await Http.PostAsJsonAsync("api/v1/assets/add", payload);

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

	public static async Task<List<string>> GetImageIds(int size, int pageNumber, string searchText)
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

	public static async Task<List<string>> GetImageSource(int size, int pageNumber, string searchText)
	{
		List<string> imageSources = new List<string>();

		foreach (string id in await GetImageIds(size, pageNumber, searchText))
		{
			imageSources.Add("http://localhost:5115/api/v1/assets/GetImageByUUID?uuid="+id);
		}
		return imageSources;
	}

	public static async Task<List<string>> GetImagesByProduct(string productId)
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
	
	public static async Task<List<string>> GetTags()
	{
		List<string> tags = new List<string>();

		try
		{
			HttpClientHandler handler = new HttpClientHandler();
			HttpClient Http = new HttpClient(handler)
			{
				BaseAddress = new Uri("http://localhost:5115/")
			};

			string apiUrl = "api/v1/assets/tags";
			tags = await Http.GetFromJsonAsync<List<string>>(apiUrl);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error: {ex.Message}");
		}

		return tags;
	}
	
	public static async Task UpdatePriority()
	{
		var payload = new Assets.ImageUploadWithoutProduct
		{
			//Content = dataUrl
		};

		// Make a new HttpClient
		using HttpClientHandler handler = new HttpClientHandler();
		using HttpClient Http = new HttpClient(handler)
		{
			BaseAddress = new Uri("http://localhost:5115/") // Replace with your API's base URL
		};
		// Post to the backend via HTTP
		var response = await Http.PostAsJsonAsync("api/v1/assets/add", payload);

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

	class GetProductAssetsIdsResponse {

		public List<Guid> ImageIds { get; set; }

		public GetProductAssetsIdsResponse(List<Guid> imageIds)
		{
			ImageIds = imageIds;
		}
	}
}