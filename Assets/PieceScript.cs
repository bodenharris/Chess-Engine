using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceScript : MonoBehaviour
{
    private bool isDragging = false;
    private int ix;
    private int iy;
    void OnMouseDown()
    {
        isDragging = true;
        iy = Mathf.RoundToInt(transform.position.y);
        ix = Mathf.RoundToInt(transform.position.x);
    }

    void OnMouseUp()
    {
        isDragging = false;
        Vector3 pos = transform.position;
        pos.x = Mathf.Round(pos.x);
        pos.y = Mathf.Round(pos.y);
        int fx = Mathf.RoundToInt(pos.x);
        int fy = Mathf.RoundToInt(pos.y);
        if (!moveIsLegal(iy, ix, fy, fx))
        {
            transform.position = new Vector3(ix, iy, transform.position.z);
        }
        else
        {
            transform.position = pos;
            Move move = new Move(iy, ix, fy, fx, BoardScript.board[iy, ix], BoardScript.board[fy, fx]);
            makeMove(move);
            BoardScript unityBoard = FindObjectOfType<BoardScript>();
            unityBoard.updateDisplayBoard();
            BoardScript.PrintCharArray(BoardScript.board);
            BoardScript.whiteTurn = !BoardScript.whiteTurn;
        }

    }

    public static bool moveIsLegal(int iy, int ix, int fy, int fx)
    {
        //Out of bounds
        if (fx > 7 || fx < 0 || fy > 7 || fy < 0)
        {
            return false;
        }

        //Ensure Movement
        if ((fx == ix) && (fy == iy))
        {
            return false;
        }

        //Check Turn
        if (char.IsUpper(BoardScript.board[iy, ix]) == !BoardScript.whiteTurn)
        {
            return false;
        }

        //Space isnt covered already by own piece
        if (BoardScript.board[fy, fx] != '#' && (BoardScript.whiteTurn == char.IsUpper(BoardScript.board[fy, fx])))
        {
            return false;
        }

        //Determine Piece to ensure correct movement
        switch (char.ToLower(BoardScript.board[iy, ix]))
        {
            case 'r':
                if (iy == fy)
                {
                    for (int i = 1; i < System.Math.Abs(ix - fx); i++)
                    {
                        if ((BoardScript.board[iy, ix + (i * ((ix - fx) < 0 ? 1 : -1))]) != '#')
                        {
                            return false;
                        }
                    }
                    break;
                }
                if (ix == fx)
                {
                    for (int i = 1; i < System.Math.Abs(iy - fy); i++)
                    {
                        if ((BoardScript.board[iy + (i * ((iy - fy) < 0 ? 1 : -1)), ix]) != '#')
                        {
                            return false;
                        }
                    }
                    break;
                }
                return false;

            case 'n':
                if ((System.Math.Abs(ix - fx) == 1 && 2 == System.Math.Abs(iy - fy)) || (System.Math.Abs(ix - fx) == 2 && 1 == System.Math.Abs(iy - fy)))
                {
                    break;
                }
                return false;

            case 'b':
                if ((System.Math.Abs(ix - fx) == System.Math.Abs(iy - fy)))
                {
                    for (int i = 1; i < System.Math.Abs(ix - fx); i++)
                    {
                        if (BoardScript.board[iy + (i * ((iy - fy) < 0 ? 1 : -1)), ix + (i * ((ix - fx) < 0 ? 1 : -1))] != '#')
                        {
                            return false;
                        }
                    }
                    break;
                }
                return false;

            case 'q':
                if (iy == fy)
                {
                    for (int i = 1; i < System.Math.Abs(ix - fx); i++)
                    {
                        if ((BoardScript.board[iy, ix + (i * ((ix - fx) < 0 ? 1 : -1))]) != '#')
                        {
                            return false;
                        }
                    }
                    break;
                }
                if (ix == fx)
                {
                    for (int i = 1; i < System.Math.Abs(iy - fy); i++)
                    {
                        if ((BoardScript.board[iy + (i * ((iy - fy) < 0 ? 1 : -1)), ix]) != '#')
                        {
                            return false;
                        }
                    }
                    break;
                }
                if ((System.Math.Abs(ix - fx) == System.Math.Abs(iy - fy)))
                {
                    for (int i = 1; i < System.Math.Abs(ix - fx); i++)
                    {
                        if (BoardScript.board[iy + (i * ((iy - fy) < 0 ? 1 : -1)), ix + (i * ((ix - fx) < 0 ? 1 : -1))] != '#')
                        {
                            return false;
                        }
                    }
                    break;
                }
                return false;

            case 'k':
                if (System.Math.Abs(ix - fx) < 2 && System.Math.Abs(iy - fy) < 2)
                {
                    break;
                }
                else if (char.IsUpper(BoardScript.board[iy, ix]) && iy == 7 && fy == 7 && System.Math.Abs(ix - fx) == 2 && !squareIsAttacked(7, 4)) //White Castles
                {
                    if (BoardScript.whiteCanLongCastle && fx == 2 && BoardScript.board[7, 3] == '#' && BoardScript.board[7, 2] == '#' && BoardScript.board[7, 1] == '#' && !squareIsAttacked(7, 3) && !squareIsAttacked(7, 2))
                    {
                        break;
                    }
                    if (BoardScript.whiteCanShortCastle && fx == 6 && BoardScript.board[7, 5] == '#' && BoardScript.board[7, 6] == '#' && !squareIsAttacked(7, 5) && !squareIsAttacked(7, 4))
                    {
                        break;
                    }
                }
                else if (char.IsLower(BoardScript.board[iy, ix]) && iy == 0 && fy == 0 && System.Math.Abs(ix - fx) == 2 && !squareIsAttacked(0, 4)) //Black Castles
                {
                    if (BoardScript.blackCanLongCastle && fx == 2 && BoardScript.board[0, 3] == '#' && BoardScript.board[0, 2] == '#' && BoardScript.board[0, 1] == '#' && !squareIsAttacked(0, 3) && !squareIsAttacked(0, 2))
                    {
                        break;
                    }
                    if (BoardScript.blackCanShortCastle && fx == 6 && BoardScript.board[0, 5] == '#' && BoardScript.board[0, 6] == '#' && !squareIsAttacked(0, 5) && !squareIsAttacked(0, 6))
                    {
                        break;
                    }
                }
                return false;

            case 'p':
                //white pawn
                if (char.IsUpper(BoardScript.board[iy, ix]))
                {
                    if ((ix == fx && BoardScript.board[iy - 1, ix] == '#' && (fy == iy - 1 || (fy == iy - 2 && iy == 6 && BoardScript.board[iy - 2, ix] == '#'))) || (System.Math.Abs(ix - fx) == 1 && fy == iy - 1 && char.IsLower(BoardScript.board[fy, fx])))
                    {
                        break;
                    }
                    //En Pessant
                    if (BoardScript.blackPawnsEP[fx] && System.Math.Abs(ix - fx) == 1 && fy == 2 && iy == 3)
                    {
                        break;
                    }
                }
                else //black pawn
                {
                    if ((ix == fx && BoardScript.board[iy + 1, ix] == '#' && (fy == iy + 1 || (fy == iy + 2 && iy == 1 && BoardScript.board[iy + 2, ix] == '#'))) || (System.Math.Abs(ix - fx) == 1 && fy == iy + 1 && char.IsUpper(BoardScript. board[fy, fx])))
                    {
                        break;
                    }
                    //En Pessant
                    if (BoardScript.whitePawnsEP[fx] && System.Math.Abs(ix - fx) == 1 && fy == 5 && iy == 4)
                    {
                        break;
                    }
                }
                return false;

            default:
                break;
        }

        return true;
    }

    void makeMove(Move move)
    {
        int iy = move.iy;
        int ix = move.ix;
        int fy = move.fy;
        int fx = move.fx;

        //Update Piece HashSets
        if (char.IsUpper(BoardScript.board[iy, ix]))
        {
            BoardScript.whitePieces.Remove((iy, ix));
            if (BoardScript.blackPieces.Contains((fy, fx)))
            {
                BoardScript.blackPieces.Remove((fy, fx));
            }
            BoardScript.whitePieces.Add((fy, fx));
        }
        else
        {
            BoardScript.blackPieces.Remove((iy, ix));
            if (BoardScript.whitePieces.Contains((fy, fx)))
            {
                BoardScript.whitePieces.Remove((fy, fx));
            }
            BoardScript.blackPieces.Add((fy, fx));
        }

        //Update EnPessant
        if (BoardScript.whiteTurn)
        {
            setFalse(BoardScript.whitePawnsEP);
        }
        else
        {
            setFalse(BoardScript.blackPawnsEP);
        }
        if (char.ToLower(BoardScript.board[iy, ix]) == 'p' && System.Math.Abs(iy - fy) == 2)
        {
            if(BoardScript.whiteTurn)
            {
                BoardScript.whitePawnsEP[ix] = true;
            }
            else
            { 
                BoardScript.blackPawnsEP[ix] = true;
            }
        }

        //Tracking King Movement and castle rights for rooks
        if (BoardScript.board[iy, ix] == 'K')
        {
            BoardScript.whiteKingPos = (fy, fx);
            BoardScript.whiteCanLongCastle = false;
            BoardScript.whiteCanShortCastle = false;
        }
        else
        if (BoardScript.board[iy, ix] == 'k')
        {
            BoardScript.blackKingPos = (fy, fx);
            BoardScript.blackCanLongCastle = false;
            BoardScript.blackCanShortCastle = false;
        }
        //Black Castle Rights
        if (iy == 0) 
        {
            if (ix == 0)
            {
                BoardScript.blackCanLongCastle = false;
            }
            else if (ix == 7)
            {
                BoardScript.blackCanShortCastle = false;
            }
        }
        //White Castle Rights
        else if (iy == 7) 
        {
            if (ix == 0)
            {
                BoardScript.whiteCanLongCastle = false;
            }
            else if (ix == 7)
            {
                BoardScript.whiteCanShortCastle = false;
            }
        }
        //Black Castle Rights
        if (fy == 0) 
        {
            if (fx == 0)
            {
                BoardScript.blackCanLongCastle = false;
            }
            else if (fx == 7)
            {
                BoardScript.blackCanShortCastle = false;
            }
        }
        //White Castle Rights
        else if (fy == 7) 
        {
            if (fx == 0)
            {
                BoardScript.whiteCanLongCastle = false;
            }
            else if (fx == 7)
            {
                BoardScript.whiteCanShortCastle = false;
            }
        }

        //White Castles
        if (BoardScript.board[iy, ix] == 'K' && iy == 7 && System.Math.Abs(ix - fx) == 2)
        {
            //White Long Castle
            if (fx == 2)
            {
                BoardScript.board[7, 2] = BoardScript.board[7, 4];
                BoardScript.board[7, 4] = '#';
                BoardScript.board[7, 0] = '#';
                BoardScript.board[7, 3] = 'R';
            }
            //White Short Castles
            if (fx == 6)
            {
                BoardScript.board[7, 6] = BoardScript.board[7, 4];
                BoardScript.board[7, 4] = '#';
                BoardScript.board[7, 7] = '#';
                BoardScript.board[7, 5] = 'R';
            }
        }
        //Black Castles
        else if (BoardScript.board[iy, ix] == 'k' && iy == 0 && System.Math.Abs(ix - fx) == 2)
        {
            //Black Long Castle
            if (fx == 2)
            {
                BoardScript.board[0, 2] = BoardScript.board[0, 4];
                BoardScript.board[0, 4] = '#';
                BoardScript.board[0, 0] = '#';
                BoardScript.board[0, 3] = 'r';
            }
            //Black Short Castle
            if (fx == 6)
            {
                BoardScript.board[0, 6] = BoardScript.board[0, 4];
                BoardScript.board[0, 4] = '#';
                BoardScript.board[0, 7] = '#';
                BoardScript.board[0, 5] = 'r';
            }
        }
        //Pawn Promotion
        else if (char.ToLower(BoardScript.board[iy, ix]) == 'p' && (fy == 0 || fy == 7))
        {
            //White Pawn Promotion
            if (fy == 0)
            {
                if (Input.GetKey(KeyCode.R))
                {
                    BoardScript.board[fy, fx] = 'R';
                    BoardScript.board[iy, ix] = '#';
                }
                else if (Input.GetKey(KeyCode.B))
                {
                    BoardScript.board[fy, fx] = 'B';
                    BoardScript.board[iy, ix] = '#';
                }
                else if (Input.GetKey(KeyCode.N))
                {
                    BoardScript.board[fy, fx] = 'N';
                    BoardScript.board[iy, ix] = '#';
                }
                else
                {
                    BoardScript.board[fy, fx] = 'Q';
                    BoardScript.board[iy, ix] = '#';
                }
            }
            //Black Pawn Promotion
            else
            {
                if (Input.GetKey(KeyCode.R))
                {
                    BoardScript.board[fy, fx] = 'r';
                    BoardScript.board[iy, ix] = '#';
                }
                else if (Input.GetKey(KeyCode.B))
                {
                    BoardScript.board[fy, fx] = 'b';
                    BoardScript.board[iy, ix] = '#';
                }
                else if (Input.GetKey(KeyCode.N))
                {
                    BoardScript.board[fy, fx] = 'n';
                    BoardScript.board[iy, ix] = '#';
                }
                else
                {
                    BoardScript.board[fy, fx] = 'q';
                    BoardScript.board[iy, ix] = '#';
                }
            }
        }
        //En Pessant
        else if (char.ToLower(BoardScript.board[iy, ix]) == 'p' && (ix != fx) && BoardScript.board[fy, fx] == '#')
        {
            Debug.Log("here");
            if (BoardScript.whiteTurn)
            {
                BoardScript.board[fy, fx] = BoardScript.board[iy, ix];
                BoardScript.board[iy, ix] = '#';
                BoardScript.board[fy + 1, fx] = '#';
                BoardScript.blackPieces.Remove((fy + 1, fx));
            }
            else
            {
                BoardScript.board[fy, fx] = BoardScript.board[iy, ix];
                BoardScript.board[iy, ix] = '#';
                BoardScript.board[fy - 1, fx] = '#';
                BoardScript.whitePieces.Remove((fy - 1, fx));
            }
        }
        else
        {
            //Make swap
            BoardScript.board[fy, fx] = BoardScript.board[iy, ix];
            BoardScript.board[iy, ix] = '#';
        }
    }
    void undoMove(Move move)
    {
        int iy = move.iy;
        int ix = move.ix;
        int fy = move.fy;
        int fx = move.fx;
        char piece = move.piece;
        char capture = move.capture;

        //Check For Castle
        if (char.ToLower(piece) == 'k' && System.Math.Abs(fx - fy) == 2)
        {
            if (piece == 'K')
            {
                //White Long Castle
                if (fx == 2)
                {
                    BoardScript.board[7, 0] = 'R';
                    BoardScript.board[7, 2] = '#';
                    BoardScript.board[7, 3] = '#';
                    BoardScript.board[7, 4] = 'K';
                }
                //White Short Castles
                if (fx == 6)
                {
                    BoardScript.board[7, 4] = 'K';
                    BoardScript.board[7, 5] = '#';
                    BoardScript.board[7, 6] = '#';
                    BoardScript.board[7, 7] = 'R';
                }
            }
            else
            {
                //Black Long Castle
                if (fx == 2)
                {
                    BoardScript.board[0, 0] = 'r';
                    BoardScript.board[0, 2] = '#';
                    BoardScript.board[0, 3] = '#';
                    BoardScript.board[0, 4] = 'k';
                }
                //Black Short Castle
                if (fx == 6)
                {
                    BoardScript.board[0, 4] = 'k';
                    BoardScript.board[0, 5] = '#';
                    BoardScript.board[0, 6] = '#';            
                    BoardScript.board[0, 7] = 'r';    
                }
            }
            return;
        }

        //Check En Pessant
        if(char.ToLower(piece) == 'p' && System.Math.Abs(ix  - fx) == 1 && capture == '#')
        {
            if (piece == 'P')
            {
                BoardScript.board[ix, iy] = 'P';
                BoardScript.board[fx, fy] = '#';
                BoardScript.board[fx, fy + 1] = 'p';
            }
            else
            {
                BoardScript.board[ix, iy] = 'p';
                BoardScript.board[fx, fy] = '#';
                BoardScript.board[fx, fy - 1] = 'P';
            }
            return;
        }

        //Make Swap
        BoardScript.board[ix, iy] = piece;
        BoardScript.board[fx, fy] = capture;
    }
    void setFalse(bool[] myArray)
    {
        for (int i = 0; i < myArray.Length; i++)
        {
            myArray[i] = false;
        }
    }

    public static bool squareIsAttacked(int fy, int fx)
    {
        return false;
    }

    void Update()
    {
        if (isDragging)
        {
            // Convert mouse position to world position
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 1; // Keep piece on correct plane (adjust for 3D if needed)
            transform.position = mousePos;
        }
    }

}

public class Move
{
    public int iy;
    public int ix;
    public int fy;
    public int fx;
    public char piece;
    public char capture;

    public Move(int iy, int ix, int fy, int fx, char piece, char capture)
    {
        this.iy = iy;
        this.ix = ix;
        this.fy = fy;
        this.fx = fx;
        this.piece = piece;
        this.capture = capture;
    }
}
