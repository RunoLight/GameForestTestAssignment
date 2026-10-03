using System;
using System.Collections.Generic;

namespace GameForestTestAssignment.Game.Effects;

public class Destroyer
{
    private const float CellsPerSecond = 14f;

    private readonly HashSet<(int, int)> _hitCells = [];

    public Destroyer(int startX, int startY, int dx, int dy, CellType colorType)
    {
        X = startX;
        Y = startY;
        Dx = dx;
        Dy = dy;
        ColorType = colorType;
        _hitCells.Add((startX, startY));
    }

    public float X { get; private set; }
    public float Y { get; private set; }
    public int Dx { get; }
    public int Dy { get; }
    public CellType ColorType { get; }
    public bool IsAlive { get; private set; } = true;

    public void Update(float deltaTime, IBoard board, Action<int, int> onHitCell)
    {
        if (!IsAlive)
            return;

        var remaining = CellsPerSecond * deltaTime;
        while (remaining > 0f && IsAlive)
        {
            var step = Math.Min(remaining, 0.2f);
            X += Dx * step;
            Y += Dy * step;
            remaining -= step;

            if (X < -0.5f || X > Board.Width - 0.5f ||
                Y < -0.5f || Y > Board.Height - 0.5f)
            {
                IsAlive = false;
                break;
            }

            var ix = (int)Math.Round(X);
            var iy = (int)Math.Round(Y);
            if (board.IsInBounds(ix, iy) && _hitCells.Add((ix, iy)))
                onHitCell(ix, iy);
        }
    }
}