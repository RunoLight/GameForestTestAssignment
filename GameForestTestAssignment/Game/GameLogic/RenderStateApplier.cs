using System.Collections.Generic;
using GameForestTestAssignment.Game.Animations;
using GameForestTestAssignment.Game.Effects;

namespace GameForestTestAssignment.Game.GameLogic;

public class RenderStateApplier(Board board, BoardRenderer boardRenderer, AnimationManager animationManager)
{
    private HighlightAnimation _highlight;

    public void SelectCell(int x, int y)
    {
        ClearSelection();
        _highlight = new HighlightAnimation(x, y);
        animationManager.Play(_highlight);
    }

    public void ClearSelection()
    {
        if (_highlight == null)
            return;

        animationManager.Stop(_highlight);
        _highlight = null;
    }

    public void ApplyRenderState()
    {
        board.ResetRenderStates();
        animationManager.ApplyTo(board.RenderState);
    }

    public void DrawEffects(IEnumerable<Destroyer> destroyers)
    {
        foreach (var destroyer in destroyers)
            boardRenderer.DrawDestroyer(destroyer.ScreenPosition, destroyer.ColorType.GetColor(), board.CellSize);
    }
}
