using SFML.Graphics;

namespace LevelEditor.Utils
{
    /// <summary>
    /// Constants for the level editor UI and grid configuration
    /// </summary>
    public static class EditorConstants
    {
        // Window settings
        public const uint WINDOW_WIDTH = 1600;
        public const uint WINDOW_HEIGHT = 900;
        public const string WINDOW_TITLE = "Level Editor";

        // Grid settings
        public const int TILE_SIZE = 32; // pixels
        public const int MAX_VISIBLE_GRID_WIDTH = 10; // tiles (before horizontal scrolling)
        public const int MAX_LEVEL_HEIGHT_TILES = 33; // ~1056px at 32px tiles (fits 1080p)
        
        // UI Layout
        public const float PREFAB_PANEL_WIDTH = 200f;
        public const float TOOLBAR_HEIGHT = 80f;
        public const float GRID_PADDING = 10f;

        // Colors
        public static readonly Color GRID_LINE_COLOR = new Color(100, 100, 100, 128);
        public static readonly Color GRID_BACKGROUND = new Color(40, 40, 45);
        public static readonly Color SELECTED_ENTITY_OUTLINE = Color.Yellow;
        public static readonly Color PREFAB_PANEL_BG = new Color(30, 30, 35);
        public static readonly Color TOOLBAR_BG = new Color(35, 35, 40);
        public static readonly Color BUTTON_BG = new Color(60, 60, 65);
        public static readonly Color BUTTON_HOVER = new Color(80, 80, 85);
        public static readonly Color BUTTON_ACTIVE = new Color(100, 150, 200);

        // Interaction
        public const float SCROLL_SPEED = 20f;
        public const float PREFAB_ICON_SIZE = 80f;
        public const float PREFAB_PADDING = 10f;
    }
}