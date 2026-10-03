using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameForestTestAssignment.Core.ScreenSystem;

public sealed class ScreenManager(GraphicsDevice device) : IScreenNavigation, IDisposable
{
    // List used because collection needs iteration in Draw Update.
    private readonly List<Screen> _screenStack = [];
    private readonly SpriteBatch _spriteBatch = new(device);

    public void PushScreen(Screen screen)
    {
        _screenStack.Add(screen);

        screen.LoadContent(_spriteBatch, device.Viewport, this);
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

    public void Dispose()
    {
        foreach (var screen in _screenStack)
            screen.UnloadContent();

        _screenStack.Clear();
        _spriteBatch.Dispose();
    }
}
