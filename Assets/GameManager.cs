using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static int DIFFICULTY = 2;

    ChessBoard board;
    List<Deck.Card> deck;
    List<bool> raised = new List<bool>();

    public HandGraphic handG;
    public ChessboardGraphic boardG;

    private bool flipped = false;
    private float timer = -1f;

    private List<Deck.Card> playerHand = new List<Deck.Card>();
    private List<Deck.Card> aiHand = new List<Deck.Card>();

    public Dictionary<Vector2Int, ChessBoard> availableMoves;

    public GameObject[] scoreUI;

    public TMPro.TMP_Text exchange;

    public TMPro.TMP_Text gameOverText;
    public bool gameOver;

    public TMPro.TMP_Text descText;
    // Start is called before the first frame update
    void Start()
    {
        board = ChessBoard.initial();
        deck = Deck.GetDeck();

        for (int i = 0; i != 5; ++i)
        {
            playerHand.Add(deck[0]);
            deck.RemoveAt(0);
            raised.Add(false);
        }
        handG.makeHand(playerHand, raised);
        updateScoreDisp(false);
        boardG.ApplyPieces(board, flipped, getHandLevel(playerHand));
        exchange.color = Color.gray;

        switch(DIFFICULTY)
        {
            case 2:
                descText.text = "CHESS POKER DELUXE";
                Camera.main.backgroundColor = new Color(0f, 0f, 0.5f);
                break;

            case 3:
                descText.text = "BONUS CHESS POKER";
                Camera.main.backgroundColor = new Color(0f, 0.5f, 0.5f);
                break;

            case 4:
                descText.text = "DOUBLE BONUS CHESS POKER";
                Camera.main.backgroundColor = new Color(0.5f, 0.25f, 0f);
                break;

            case 5:
                descText.text = "TRIPLE BONUS CHESS POKER";
                Camera.main.backgroundColor = new Color(0.25f, 0f, 0f);
                break;
        }
    }

    public static int getHandLevel(List<Deck.Card> hand)
    {
        if(Deck.IsFlush(hand) || Deck.IsStraight(hand))
        {
            return 4;
        }
        if (Deck.IsThreeOfAKind(hand))
        {
            return 3;
        }
        if (Deck.IsTwoPair(hand))
        {
            return 2;
        }
        if (Deck.IsOnePair(hand))
        {
            return 1;
        }
        return 0;
    }

    void updateScoreDisp(bool clear)
    {
        for (int i = 0; i < scoreUI.Length; ++i)
        {
            foreach (var v in scoreUI[i].GetComponentsInChildren<Image>())
            {
                scoreUI[i].GetComponentInChildren<TMP_Text>(v).color = (clear || i > getHandLevel(playerHand)) ? Color.white : Color.yellow;
                v.color = (clear || i > getHandLevel(playerHand)) ? Color.white : Color.yellow;
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
                // eval aihand
                applyMove(ChessPokerAI.minimax(board, DIFFICULTY,true, aiHand).Item1);
                timer = -1;
            }
        }
    }

    private void applyMove(ChessBoard next)
    {
        board = next;
        board = board.flipBoard();
        flipped = !flipped;
        boardG.ApplyPieces(board, flipped, flipped ? -1 : getHandLevel(playerHand));
        updateScoreDisp(flipped);
        if(board.gameOverActive())
        {
            // show text
            gameOverText.text = flipped ? "WHITE WINS." : "BLACK WINS.";
        }

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

                // new cards
                raised = new List<bool> { true, true, true, true, true };
                exchangeCards();
            }
            // no matter what, 
            // unhighlight the moves
            availableMoves = null;
            foreach (var c in FindObjectsOfType<ChessTileListener>())
            {
                var baseCol = c.GetComponent<SpriteRenderer>().color;
                c.GetComponent<SpriteRenderer>().color = new Color(baseCol.r, baseCol.g, baseCol.b, 1f);
            }
        }
        else
        {
            // only piece permitted is based on player's hand value
            if (!pieceMatchesValue(board.board[x,y], getHandLevel(playerHand)))
            {
                return;
            }

            availableMoves = new Dictionary<Vector2Int, ChessBoard>();

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

    public static bool pieceMatchesValue(char c, int hVal)
    {
        switch(hVal)
        {
            case 4: return c == 'P' || c == 'K' || c == 'N' || c == 'B' || c == 'R' || c == 'Q';
            case 3: return c == 'P' || c == 'K' || c == 'N' || c == 'B' || c == 'R';
            case 2: return c == 'P' || c == 'K' || c == 'N' || c == 'B';
            case 1: return c == 'P' || c == 'K' || c == 'N';
            case 0: return c == 'P' || c == 'K';
        }
        return false;
    }

    public void cardClicked(int index)
    {
        if (flipped)
        {
            // not player turn
            return;
        }

        raised[index] = !raised[index];
        handG.makeHand(playerHand, raised);
        if(raised.Any(r => r))
        {
            exchange.color = Color.white;
        }
        else
        {
            exchange.color = Color.gray;
        }
    }

    public void doExchange()
    {
        if(flipped)
        {
            // not player turn
            return;
        }
        exchangeCards();
        applyMove(board);
    }

    private void exchangeCards()
    {
        exchange.color = Color.gray;
        for (int i = 0; i != 5; ++i)
        {
            if (raised[i])
            {
                Deck.Card oldCard = playerHand[i];
                playerHand[i] = deck[0];
                deck.RemoveAt(0);
                // put on bottom of deck
                deck.Add(oldCard);
                raised[i] = false;
            }
        }
        handG.makeHand(playerHand, raised);
        Deck.Shuffle(deck);
    }

    //private 
}
