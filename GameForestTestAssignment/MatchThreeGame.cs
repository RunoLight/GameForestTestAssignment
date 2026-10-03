#region

using GameForestTestAssignment.Core;
using GameForestTestAssignment.Core.ScreenSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#endregion

namespace GameForestTestAssignment;

public class MatchThreeGame : Microsoft.Xna.Framework.Game
{
    // ReSharper disable once NotAccessedField.Local
    private readonly GraphicsDeviceManager _graphics;
    private ScreenManager _screenManager;

    public MatchThreeGame()
    {
        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = 1280, PreferredBackBufferHeight = 720
        };
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();
        _screenManager = new ScreenManager(GraphicsDevice);

        _screenManager.PushScreen(new MainMenuScreen());
    }

    protected override void LoadContent()
    {
        PersistentResources.Initialize(GraphicsDevice, Content);
    }

    protected override void UnloadContent()
    {
        PersistentResources.Dispose();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _screenManager.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        _screenManager.Draw();
        base.Draw(gameTime);
    }
}