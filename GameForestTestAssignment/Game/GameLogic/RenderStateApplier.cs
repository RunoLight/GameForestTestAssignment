using System.Collections.Generic;
using GameForestTestAssignment.Game.Animations;
using GameForestTestAssignment.Game.Effects;

namespace GameForestTestAssignment.Game.GameLogic;

public class RenderStateApplier(BoardRenderer boardRenderer, AnimationManager animationManager)
{
    private HighlightAnimation _highlight;

    public BoardRenderState RenderState { get; } = new(Board.Width, Board.Height);

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
        RenderState.Reset();
        animationManager.ApplyTo(RenderState);
    }

    public void DrawEffects(IEnumerable<Destroyer> destroyers)
    {
        foreach (var destroyer in destroyers)
            boardRenderer.DrawDestroyer(destroyer.X, destroyer.Y, destroyer.ColorType.GetColor());
    }
}