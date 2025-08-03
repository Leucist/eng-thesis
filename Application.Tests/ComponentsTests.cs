using Application.Components;

namespace Application.Tests
{
    public class ComponentsTests
    {
        [Fact]
        public void TransformComponent_ShouldHave_X_Y_Width_Height() {
            float x = 10f,
                y = 20f,
                width = 150f,
                height = 82.5f;

            var component = new TransformComponent(x, y, width, height);

            Assert.Equal(component.X, x);
            Assert.Equal(component.Y, y);
            Assert.Equal(component.Width, width);
            Assert.Equal(component.Height, height);
        }

        [Fact]
        public void TransformComponent_Move_ShouldChangeCoords() {
            float x = 10f,
                y = 20f,
                width = 150f,
                height = 82.5f;
            var component = new TransformComponent(x, y, width, height);

            component.Move(x, -y/2);

            Assert.Equal(component.X, x+x);
            Assert.Equal(component.Y, y-(y/2));
            Assert.Equal(component.Width, width);
            Assert.Equal(component.Height, height);
        }
    }
}