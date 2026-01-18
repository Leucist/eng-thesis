using SFML.Graphics;
using SFML.System;
using SFML.Window;
using LevelEditor.Core;
using LevelEditor.Utils;

namespace LevelEditor.UI
{
    /// <summary>
    /// Bottom toolbar with action buttons for level editing
    /// </summary>
    public class Toolbar
    {
        private EditorState _state;
        private Vector2f _position;
        private Vector2f _size;
        private RectangleShape _background;
        private List<Button> _buttons = new();

        // Actions
        public Action? OnNewLevel { get; set; }
        public Action? OnOpenLevel { get; set; }
        public Action? OnSaveLevel { get; set; }
        public Action? OnSwitchToTiles { get; set; }
        public Action? OnSwitchToCharacters { get; set; }
        public Action? OnSwitchToBackgrounds { get; set; }
        public Action? OnDeleteMode { get; set; }

        public Toolbar(EditorState state, Vector2f position, Vector2f size, Font font)
        {
            _state = state;
            _position = position;
            _size = size;

            _background = new RectangleShape(size)
            {
                Position = position,
                FillColor = EditorConstants.TOOLBAR_BG
            };

            CreateButtons(font);
        }

        private void CreateButtons(Font font)
        {
            float buttonWidth = 120f;
            float buttonHeight = 40f;
            float spacing = 10f;
            float xOffset = _position.X + spacing;
            float yOffset = _position.Y + (_size.Y - buttonHeight) / 2f;

            // File operations (right side)
            float rightX = _position.X + _size.X - spacing - buttonWidth;

            var saveButton = new Button(
                new Vector2f(rightX, yOffset),
                new Vector2f(buttonWidth, buttonHeight),
                "Save Level",
                font
            );
            saveButton.OnClick = () => OnSaveLevel?.Invoke();
            _buttons.Add(saveButton);

            var openButton = new Button(
                new Vector2f(rightX - buttonWidth - spacing, yOffset),
                new Vector2f(buttonWidth, buttonHeight),
                "Open Level",
                font
            );
            openButton.OnClick = () => OnOpenLevel?.Invoke();
            _buttons.Add(openButton);

            var newButton = new Button(
                new Vector2f(rightX - 2 * (buttonWidth + spacing), yOffset),
                new Vector2f(buttonWidth, buttonHeight),
                "New Level",
                font
            );
            newButton.OnClick = () => OnNewLevel?.Invoke();
            _buttons.Add(newButton);

            // Category switches (left side)
            var tilesButton = new Button(
                new Vector2f(xOffset, yOffset),
                new Vector2f(buttonWidth, buttonHeight),
                "Tiles",
                font
            );
            tilesButton.OnClick = () =>
            {
                _state.SetToolForCategory(PrefabCategory.Tiles);
                OnSwitchToTiles?.Invoke();
                UpdateToolButtons();
            };
            _buttons.Add(tilesButton);
            xOffset += buttonWidth + spacing;

            var charactersButton = new Button(
                new Vector2f(xOffset, yOffset),
                new Vector2f(buttonWidth, buttonHeight),
                "Characters",
                font
            );
            charactersButton.OnClick = () =>
            {
                _state.SetToolForCategory(PrefabCategory.Characters);
                OnSwitchToCharacters?.Invoke();
                UpdateToolButtons();
            };
            _buttons.Add(charactersButton);
            xOffset += buttonWidth + spacing;

            var backgroundsButton = new Button(
                new Vector2f(xOffset, yOffset),
                new Vector2f(buttonWidth, buttonHeight),
                "Backgrounds",
                font
            );
            backgroundsButton.OnClick = () =>
            {
                // _state.SetToolForCategory(PrefabCategory.Backgrounds);
                _state.CurrentTool = EditorTool.PlaceBackground;
                OnSwitchToBackgrounds?.Invoke();
                UpdateToolButtons();
            };
            _buttons.Add(backgroundsButton);
            xOffset += buttonWidth + spacing;

            var deleteButton = new Button(
                new Vector2f(xOffset, yOffset),
                new Vector2f(buttonWidth, buttonHeight),
                "Delete",
                font
            );
            deleteButton.OnClick = () =>
            {
                _state.CurrentTool = EditorTool.Delete;
                OnDeleteMode?.Invoke();
                UpdateToolButtons();
            };
            _buttons.Add(deleteButton);

            UpdateToolButtons();
        }

        private void UpdateToolButtons()
        {
            // Update active state based on current tool/category
            foreach (var button in _buttons)
            {
                button.IsActive = false;

                if (button.Label == "Tiles" && _state.CurrentCategory == PrefabCategory.Tiles && _state.CurrentTool == EditorTool.PlaceTile)
                    button.IsActive = true;
                else if (button.Label == "Characters" && _state.CurrentCategory == PrefabCategory.Characters && _state.CurrentTool == EditorTool.PlaceCharacter)
                    button.IsActive = true;
                else if (button.Label == "Backgrounds" && _state.CurrentTool == EditorTool.PlaceBackground)
                    button.IsActive = true;
                    // continue;
                else if (button.Label == "Delete" && _state.CurrentTool == EditorTool.Delete)
                    button.IsActive = true;
            }
        }

        public void HandleMouseMove(Vector2i mousePos)
        {
            foreach (var button in _buttons)
            {
                button.HandleMouseMove(mousePos);
            }
        }

        public void HandleMouseClick(Vector2i mousePos)
        {
            foreach (var button in _buttons)
            {
                if (button.HandleMouseClick(mousePos))
                    break;
            }
        }

        public void Update()
        {
            foreach (var button in _buttons)
            {
                button.Update();
            }
        }

        public void Draw(RenderWindow window)
        {
            window.Draw(_background);
            foreach (var button in _buttons)
            {
                button.Draw(window);
            }
        }
    }
}