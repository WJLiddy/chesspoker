using System;
using System.Collections.Generic;

public class ChessBoard
{
    public char[,] board;
    public static readonly int BOARD_DIM = 6;

    public static ChessBoard initial()
    {
        ChessBoard cb = new ChessBoard();
        char[,] board = new char[BOARD_DIM, BOARD_DIM];
        board[0, 0] = 'r'; board[1, 0] = 'n'; board[2, 0] = 'q'; board[3, 0] = 'k'; board[4, 0] = 'b'; board[5, 0] = 'n';
        board[0, 1] = 'p'; board[1, 1] = 'p'; board[2, 1] = 'p'; board[3, 1] = 'p'; board[4, 1] = 'p'; board[5, 1] = 'p'; 

        board[0, 4] = 'P'; board[1, 4] = 'P'; board[2, 4] = 'P'; board[3, 4] = 'P'; board[4, 4] = 'P'; board[5, 4] = 'P';
        board[0, 5] = 'R'; board[1, 5] = 'N'; board[2, 5] = 'Q'; board[3, 5] = 'K'; board[4, 5] = 'B'; board[5, 5] = 'N';

        cb.board = board;
        return cb;
    }


    // no obvious way to make this faster. could conv char[,] to string but that is a last resort.
    public ChessBoard duplicate()
    {
        ChessBoard cb = new ChessBoard();
        cb.board = board.Clone() as char[,];
        return cb;
    }

    // these pieces are on the team that's to move
    public bool active(char c)
    {
        return c == 'P' || c == 'R' || c == 'N' || c == 'B' || c == 'Q' || c == 'K';
    }

    // print the board state to a string
    public string printBoard()
    {
        string printed = "";
        for (int y = 0; y != BOARD_DIM; ++y)
        {
            for (int x = 0; x != BOARD_DIM; ++x)
            {
                if (board[x, y] != '\0')
                {
                    printed += board[x, y].ToString();
                }
                else
                {
                    printed += " ";
                }
            }
            printed += "\n";
        }
        return printed;
    }

    // is given square part of the board
    public static bool inBounds(int x, int y)
    {
        return x >= 0 && x < BOARD_DIM && y >= 0 && y < BOARD_DIM;
    }

    // precached moves for faster move generation
    static private int[][] pawnMoves =
    {
        new int[] { 0, -1},
    };

    static private int[][] knightMoves =
    {
        new int[] {-2, -1},
        new int[] {-2, 1},
        new int[] {-1, -2},
        new int[] {-1, 2},
        new int[] {1, -2},
        new int[] {1, 2},
        new int[] {2, -1},
        new int[] {2, 1},
    };

    static private int[][] kingMoves =
    {
        new int[] {-1, -1},
        new int[] {0, -1},
        new int[] {1, -1},
        new int[] {-1, 0},
        new int[] {1, 0},
        new int[] {-1, 1},
        new int[] {0, 1},
        new int[] {1, 1},
    };

    // sliders
    static int[][] bishopMoves =
    {
        new int[] {1,1},
        new int[] {-1,1},
        new int[] {1,-1},
        new int[] {-1,-1}
    };

    static int[][] rookMoves =
    {
        new int[] {0,1},
        new int[] {0,-1},
        new int[] {1,0},
        new int[] {-1,0}
    };  

    private ChessBoard movePiece(int oldX, int oldY, int newX, int newY)
    {
        ChessBoard newBoard = duplicate();
        newBoard.board[newX, newY] = newBoard.board[oldX,oldY];
        newBoard.board[oldX, oldY] = '\0';
        return newBoard;
    }

    public List<ChessBoard> generatePMoves(int x, int y)
    {
        List<ChessBoard> boards = new List<ChessBoard>();
        int newX = x + pawnMoves[0][0];
        int newY = y + pawnMoves[0][1];
        if (inBounds(newX, newY) && board[newX, newY] == '\0')
        {
            var nbState = movePiece(x, y, newX, newY);
            // queen promote
            if(newY == 0)
            {
                nbState.board[newX, newY] = 'Q';
            }
            boards.Add(nbState);
        }
        return boards;
    }

    public List<ChessBoard> generateNMoves(int x, int y)
    {
        List<ChessBoard> boards = new List<ChessBoard>();
        foreach (var p in knightMoves)
        {
            int newX = x + p[0];
            int newY = y + p[1];
            if (inBounds(newX, newY) && !active(board[newX, newY]))
            {
                boards.Add(movePiece(x, y, newX, newY));
            }
        }
        return boards;
    }

    public List<ChessBoard> generateKMoves(int x, int y)
    {
        List<ChessBoard> boards = new List<ChessBoard>();
        foreach (var p in kingMoves)
        {
            int newX = x + p[0];
            int newY = y + p[1];
            if (inBounds(newX, newY) && !active(board[newX, newY]))
            {
                boards.Add(movePiece(x, y, newX, newY));
            }
        }
        return boards;
    }

    public List<ChessBoard> generateBMoves(int x, int y)
    {
        List<ChessBoard> boards = new List<ChessBoard>();

        foreach (var p in bishopMoves)
        {
            int newX = x + p[0];
            int newY = y + p[1];
            // if open or enemy
            while (inBounds(newX, newY) && (board[newX, newY] == '\0' || !active(board[newX, newY])))
            {
                boards.Add(movePiece(x, y, newX, newY));
                // if open continue
                if (board[newX, newY] == '\0')
                {
                    newX += p[0];
                    newY += p[1];
                }
                else
                {
                    // otherwise enemy, stop
                    break;
                }
            }
        }
        return boards;
    }

    public List<ChessBoard> generateRMoves(int x, int y)
    {
        List<ChessBoard> boards = new List<ChessBoard>();

        foreach (var p in rookMoves)
        {
            int newX = x + p[0];
            int newY = y + p[1];
            // if open or enemy
            while (inBounds(newX, newY) && (board[newX, newY] == '\0' || !active(board[newX,newY])))
            {
                boards.Add(movePiece(x, y, newX, newY));  
                // if open continue
                if(board[newX, newY] == '\0')
                {
                    newX += p[0];
                    newY += p[1];
                }
                else
                {
                    // otherwise enemy, stop
                    break;
                }    
            }
        }
        return boards;
    }

    public ChessBoard flipBoard()
    {
        ChessBoard newBoard = new ChessBoard();
        newBoard.board = new char[BOARD_DIM, BOARD_DIM];
        for (int y = 0; y != BOARD_DIM; ++y)
        {
            for (int x = 0; x != BOARD_DIM; ++x)
            {
                var c = board[(BOARD_DIM-1) - x, (BOARD_DIM-1) - y];
                if (c == '\0')
                {
                    continue;
                }
                // lower to upper
                if (c < 91)
                {
                    newBoard.board[x, y] = (char)(c + ' ');
                }
                else
                {
                    newBoard.board[x, y] = (char)(c - ' ');
                }
            }
        }
        return newBoard;
    }

    public List<ChessBoard> movesForPiece(int x, int y)
    {
        switch (board[x, y])
        {
            case '\0': return null;
            case 'K': return generateKMoves(x, y);
            case 'Q': var q = generateRMoves(x, y); q.AddRange(generateBMoves(x, y)); return q;
            case 'R': return generateRMoves(x, y);
            case 'B': return generateBMoves(x, y);
            case 'N': return generateNMoves(x, y);
            case 'P': return generatePMoves(x, y);
        }
        return null;
    }

    public List<ChessBoard> generateMoves()
    {
        List<ChessBoard> generated = new List<ChessBoard>();
        for (int y = 0; y != BOARD_DIM; ++y)
        {
            for (int x = 0; x != BOARD_DIM; ++x)
            {
                var moves = movesForPiece(x, y);
                if (moves != null)
                {
                    generated.AddRange(moves);
                }
            }
        }
        return generated;
    }
}
