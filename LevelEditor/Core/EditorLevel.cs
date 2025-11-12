using System.Text.Json;
using Application.Components;
using Application.Worlds;
using Application.Systems;
using Application.Serialization;
using LevelEditor.Utils;

namespace LevelEditor.Core
{
    /// <summary>
    /// Represents the current level being edited, with grid-based entity placement
    /// </summary>
    public class EditorLevel
    {
        public int WidthInTiles { get; private set; }
        public int HeightInTiles { get; private set; }
        public List<PlacedEntity> Entities { get; private set; } = new();
        public string? CurrentFilePath { get; set; } = null;

        // Grid management - track which grid cells are occupied
        private Dictionary<(int x, int y), PlacedEntity> _gridOccupancy = new();

        public EditorLevel(int widthInTiles, int heightInTiles)
        {
            WidthInTiles = widthInTiles;
            HeightInTiles = heightInTiles;
        }

        public bool IsGridPositionValid(int gridX, int gridY)
        {
            return gridX >= 0 && gridX < WidthInTiles && 
                   gridY >= 0 && gridY < HeightInTiles;
        }

        public bool IsGridPositionOccupied(int gridX, int gridY)
        {
            return _gridOccupancy.ContainsKey((gridX, gridY));
        }

        public PlacedEntity? GetEntityAtGridPosition(int gridX, int gridY)
        {
            _gridOccupancy.TryGetValue((gridX, gridY), out var entity);
            return entity;
        }

        public bool PlaceEntity(PlacedEntity entity)
        {
            if (!IsGridPositionValid(entity.GridX, entity.GridY))
                return false;

            // For backgrounds, allow placement anywhere (they don't occupy grid)
            bool isBackground = entity.Components.OfType<GraphicsComponent>()
                .Any(gc => {
                    // Check if this is a background by looking at transform
                    var transform = entity.Components.OfType<TransformComponent>().FirstOrDefault();
                    return transform != null && transform.X == 0 && transform.Y == 0;
                });

            if (!isBackground && IsGridPositionOccupied(entity.GridX, entity.GridY))
                return false;

            // Update entity position in pixels
            float pixelX = entity.GridX * EditorConstants.TILE_SIZE;
            float pixelY = entity.GridY * EditorConstants.TILE_SIZE;
            entity.UpdatePosition(entity.GridX, entity.GridY, pixelX, pixelY);

            Entities.Add(entity);
            
            if (!isBackground)
            {
                _gridOccupancy[(entity.GridX, entity.GridY)] = entity;
            }

            return true;
        }

        public void RemoveEntity(PlacedEntity entity)
        {
            Entities.Remove(entity);
            _gridOccupancy.Remove((entity.GridX, entity.GridY));
        }

        public void Clear()
        {
            Entities.Clear();
            _gridOccupancy.Clear();
        }

        /// <summary>
        /// Converts the editor level to WorldDTO for saving
        /// </summary>
        public WorldDTO ToWorldDTO()
        {
            var dto = new WorldDTO
            {
                Entities = Entities.Select(e => e.Components).ToList(),
                Systems = CreateDefaultSystemTypes() // TODO: Make this configurable
            };
            return dto;
        }

        /// <summary>
        /// Creates default system type list for a level
        /// TODO: Allow user to configure this, or make it template-based
        /// </summary>
        private List<string> CreateDefaultSystemTypes()
        {
            // For now, returns list with default system types
            return new List<string>
            {
                "Application.Systems.MovementSystem",
                "Application.Systems.RenderingSystem",
                // "Application.Systems.AnimationSystem", // TODO: Not implemented yet
                // "Application.Systems.CollisionSystem"
            };
        }

        /// <summary>
        /// Saves the level to a JSON file
        /// </summary>
        public void SaveToFile(string filePath)
        {
            var dto = ToWorldDTO();
            var options = WorldSerializationOptions.GetOptions();
            string json = JsonSerializer.Serialize(dto, options);
            File.WriteAllText(filePath, json);
            CurrentFilePath = filePath;
            Console.WriteLine($"Level saved to: {filePath}");
        }

        /// <summary>
        /// Loads a level from a JSON file
        /// </summary>
        public static EditorLevel LoadFromFile(string filePath)
        {
            string json = File.ReadAllText(filePath);
            var options = WorldSerializationOptions.GetOptions();
            var dto = JsonSerializer.Deserialize<WorldDTO>(json, options);

            if (dto == null)
                throw new Exception("Failed to deserialize WorldDTO");

            // Calculate grid dimensions from entities
            int maxX = 0, maxY = 0;
            foreach (var componentList in dto.Entities)
            {
                var transform = componentList.OfType<TransformComponent>().FirstOrDefault();
                if (transform != null)
                {
                    int gridX = (int)(transform.X / EditorConstants.TILE_SIZE);
                    int gridY = (int)(transform.Y / EditorConstants.TILE_SIZE);
                    maxX = Math.Max(maxX, gridX + 1);
                    maxY = Math.Max(maxY, gridY + 1);
                }
            }

            // Create level with calculated dimensions (or use defaults)
            var level = new EditorLevel(
                Math.Max(maxX, 10), 
                Math.Max(maxY, 10)
            );

            // Convert WorldDTO entities to PlacedEntities
            foreach (var componentList in dto.Entities)
            {
                var transform = componentList.OfType<TransformComponent>().FirstOrDefault();
                if (transform != null)
                {
                    int gridX = (int)(transform.X / EditorConstants.TILE_SIZE);
                    int gridY = (int)(transform.Y / EditorConstants.TILE_SIZE);

                    var placedEntity = new PlacedEntity(
                        "LoadedEntity", // TODO: Try to infer prefab name
                        componentList,
                        gridX,
                        gridY
                    );

                    level.PlaceEntity(placedEntity);
                }
            }

            level.CurrentFilePath = filePath;
            Console.WriteLine($"Level loaded from: {filePath}");
            return level;
        }
    }
}