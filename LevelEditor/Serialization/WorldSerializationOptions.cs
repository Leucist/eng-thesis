using System.Text.Json;

namespace LevelEditor.Serialization
{
    public static class WorldSerializationOptions
    {
        public static JsonSerializerOptions GetOptions()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                IncludeFields = true,
                PropertyNameCaseInsensitive = true
            };
            return options;
        }
    }
}