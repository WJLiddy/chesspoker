using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessboardGraphic : MonoBehaviour
{
    public GameObject whiteSq;
    public GameObject blackSq;

    private List<GameObject> list = new List<GameObject>();

    public GameObject pieceP;
    public GameObject pieceR;
    public GameObject pieceN;
    public GameObject pieceB;
    public GameObject pieceQ;
    public GameObject pieceK;

    public GameObject piecep;
    public GameObject piecer;
    public GameObject piecen;
    public GameObject pieceb;
    public GameObject pieceq;
    public GameObject piecek;



    // Start is called before the first frame update
    void Awake()
    {
        for (int x = 0; x != ChessBoard.BOARD_DIM; x++)
        {
            for (int y = 0; y != ChessBoard.BOARD_DIM; y++)
            {
                GameObject v;
                if ((x + y) % 2 == 0)
                {
                    v = Instantiate(whiteSq);
                }
                else
                {
                    v = Instantiate(blackSq);
                }
                v.transform.SetParent(this.transform);
                v.transform.localPosition = new Vector3(x, y, 1);
                v.GetComponent<ChessTileListener>().tileX = (ChessBoard.BOARD_DIM - 1) - x;
                v.GetComponent<ChessTileListener>().tileY = (ChessBoard.BOARD_DIM - 1) - y;
            }
        }
        list = new List<GameObject>();
    }

    public void ApplyPieces(ChessBoard cb, bool flipped, int activePlayerHandVal)
    {
        foreach(var v in list)
        {
            Destroy(v);
        }
        list.Clear();
        // reflip board if flipped
        ChessBoard drawCB = cb;
        if(flipped)
        {
            drawCB = cb.flipBoard();
        }

        for (int x = 0; x != ChessBoard.BOARD_DIM; x++)
        {
            for (int y = 0; y != ChessBoard.BOARD_DIM; y++)
            {
                GameObject v;
                // lol. lmao even. thanks copilot.
                switch(drawCB.board[x,y])
                {
                    case 'P':
                    v = Instantiate(pieceP);
                        if(GameManager.pieceMatchesValue('P',activePlayerHandVal))
                        {
                            v.GetComponent<SpriteRenderer>().color = Color.yellow;
                        }
                    break;
                    case 'R':
                    v = Instantiate(pieceR);
                        if (GameManager.pieceMatchesValue('R', activePlayerHandVal))
                        {
                            v.GetComponent<SpriteRenderer>().color = Color.yellow;
                        }
                        break;
                    case 'N':
                    v = Instantiate(pieceN);
                        if (GameManager.pieceMatchesValue('N', activePlayerHandVal))
                        {
                            v.GetComponent<SpriteRenderer>().color = Color.yellow;
                        }
                        break;
                    case 'B':
                    v = Instantiate(pieceB);
                        if (GameManager.pieceMatchesValue('B', activePlayerHandVal))
                        {
                            v.GetComponent<SpriteRenderer>().color = Color.yellow;
                        }
                        break;
                    case 'Q':
                    v = Instantiate(pieceQ);
                        if (GameManager.pieceMatchesValue('Q', activePlayerHandVal))
                        {
                            v.GetComponent<SpriteRenderer>().color = Color.yellow;
                        }
                        break;
                    case 'K':
                    v = Instantiate(pieceK);
                        if (GameManager.pieceMatchesValue('K', activePlayerHandVal))
                        {
                            v.GetComponent<SpriteRenderer>().color = Color.yellow;
                        }
                        break;

                    case 'p':
                    v = Instantiate(piecep);
                    break;
                    case 'r':
                    v = Instantiate(piecer);
                    break;
                    case 'n':
                    v = Instantiate(piecen);
                    break;
                    case 'b':
                    v = Instantiate(pieceb);
                    break;
                    case 'q':   
                    v = Instantiate(pieceq);
                    break;
                    case 'k':
                    v = Instantiate(piecek);
                    break;
                    default:
                    continue;
                }
                v.transform.SetParent(this.transform);
                v.transform.localPosition = new Vector3((ChessBoard.BOARD_DIM - 1) - x, (ChessBoard.BOARD_DIM - 1) - y, 0);
                list.Add(v);
            }
        }
    }

}
