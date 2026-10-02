#region

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

#endregion

namespace GameForestTestAssignment.Game.Effects;

public class Destroyer
{
    private const float CellsPerSecond = 14f;

    private readonly HashSet<(int, int)> _hitCells = [];

    private float _cellX;
    private float _cellY;

    public Destroyer(int startX, int startY, int dx, int dy, CellType colorType, Vector2 startScreenPosition)
    {
        _cellX = startX;
        _cellY = startY;
        Dx = dx;
        Dy = dy;
        ColorType = colorType;
        ScreenPosition = startScreenPosition;
        _hitCells.Add((startX, startY));
    }

    public int Dx { get; }
    public int Dy { get; }
    public CellType ColorType { get; }
    public bool IsAlive { get; private set; } = true;
    public Vector2 ScreenPosition { get; private set; }

    public void Update(float deltaTimeMs, Board board, Action<int, int> onHitCell)
    {
        if (!IsAlive)
            return;

        var remaining = CellsPerSecond * (deltaTimeMs / 1000f);
        while (remaining > 0f && IsAlive)
        {
            var step = Math.Min(remaining, 0.2f);
            _cellX += Dx * step;
            _cellY += Dy * step;
            remaining -= step;

            if (_cellX < -0.5f || _cellX > Board.Width - 0.5f ||
                _cellY < -0.5f || _cellY > Board.Height - 0.5f)
            {
                IsAlive = false;
                break;
            }

            var ix = (int)Math.Round(_cellX);
            var iy = (int)Math.Round(_cellY);
            if (Board.IsInBounds(ix, iy) && _hitCells.Add((ix, iy)))
                onHitCell(ix, iy);
        }

        ScreenPosition = board.BoardPosition + new Vector2(
            _cellX * board.CellSize + board.CellSize * 0.5f,
            _cellY * board.CellSize + board.CellSize * 0.5f);
    }
}