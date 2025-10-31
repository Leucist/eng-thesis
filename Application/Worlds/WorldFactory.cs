using System.Text.Json;

namespace Application.Worlds
{
    public static class WorldFactory
    {
        public static World InitialWorld => GetTestWorld() /*GetMainMenuWorld()*/;

        public static World GetTestWorld() {
            throw new NotImplementedException("Test world doesn't exist yet.");
            return LoadFromTemplate("Test");
        }

        public static World GetMainMenuWorld() {
            throw new NotImplementedException("Main Menu world doesn't exist yet.");
            return LoadFromTemplate("MainMenu");
        }
        
        private static World Load(string absolutePath) {
            string json = File.ReadAllText(absolutePath);
            WorldDTO dto = JsonSerializer.Deserialize<WorldDTO>(json)!;

            World world = new(dto.Entities, dto.Systems);
            // todo: Background and size in tiles remain unused

            return world;
        }

        public static World LoadFromTemplate(string worldName) => Load(Pathfinder.GetWorldTemplatePath(worldName));

        public static World LoadFromSaves(string saveName) => Load(Pathfinder.GetWorldSavePath(saveName));
    }
}