# GameForestTestAssignment

A match-3 game built with MonoGame 3.8 (.NET 9, DirectX). 8×8 board, five piece types, 60-second rounds.

## Build and run

Requires Windows and the .NET 9 SDK.

```bash
cd GameForestTestAssignment
dotnet tool restore          # MGCB content builder, needed once
dotnet run
```

Run the tests from the repository root:

```bash
dotnet test
```

## Controls

- Click a piece, then click an adjacent one to swap them. Clicking a non-adjacent piece moves the selection to it; clicking the selected piece again or clicking outside the board clears the selection.
- You can also drag a piece towards its neighbour (mouse swipe).
- `Esc` exits the game.

## Rules

- A swap counts only if it forms a line of three or more identical pieces; otherwise the pieces swap back.
- Every destroyed piece is worth 10 points, including pieces destroyed by bonuses.
- Destroyed pieces are replaced by the pieces above falling down, and new pieces drop in from the top. Lines formed by falling pieces are cleared as well (cascades).
- If no moves are left, the board is shuffled, keeping all pieces and bonuses.
- When time runs out, input is locked, the current cascade plays out to the end, and only then the Game Over screen is shown.

### Bonuses

| Combination | Bonus |
|---|---|
| 4 in a row | Line, oriented along the match |
| 5 or more in a row | Bomb |
| Horizontal and vertical lines crossing (L/T) | Bomb at the intersection |

The bonus appears in the cell the player moved a piece into; during cascades it appears in the middle of the line.

- **Line** releases two destroyers flying in opposite directions along its axis. They destroy every piece on their way to the edge of the board.
- **Bomb** explodes after 0.25 s and destroys the 3×3 area around it.
- A bonus is triggered when its cell is part of a match, is hit by a destroyer or caught in an explosion, so chain reactions are possible. Each bonus triggers only once.

## Architecture

```
GameForestTestAssignment/
  Core/                 screens (menu, game, Game Over), button, shared resources
  Game/
    Board, Cell, ...    board model
    MatchDetection/     match detection, move resolution planning, possible move search
    GameLogic/          turn state machine and resolution services
    Bonuses/            bonuses and their activation
    Animations/         animations and the animation manager
    Effects/            destroyers, particles, score popups, screen shake
    PlayerInput/        mouse → swap command
    GameSession.cs      composition root of the game screen
GameForestTestAssignment.Tests/   NUnit tests for the game logic
```

Key decisions:

- **`GameSession`** creates and wires all objects of the game screen. Other classes receive their dependencies through constructors and never create them themselves.
- **`GameStateMachine`** drives a turn through `Idle → Swapping → (SwapBack | Resolving) → Falling → Idle`. `GameEngine` only switches between these states and passes the resolution plan on.
- **`MatchResolver`** builds an immutable `ResolutionPlan` from the board state: which cells to remove, where to spawn bonuses and which bonuses to activate.
- **`ResolutionProcessor`** executes the plan through an effect queue (`BoardEffectQueue`). Removing a cell, activating a bonus, launching a destroyer and exploding a bomb are separate entries. The services (`RemovalService`, `BonusManager`, `BombService`, `DestroyerService`) never call each other; they push effects into the queue, so chain reactions work without circular dependencies.
- **`AnimationManager`** is the single owner of animations. Game logic can wait for all animations of a given type or subscribe to the completion of a specific one: a cell is cleared in the model only after its disappear animation has finished.
- The model (`Board`), pixel geometry (`BoardLayout`) and per-cell visual state (`BoardRenderState`, offsets in cells) are kept separate, so the logic can be tested without graphics.
- All time values are in seconds.

## Tests

The tests cover the game logic without graphics or input:

- match and intersection detection;
- bonus spawning rules;
- possible move search, board generation and shuffling;
- gravity;
- bonus activation and chain reactions (line, bomb, line → bomb, line → line), scoring;
- state machine transitions: successful and failed swaps, cascades, shuffling when no moves are left;
- destroyer, animation manager, timer.

Boards in tests are described with strings (`TestBoard.FromRows("RRGR")`).

## Screenshots

<img width="1273" height="715" alt="image" src="https://github.com/user-attachments/assets/9c9b9a99-41a0-4ce2-b168-b08d18e413cf" />
<img width="1276" height="714" alt="image" src="https://github.com/user-attachments/assets/cee103fb-241e-4d6f-8310-49a4e82d1edc" />
<img width="1273" height="714" alt="image" src="https://github.com/user-attachments/assets/079832a9-da9b-4ae1-b1bc-6622ed976508" />


