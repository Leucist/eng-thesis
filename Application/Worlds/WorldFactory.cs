using System.Text.Json;

using Application.Entities;

namespace Application.Worlds
{
    public class WorldFactory
    {
        public World InitialWorld => GetTestWorld() /*GetMainMenuWorld()*/;

        public World GetTestWorld() {
            World world = new World();
            return world;
        }

        public World GetMainMenuWorld() {
            return new World(/*EntityManager.Instance.GetMainMenuPlayer()*/);
        }

        public World LoadFromSaves(string worldName) {
            string filePath = Pathfinder.GetWorldPath(worldName);

            string json = File.ReadAllText(filePath);
            WorldDTO dto = JsonSerializer.Deserialize<WorldDTO>(json)!;

            World world = new(dto.Entities, dto.Systems);
            // todo: Background and size in tiles remain unused

            return world;
        }
    }
}