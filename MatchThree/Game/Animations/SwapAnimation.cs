using MatchThree.Utils;
using Microsoft.Xna.Framework;

namespace MatchThree.Game.Animations;

public class SwapAnimation(int x, int y, Vector2 startOffset, float duration) : AnimationWithDuration(duration)
{
    public override void Apply(BoardRenderState renderState)
    {
        var t = Easing.EaseOutQuad(Progress);
        renderState.SetOffset(x, y, Vector2.Lerp(startOffset, Vector2.Zero, t));
    }
}
