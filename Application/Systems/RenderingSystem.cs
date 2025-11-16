using Application.Components;
using Application.Entities;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Application.Systems
{
    public class RenderingSystem : ASystem
    {
        // TODO (?) - May be moved to some app constants class or json
        private const string TITLE = "APPLICATION";
        private const int WIDTH             = 1920;
        private const int HEIGHT            = 1080;
        private const int CANVAS_WIDTH      = 640;
        private const int CANVAS_HEIGHT     = 352;
        private const int CANVAS_MULTIPLIER = 3;

        private readonly RenderWindow _renderWindow;
        private readonly RenderTexture _canvas;
        private readonly Sprite _canvasSprite;

        // private readonly GraphicsCache _graphicsCache;

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
            _renderWindow       = new(videoMode, TITLE);
            _canvas             = new(CANVAS_WIDTH, CANVAS_HEIGHT);
            _canvasSprite       = new(_canvas.Texture);
            _canvasSprite.Scale = new Vector2f(CANVAS_MULTIPLIER, CANVAS_MULTIPLIER);

            // _graphicsCache = new();
            _renderWindow.Closed += OnWindowClosed;
        }

        private void OnWindowClosed(object? sender, EventArgs e)
        {
            // Close the window and exit when the close button is clicked
            _renderWindow.Close();
            Environment.Exit(0);
        }

        public override void Update() {
            // TODO: Update rendering so only the modified parts get redrawn, not the whole canvas - IF not built-in already
            _renderWindow.DispatchEvents();
            // Clearing the canvas before redrawing entities
            _canvas.Clear();

            // Calling the base Update method to invoke PerformSystemAction
            base.Update();

            // Eventually displaying the updated content
            _canvas.Display();
            _renderWindow.Clear();
            _renderWindow.Draw(_canvasSprite);
            _renderWindow.Display();
        }

        protected override void PerformSystemAction(Dictionary<ComponentType, Component> entityComponents) {
            var transformComponent  = (TransformComponent)  entityComponents[ComponentType.Transform];
            var graphicsComponent   = (GraphicsComponent)   entityComponents[ComponentType.Graphics];

            // Update the GC position and orientation according to the TC
            Vector2f sfmlPosition   = new(transformComponent.X, transformComponent.Y);
            Vector2f sfmlDirection  = new(transformComponent.Direction, 1); // Direction states whether the sprite has to be reflected
            
            graphicsComponent.Sprite.Position   = sfmlPosition;
            graphicsComponent.Sprite.Scale      = sfmlDirection;
            
            // Draw the entity on the canvas
            _canvas.Draw(graphicsComponent.Sprite);
        }
    }
}