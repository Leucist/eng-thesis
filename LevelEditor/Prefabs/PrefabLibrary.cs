using System.Text.Json;
using LevelEditor.Serialization;
using LevelEditor.Core;

namespace LevelEditor.Prefabs
{
    /// <summary>
    /// Manages loading and caching of prefab definitions from JSON files
    /// </summary>
    public class PrefabLibrary
    {
        private Dictionary<PrefabCategory, List<PrefabDefinition>> _prefabs = new();
        private JsonSerializerOptions _jsonOptions;

        public PrefabLibrary()
        {
            _jsonOptions = WorldSerializationOptions.GetOptions();
            LoadAllPrefabs();
        }

        private void LoadAllPrefabs()
        {
            _prefabs[PrefabCategory.Tiles] = LoadPrefabsFromDirectory(
                Pathfinder.GetTilePrefabsDirectory()
            );
            
            _prefabs[PrefabCategory.Characters] = LoadPrefabsFromDirectory(
                Pathfinder.GetCharacterPrefabsDirectory()
            );
            
            _prefabs[PrefabCategory.Backgrounds] = LoadPrefabsFromDirectory(
                Pathfinder.GetBackgroundPrefabsDirectory()
            );
        }

        private List<PrefabDefinition> LoadPrefabsFromDirectory(string directory)
        {
            var prefabs = new List<PrefabDefinition>();

            if (!Directory.Exists(directory))
            {
                Console.WriteLine($"Prefab directory not found: {directory}");
                return prefabs;
            }

            var jsonFiles = Directory.GetFiles(directory, "*.json");
            
            foreach (var file in jsonFiles)
            {
                try
                {
                    string json = File.ReadAllText(file);
                    var prefab = JsonSerializer.Deserialize<PrefabDefinition>(json, _jsonOptions);
                    
                    if (prefab != null)
                    {
                        prefabs.Add(prefab);
                        Console.WriteLine($"Loaded prefab: {prefab.Name}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load prefab from {file}: {ex.Message}");
                }
            }

            return prefabs;
        }

        public List<PrefabDefinition> GetPrefabs(PrefabCategory category)
        {
            return _prefabs.GetValueOrDefault(category, new List<PrefabDefinition>());
        }

        public PrefabDefinition? GetPrefabByName(string name)
        {
            foreach (var categoryPrefabs in _prefabs.Values)
            {
                var prefab = categoryPrefabs.FirstOrDefault(p => p.Name == name);
                if (prefab != null) return prefab;
            }
            return null;
        }

        public void ReloadPrefabs()
        {
            _prefabs.Clear();
            LoadAllPrefabs();
        }
    }
}