using Application.Components;
using Application.Entities;
using Application.GraphicsUtils;
using static Application.AppConstants;
using SFML.Graphics;
using SFML.System;

namespace Application.Systems
{
    public class RenderingSystem : ASystem
    {
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
            _renderWindow = WindowManager.Window;
            
            _canvas             = new(CANVAS_WIDTH, CANVAS_HEIGHT);
            _canvasSprite       = new(_canvas.Texture);
            _canvasSprite.Scale = new Vector2f(CANVAS_MULTIPLIER, CANVAS_MULTIPLIER);
        }

        public override void Update() {
            // TODO: Update rendering so only the modified parts get redrawn, not the whole canvas - IF not built-in already
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

            // To avoid redundant calls, just store the reference
            Sprite sprite = graphicsComponent.Sprite;
            
            // Correction for proper sprite reflection
            if (transformComponent.Direction < 0) sfmlPosition.X += sprite.GetLocalBounds().Width;

            sprite.Position = sfmlPosition;
            sprite.Scale    = sfmlDirection;
            
            // Draw the entity on the canvas
            _canvas.Draw(graphicsComponent.Sprite);


            // * Enable HitBox highlighting
            // Create a rectangle for the perimeter
            var outline = new RectangleShape();
            outline.Size = new Vector2f(sprite.GetLocalBounds().Width, sprite.GetLocalBounds().Height);
            outline.Position = new Vector2f(transformComponent.X, transformComponent.Y);
            outline.FillColor = Color.Transparent;  // Transparent background
            outline.OutlineColor = Color.Yellow;    // Outline color
            outline.OutlineThickness = 1;           // Outline thickness
            // Display on the canvas
            _canvas.Draw(outline);
            // * - - -
        }
    }
}