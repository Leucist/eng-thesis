using Application.Entities;

namespace Application
{
    public class WorldFactory
    {
        public World InitialWorld => GetMainMenuWorld();

        public World GetMainMenuWorld() {
            return new World(EntityManager.Instance.GetMainMenuPlayer());
        }
    }
}