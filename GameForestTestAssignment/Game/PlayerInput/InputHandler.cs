using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GameForestTestAssignment.Game.PlayerInput;

public class InputHandler(int cellSize, Vector2 boardPosition)
{
    private const float DragThreshold = 0.5f;
    private bool _deselectOnRelease;

    private Vector2? _dragOrigin;
    private MouseState _previousMouse = Mouse.GetState();
    private (int X, int Y)? _selectedCell;

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

        var position = mouseState.Position.ToVector2();
        var isPressed = mouseState.LeftButton == ButtonState.Pressed;
        var wasPressed = _previousMouse.LeftButton == ButtonState.Pressed;

        if (isPressed && !wasPressed)
            HandlePress(position);
        else if (isPressed)
            HandleDrag(position);
        else if (wasPressed)
            HandleRelease();

        _previousMouse = mouseState;
    }

    public void ClearSelection()
    {
        _selectedCell = null;
        _dragOrigin = null;
        _deselectOnRelease = false;
        SelectionCleared?.Invoke();
    }

    private void HandlePress(Vector2 position)
    {
        if (!TryGetCell(position, out var cell))
        {
            ClearSelection();
            return;
        }

        if (_selectedCell is { } selected)
        {
            if (cell == selected)
            {
                _dragOrigin = position;
                _deselectOnRelease = true;
                return;
            }

            if (Math.Abs(cell.X - selected.X) + Math.Abs(cell.Y - selected.Y) == 1)
            {
                RequestSwap(selected, cell);
                return;
            }
        }

        _selectedCell = cell;
        _dragOrigin = position;
        _deselectOnRelease = false;
        CellSelected?.Invoke(cell.X, cell.Y);
    }

    private void HandleDrag(Vector2 position)
    {
        if (_dragOrigin is not { } origin || _selectedCell is not { } selected)
            return;

        var delta = position - origin;
        if (delta.Length() < cellSize * DragThreshold)
            return;

        (int X, int Y) target = Math.Abs(delta.X) > Math.Abs(delta.Y)
            ? (selected.X + Math.Sign(delta.X), selected.Y)
            : (selected.X, selected.Y + Math.Sign(delta.Y));

        if (IsValidCell(target.X, target.Y))
        {
            RequestSwap(selected, target);
            return;
        }

        _dragOrigin = null;
        _deselectOnRelease = false;
    }

    private void HandleRelease()
    {
        if (_dragOrigin != null && _deselectOnRelease)
            ClearSelection();

        _dragOrigin = null;
        _deselectOnRelease = false;
    }

    private void RequestSwap((int X, int Y) from, (int X, int Y) to)
    {
        ClearSelection();
        SwapRequested?.Invoke(new SwapCommand(from.X, from.Y, to.X, to.Y));
    }

    private bool TryGetCell(Vector2 position, out (int X, int Y) cell)
    {
        cell = ((int)MathF.Floor((position.X - boardPosition.X) / cellSize),
            (int)MathF.Floor((position.Y - boardPosition.Y) / cellSize));
        return IsValidCell(cell.X, cell.Y);
    }

    private static bool IsValidCell(int x, int y)
    {
        return x is >= 0 and < Board.Width && y is >= 0 and < Board.Height;
    }
}