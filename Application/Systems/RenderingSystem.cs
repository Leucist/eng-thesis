using Application.Components;
using Application.Entities;
using SFML.Graphics;
using SFML.Window;

namespace Application.Systems
{
    public class RenderingSystem : System
    {
        // TODO (?) - May be moved to some app constants class or json
        private const string TITLE  = "APPLICATION";
        private const int WIDTH     = 1920;
        private const int HEIGHT    = 1080;

        private RenderWindow _renderWindow;

        public RenderingSystem(EntityManager entityManager)
            : base(
                entityManager,
                [
                    ComponentType.Graphics,
                    ComponentType.Transform,
                ]
            ) 
        {
            VideoMode videoMode = new(WIDTH, HEIGHT);
            _renderWindow = new(videoMode, TITLE);
        }

        public override void Update() {
            // * Some general actions related to the game window *
            throw new NotImplementedException(); // - a gentle reminder

            // Calling the base Update method to invoke PerformSystemAction
            base.Update();
        }

        protected override void PerformSystemAction(Dictionary<ComponentType, Component> entityComponents) {
            var transformComponent  = (TransformComponent)  entityComponents[ComponentType.Transform];
            var graphicsComponent   = (GraphicsComponent)   entityComponents[ComponentType.Graphics];

            throw new NotImplementedException();
        }
    }
}