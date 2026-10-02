#region

using Microsoft.Xna.Framework;

#endregion

namespace GameForestTestAssignment.Game;

public class BoardRenderState
{
    private readonly CellRenderState[,] _states;

    public BoardRenderState(int width, int height)
    {
        _states = new CellRenderState[width, height];
        Reset();
    }

    public CellRenderState this[int x, int y] => _states[x, y];

    public void Reset()
    {
        for (var x = 0; x < _states.GetLength(0); x++)
        for (var y = 0; y < _states.GetLength(1); y++)
            _states[x, y] = new CellRenderState
            {
                Offset = Vector2.Zero,
                Rotation = 0f,
                Scale = 1f,
                Alpha = 1f
            };
    }

    public void SetOffset(int x, int y, Vector2 value)
    {
        _states[x, y] = _states[x, y] with { Offset = value };
    }

    public void SetRotation(int x, int y, float value)
    {
        _states[x, y] = _states[x, y] with { Rotation = value };
    }

    public void SetScale(int x, int y, float value)
    {
        _states[x, y] = _states[x, y] with { Scale = value };
    }

    public void SetAlpha(int x, int y, float value)
    {
        _states[x, y] = _states[x, y] with { Alpha = value };
    }

    public record struct CellRenderState
    {
        public float Alpha;
        public Vector2 Offset;
        public float Rotation;
        public float Scale;
    }
}