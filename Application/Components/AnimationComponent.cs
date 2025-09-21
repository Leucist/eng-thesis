namespace Application.Components
{
    public class AnimationComponent(GraphicsComponent graphicsComponent) : Component(ComponentType.Animation)
    {
        // Added composition which deviates from the whole separation of the components,
        // ..yet it allows to lower the load on the RenderingSystem - separating animation management and pure rendering.
        private readonly GraphicsComponent _graphicsComponent = graphicsComponent;

        private int _frameNum = 0;
        private AnimationType _animationType = AnimationType.Idle;
        private readonly Dictionary<AnimationType, string[]> _animations;

        public void Next() => SwitchToNextFrame();

        private void SwitchToNextFrame() {
            var frameSequence = _animations[_animationType];
            _frameNum = (_frameNum + 1) % frameSequence.Length;
            _graphicsComponent.SetTexture(frameSequence[_frameNum]);
        }

        public void SetAnimationType(AnimationType animationType) {
            if (_animations.ContainsKey(animationType)) _animationType = animationType;
        }
    }
}