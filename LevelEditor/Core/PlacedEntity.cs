using Application.Components;
using SFML.Graphics;
using SFML.System;

namespace LevelEditor.Core
{
    /// <summary>
    /// Represents an entity placed on the editor grid with its components and visual representation
    /// </summary>
    public class PlacedEntity
    {
        public string PrefabName { get; set; }
        public List<Component> Components { get; set; }
        public int GridX { get; set; } // Position in grid coordinates
        public int GridY { get; set; }
        
        // Visual representation
        public Sprite? Sprite { get; set; }
        public RectangleShape SelectionOutline { get; set; }
        public bool IsSelected { get; set; } = false;

        public PlacedEntity(string prefabName, List<Component> components, int gridX, int gridY)
        {
            PrefabName = prefabName;
            Components = components;
            GridX = gridX;
            GridY = gridY;

            // Create selection outline (will be positioned later)
            SelectionOutline = new RectangleShape
            {
                FillColor = Color.Transparent,
                OutlineColor = Utils.EditorConstants.SELECTED_ENTITY_OUTLINE,
                OutlineThickness = 2f
            };

            InitializeVisual();
        }

        private void InitializeVisual()
        {
            // Try to get GraphicsComponent for visual representation
            var graphicsComp = Components.OfType<GraphicsComponent>().FirstOrDefault();
            if (graphicsComp != null)
            {
                try
                {
                    Sprite = graphicsComp.GetSprite();
                }
                catch
                {
                    // If texture loading fails, we'll use a placeholder
                    Sprite = null;
                }
            }

            // Get dimensions from TransformComponent
            var transform = Components.OfType<TransformComponent>().FirstOrDefault();
            if (transform != null)
            {
                SelectionOutline.Size = new Vector2f(transform.Width, transform.Height);
            }
        }

        public void UpdatePosition(int gridX, int gridY, float pixelX, float pixelY)
        {
            GridX = gridX;
            GridY = gridY;

            // Update TransformComponent
            var transform = Components.OfType<TransformComponent>().FirstOrDefault();
            if (transform != null)
            {
                // Use reflection to update private fields (since Move() is relative)
                var xField = typeof(TransformComponent).GetField("_x", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var yField = typeof(TransformComponent).GetField("_y", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                xField?.SetValue(transform, pixelX);
                yField?.SetValue(transform, pixelY);
            }

            // Update visual positions
            if (Sprite != null)
            {
                Sprite.Position = new Vector2f(pixelX, pixelY);
            }
            SelectionOutline.Position = new Vector2f(pixelX, pixelY);
        }

        public void SetSelected(bool selected)
        {
            IsSelected = selected;
        }

        public void Draw(RenderWindow window)
        {
            if (Sprite != null)
            {
                window.Draw(Sprite);
            }
            else
            {
                // Draw placeholder rectangle if no sprite
                var placeholder = new RectangleShape(SelectionOutline.Size)
                {
                    Position = SelectionOutline.Position,
                    FillColor = new Color(150, 150, 150, 100),
                    OutlineColor = Color.White,
                    OutlineThickness = 1f
                };
                window.Draw(placeholder);
            }

            if (IsSelected)
            {
                window.Draw(SelectionOutline);
            }
        }
    }
}