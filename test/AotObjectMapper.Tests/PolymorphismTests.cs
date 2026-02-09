using System.Drawing;
using AotObjectMapper.Abstractions.Exceptions;

namespace AotObjectMapper.Tests;

public partial class PolymorphismTests
{

    public interface IAnimal    { public string Name { get; set; } }
    public interface IAnimalDto { public string Name { get; set; } }

    public class Dog    : IAnimal    { public string Name { get; set; } = String.Empty; }
    public class DogDto : IAnimalDto { public string Name { get; set; } = String.Empty; }

    public class Cat    : IAnimal    { public string Name { get; set; } = String.Empty; }
    public class CatDto : IAnimalDto { public string Name { get; set; } = String.Empty; }

    public class Bird    : IAnimal    { public string Name { get; set; } = String.Empty; }
    public class BirdDto : IAnimalDto { public string Name { get; set; } = String.Empty; }

    public class Wolf    : Dog    { public Color FurColor { get; set; } };
    public class WolfDto : DogDto { public Color FurColor { get; set; } };

    [GenerateMapper]
    [Map<IAnimal, IAnimalDto>]
    [Map<Dog, DogDto>]
    [Map<Cat, CatDto>]
    [UseMap<WolfMapper, Wolf, WolfDto>]
    public partial class AnimalMapper;

    [GenerateMapper]
    [Map<Wolf, WolfDto>]
    public partial class WolfMapper;

    [Fact]
    public void Map_IAnimal_Dog_Dispatches_To_DogDto()
    {
        IAnimal source = new Dog { Name = "Piper" };

        var result = AnimalMapper.Map(source);

        result.ShouldBeOfType<DogDto>();
        result.Name.ShouldBe("Piper");
    }

    [Fact]
    public void Map_IAnimal_Cat_Dispatches_To_CatDto()
    {
        IAnimal source = new Cat { Name = "Mittens" };

        var result = AnimalMapper.Map(source);

        result.ShouldBeOfType<CatDto>();
        result.Name.ShouldBe("Mittens");
    }

    [Fact]
    public void Map_Dog_Wolf_Uses_WolfMapper()
    {
        Dog source = new Wolf { Name = "Moro", FurColor = Color.Gray };

        var result = AnimalMapper.Map(source);

        var wolf = result.ShouldBeOfType<WolfDto>();
        wolf.Name.ShouldBe("Moro");
        wolf.FurColor.ShouldBe(Color.Gray);
    }

    [Fact]
    public void Map_IAnimal_Wolf_Dispatches_To_WolfDto()
    {
        IAnimal source = new Wolf { Name = "Ghost", FurColor = Color.GhostWhite };

        var result = AnimalMapper.Map(source);

        var wolf = result.ShouldBeOfType<WolfDto>();
        wolf.Name.ShouldBe("Ghost");
        wolf.FurColor.ShouldBe(Color.GhostWhite);
    }

    [Fact]
    public void Map_IAnimal_Unhandled_Type_Throws()
    {
        IAnimal source = new Bird() { Name = "Iago" };

        var ex = Should.Throw<UnhandledPolymorphicTypeException>(() => AnimalMapper.Map(source));

        ex.Message.ShouldBe("Could not map type `AotObjectMapper.Tests.PolymorphismTests+Bird` to `AotObjectMapper.Tests.PolymorphismTests.IAnimalDto` - no matching destination type found.");
    }
}