using SFML.Graphics;
using SFML.System;
using SFML.Window;
using LevelEditor.Core;
using LevelEditor.Utils;
using LevelEditor.Prefabs;
using Application.Components;

namespace LevelEditor.UI
{
    /// <summary>
    /// Manages the main grid area where entities are placed
    /// </summary>
    public class EditorGrid
    {
        private EditorLevel _level;
        private EditorState _state;
        private PrefabLibrary _prefabLibrary;

        // Grid view settings
        private Vector2f _gridPosition;
        private Vector2f _gridSize;
        private float _scrollOffsetX = 0f;
        private float _cellSize; // Calculated based on available space

        // Visual elements
        private RectangleShape _background;
        private List<RectangleShape> _gridLines = new();

        public EditorGrid(EditorLevel level, EditorState state, PrefabLibrary prefabLibrary, Vector2f position, Vector2f size)
        {
            _level = level;
            _state = state;
            _prefabLibrary = prefabLibrary;
            _gridPosition = position;
            _gridSize = size;

            _background = new RectangleShape(size)
            {
                Position = position,
                FillColor = EditorConstants.GRID_BACKGROUND
            };

            CalculateCellSize();
            GenerateGridLines();
        }

        private void CalculateCellSize()
        {
            // Calculate cell size to fit the grid
            int visibleWidth = Math.Min(_level.WidthInTiles, EditorConstants.MAX_VISIBLE_GRID_WIDTH);
            int visibleHeight = Math.Min(_level.HeightInTiles, EditorConstants.MAX_LEVEL_HEIGHT_TILES);
            // int visibleHeight = _level.HeightInTiles;
            
            // Calculate based on both width and height constraints
            float cellSizeByWidth = (_gridSize.X - 2 * EditorConstants.GRID_PADDING) / visibleWidth;
            float cellSizeByHeight = (_gridSize.Y - 2 * EditorConstants.GRID_PADDING) / visibleHeight;
            
            // Use the smaller of the two to ensure everything fits
            _cellSize = Math.Min(cellSizeByWidth, cellSizeByHeight);
            
            // Set a minimum cell size so tiles don't become too small, may be removed~
            _cellSize = Math.Max(_cellSize, 32f); // Minimum 32 pixels per cell


            // // Calculate cell size to fit the grid
            // float maxVisibleWidth = Math.Min(_level.WidthInTiles, EditorConstants.MAX_VISIBLE_GRID_WIDTH);
            // _cellSize = (_gridSize.X - 2 * EditorConstants.GRID_PADDING) / maxVisibleWidth;
        }

        private void GenerateGridLines()
        {
            _gridLines.Clear();

            int visibleWidth = Math.Min(_level.WidthInTiles, EditorConstants.MAX_VISIBLE_GRID_WIDTH);
            int visibleHeight = Math.Min(_level.HeightInTiles, EditorConstants.MAX_LEVEL_HEIGHT_TILES);
            // int visibleHeight = _level.HeightInTiles;

            // Vertical lines
            for (int x = 0; x <= visibleWidth; x++)
            {
                var line = new RectangleShape(new Vector2f(1f, _cellSize * visibleHeight))
                {
                    Position = new Vector2f(_gridPosition.X + EditorConstants.GRID_PADDING + x * _cellSize, 
                                           _gridPosition.Y + EditorConstants.GRID_PADDING),
                    FillColor = EditorConstants.GRID_LINE_COLOR
                };
                _gridLines.Add(line);
            }

            // Horizontal lines
            for (int y = 0; y <= visibleHeight; y++)
            {
                var line = new RectangleShape(new Vector2f(_cellSize * visibleWidth, 1f))
                {
                    Position = new Vector2f(_gridPosition.X + EditorConstants.GRID_PADDING,
                                           _gridPosition.Y + EditorConstants.GRID_PADDING + y * _cellSize),
                    FillColor = EditorConstants.GRID_LINE_COLOR
                };
                _gridLines.Add(line);
            }
        }

        public void SetLevelBackground(string backgroundImagePath)
        {
            // Remove existing background entity if any
            var existingBg = _level.Entities.FirstOrDefault(e => 
                e.Components.OfType<TransformComponent>().Any(t => t.X == 0 && t.Y == 0 && 
                e.Components.OfType<GraphicsComponent>().Any()));
            
            if (existingBg != null)
            {
                _level.RemoveEntity(existingBg);
            }
            
            // Create background entity
            var components = new List<Component>
            {
                new TransformComponent
                (
                    0, 0, 
                    _level.WidthInTiles * EditorConstants.TILE_SIZE,    // <- actual sizes before scaling
                    _level.HeightInTiles * EditorConstants.TILE_SIZE    // <-/
                ),
                new GraphicsComponent(backgroundImagePath)
            };
            
            var bgEntity = new PlacedEntity("Background", components, 0, 0);
            _level.PlaceEntity(bgEntity);
            Console.WriteLine($"Set background to: {backgroundImagePath}");
        }

        public void HandleMouseClick(Vector2i mousePos)
        {
            var gridCoords = ScreenToGrid(mousePos);
            if (gridCoords == null) return;

            (int gridX, int gridY) = gridCoords.Value;

            switch (_state.CurrentTool)
            {
                case EditorTool.Select:
                    HandleSelect(gridX, gridY);
                    break;

                case EditorTool.PlaceTile:
                case EditorTool.PlaceCharacter:
                // case EditorTool.PlaceBackground:
                    HandlePlaceEntity(gridX, gridY);
                    break;

                case EditorTool.Delete:
                    HandleDelete(gridX, gridY);
                    break;
            }
        }

        private void HandleSelect(int gridX, int gridY)
        {
            var entity = _level.GetEntityAtGridPosition(gridX, gridY);
            
            // Deselect previous
            if (_state.SelectedEntity != null)
            {
                _state.SelectedEntity.SetSelected(false);
            }

            // Select new
            _state.SelectedEntity = entity;
            if (entity != null)
            {
                entity.SetSelected(true);
            }
        }

        private void HandlePlaceEntity(int gridX, int gridY)
        {
            if (_state.SelectedPrefabName == null) return;

            var prefab = _prefabLibrary.GetPrefabByName(_state.SelectedPrefabName);
            if (prefab == null) return;

            // // For backgrounds, place at (0, 0) regardless of click position
            // bool isBackground = _state.CurrentTool == EditorTool.PlaceBackground;
            // if (isBackground)
            // {
            //     gridX = 0;
            //     gridY = 0;
            // }

            var components = prefab.CloneComponents();
            var placedEntity = new PlacedEntity(prefab.Name, components, gridX, gridY);

            if (_level.PlaceEntity(placedEntity))
            {
                Console.WriteLine($"Placed {prefab.Name} at ({gridX}, {gridY})");
            }
            else
            {
                Console.WriteLine($"Cannot place entity at ({gridX}, {gridY}) - position occupied or invalid");
            }
        }

        private void HandleDelete(int gridX, int gridY)
        {
            var entity = _level.GetEntityAtGridPosition(gridX, gridY);
            if (entity != null)
            {
                _level.RemoveEntity(entity);
                if (_state.SelectedEntity == entity)
                {
                    _state.SelectedEntity = null;
                }
                Console.WriteLine($"Deleted entity at ({gridX}, {gridY})");
            }
        }

        private (int x, int y)? ScreenToGrid(Vector2i mousePos)
        {
            // Check if mouse is within grid bounds
            if (mousePos.X < _gridPosition.X + EditorConstants.GRID_PADDING ||
                mousePos.X > _gridPosition.X + _gridSize.X - EditorConstants.GRID_PADDING ||
                mousePos.Y < _gridPosition.Y + EditorConstants.GRID_PADDING ||
                mousePos.Y > _gridPosition.Y + _gridSize.Y - EditorConstants.GRID_PADDING)
            {
                return null;
            }

            int gridX = (int)((mousePos.X - _gridPosition.X - EditorConstants.GRID_PADDING + _scrollOffsetX) / _cellSize);
            int gridY = (int)((mousePos.Y - _gridPosition.Y - EditorConstants.GRID_PADDING) / _cellSize);

            if (!_level.IsGridPositionValid(gridX, gridY))
                return null;

            return (gridX, gridY);
        }

        public void HandleScroll(float delta)
        {
            // Horizontal scrolling for wide levels
            if (_level.WidthInTiles > EditorConstants.MAX_VISIBLE_GRID_WIDTH)
            {
                _scrollOffsetX -= delta * EditorConstants.SCROLL_SPEED;
                float maxScroll = (_level.WidthInTiles - EditorConstants.MAX_VISIBLE_GRID_WIDTH) * _cellSize;
                _scrollOffsetX = Math.Clamp(_scrollOffsetX, 0, maxScroll);
            }
        }

        public void Draw(RenderWindow window)
        {
            window.Draw(_background);

            // Draw entities
            // Create a view for scrolling
            var originalView = window.GetView();
            var gridView = new View(originalView);
            
            foreach (var entity in _level.Entities)
            {
                // Adjust position based on scroll offset
                float adjustedX = _gridPosition.X + EditorConstants.GRID_PADDING + 
                                 entity.GridX * _cellSize - _scrollOffsetX;
                float adjustedY = _gridPosition.Y + EditorConstants.GRID_PADDING + 
                                 entity.GridY * _cellSize;

                // Only draw if visible
                if (adjustedX + _cellSize >= _gridPosition.X && 
                    adjustedX <= _gridPosition.X + _gridSize.X)
                {
                    // Temporarily update entity visual position for drawing
                    var tempSprite = entity.Sprite;
                    var tempOutline = entity.SelectionOutline;
                    
                    if (tempSprite != null)
                    {
                        var originalPos = tempSprite.Position;
                        tempSprite.Position = new Vector2f(adjustedX, adjustedY);
                        tempSprite.Scale = new Vector2f(_cellSize / EditorConstants.TILE_SIZE, 
                                                        _cellSize / EditorConstants.TILE_SIZE);
                        window.Draw(tempSprite);
                        tempSprite.Position = originalPos;
                        tempSprite.Scale = new Vector2f(1f, 1f);
                    }
                    
                    if (entity.IsSelected)
                    {
                        var originalPos = tempOutline.Position;
                        var originalSize = tempOutline.Size;
                        tempOutline.Position = new Vector2f(adjustedX, adjustedY);
                        tempOutline.Size = new Vector2f(_cellSize, _cellSize);
                        window.Draw(tempOutline);
                        tempOutline.Position = originalPos;
                        tempOutline.Size = originalSize;
                    }
                }
            }

            // Draw grid lines
            foreach (var line in _gridLines)
            {
                window.Draw(line);
            }
        }

        public void UpdateLevel(EditorLevel newLevel)
        {
            _level = newLevel;
            CalculateCellSize();
            GenerateGridLines();
            _scrollOffsetX = 0;
        }
    }
}