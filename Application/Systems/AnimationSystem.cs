using Application.Components;
using Application.Entities;

namespace Application.Systems
{
    public class AnimationSystem(EntityManager entityManager) : System(entityManager, [ComponentType.Animation])
    {
        protected override void PerformSystemAction(Dictionary<ComponentType, Component> entity) {
            ((AnimationComponent) entity[ComponentType.Animation]).Next();
        }
    }
}