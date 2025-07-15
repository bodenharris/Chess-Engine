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
        if (!moveIsPsuedoLegal(iy, ix, fy, fx))
        {
            transform.position = new Vector3(ix, iy, transform.position.z);
        }
        else
        {
            Move move = new Move(iy, ix, fy, fx, BoardScript.board[iy, ix], BoardScript.board[fy, fx]);
            makeMove(move);
            BoardScript.whitePsuedoLegalMoves = generatePsuedoLegalMoves(BoardScript.whitePieces);
            BoardScript.blackPsuedoLegalMoves = generatePsuedoLegalMoves(BoardScript.blackPieces);
            if (BoardScript.whiteTurn && squareIsAttacked(BoardScript.whiteKingPos.row, BoardScript.whiteKingPos.col))
            {
                undoMove(move);
                transform.position = new Vector3(ix, iy, transform.position.z);
            }
            else
            {
                transform.position = pos;
                BoardScript unityBoard = FindObjectOfType<BoardScript>();
                unityBoard.updateDisplayBoard();
                BoardScript.whitePsuedoLegalMoves = generatePsuedoLegalMoves(BoardScript.whitePieces);
                BoardScript.blackPsuedoLegalMoves = generatePsuedoLegalMoves(BoardScript.blackPieces);
                BoardScript.whiteTurn = !BoardScript.whiteTurn;
                if (BoardScript.whiteTurn)
                {
                    BoardScript.whiteLegalMoves = generateLegalMoves(BoardScript.whitePieces);
                    Debug.Log("White moves: ");
                    PrintMoves(BoardScript.whiteLegalMoves);
                    if (BoardScript.whiteLegalMoves.Count == 0)
                    {
                        Debug.Log("Game Over");
                        if (squareIsAttacked(BoardScript.whiteKingPos.row, BoardScript.whiteKingPos.col))
                        {
                            Debug.Log("Black wins by checkmate!");
                        }
                        else
                        {
                            Debug.Log("Stalemate!");
                        }
                    }
                }
                else
                {
                    BoardScript.blackLegalMoves = generateLegalMoves(BoardScript.blackPieces);
                    Debug.Log("Black moves: ");
                    PrintMoves(BoardScript.blackLegalMoves);
                    if(BoardScript.blackLegalMoves.Count == 0)
                    {
                        Debug.Log("Game Over");
                        if (squareIsAttacked(BoardScript.blackKingPos.row, BoardScript.blackKingPos.col))
                        {
                            Debug.Log("White wins by checkmate!");
                        }
                        else
                        {
                            Debug.Log("Stalemate!");
                        }
                    }
                }
            }
        }
    }

    public static bool moveIsPsuedoLegal(int iy, int ix, int fy, int fx)
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
                if (System.Math.Abs(ix - fx) < 2 && System.Math.Abs(iy - fy) < 2 && !squareIsAttacked(fy, fx))
                {
                    break;
                }
                //White Castles
                else if (char.IsUpper(BoardScript.board[iy, ix]) && iy == 7 && fy == 7 && System.Math.Abs(ix - fx) == 2 && !squareIsAttacked(7, 4)) 
                {
                    if (BoardScript.whiteCanLongCastle && BoardScript.whiteCanCastle && fx == 2 && BoardScript.board[7, 3] == '#' && BoardScript.board[7, 2] == '#' && BoardScript.board[7, 1] == '#' && !squareIsAttacked(7, 3) && !squareIsAttacked(7, 2))
                    {
                        break;
                    }
                    if (BoardScript.whiteCanShortCastle && BoardScript.whiteCanCastle && fx == 6 && BoardScript.board[7, 5] == '#' && BoardScript.board[7, 6] == '#' && !squareIsAttacked(7, 5) && !squareIsAttacked(7, 4))
                    {
                        break;
                    }
                }
                //Black Castles
                else if (char.IsLower(BoardScript.board[iy, ix]) && iy == 0 && fy == 0 && System.Math.Abs(ix - fx) == 2 && !squareIsAttacked(0, 4)) 
                {
                    if (BoardScript.blackCanLongCastle && BoardScript.blackCanCastle && fx == 2 && BoardScript.board[0, 3] == '#' && BoardScript.board[0, 2] == '#' && BoardScript.board[0, 1] == '#' && !squareIsAttacked(0, 3) && !squareIsAttacked(0, 2))
                    {
                        break;
                    }
                    if (BoardScript.blackCanShortCastle && BoardScript.blackCanCastle && fx == 6 && BoardScript.board[0, 5] == '#' && BoardScript.board[0, 6] == '#' && !squareIsAttacked(0, 5) && !squareIsAttacked(0, 6))
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
    public static bool moveIsLegal(int iy, int ix, int fy, int fx)
    { 
        if(moveIsPsuedoLegal(iy, ix, fy, fx))
        {
            Move move = new Move(iy, ix, fy, fx, BoardScript.board[iy,  ix], BoardScript.board[fy, fx]);
            makeMove(move);
            if (BoardScript.whiteTurn) 
            {
                if (squareIsAttacked(BoardScript.whiteKingPos.row, BoardScript.whiteKingPos.col))
                {
                    undoMove(move);
                    return false;
                }
                undoMove(move);
                return true;
            }
            else
            {
                if (squareIsAttacked(BoardScript.blackKingPos.row, BoardScript.blackKingPos.col))
                {
                    undoMove(move);
                    return false;
                }
                undoMove(move);
                return true;
            }
        }
        return false;
    }
    static void makeMove(Move move)
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
            BoardScript.whiteCanCastle = false;
        }
        else
        if (BoardScript.board[iy, ix] == 'k')
        {
            BoardScript.blackKingPos = (fy, fx);
            BoardScript.blackCanCastle = false;
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
                BoardScript.board[7, 0] = '#';
                BoardScript.board[7, 2] = 'K';
                BoardScript.board[7, 3] = 'R';
                BoardScript.board[7, 4] = '#';
                BoardScript.whitePieces.Remove((7, 0));
                BoardScript.whitePieces.Add((7, 3));
            }
            //White Short Castles
            if (fx == 6)
            {
                
                BoardScript.board[7, 4] = '#';
                BoardScript.board[7, 5] = 'R';
                BoardScript.board[7, 6] = 'K';
                BoardScript.board[7, 7] = '#';
                BoardScript.whitePieces.Remove((7, 7));
                BoardScript.whitePieces.Add((7, 5));
            }
        }
        //Black Castles
        else if (BoardScript.board[iy, ix] == 'k' && iy == 0 && System.Math.Abs(ix - fx) == 2)
        {
            //Black Long Castle
            if (fx == 2)
            {
                BoardScript.board[0, 0] = '#';
                BoardScript.board[0, 2] = 'k';
                BoardScript.board[0, 3] = 'r';
                BoardScript.board[0, 4] = '#';
                BoardScript.blackPieces.Remove((0, 0));
                BoardScript.blackPieces.Add((0, 3));
            }
            //Black Short Castle
            if (fx == 6)
            {
                BoardScript.board[0, 4] = '#';
                BoardScript.board[0, 5] = 'r';
                BoardScript.board[0, 6] = 'k';
                BoardScript.board[0, 7] = '#';        
                BoardScript.blackPieces.Remove((0, 7));
                BoardScript.blackPieces.Add((0, 5));
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
    static void undoMove(Move move)
    {
        int iy = move.iy;
        int ix = move.ix;
        int fy = move.fy;
        int fx = move.fx;
        char piece = move.piece;
        char capture = move.capture;

        //Check For King Moves
        if(char.ToLower(piece) == 'k')
        {
            if (piece == 'K')
            {
                BoardScript.whiteKingPos = (iy, ix);
            }
            else
            {
                BoardScript.blackKingPos = (iy, ix);
            }
        }

        //Check For Castle
        if (char.ToLower(piece) == 'k' && System.Math.Abs(ix - fx) == 2)
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
                    BoardScript.whiteCanLongCastle = true;
                    BoardScript.whiteCanCastle = true;
                    BoardScript.whitePieces.Add((7, 0));
                    BoardScript.whitePieces.Remove((7, 2));
                    BoardScript.whitePieces.Remove((7, 3));
                    BoardScript.whitePieces.Add((7, 4));
                }
                //White Short Castles
                if (fx == 6)
                {
                    BoardScript.board[7, 4] = 'K';
                    BoardScript.board[7, 5] = '#';
                    BoardScript.board[7, 6] = '#';
                    BoardScript.board[7, 7] = 'R';
                    BoardScript.whiteCanShortCastle = true;
                    BoardScript.whiteCanCastle = true;
                    BoardScript.whitePieces.Add((7, 4));
                    BoardScript.whitePieces.Remove((7, 5));
                    BoardScript.whitePieces.Remove((7, 6));
                    BoardScript.whitePieces.Add((7, 7));
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
                    BoardScript.blackCanLongCastle = true;
                    BoardScript.blackCanCastle = true;
                    BoardScript.blackPieces.Add((0, 0));
                    BoardScript.blackPieces.Remove((0, 2));
                    BoardScript.blackPieces.Remove((0, 3));
                    BoardScript.blackPieces.Add((0, 4));
                }
                //Black Short Castle
                if (fx == 6)
                {
                    BoardScript.board[0, 4] = 'k';
                    BoardScript.board[0, 5] = '#';
                    BoardScript.board[0, 6] = '#';            
                    BoardScript.board[0, 7] = 'r';
                    BoardScript.blackCanShortCastle = true;
                    BoardScript.blackCanCastle = true;
                    BoardScript.blackPieces.Add((0, 4));
                    BoardScript.blackPieces.Remove((0, 5));
                    BoardScript.blackPieces.Remove((0, 6));
                    BoardScript.blackPieces.Add((0, 7));
                }
            }

            return;
        }

        //Check En Pessant
        if(char.ToLower(piece) == 'p' && System.Math.Abs(ix  - fx) == 1 && capture == '#')
        {
            if (piece == 'P')
            {
                BoardScript.board[iy, ix] = 'P';
                BoardScript.board[fy, fx] = '#';
                BoardScript.board[fy + 1, fx] = 'p';
                BoardScript.blackPawnsEP[fx] = true;
                BoardScript.whitePieces.Add((iy, ix));
                BoardScript.whitePieces.Remove((fy, fx));
                BoardScript.blackPieces.Add((fy + 1, fx));
            }
            else
            {
                BoardScript.board[iy, ix] = 'p';
                BoardScript.board[fy, fx] = '#';
                BoardScript.board[fy - 1, fx] = 'P';
                BoardScript.whitePawnsEP[fx] = true;
                BoardScript.blackPieces.Add((iy, ix));
                BoardScript.blackPieces.Remove((fy, fx));
                BoardScript.whitePieces.Add((fy - 1, fx));
            }
            return;
        }

        //Make Swap
        BoardScript.board[iy, ix] = piece;
        BoardScript.board[fy, fx] = capture;
        if (char.IsUpper(piece))
        {
            BoardScript.whitePieces.Remove((fy, fx));
            BoardScript.whitePieces.Add((iy, ix));
        }
        else
        {
            BoardScript.blackPieces.Remove((fy, fx));
            BoardScript.blackPieces.Add((iy, ix));
        }
    }
    static void setFalse(bool[] myArray)
    {
        for (int i = 0; i < myArray.Length; i++)
        {
            myArray[i] = false;
        }
    }

    public static bool squareIsAttackedHelper(Move  move)
    {
        int iy = move.iy;
        int ix = move.ix;
        int fy = move.fy;
        int fx = move.fx;

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
                return false;

            default:
                break;
        }

        return true;
    }

    public static bool squareIsAttacked(int fy, int fx)
    {
        //Check for attacked by black pieces
        if (BoardScript.whiteTurn)
        {
            //Check for pawns
            if (fy > 0)
            {
                if (fx < 7 && BoardScript.board[fy - 1, fx + 1] == 'p') return true;
                if (fx > 0 && BoardScript.board[fy - 1, fx - 1] == 'p') return true;
            }
            //Check for pieces
            foreach(Move move in BoardScript.blackPsuedoLegalMoves)
            {
                if(move.piece != 'p' && move.fx == fx && move.fy == fy)
                {
                    return true;
                }
            }
        }
        //Check for attacked by white pieces
        else
        {
            //Check for pawns
            if (fy < 7)
            {
                if (fx < 7 && BoardScript.board[fy + 1, fx + 1] == 'P') return true;
                if (fx > 0 && BoardScript.board[fy + 1, fx - 1] == 'P') return true;
            }
            foreach (Move move in BoardScript.whitePsuedoLegalMoves)
            {
                if (move.piece != 'P' && move.fx == fx && move.fy == fy)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static List<Move> generateLegalMoves(HashSet<(int row, int col)>  pieces)
    {
        List<Move> moves = new List<Move>();
        HashSet<(int row, int col)> snapshot = new HashSet<(int, int)>(pieces);
        foreach ((int row, int col) piece in snapshot)
        {
            for(int fx = 0; fx < 8; fx++)
            {
                for (int fy = 0; fy < 8; fy++)
                {
                    if(moveIsLegal(piece.row, piece.col, fy, fx))
                    {
                        Move move = new Move(piece.row, piece.col, fy, fx, BoardScript.board[piece.row, piece.col], BoardScript.board[fy, fx]);
                        moves.Add(move);
                    }
                }
            }
        }
        return moves;
    }

    public static List<Move> generatePsuedoLegalMoves(HashSet<(int row, int col)> pieces)
    {
        List<Move> moves = new List<Move>();
        foreach ((int row, int col) piece in pieces)
        {
            
            for (int fx = 0; fx < 8; fx++)
            {
                for (int fy = 0; fy < 8; fy++)
                {
                    if (moveIsPsuedoLegal2(piece.row, piece.col, fy, fx))
                    {
                        Move move = new Move(piece.row, piece.col, fy, fx, BoardScript.board[piece.row, piece.col], BoardScript.board[fy, fx]);
                        moves.Add(move);
                    }
                }
            }
        }
        return moves;
    }

    public static void PrintMoves(List<Move> moves)
    {
        Debug.Log("All moves:");
        foreach (Move move in moves)
        {
            string moveStr = $"({move.iy}, {move.ix}) -> ({move.fy}, {move.fx}) | Piece: {move.piece}, Capture: {move.capture}";
            Debug.Log(moveStr);
        }
    }

    public static bool moveIsPsuedoLegal2(int iy, int ix, int fy, int fx)
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
                //White Castles
                else if (char.IsUpper(BoardScript.board[iy, ix]) && iy == 7 && fy == 7 && System.Math.Abs(ix - fx) == 2)
                {
                    if (BoardScript.whiteCanLongCastle && BoardScript.whiteCanCastle && fx == 2 && BoardScript.board[7, 3] == '#' && BoardScript.board[7, 2] == '#' && BoardScript.board[7, 1] == '#' && !squareIsAttacked(7, 3) && !squareIsAttacked(7, 2))
                    {
                        break;
                    }
                    if (BoardScript.whiteCanShortCastle && BoardScript.whiteCanCastle && fx == 6 && BoardScript.board[7, 5] == '#' && BoardScript.board[7, 6] == '#' && !squareIsAttacked(7, 5) && !squareIsAttacked(7, 4))
                    {
                        break;
                    }
                }
                //Black Castles
                else if (char.IsLower(BoardScript.board[iy, ix]) && iy == 0 && fy == 0 && System.Math.Abs(ix - fx) == 2 && !squareIsAttacked(0, 4))
                {
                    if (BoardScript.blackCanLongCastle && BoardScript.blackCanCastle && fx == 2 && BoardScript.board[0, 3] == '#' && BoardScript.board[0, 2] == '#' && BoardScript.board[0, 1] == '#' && !squareIsAttacked(0, 3) && !squareIsAttacked(0, 2))
                    {
                        break;
                    }
                    if (BoardScript.blackCanShortCastle && BoardScript.blackCanCastle && fx == 6 && BoardScript.board[0, 5] == '#' && BoardScript.board[0, 6] == '#' && !squareIsAttacked(0, 5) && !squareIsAttacked(0, 6))
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
                    if ((ix == fx && BoardScript.board[iy + 1, ix] == '#' && (fy == iy + 1 || (fy == iy + 2 && iy == 1 && BoardScript.board[iy + 2, ix] == '#'))) || (System.Math.Abs(ix - fx) == 1 && fy == iy + 1 && char.IsUpper(BoardScript.board[fy, fx])))
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
    public bool longCastleRightsUpdate;
    public bool shortCastleRightsUpdate;

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
