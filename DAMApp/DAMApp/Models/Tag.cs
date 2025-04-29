namespace DAMApp.Models;

public class Tag : IHideable
{
    public Guid UUID { get; set; }
    public string Name { get; set; }
    public bool isAddedByUser { get; set; }
    public bool IsShown { get; set; } = true;
}

