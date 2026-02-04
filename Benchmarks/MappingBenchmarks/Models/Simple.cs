using System.Drawing;

namespace MappingBenchmarks.Models;

public class Simple
{
    public int      Id                { get; set; }
    public string   Name              { get; set; } = string.Empty;
    public int      Age               { get; set; }
    public DateTime CreatedAt         { get; set; }
    public decimal  Amount            { get; set; }
    public string   FavouriteIceCream { get; set; } = string.Empty;
    public Color    FavouriteColor    { get; set; }
}

public class SimpleDto
{
    public int      Id                { get; set; }
    public string   Name              { get; set; } = string.Empty;
    public int      Age               { get; set; }
    public DateTime CreatedAt         { get; set; }
    public decimal  Amount            { get; set; }
    public string   FavouriteIceCream { get; set; } = string.Empty;
    public Color    FavouriteColor    { get; set; }
}