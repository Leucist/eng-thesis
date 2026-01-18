using SFML.Graphics;
using SFML.System;
using SFML.Window;
using LevelEditor.Utils;

namespace LevelEditor.UI
{
    /// <summary>
    /// Simple button implementation for SFML UI
    /// </summary>
    public class Button
    {
        private RectangleShape _shape;
        private Text _label;
        private bool _isHovered = false;
        private bool _isActive = false;

        public Vector2f Position { get; set; }
        public Vector2f Size { get; set; }
        public string Label { get; set; }
        public Action? OnClick { get; set; }

        public bool IsActive
        {
            get => _isActive;
            set => _isActive = value;
        }

        public Button(Vector2f position, Vector2f size, string label, Font font)
        {
            Position = position;
            Size = size;
            Label = label;

            _shape = new RectangleShape(size)
            {
                Position = position,
                FillColor = EditorConstants.BUTTON_BG,
                OutlineThickness = 1f,
                OutlineColor = new Color(80, 80, 80)
            };

            _label = new Text(label, font, 14)
            {
                FillColor = Color.White
            };
            CenterLabel();
        }

        private void CenterLabel()
        {
            var bounds = _label.GetLocalBounds();
            _label.Origin = new Vector2f(bounds.Width / 2f, bounds.Height / 2f);
            _label.Position = new Vector2f(
                Position.X + Size.X / 2f,
                Position.Y + Size.Y / 2f - 2f // Slight adjustment for visual centering
            );
        }

        public void HandleMouseMove(Vector2i mousePos)
        {
            bool wasHovered = _isHovered;
            _isHovered = _shape.GetGlobalBounds().Contains(mousePos.X, mousePos.Y);

            if (_isHovered != wasHovered)
            {
                UpdateVisualState();
            }
        }

        public bool HandleMouseClick(Vector2i mousePos)
        {
            if (_isHovered)
            {
                OnClick?.Invoke();
                return true;
            }
            return false;
        }

        private void UpdateVisualState()
        {
            if (_isActive)
            {
                _shape.FillColor = EditorConstants.BUTTON_ACTIVE;
            }
            else if (_isHovered)
            {
                _shape.FillColor = EditorConstants.BUTTON_HOVER;
            }
            else
            {
                _shape.FillColor = EditorConstants.BUTTON_BG;
            }
        }

        public void Update()
        {
            UpdateVisualState();
        }

        public void Draw(RenderWindow window)
        {
            window.Draw(_shape);
            window.Draw(_label);
        }
    }
}