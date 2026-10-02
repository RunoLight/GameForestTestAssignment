#region

using System;
using System.Collections.Generic;
using GameForestTestAssignment.Game.Animations;
using GameForestTestAssignment.Game.Effects;
using Microsoft.Xna.Framework;

#endregion

namespace GameForestTestAssignment.Game.GameLogic;

public class RenderStateApplier(Board board, BoardRenderer boardRenderer)
{
    private HighlightAnimation _highlightAnim = null!;
    private int _selectedX = -1, _selectedY = -1;

    public void SelectCell(int x, int y)
    {
        _selectedX = x;
        _selectedY = y;
        _highlightAnim = new HighlightAnimation();
    }

    public void ClearSelection()
    {
        _selectedX = -1;
        _selectedY = -1;
        _highlightAnim = null;
    }

    public void UpdateHighlight(float deltaTime)
    {
        _highlightAnim?.Update(deltaTime);
    }

    public void ApplyRenderState(
        Dictionary<(int X, int Y), SwapAnimation> swapAnimations,
        Dictionary<(int X, int Y), FallAnimation> fallAnimations,
        Dictionary<(int X, int Y), DisappearAnimation> removalAnimations
    )
    {
        board.ResetRenderStates();

        foreach (var (pos, anim) in swapAnimations)
        {
            var offset = anim.GetCurrentPosition() - GetCellScreenPos(pos.X, pos.Y);
            board.RenderState.SetOffset(pos.X, pos.Y, offset);
        }

        foreach (var (pos, anim) in fallAnimations)
        {
            var offset = new Vector2(0f, anim.GetCurrentY() - pos.Y * board.CellSize);
            board.RenderState.SetOffset(pos.X, pos.Y, offset);
        }

        foreach (var (pos, anim) in removalAnimations)
        {
            if (!Board.IsInBounds(pos.X, pos.Y)) continue;
            board.RenderState.SetScale(pos.X, pos.Y, Math.Max(0.01f, anim.GetScale()));
            board.RenderState.SetAlpha(pos.X, pos.Y, anim.GetAlpha());
        }

        if (_highlightAnim != null && _selectedX >= 0 && _selectedY >= 0)
        {
            board.RenderState.SetRotation(_selectedX, _selectedY, _highlightAnim.GetRotation());
            board.RenderState.SetScale(_selectedX, _selectedY, _highlightAnim.GetScalePulse());
        }
    }

    public void DrawEffects(IEnumerable<Destroyer> destroyers)
    {
        foreach (var destroyer in destroyers)
            boardRenderer.DrawDestroyer(destroyer.ScreenPosition, destroyer.ColorType.GetColor(), board.CellSize);
    }

    private Vector2 GetCellScreenPos(int x, int y)
    {
        return board.BoardPosition + new Vector2(x * board.CellSize, y * board.CellSize);
    }
}