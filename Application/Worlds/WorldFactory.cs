using Application.Entities;

namespace Application
{
    public class WorldFactory
    {
        public World InitialWorld => GetMainMenuWorld();

        public World GetMainMenuWorld() {
            throw new NotImplementedException();
            return new World(EntityManager.Instance.GetMainMenuPlayer());
        }
    }
}