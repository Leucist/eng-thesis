using System.Text.Json;

namespace Application.Worlds
{
    public class WorldFactory
    {
        public World InitialWorld => GetTestWorld() /*GetMainMenuWorld()*/;

        public World GetTestWorld() {
            throw new NotImplementedException("Test world doesn't exist yet.");
            return LoadFromTemplate("Test");
        }

        public World GetMainMenuWorld() {
            throw new NotImplementedException("Main Menu world doesn't exist yet.");
            return LoadFromTemplate("MainMenu");
        }
        
        private World Load(string absolutePath) {
            string json = File.ReadAllText(absolutePath);
            WorldDTO dto = JsonSerializer.Deserialize<WorldDTO>(json)!;

            World world = new(dto.Entities, dto.Systems);
            // todo: Background and size in tiles remain unused

            return world;
        }

        public World LoadFromTemplate(string worldName) => Load(Pathfinder.GetWorldTemplatePath(worldName));

        public World LoadFromSaves(string saveName) => Load(Pathfinder.GetWorldSavePath(saveName));
    }
}