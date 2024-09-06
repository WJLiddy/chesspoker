using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static int DIFFICULTY = 2;

    ChessBoard board;
    ChessBoard aiNextBoard;
    List<Deck.Card> deck;
    private List<Deck.Card> playerHand = new List<Deck.Card>();
    private List<Deck.Card> aiHand = new List<Deck.Card>();

    List<bool> playerRaisedCard = new List<bool>();
    List<bool> aiRaisedCard = new List<bool>();

    public HandGraphic playerHandGraphic;
    public HandGraphic aiHandGraphic;

    public ChessboardGraphic boardG;

    private bool flipped = false;
    private float timer = -2f;

    public bool gameOverFlag = false;

    public Dictionary<Vector2Int, ChessBoard> availablePlayerMoves;

    public GameObject[] scoreUI;

    public TMPro.TMP_Text exchange;
    public TMPro.TMP_Text nextGameText;

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

            aiHand.Add(deck[0]);
            deck.RemoveAt(0);

            playerRaisedCard.Add(false);
        }

        playerHandGraphic.renderHand(playerHand, playerRaisedCard, true);
        aiHandGraphic.renderHand(aiHand, new List<bool> { false, false, false, false, false }, false);

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

            if (timer > -1.5 && aiNextBoard == null)
            {
                Debug.Log("AI HAS " + getHandLevel(aiHand));
                aiNextBoard = ChessPokerAI.minimax(board, DIFFICULTY, true, aiHand).Item1;
                aiRaisedCard = getAIExchange(!aiNextBoard.flipBoard().equals(board));
                aiHandGraphic.renderHand(aiHand, aiRaisedCard, true);
            }
                  
            if (timer > 0)
            {
                // AI chose not to make a move - so improve hand.
                exchangeCardsAI(); 
                applyMove(aiNextBoard);
                timer = -2;
                aiNextBoard  = null;
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
            gameOverFlag = true;
            // show text
            gameOverText.text = flipped ? "WHITE WINS." : "BLACK WINS.";
            // show game over
            nextGameText.text = flipped ? "NEXT GAME" : "TRY AGAIN";
            if (flipped)
            {
                DIFFICULTY += 1;
            }
        }
    }

    public void nextGame()
    {
        if(!gameOverFlag)
        {
            return;
        }
        SceneManager.LoadScene(0);
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

        if (availablePlayerMoves != null)
        {
            // look up the move and apply it
            if(availablePlayerMoves.ContainsKey(new Vector2Int( x, y )))
            {
                // move's good
                applyMove(availablePlayerMoves[new Vector2Int(x, y)]);

                // new cards
                playerRaisedCard = new List<bool> { true, true, true, true, true };
                exchangeCards();
            }
            // no matter what, 
            // unhighlight the moves
            availablePlayerMoves = null;
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

            availablePlayerMoves = new Dictionary<Vector2Int, ChessBoard>();

            foreach (var v in board.movesForPiece(x, y))
            {
                var diff = getMoveDiff(v,x,y);
                availablePlayerMoves[diff] = v;
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

        playerRaisedCard[index] = !playerRaisedCard[index];
        playerHandGraphic.renderHand(playerHand, playerRaisedCard, true);
        if(playerRaisedCard.Any(r => r))
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
            if (playerRaisedCard[i])
            {
                Deck.Card oldCard = playerHand[i];
                playerHand[i] = deck[0];
                deck.RemoveAt(0);
                // put on bottom of deck
                deck.Add(oldCard);
                playerRaisedCard[i] = false;
            }
        }
        playerHandGraphic.renderHand(playerHand, playerRaisedCard, true);
        Deck.Shuffle(deck);
        Debug.Assert(deck.Count == (7*4 - 10));
    }

    private List<bool> getAIExchange(bool didMove)
    {
        var aiRaised = new List<bool> { false, false, false, false, false };
        if (didMove)
        {
            aiRaised = new List<bool> { true, true, true, true, true };
        }
        else
        {
            aiRaised = new List<bool> { true, true, true, true, true };
        }
        return aiRaised;
    }

    private void exchangeCardsAI()
    {
        for (int i = 0; i != 5; ++i)
        {
            if (aiRaisedCard[i])
            {
                Deck.Card oldCard = aiHand[i];
                aiHand[i] = deck[0];
                deck.RemoveAt(0);
                // put on bottom of deck
                deck.Add(oldCard);
                aiRaisedCard[i] = false;
            }
        }
        aiHandGraphic.renderHand(aiHand, aiRaisedCard, false);
        Deck.Shuffle(deck);
        Debug.Assert(deck.Count == (7 * 4 - 10));
        aiRaisedCard = new List<bool> { false, false, false, false, false };
    }

    //private 
}
