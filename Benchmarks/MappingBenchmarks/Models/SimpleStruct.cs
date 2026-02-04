using System.Drawing;

namespace MappingBenchmarks.Models;

public struct SimpleStruct
{
    public int      Id                { get; set; }
    public string   Name              { get; set; }
    public int      Age               { get; set; }
    public DateTime CreatedAt         { get; set; }
    public decimal  Amount            { get; set; }
    public string   FavouriteIceCream { get; set; }
    public Color    FavouriteColor    { get; set; }
}

public struct SimpleStructDto
{
    public int      Id                { get; set; }
    public string   Name              { get; set; }
    public int      Age               { get; set; }
    public DateTime CreatedAt         { get; set; }
    public decimal  Amount            { get; set; }
    public string   FavouriteIceCream { get; set; }
    public Color    FavouriteColor    { get; set; }
}