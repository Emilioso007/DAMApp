using System.ComponentModel.DataAnnotations;

namespace DAMApp.Models;

public class Product
{
    [Key] public Guid UUID { get; set; }
    public string Name { get; set; }

}