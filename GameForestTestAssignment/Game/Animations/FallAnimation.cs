using GameForestTestAssignment.Utils;
using Microsoft.Xna.Framework;

namespace GameForestTestAssignment.Game.Animations;

public class FallAnimation(int x, int y, float startOffsetY, float duration) : AnimationWithDuration(duration)
{
    public override void Apply(BoardRenderState renderState)
    {
        var t = Easing.EaseOutCubic(Progress);
        renderState.SetOffset(x, y, new Vector2(0f, startOffsetY * (1f - t)));
    }
}
