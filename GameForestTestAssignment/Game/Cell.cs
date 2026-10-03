using GameForestTestAssignment.Game.Bonuses;

namespace GameForestTestAssignment.Game;

public class Cell
{
    public CellType CellType { get; set; } = CellType.None;
    public Bonus Bonus { get; set; }

    public bool IsEmpty => CellType == CellType.None;

    public void Reset()
    {
        CellType = CellType.None;
        Bonus = null;
    }
}