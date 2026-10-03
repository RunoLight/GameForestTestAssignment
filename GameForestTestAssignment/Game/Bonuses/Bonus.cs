namespace GameForestTestAssignment.Game.Bonuses;

public abstract class Bonus(BonusType type, CellType colorType)
{
    public BonusType Type { get; } = type;
    public CellType ColorType { get; } = colorType;
}
