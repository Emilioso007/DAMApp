namespace DAMApp.Services;
using DAMApp.Models;

public class ReadService
{
	public async Task<List<string>> GetImageSource(int size, int pageNumber, string searchText)
	{
		List<string> imageSources = new List<string>();

		foreach (string id in await GetImageIds(size, pageNumber, searchText))
		{
			imageSources.Add("http://localhost:5115/api/v1/assets/GetImageByUUID?uuid="+id);
		}
		return imageSources;
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
	
	public async Task<string> GetImageSourceById(string id)
	{
		return ("http://localhost:5115/api/v1/assets/GetImageByUUID?uuid="+id);
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
	
	class GetProductAssetsIdsResponse {
		
		public List<Guid> ImageIds { get; set; }

		public GetProductAssetsIdsResponse(List<Guid> imageIds)
		{
			ImageIds = imageIds;
		}
	}
	
}