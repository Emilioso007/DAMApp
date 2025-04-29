namespace DAMApp.Models;

public class Product : IHideable
{
	public string Name { get; set; }
	public string Url { get; set; }
	public string ProductId { get; set; }
	public bool IsShown { get; set; } = true;
}