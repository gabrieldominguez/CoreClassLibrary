using CoreClassLibrary.Objects.DLR;
using Xunit;
using System.ComponentModel.DataAnnotations;

namespace CoreClassLibrary.Tests;

public class Entity
    {
    [Key]
    public Guid Id { get; set; }
    public string? Desc { get; set; }
    }

public class DeltaTests()
    {
    [Fact]
    public void Delta()
        {
        Entity entity = new Entity { Id = Guid.NewGuid(), Desc = "A" };
        dynamic delta1 = new Delta<Entity>();
        delta1.Desc = "B";

        dynamic delta2 = new Delta<Entity>();
        delta2.Desc = "C";

        delta1.Patch(entity);
        Assert.Equal(delta1.Desc, entity.Desc);

        delta2.Patch(entity);
        Assert.Equal(delta2.Desc, entity.Desc);
        }

    // DELTA NO PUEDE SER SERIALIZADO
    /*
    [Fact]
    public void CantSerializeDelta()
        {
        dynamic delta = new Delta<Entity>();
        delta.Desc = "B";

        JsonSerializationService serializer = new JsonSerializationService();

        Assert.Throws<NotSupportedException>(() => serializer.Serialize<Delta<Entity>>((Delta<Entity>)delta));
        }
    */
    }
