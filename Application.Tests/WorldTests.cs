namespace Application.Tests
{
    public class WorldTests
    {
        private WorldFactory _factory = new();

        [Fact]
        public void WorldHasPlayer() {
            var world = _factory.InitialWorld;

            Assert.True(world.Player is not null);
        }
    }
}