using GameForestTestAssignment.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameForestTestAssignment.Core.ScreenSystem;

public class GameScreen : Screen
{
    private bool _gameOverTriggered;
    private GameSession _session;

    public override void OnEnter()
    {
        _gameOverTriggered = false;
    }

    public override void LoadContent(SpriteBatch spriteBatch, Viewport viewport, IScreenNavigation screenNavigation)
    {
        base.LoadContent(spriteBatch, viewport, screenNavigation);
        _session = new GameSession(SpriteBatch, viewport);
    }

    public override void Update(GameTime gameTime)
    {
        if (_gameOverTriggered)
            return;

        _session.Update(gameTime);

        if (_session.IsGameOver)
        {
            _gameOverTriggered = true;
            ScreenNavigation.PushScreen(new GameOverScreen(_session.Score));
        }
    }

    public override void Draw()
    {
        _session.Draw(SpriteBatch, Font, Viewport);
    }
}