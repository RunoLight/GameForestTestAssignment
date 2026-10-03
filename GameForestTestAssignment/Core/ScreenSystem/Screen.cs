using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameForestTestAssignment.Core.ScreenSystem;

public abstract class Screen
{
    protected IScreenNavigation ScreenNavigation { get; private set; }
    protected Viewport Viewport { get; private set; }
    protected SpriteBatch SpriteBatch { get; private set; }
    protected SpriteFont Font { get; private set; }

    public virtual void OnEnter()
    {
    }

    public virtual void Update(GameTime gameTime)
    {
    }

    public virtual void Draw()
    {
    }

    public virtual void LoadContent(SpriteBatch spriteBatch, Viewport viewport, IScreenNavigation screenNavigation)
    {
        Viewport = viewport;
        SpriteBatch = spriteBatch;
        ScreenNavigation = screenNavigation;
        Font = PersistentResources.Font;
    }

    public virtual void UnloadContent()
    {
        SpriteBatch?.Dispose();
        Font = null;
        SpriteBatch = null;
    }
}