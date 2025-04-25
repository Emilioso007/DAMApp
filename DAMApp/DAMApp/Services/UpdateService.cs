namespace DAMApp.Services;

public class UpdateService
{
	
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
	
}