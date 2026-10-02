namespace GameForestTestAssignment.Game.Bonuses;

public class LineBonus(CellType colorType, BonusOrientation orientation) : Bonus(BonusType.Line, colorType)
{
    public BonusOrientation Orientation { get; } = orientation;
}