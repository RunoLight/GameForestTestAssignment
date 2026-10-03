using System;
using GameForestTestAssignment.Game.Animations;

namespace GameForestTestAssignment.Game.GameLogic;

public class GravityService(IBoard board, AnimationManager animationManager)
{
    public void Apply()
    {
        for (var x = 0; x < Board.Width; x++)
        {
            var writePos = Board.Height - 1;
            for (var readPos = Board.Height - 1; readPos >= 0; readPos--)
            {
                if (board[x, readPos].IsEmpty)
                    continue;

                if (writePos != readPos)
                {
                    board.MoveCell(x, readPos, x, writePos);
                    var duration = Math.Max(120f, Math.Abs(writePos - readPos) * 90f);
                    var startOffsetY = (readPos - writePos) * board.CellSize;
                    animationManager.Play(new FallAnimation(x, writePos, startOffsetY, duration));
                }

                writePos--;
            }

            var emptyCount = writePos + 1;
            for (var fillPos = writePos; fillPos >= 0; fillPos--)
            {
                board[x, fillPos].CellType = CellTypeExtensions.RandomType();
                board[x, fillPos].Bonus = null;

                var duration = Math.Max(160f, emptyCount * 90f);
                var startOffsetY = -emptyCount * board.CellSize;
                animationManager.Play(new FallAnimation(x, fillPos, startOffsetY, duration));
            }
        }
    }
}
