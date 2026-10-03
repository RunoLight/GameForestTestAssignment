namespace GameForestTestAssignment.Game.Bonuses;

public abstract class Bonus(CellType colorType)
{
    public CellType ColorType { get; } = colorType;
}