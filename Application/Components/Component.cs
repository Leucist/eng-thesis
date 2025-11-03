using System.Text.Json.Serialization;

namespace Application.Components
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(TransformComponent), "transform")]
    [JsonDerivedType(typeof(PhysicsComponent), "physics")]
    [JsonDerivedType(typeof(GraphicsComponent), "graphics")]
    public abstract class Component(ComponentType type)
    {
        public ComponentType Type { get; } = type;
    }
}