namespace LevelEditor.Core
{
    /// <summary>
    /// Represents the current tool/mode in the level editor
    /// </summary>
    public enum EditorTool
    {
        Select,    // Select and manipulate entities
        PlaceTile, // Place tile prefabs
        PlaceCharacter, // Place character prefabs
        PlaceBackground, // Place background prefabs
        Delete     // Delete entities
    }

    /// <summary>
    /// Represents which prefab category is currently displayed
    /// </summary>
    public enum PrefabCategory
    {
        Tiles,
        Characters
    }

    /// <summary>
    /// Manages the current state of the editor (tool, selection, etc.)
    /// </summary>
    public class EditorState
    {
        public EditorTool CurrentTool { get; set; } = EditorTool.Select;
        public PrefabCategory CurrentCategory { get; set; } = PrefabCategory.Tiles;
        
        // Currently selected entity on the grid (for editing/deleting)
        public PlacedEntity? SelectedEntity { get; set; } = null;
        
        // Currently selected prefab from the panel (for placing)
        public string? SelectedPrefabName { get; set; } = null;
        public string? SelectedBackgroundImage { get; set; } = null;
        
        // Dragging state
        public bool IsDragging { get; set; } = false;
        public PlacedEntity? DraggedEntity { get; set; } = null;

        public void ClearSelection()
        {
            SelectedEntity = null;
            SelectedPrefabName = null;
            SelectedBackgroundImage = null;
        }

        public void SetToolForCategory(PrefabCategory category)
        {
            CurrentCategory = category;
            CurrentTool = category switch
            {
                PrefabCategory.Tiles => EditorTool.PlaceTile,
                PrefabCategory.Characters => EditorTool.PlaceCharacter,
                // PrefabCategory.Backgrounds => EditorTool.PlaceBackground,
                _ => EditorTool.Select
            };
        }
    }
}