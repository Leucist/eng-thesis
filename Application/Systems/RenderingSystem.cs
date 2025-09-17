using Application.Components;
using Application.Entities;

namespace Application.Systems
{
    public class RenderingSystem(EntityManager entityManager)
        : System(
            entityManager,
            [ComponentType.Graphics]
        )
    {
        public override void Update() {
            var componentBundles = GatherComponents();
        }
    }
}