using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    ChessBoard board;
    List<Deck.Card> deck;

    public HandGraphic handG;
    public ChessboardGraphic boardG;

    private bool flipped = false;
    private float timer = -1f;

    private List<Deck.Card> playerHand = new List<Deck.Card>();

    public Dictionary<Vector2Int, ChessBoard> availableMoves;

    public GameObject[] scoreUI;

    // Start is called before the first frame update
    void Start()
    {
        board = ChessBoard.initial();
        deck = Deck.GetDeck();
        boardG.ApplyPieces(board, flipped);

        for (int i = 0; i != 5; ++i)
        {
            playerHand.Add(deck[0]);
            deck.RemoveAt(0);
        }
        handG.makeHand(playerHand);
        updateScoreDisp();
    }

    int getPlayerHandLevel()
    {
        if(Deck.IsFourOfAKind(playerHand))
        {
            return 5;
        }
        if (Deck.IsFullHouse(playerHand))
        {
            return 4;
        }
        if (Deck.IsFlush(playerHand))
        {
            return 3;
        }
        if (Deck.IsStraight(playerHand))
        {
            return 2;
        }
        if (Deck.IsThreeOfAKind(playerHand))
        {
            return 1;
        }
        return 0;
    }

    void updateScoreDisp()
    {
        for (int i = 0; i < scoreUI.Length; ++i)
        {
            foreach (var v in scoreUI[i].GetComponentsInChildren<Image>())
            {
                v.color = (i + 1 != getPlayerHandLevel()) ? Color.white : Color.yellow;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!flipped)
        {
            // await player move

        }
        else
        {
            timer += Time.deltaTime;
            if (timer > 0)
            {
                applyMove(board.generateMoves()[0]);
                timer = -1;
            }
        }
    }

    private void applyMove(ChessBoard next)
    {
        board = next;
        board = board.flipBoard();
        flipped = !flipped;
        boardG.ApplyPieces(board, flipped);
    }

    // find what actually changed on the boardstate and highlight. Exlude start sq
    public Vector2Int getMoveDiff(ChessBoard next, int xStart, int yStart)
    {
        for (int x = 0; x != ChessBoard.BOARD_DIM; x++)
        {
            for (int y = 0; y != ChessBoard.BOARD_DIM; y++)
            {
                if (board.board[x, y] != next.board[x, y] && (x != xStart || y != yStart))
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return new Vector2Int(-1, -1);
    }

    public void tileClicked(int x, int y)
    {
        if(flipped)
        {
            // not player turn
            return;
        }

        if (availableMoves != null)
        {
            // look up the move and apply it
            if(availableMoves.ContainsKey(new Vector2Int( x, y )))
            {
                // move's good
                applyMove(availableMoves[new Vector2Int(x, y)]);
            }
            availableMoves = null;
            // clear movediff
            foreach (var c in FindObjectsOfType<ChessTileListener>())
            {
                var baseCol = c.GetComponent<SpriteRenderer>().color;
                c.GetComponent<SpriteRenderer>().color = new Color(baseCol.r, baseCol.g, baseCol.b, 1f);
            }
        }
        else
        {
            availableMoves = new Dictionary<Vector2Int, ChessBoard> ();
            foreach (var v in board.movesForPiece(x, y))
            {
                var diff = getMoveDiff(v,x,y);
                availableMoves[diff] = v;
                // highlight the getMoveDiff
                foreach(var c in FindObjectsOfType<ChessTileListener>())
                {
                    if(c.tileX == diff[0] && c.tileY == diff[1])
                    {
                        var baseCol = c.GetComponent<SpriteRenderer>().color;
                        c.GetComponent<SpriteRenderer>().color = new Color(baseCol.r, baseCol.g, baseCol.b, 0.5f);
                    }
                }
            }
        }
    }
}
