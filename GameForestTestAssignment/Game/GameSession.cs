using System;
using GameForestTestAssignment.Core;
using GameForestTestAssignment.Game.Animations;
using GameForestTestAssignment.Game.Bonuses;
using GameForestTestAssignment.Game.Effects;
using GameForestTestAssignment.Game.GameLogic;
using GameForestTestAssignment.Game.MatchDetection;
using GameForestTestAssignment.Game.PlayerInput;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameForestTestAssignment.Game;

/// <summary>
///     Game session facade: owns whole game logic,
///     hides details of Board / GameEngine / Screen managers.
/// </summary>
public sealed class GameSession
{
    private const float RoundDurationSeconds = 60f;

    private const int BoardMarginX = 100;
    private const int BoardMarginY = 120;
    private const int BoardTopOffset = 80;
    private const int MinCellSize = 40;
    private const int MaxCellSize = 80;

    private const float TimerBarWidth = 200f;
    private const float TimerBarHeight = 6f;
    private const float TimerBarY = 18f;
    private const float ScoreMargin = 20f;
    private const float TimerMargin = 20f;
    private const float TimerWarningThreshold = 10f;

    private static readonly Color BackgroundColor = new(12, 12, 28);
    private static readonly Color TimerBarBackground = new(40, 40, 60);
    private static readonly Color TimerBarColor = Color.Cyan;
    private static readonly Color TimerBarWarning = Color.OrangeRed;

    private readonly Board _board;
    private readonly BoardRenderer _boardRenderer;
    private readonly DestroyerService _destroyerService;
    private readonly GameEngine _engine;
    private readonly ParticlePool _particlePool;
    private readonly RenderStateApplier _renderStateApplier;
    private readonly ScoreManager _scoreManager;
    private readonly TimerManager _timerManager;

    public GameSession(SpriteBatch spriteBatch, Viewport viewport)
    {
        var layout = CreateLayout(viewport);
        _board = new Board();
        _board.GenerateRandomBoard();

        _boardRenderer = new BoardRenderer(spriteBatch, layout);
        _scoreManager = new ScoreManager();
        _timerManager = new TimerManager(RoundDurationSeconds);
        _particlePool = new ParticlePool();

        var animationManager = new AnimationManager();
        var effects = new BoardEffectQueue();
        var bonusManager = new BonusManager(effects);
        var removalService =
            new RemovalService(_board, layout, animationManager, _scoreManager, _particlePool, effects);
        var bombService = new BombService(layout, _particlePool, effects);
        _destroyerService = new DestroyerService(_board, effects);
        var resolution = new ResolutionProcessor(
            _board, animationManager, effects, bonusManager, removalService, bombService, _destroyerService);

        var gravityService = new GravityService(_board, animationManager);
        var stateMachine = new GameStateMachine(_board, new MatchDetector(), animationManager, gravityService);
        _renderStateApplier = new RenderStateApplier(_boardRenderer, animationManager);

        var inputHandler = new InputHandler(layout.CellSize, layout.Position);
        inputHandler.CellSelected += _renderStateApplier.SelectCell;
        inputHandler.SelectionCleared += _renderStateApplier.ClearSelection;

        _engine = new GameEngine(stateMachine, inputHandler, animationManager, resolution);
    }

    public int Score => _scoreManager.Score;
    public bool IsGameOver => _timerManager.IsExpired;

    public void Update(GameTime gameTime)
    {
        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        _timerManager.Update(deltaTime);
        _engine.Update(deltaTime);
        _particlePool.Update(deltaTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _renderStateApplier.ApplyRenderState();
        _boardRenderer.DrawBoard(_board, _renderStateApplier.RenderState);
        _renderStateApplier.DrawEffects(_destroyerService.Destroyers);
        _particlePool.Draw(spriteBatch);
    }

    public void DrawUi(SpriteBatch spriteBatch, SpriteFont font, Viewport viewport)
    {
        DrawScore(spriteBatch, font);
        DrawTimer(spriteBatch, font, viewport);
        DrawTimerBar(spriteBatch, viewport);
    }

    public void DrawBackground(SpriteBatch spriteBatch, Viewport viewport)
    {
        spriteBatch.Draw(
            PersistentResources.WhitePixel,
            new Rectangle(0, 0, viewport.Width, viewport.Height),
            BackgroundColor);
    }

    private static BoardLayout CreateLayout(Viewport viewport)
    {
        var availableWidth = viewport.Width - BoardMarginX;
        var availableHeight = viewport.Height - BoardMarginY;

        var cellSize = Math.Min(availableWidth / Board.Width, availableHeight / Board.Height);
        cellSize = Math.Clamp(cellSize, MinCellSize, MaxCellSize);

        var boardPosition = new Vector2(
            (viewport.Width - cellSize * Board.Width) / 2f,
            BoardTopOffset + (availableHeight - cellSize * Board.Height) / 2f
        );

        return new BoardLayout(cellSize, boardPosition);
    }

    private void DrawScore(SpriteBatch spriteBatch, SpriteFont font)
    {
        var text = $"Score: {_scoreManager.Score}";
        spriteBatch.DrawString(font, text,
            new Vector2(ScoreMargin, ScoreMargin),
            Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
    }

    private void DrawTimer(SpriteBatch spriteBatch, SpriteFont font, Viewport viewport)
    {
        var remaining = _timerManager.TimeRemaining;
        var text = $"Time: {remaining:F1}s";
        var color = remaining <= TimerWarningThreshold ? TimerBarWarning : Color.White;
        var size = font.MeasureString(text);

        spriteBatch.DrawString(font, text,
            new Vector2(viewport.Width - size.X - TimerMargin, TimerMargin),
            color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
    }

    private void DrawTimerBar(SpriteBatch spriteBatch, Viewport viewport)
    {
        var remaining = _timerManager.TimeRemaining;
        var ratio = Math.Clamp(remaining / RoundDurationSeconds, 0f, 1f);

        var barX = viewport.Width / 2f - TimerBarWidth / 2f;
        var backgroundRect = new Rectangle(
            (int)barX, (int)TimerBarY, (int)TimerBarWidth, (int)TimerBarHeight);
        var fillRect = new Rectangle(
            (int)barX, (int)TimerBarY, (int)(TimerBarWidth * ratio), (int)TimerBarHeight);
        var fillColor = remaining <= TimerWarningThreshold ? TimerBarWarning : TimerBarColor;

        spriteBatch.Draw(PersistentResources.WhitePixel, backgroundRect, TimerBarBackground);
        spriteBatch.Draw(PersistentResources.WhitePixel, fillRect, fillColor);
    }
}