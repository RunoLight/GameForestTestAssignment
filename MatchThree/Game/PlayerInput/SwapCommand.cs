namespace MatchThree.Game.PlayerInput;

public readonly record struct SwapCommand(int StartX, int StartY, int EndX, int EndY);