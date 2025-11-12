using Application.Worlds;

namespace Application.Tests
{
    public class WorldTests
    {
        // private WorldFactory _factory = new();

        // TODO: May be better tested when loading prefabs from JSON implemented. 
        // - Commented for now, as only entity properties can be compared, not the entites themselves as for unique id's
        // - I see no point in creating separate properties as World.Name/Type nor overriding Equals just for the sake of tests.

        // [Fact]
        // public void MainMenuWorld_HasCustomPlayer() {
        //     var world = _factory.GetMainMenuWorld();
        //     var player = EntityManager.Instance.GetMainMenuPlayer();

        //     Assert.Equal(world.Player, player);
        // }
    }
}