namespace GameForestTestAssignment.Game.Animations;

public interface IAnimation
{
    bool IsComplete { get; }
    void Update(float deltaTime);
    void Apply(BoardRenderState renderState);
}
