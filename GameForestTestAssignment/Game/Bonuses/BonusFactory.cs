using System;
using System.ComponentModel;

namespace GameForestTestAssignment.Game.Bonuses;

public static class BonusFactory
{
    public static Bonus Create(
        BonusType type, CellType colorType, BonusOrientation orientation = BonusOrientation.Horizontal
    )
    {
        return type switch
        {
            BonusType.Line => new LineBonus(colorType, orientation),
            BonusType.Bomb => new BombBonus(colorType),
            BonusType.None => throw new InvalidEnumArgumentException(),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown bonus type")
        };
    }
}