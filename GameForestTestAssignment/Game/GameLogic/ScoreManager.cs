namespace GameForestTestAssignment.Game.GameLogic;

public class ScoreManager
{
    private const int CellReward = 10;
    public int Score { get; private set; }

    public int AwardCell()
    {
        Score += CellReward;
        return CellReward;
    }
}