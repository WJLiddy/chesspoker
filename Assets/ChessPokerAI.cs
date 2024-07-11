using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Extension
{
    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}

public class ChessPokerAI : MonoBehaviour
{
    public static int materialEval(ChessBoard c)
    {
        var sum = 0;
        for (int x = 0; x != ChessBoard.BOARD_DIM; ++x)
        {
            for (int y = 0; y != ChessBoard.BOARD_DIM; ++y)
            {
                switch(c.board[x,y])
                {
                    // classical chess vals. slightly favor pushing pawns.
                    case 'K': sum += 10000; break;
                    case 'Q': sum += 90; break;
                    case 'R': sum += 50; break;
                    case 'B': sum += 30; break;
                    case 'N': sum += 30; break;
                    case 'P': sum += 10 + ( 3 * (4 - y)); break;

                    case 'k': sum -= 10000; break;
                    case 'q': sum -= 90; break;
                    case 'r': sum -= 50; break;
                    case 'b': sum -= 30; break;
                    case 'n': sum -= 30; break;
                    case 'p': sum -= (10 + (3 * (y - 1))); break;
                }
            }
        }
        return sum;
    }
      
    // next, add handlevel, only pass in what's moveable.
    // huge improvement would be to add 1 pt to eval for a check
    // returns a null chessboard if we should try to remove the hand.
    public static Tuple<ChessBoard,int> minimax(ChessBoard node, int depth, bool maximize, List<Deck.Card> hand)
    {
        if (depth == 0 || gameOver(node))
        {
            if (maximize)
            {
                // if we are in maximize that means we got here from a minimize node.
                return new Tuple<ChessBoard, int>(node,materialEval(node));
            } 
            else
            {
                // if we are in minimize that means we got here from a maximize node.
                return new Tuple<ChessBoard, int>(node,-materialEval(node));
            }
        }

        if (maximize)
        {
            Tuple<ChessBoard, int> bestMove = new Tuple<ChessBoard, int>(null, int.MinValue);

            //shuffle eq moves, so equivalent moves are not preferred.
            var shuf = node.generateMoves(GameManager.getHandLevel(hand));
            shuf.Shuffle();
            foreach (var n in shuf)
            {
                // improve pair and straighr
                var result = minimax(n.flipBoard(), depth - 1, false, hand);
                if (result.Item2 > bestMove.Item2)
                {
                    bestMove = new Tuple<ChessBoard, int>(n, result.Item2);
                }
            }
            return bestMove;
        }
        else
        {
            Tuple<ChessBoard, int> worstMove = new Tuple<ChessBoard, int>(null, int.MaxValue);
            // always assume the opponent has max move
            foreach (var n in node.generateMoves(10))
            {
                var result = minimax(n.flipBoard(), depth - 1, true, hand);
                if (result.Item2 < worstMove.Item2)
                {
                    worstMove = new Tuple<ChessBoard, int>(n, result.Item2);
                }
            }
            return worstMove;
        }
    }

    static bool gameOver(ChessBoard state)
    {
        return materialEval(state) < -500 || materialEval(state) > 500;
    }

}
