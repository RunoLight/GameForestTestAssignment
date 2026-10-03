using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameForestTestAssignment.Core.ScreenSystem;

public class ScreenManager(GraphicsDevice device) : IScreenNavigation
{
    // List used because collection needs iteration in Draw Update.
    private readonly List<Screen> _screenStack = [];

    public void PushScreen(Screen screen)
    {
        _screenStack.Add(screen);

        screen.LoadContent(new SpriteBatch(device), device.Viewport, this);
        screen.OnEnter();
    }

    public void PopToFirstScreen()
    {
        if (_screenStack.Count == 0)
            return;

        while (_screenStack.Count > 1)
        {
            _screenStack[^1].UnloadContent();
            _screenStack.RemoveAt(_screenStack.Count - 1);
        }

        _screenStack[0].OnEnter();
    }

    public void Update(GameTime gameTime)
    {
        if (_screenStack.Count > 0)
            _screenStack[^1].Update(gameTime);
    }

    public void Draw()
    {
        foreach (var screen in _screenStack)
            screen.Draw();
    }
}