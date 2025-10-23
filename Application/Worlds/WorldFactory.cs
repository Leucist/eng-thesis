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
    }
}