#region

using System;
using System.Collections.Generic;
using GameForestTestAssignment.Game.Animations;
using GameForestTestAssignment.Game.Bonuses;

#endregion

namespace GameForestTestAssignment.Game.GameLogic;

public class GravityService(Board board, AnimationManager animationManager, BonusManager bonusManager)
{
    public void Apply(Dictionary<(int X, int Y), FallAnimation> fallAnimations)
    {
        fallAnimations.Clear();
        bonusManager.Clear();

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
                    var anim = new FallAnimation(readPos * board.CellSize, writePos * board.CellSize, duration);
                    fallAnimations[(x, writePos)] = anim;
                    animationManager.AddAnimation(anim);
                }

                writePos--;
            }

            var emptyCount = writePos + 1;
            for (var fillPos = writePos; fillPos >= 0; fillPos--)
            {
                board[x, fillPos].CellType = CellTypeExtensions.RandomType();
                board[x, fillPos].Bonus = null;
                board[x, fillPos].Row = fillPos;
                board[x, fillPos].Col = x;

                var startY = (fillPos - emptyCount) * board.CellSize;
                var endY = fillPos * board.CellSize;
                var duration = Math.Max(160f, emptyCount * 90f);
                var anim = new FallAnimation(startY, endY, duration);
                fallAnimations[(x, fillPos)] = anim;
                animationManager.AddAnimation(anim);
            }
        }
    }
}