using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GameForestTestAssignment.Game.PlayerInput;

public class InputHandler(int cellSize, Vector2 boardPosition)
{
    private MouseState _previousMouse = Mouse.GetState();
    private (int x, int y)? _selectedCell;

    public bool Enabled { get; set; } = true;

    public event Action<SwapCommand> SwapRequested;
    public event Action<int, int> CellSelected;
    public event Action SelectionCleared;

    public void Update()
    {
        var mouseState = Mouse.GetState();
        if (!Enabled)
        {
            _previousMouse = mouseState;
            return;
        }

        if (mouseState.LeftButton == ButtonState.Pressed &&
            _previousMouse.LeftButton == ButtonState.Released)
            HandleClick(mouseState.Position.ToVector2());

        _previousMouse = mouseState;
    }

    public void ClearSelection()
    {
        _selectedCell = null;
        SelectionCleared?.Invoke();
    }

    private void HandleClick(Vector2 mousePosition)
    {
        var cellX = (int)((mousePosition.X - boardPosition.X) / cellSize);
        var cellY = (int)((mousePosition.Y - boardPosition.Y) / cellSize);

        if (!IsValidCell(cellX, cellY))
            return;

        if (_selectedCell == null)
        {
            _selectedCell = new ValueTuple<int, int>(cellX, cellY);
            CellSelected?.Invoke(cellX, cellY);
        }
        else
        {
            var sx = _selectedCell.Value.x;
            var sy = _selectedCell.Value.y;

            if (cellX == sx && cellY == sy)
            {
                ClearSelection();
            }
            else if (Math.Abs(cellX - sx) + Math.Abs(cellY - sy) == 1)
            {
                ClearSelection();
                SwapRequested?.Invoke(new SwapCommand(sx, sy, cellX, cellY));
            }
            else
            {
                ClearSelection();
            }
        }
    }

    private static bool IsValidCell(int x, int y)
    {
        return x >= 0 && x < Board.Width &&
               y >= 0 && y < Board.Height;
    }
}