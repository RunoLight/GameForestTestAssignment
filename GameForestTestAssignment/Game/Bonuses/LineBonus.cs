namespace GameForestTestAssignment.Game.Bonuses;

public class LineBonus(CellType colorType, Orientation orientation) : Bonus(colorType)
{
    public Orientation Orientation { get; } = orientation;
}
