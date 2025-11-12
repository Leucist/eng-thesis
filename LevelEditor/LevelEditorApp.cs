using SFML.Graphics;
using SFML.System;
using SFML.Window;
using LevelEditor.Core;
using LevelEditor.Prefabs;
using LevelEditor.UI;
using LevelEditor.Utils;

namespace LevelEditor
{
    /// <summary>
    /// Main application class for the SFML-based level editor
    /// </summary>
    public class LevelEditorApp
    {
        private RenderWindow _window;
        private Font _font;

        // Core components
        private EditorState _state;
        private EditorLevel _level;
        private PrefabLibrary _prefabLibrary;

        // UI components
        private EditorGrid _grid;
        private PrefabPanel _prefabPanel;
        private Toolbar _toolbar;

        public LevelEditorApp()
        {
            // Initialize window
            _window = new RenderWindow(
                new VideoMode(EditorConstants.WINDOW_WIDTH, EditorConstants.WINDOW_HEIGHT),
                EditorConstants.WINDOW_TITLE
            );
            _window.SetFramerateLimit(60);
            _window.Closed += OnClosed;
            _window.MouseButtonPressed += OnMouseButtonPressed;
            _window.MouseMoved += OnMouseMoved;
            _window.MouseWheelScrolled += OnMouseWheelScrolled;

            _font = LoadFont();

            // Initialize core components
            _state = new EditorState();
            _prefabLibrary = new PrefabLibrary();
            _level = new EditorLevel(10, 10); // Default 10x10 level

            // Calculate UI layout
            float gridWidth = EditorConstants.WINDOW_WIDTH - EditorConstants.PREFAB_PANEL_WIDTH;
            float gridHeight = EditorConstants.WINDOW_HEIGHT - EditorConstants.TOOLBAR_HEIGHT;

            // Initialize UI components
            _grid = new EditorGrid(
                _level,
                _state,
                _prefabLibrary,
                new Vector2f(0, 0),
                new Vector2f(gridWidth, gridHeight)
            );

            _prefabPanel = new PrefabPanel(
                _state,
                _prefabLibrary,
                new Vector2f(gridWidth, 0),
                new Vector2f(EditorConstants.PREFAB_PANEL_WIDTH, gridHeight),
                _font
            );

            _toolbar = new Toolbar(
                _state,
                new Vector2f(0, gridHeight),
                new Vector2f(EditorConstants.WINDOW_WIDTH, EditorConstants.TOOLBAR_HEIGHT),
                _font
            );

            // Wire up toolbar actions
            _toolbar.OnNewLevel = ShowNewLevelDialog;
            _toolbar.OnOpenLevel = ShowOpenLevelDialog;
            _toolbar.OnSaveLevel = ShowSaveLevelDialog;
            _toolbar.OnSwitchToTiles = () => _prefabPanel.RefreshPrefabs();
            _toolbar.OnSwitchToCharacters = () => _prefabPanel.RefreshPrefabs();
            _toolbar.OnSwitchToBackgrounds = () => _prefabPanel.SwitchToBackgroundMode();
            _toolbar.OnDeleteMode = () => { }; // Just switches tool

            Console.WriteLine("Level Editor initialized successfully!");
        }

        private Font LoadFont()
        {
            // Downloaded Montserrat medium here
            string fontPath = Path.Combine(Application.Pathfinder.GetSolutionDirectory(), "LevelEditor", "font.ttf");
            return new Font(fontPath);
        }

        public void Run()
        {
            while (_window.IsOpen)
            {
                _window.DispatchEvents();
                Update();
                Render();
            }
        }

        private void Update()
        {
            _toolbar.Update();
        }

        private void Render()
        {
            _window.Clear(new Color(25, 25, 30));

            _grid.Draw(_window);
            _prefabPanel.Draw(_window);
            _toolbar.Draw(_window);

            _window.Display();
        }

        private void OnClosed(object? sender, EventArgs e)
        {
            _window.Close();
        }

        private void OnMouseButtonPressed(object? sender, MouseButtonEventArgs e)
        {
            if (e.Button != Mouse.Button.Left) return;

            Vector2i mousePos = new Vector2i(e.X, e.Y);

            // Check toolbar first
            float gridHeight = EditorConstants.WINDOW_HEIGHT - EditorConstants.TOOLBAR_HEIGHT;
            if (e.Y > gridHeight)
            {
                _toolbar.HandleMouseClick(mousePos);
                return;
            }

            // Check prefab panel
            float gridWidth = EditorConstants.WINDOW_WIDTH - EditorConstants.PREFAB_PANEL_WIDTH;
            if (e.X > gridWidth)
            {
                _prefabPanel.HandleMouseClick(mousePos);
                
                // Check if background was selected
                if (_state.SelectedBackgroundImage != null)
                {
                    _grid.SetLevelBackground(_state.SelectedBackgroundImage);
                    _state.SelectedBackgroundImage = null; // Reset after placement
                }
                return;
            }

            // Handle grid click
            _grid.HandleMouseClick(mousePos);
        }

        private void OnMouseMoved(object? sender, MouseMoveEventArgs e)
        {
            Vector2i mousePos = new Vector2i(e.X, e.Y);
            _toolbar.HandleMouseMove(mousePos);
            _prefabPanel.HandleMouseMove(mousePos);
        }

        private void OnMouseWheelScrolled(object? sender, MouseWheelScrollEventArgs e)
        {
            Vector2i mousePos = Mouse.GetPosition(_window);
            float gridWidth = EditorConstants.WINDOW_WIDTH - EditorConstants.PREFAB_PANEL_WIDTH;

            if (mousePos.X > gridWidth)
            {
                // Scroll prefab panel
                _prefabPanel.HandleScroll(e.Delta);
            }
            else
            {
                // Scroll grid horizontally
                _grid.HandleScroll(e.Delta);
            }
        }

        private void ShowNewLevelDialog()
        {
            // TODO: Implement a dialog to get width/height
            // For now, using console input as a placeholder
            Console.WriteLine("Enter level width (in tiles):");
            if (int.TryParse(Console.ReadLine(), out int width))
            {
                Console.WriteLine("Enter level height (in tiles):");
                if (int.TryParse(Console.ReadLine(), out int height))
                {
                    CreateNewLevel(width, height);
                }
            }
        }

        private void CreateNewLevel(int width, int height)
        {
            _level = new EditorLevel(width, height);
            _grid.UpdateLevel(_level);
            _state.ClearSelection();
            Console.WriteLine($"Created new level: {width}x{height} tiles");
        }

        private void ShowOpenLevelDialog()
        {
            // TODO: Implement file dialog
            // For now, using console input
            Console.WriteLine("Enter level file path:");
            string? path = Console.ReadLine();
            
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                try
                {
                    _level = EditorLevel.LoadFromFile(path);
                    _grid.UpdateLevel(_level);
                    _state.ClearSelection();
                    Console.WriteLine($"Loaded level from: {path}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load level: {ex.Message}");
                }
            }
        }

        private void ShowSaveLevelDialog()
        {
            // TODO: Implement file dialog
            // For now, using console input
            Console.WriteLine("Enter filename to save (without .json):");
            string? name = Console.ReadLine();
            
            if (!string.IsNullOrEmpty(name))
            {
                try
                {
                    string path = Pathfinder.GetWorldSavePath(name);
                    _level.SaveToFile(path);
                    Console.WriteLine($"Level saved to: {path}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to save level: {ex.Message}");
                }
            }
        }
    }
}