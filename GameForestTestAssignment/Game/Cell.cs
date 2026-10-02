#region

using GameForestTestAssignment.Game.Bonuses;

#endregion

namespace GameForestTestAssignment.Game;

public class Cell
{
    public Cell()
    {
        CellType = CellType.None;
    }

    public Cell(CellType cellType)
    {
        CellType = cellType;
        Bonus = null;
    }

    public int Row { get; set; }
    public int Col { get; set; }
    public CellType CellType { get; set; }
    public Bonus Bonus { get; set; }

    public bool IsEmpty => CellType == CellType.None;

    public void Reset()
    {
        Row = Col = 0;
        CellType = CellType.None;
        Bonus = null;
    }
}