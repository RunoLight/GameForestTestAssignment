using MatchThree.Game.Bonuses;

namespace MatchThree.Game;

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