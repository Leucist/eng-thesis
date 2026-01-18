using Application.Components;

namespace LevelEditor.Prefabs
{
    /// <summary>
    /// Represents a prefab template loaded from JSON.
    /// Structure matches the JSON format: { "Name": "...", "PreviewTexture": "...", "Components": [...] }
    /// </summary>
    public class PrefabDefinition
    {
        public string Name { get; set; } = "";
        public string PreviewTexture { get; set; } = ""; // Path to texture for preview in panel
        public List<Component> Components { get; set; } = new();

        /// <summary>
        /// Creates a deep copy of this prefab's components for placing on the grid
        /// </summary>
        public List<Component> CloneComponents()
        {
            // For now a simple copy is used 
            // If the deep cloning is ever needed to clone the component internal state etc., implement ICloneable on components
            var cloned = new List<Component>();
            
            foreach (var component in Components)
            {
                // Create new instances based on type
                if (component is TransformComponent tc)
                {
                    cloned.Add(new TransformComponent(tc.X, tc.Y, tc.Width, tc.Height));
                }
                else if (component is GraphicsComponent gc)
                {
                    var sprite = gc.GetSprite();
                    var texture = sprite.Texture;
                    PreviewTexture = gc.TexturePath;
                    cloned.Add(new GraphicsComponent(PreviewTexture));
                }
                // TODO: Add other component types as needed (CollisionComponent, etc.)
                else
                {
                    // Generic fallback - just reference the same component
                    // This may need refinement based on your component types
                    cloned.Add(component);
                }
            }

            return cloned;
        }
    }
}