namespace DAMApp.Models;

public class Image
{
	public string ImageId { get; set; }
	
	public string Url => "http://localhost:5115/api/v1/assets/GetImageByUUID?uuid="+ImageId;
}