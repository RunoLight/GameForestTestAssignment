#region

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace GameForestTestAssignment.Core;

public static class PersistentResources
{
    public static Texture2D WhitePixel { get; private set; } = null!;
    public static SpriteFont Font { get; private set; } = null!;

    public static void Initialize(GraphicsDevice device, ContentManager content)
    {
        WhitePixel = new Texture2D(device, 1, 1);
        WhitePixel.SetData([Color.White]);

        Font = content.Load<SpriteFont>("Fonts");
    }
}