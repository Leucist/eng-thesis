using Application.Entities;

namespace Application.Tests
{
    public class WorldTests
    {
        private WorldFactory _factory = new();

        [Fact]
        public void InitialWorld_is_MainMenuWorld() {
            var initWorld = _factory.InitialWorld;
            var mmWorld = _factory.GetMainMenuWorld();

            Assert.Equal(mmWorld, initWorld);
        }

        [Fact]
        public void MainMenuWorld_HasCustomPlayer() {
            var world = _factory.GetMainMenuWorld();
            var player = EntityManager.Instance.GetMainMenuPlayer();

            Assert.Equal(world.Player, player);
        }
    }
}