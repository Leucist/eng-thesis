using SFML.Graphics;
using SFML.System;
using SFML.Window;
using LevelEditor.Core;
using LevelEditor.Prefabs;
using LevelEditor.Utils;

namespace LevelEditor.UI
{
    /// <summary>
    /// Vertical panel on the right side displaying available prefabs
    /// </summary>
    public class PrefabPanel
    {
        private EditorState _state;
        private PrefabLibrary _prefabLibrary;
        private Vector2f _position;
        private Vector2f _size;
        private float _scrollOffset = 0f;

        private RectangleShape _background;
        private List<PrefabButton> _prefabButtons = new();
        private Font _font;

        private bool _isBackgroundMode = false;
        private List<BackgroundButton> _backgroundButtons = new();

        public PrefabPanel(EditorState state, PrefabLibrary prefabLibrary, Vector2f position, Vector2f size, Font font)
        {
            _state = state;
            _prefabLibrary = prefabLibrary;
            _position = position;
            _size = size;
            _font = font;

            _background = new RectangleShape(size)
            {
                Position = position,
                FillColor = EditorConstants.PREFAB_PANEL_BG
            };

            RefreshPrefabs();
        }

        public void SwitchToBackgroundMode()
        {
            _isBackgroundMode = true;
            _prefabButtons.Clear();
            LoadBackgroundImages();
        }

        public void SwitchToPrefabMode()
        {
            _isBackgroundMode = false;
            _backgroundButtons.Clear();
            RefreshPrefabs();
        }

        private void LoadBackgroundImages()
        {
            _backgroundButtons.Clear();
            string bgDir = Pathfinder.GetBackgroundDirectory();
            
            if (!Directory.Exists(bgDir)) return;
            
            var imageFiles = Directory.GetFiles(bgDir, "*.png")
                .Concat(Directory.GetFiles(bgDir, "*.jpg"));
            
            float yOffset = EditorConstants.PREFAB_PADDING;
            int index = 0;
            
            foreach (var imagePath in imageFiles)
            {
                string relativePath = "Backgrounds/" + Path.GetFileNameWithoutExtension(imagePath);
                int col = index % 2;
                int row = index / 2;
                
                float x = _position.X + EditorConstants.PREFAB_PADDING + 
                        col * (EditorConstants.PREFAB_ICON_SIZE + EditorConstants.PREFAB_PADDING);
                float y = _position.Y + EditorConstants.PREFAB_PADDING + 
                        row * (EditorConstants.PREFAB_ICON_SIZE + EditorConstants.PREFAB_PADDING + 20f);
                
                var button = new BackgroundButton(new Vector2f(x, y), relativePath, _font);
                _backgroundButtons.Add(button);
                index++;
            }
        }

        public void RefreshPrefabs()
        {
            _prefabButtons.Clear();
            var prefabs = _prefabLibrary.GetPrefabs(_state.CurrentCategory);

            float yOffset = EditorConstants.PREFAB_PADDING;
            int columns = 2; // Two columns of prefabs
            int index = 0;

            foreach (var prefab in prefabs)
            {
                int col = index % columns;
                int row = index / columns;

                float x = _position.X + EditorConstants.PREFAB_PADDING + 
                         col * (EditorConstants.PREFAB_ICON_SIZE + EditorConstants.PREFAB_PADDING);
                float y = _position.Y + EditorConstants.PREFAB_PADDING + 
                         row * (EditorConstants.PREFAB_ICON_SIZE + EditorConstants.PREFAB_PADDING + 20f);

                var button = new PrefabButton(
                    new Vector2f(x, y),
                    prefab,
                    _font
                );

                _prefabButtons.Add(button);
                index++;
            }
        }

        public void HandleMouseClick(Vector2i mousePos)
        {
            // Background
            if (_isBackgroundMode)
            {
                foreach (var button in _backgroundButtons)
                {
                    if (button.HandleClick(mousePos, _scrollOffset))
                    {
                        _state.SelectedBackgroundImage = button.ImagePath;
                        Console.WriteLine($"Selected background: {button.ImagePath}");
                        return;
                    }
                }
            }
            // Button
            else {
                foreach (var button in _prefabButtons)
                {
                    if (button.HandleClick(mousePos, _scrollOffset))
                    {
                        _state.SelectedPrefabName = button.Prefab.Name;
                        UpdateButtonStates();
                        Console.WriteLine($"Selected prefab: {button.Prefab.Name}");
                        return;
                    }
                }
            }
        }

        public void HandleMouseMove(Vector2i mousePos)
        {
            foreach (var button in _prefabButtons)
            {
                button.HandleMouseMove(mousePos, _scrollOffset);
            }
        }

        public void HandleScroll(float delta)
        {
            _scrollOffset -= delta * EditorConstants.SCROLL_SPEED;
            _scrollOffset = Math.Max(0, _scrollOffset);
        }

        private void UpdateButtonStates()
        {
            foreach (var button in _prefabButtons)
            {
                button.IsSelected = button.Prefab.Name == _state.SelectedPrefabName;
            }
        }

        public void Draw(RenderWindow window)
        {
            window.Draw(_background);

            // Draw prefab buttons with scroll offset
            foreach (var button in _prefabButtons)
            {
                button.Draw(window, _scrollOffset);
            }
        }


        // * - - - INNER CLASSES - - - *

        private class BackgroundButton
        {
            public string ImagePath { get; }
            private RectangleShape _iconBackground;
            private Sprite? _iconSprite;
            private Text _nameText;
            private bool _isHovered = false;
            public bool IsSelected { get; set; } = false;
            
            public BackgroundButton(Vector2f position, string imagePath, Font font)
            {
                ImagePath = imagePath;

                _iconBackground = new RectangleShape(new Vector2f(
                    EditorConstants.PREFAB_ICON_SIZE, 
                    EditorConstants.PREFAB_ICON_SIZE))
                {
                    Position = position,
                    FillColor = new Color(50, 50, 55),
                    OutlineThickness = 2f,
                    OutlineColor = new Color(70, 70, 75)
                };

                // Try to load preview texture
                try
                {
                    var texture = GraphicsCache.GetTextureFromCache(
                        // Pathfinder.GetBackgroundDirectory(ImagePath)
                        ImagePath
                    );
                    _iconSprite = new Sprite(texture);
                    
                    // Scale to fit icon
                    float scaleX = EditorConstants.PREFAB_ICON_SIZE / texture.Size.X;
                    float scaleY = EditorConstants.PREFAB_ICON_SIZE / texture.Size.Y;
                    float scale = Math.Min(scaleX, scaleY);
                    _iconSprite.Scale = new Vector2f(scale, scale);
                    _iconSprite.Position = position;
                }
                catch
                {
                    _iconSprite = null;
                }

                _nameText = new Text(
                    Path.GetFileNameWithoutExtension(ImagePath), 
                    font, 12
                ) {
                    Position = new Vector2f(position.X, position.Y + EditorConstants.PREFAB_ICON_SIZE + 2),
                    FillColor = Color.White
                };
            }

            public void HandleMouseMove(Vector2i mousePos, float scrollOffset)
            {
                var adjustedBounds = GetAdjustedBounds(scrollOffset);
                _isHovered = adjustedBounds.Contains(mousePos.X, mousePos.Y);
                UpdateVisualState();
            }

            public bool HandleClick(Vector2i mousePos, float scrollOffset)
            {
                var adjustedBounds = GetAdjustedBounds(scrollOffset);
                return adjustedBounds.Contains(mousePos.X, mousePos.Y);
            }

            private FloatRect GetAdjustedBounds(float scrollOffset)
            {
                var bounds = _iconBackground.GetGlobalBounds();
                bounds.Top -= scrollOffset;
                return bounds;
            }

            private void UpdateVisualState()
            {
                if (IsSelected)
                {
                    _iconBackground.OutlineColor = EditorConstants.SELECTED_ENTITY_OUTLINE;
                    _iconBackground.OutlineThickness = 3f;
                }
                else if (_isHovered)
                {
                    _iconBackground.OutlineColor = EditorConstants.BUTTON_HOVER;
                    _iconBackground.OutlineThickness = 2f;
                }
                else
                {
                    _iconBackground.OutlineColor = new Color(70, 70, 75);
                    _iconBackground.OutlineThickness = 2f;
                }
            }

            public void Draw(RenderWindow window, float scrollOffset)
            {
                // Adjust positions for scrolling
                var originalIconPos = _iconBackground.Position;
                var originalTextPos = _nameText.Position;

                _iconBackground.Position = new Vector2f(originalIconPos.X, originalIconPos.Y - scrollOffset);
                _nameText.Position = new Vector2f(originalTextPos.X, originalTextPos.Y - scrollOffset);

                window.Draw(_iconBackground);
                
                if (_iconSprite != null)
                {
                    var originalSpritePos = _iconSprite.Position;
                    _iconSprite.Position = new Vector2f(originalSpritePos.X, originalSpritePos.Y - scrollOffset);
                    window.Draw(_iconSprite);
                    _iconSprite.Position = originalSpritePos;
                }

                window.Draw(_nameText);

                // Restore original positions
                _iconBackground.Position = originalIconPos;
                _nameText.Position = originalTextPos;
            }
        }

        private class PrefabButton
        {
            public PrefabDefinition Prefab { get; }
            private RectangleShape _iconBackground;
            private Sprite? _iconSprite;
            private Text _nameText;
            private bool _isHovered = false;
            public bool IsSelected { get; set; } = false;

            public PrefabButton(Vector2f position, PrefabDefinition prefab, Font font)
            {
                Prefab = prefab;

                _iconBackground = new RectangleShape(new Vector2f(
                    EditorConstants.PREFAB_ICON_SIZE, 
                    EditorConstants.PREFAB_ICON_SIZE))
                {
                    Position = position,
                    FillColor = new Color(50, 50, 55),
                    OutlineThickness = 2f,
                    OutlineColor = new Color(70, 70, 75)
                };

                // Try to load preview texture
                try
                {
                    var texture = GraphicsCache.GetTextureFromCache(
                        Pathfinder.GetFullTextureFilePath(prefab.PreviewTexture)
                    );
                    _iconSprite = new Sprite(texture);
                    
                    // Scale to fit icon
                    float scaleX = EditorConstants.PREFAB_ICON_SIZE / texture.Size.X;
                    float scaleY = EditorConstants.PREFAB_ICON_SIZE / texture.Size.Y;
                    float scale = Math.Min(scaleX, scaleY);
                    _iconSprite.Scale = new Vector2f(scale, scale);
                    _iconSprite.Position = position;
                }
                catch
                {
                    _iconSprite = null;
                }

                _nameText = new Text(prefab.Name, font, 12)
                {
                    Position = new Vector2f(position.X, position.Y + EditorConstants.PREFAB_ICON_SIZE + 2),
                    FillColor = Color.White
                };
            }

            public void HandleMouseMove(Vector2i mousePos, float scrollOffset)
            {
                var adjustedBounds = GetAdjustedBounds(scrollOffset);
                _isHovered = adjustedBounds.Contains(mousePos.X, mousePos.Y);
                UpdateVisualState();
            }

            public bool HandleClick(Vector2i mousePos, float scrollOffset)
            {
                var adjustedBounds = GetAdjustedBounds(scrollOffset);
                return adjustedBounds.Contains(mousePos.X, mousePos.Y);
            }

            private FloatRect GetAdjustedBounds(float scrollOffset)
            {
                var bounds = _iconBackground.GetGlobalBounds();
                bounds.Top -= scrollOffset;
                return bounds;
            }

            private void UpdateVisualState()
            {
                if (IsSelected)
                {
                    _iconBackground.OutlineColor = EditorConstants.SELECTED_ENTITY_OUTLINE;
                    _iconBackground.OutlineThickness = 3f;
                }
                else if (_isHovered)
                {
                    _iconBackground.OutlineColor = EditorConstants.BUTTON_HOVER;
                    _iconBackground.OutlineThickness = 2f;
                }
                else
                {
                    _iconBackground.OutlineColor = new Color(70, 70, 75);
                    _iconBackground.OutlineThickness = 2f;
                }
            }

            public void Draw(RenderWindow window, float scrollOffset)
            {
                // Adjust positions for scrolling
                var originalIconPos = _iconBackground.Position;
                var originalTextPos = _nameText.Position;

                _iconBackground.Position = new Vector2f(originalIconPos.X, originalIconPos.Y - scrollOffset);
                _nameText.Position = new Vector2f(originalTextPos.X, originalTextPos.Y - scrollOffset);

                window.Draw(_iconBackground);
                
                if (_iconSprite != null)
                {
                    var originalSpritePos = _iconSprite.Position;
                    _iconSprite.Position = new Vector2f(originalSpritePos.X, originalSpritePos.Y - scrollOffset);
                    window.Draw(_iconSprite);
                    _iconSprite.Position = originalSpritePos;
                }

                window.Draw(_nameText);

                // Restore original positions
                _iconBackground.Position = originalIconPos;
                _nameText.Position = originalTextPos;
            }
        }
    }
}