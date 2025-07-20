using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Linq;

public class PieceScript : MonoBehaviour
{
    private bool isDragging = false;
    private int ix;
    private int iy;
    public static int engineDepth = 4;
    public static int nodesSearched = 0;
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
        Move move = new Move(iy, ix, fy, fx, BoardScript.board[iy, ix], BoardScript.board[fy, fx]);
        if (!moveIsLegal(move))
        {
            transform.position = new Vector3(ix, iy, transform.position.z);
        }
        else
        {
            makeMove(move);
            transform.position = pos;
            BoardScript unityBoard = FindObjectOfType<BoardScript>();
            unityBoard.updateDisplayBoard();
            BoardScript.whiteTurn = !BoardScript.whiteTurn;
            BoardScript.whitePsuedoLegalMoves = generatePsuedoLegalMoves(BoardScript.whitePieces);
            BoardScript.blackPsuedoLegalMoves = generatePsuedoLegalMoves(BoardScript.blackPieces);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            if(engineMove(engineDepth, false, -100000, 100000) == -100000)
            {
                UnityEngine.Debug.Log("White wins by checkmate");
                return;
            }
            stopwatch.Stop();
            UnityEngine.Debug.Log("Engine move took: " + stopwatch.ElapsedMilliseconds + " ms");
            UnityEngine.Debug.Log("Nodes searched: " + nodesSearched);
            nodesSearched = 0;
            makeMove(BoardScript.bestMove);
            unityBoard.updateDisplayBoard();
            BoardScript.whiteTurn = !BoardScript.whiteTurn;
            UnityEngine.Debug.Log(evalPos());
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

        //Space isnt covered already by own piece
        if (BoardScript.board[fy, fx] != '#' && (char.IsUpper(BoardScript.board[iy, ix]) == char.IsUpper(BoardScript.board[fy, fx])))
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
                else if (char.IsUpper(BoardScript.board[iy, ix]) && iy == 7 && fy == 7 && System.Math.Abs(ix - fx) == 2 && !squareIsAttacked(7, 4)) 
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
                //Black Castles
                else if (char.IsLower(BoardScript.board[iy, ix]) && iy == 0 && fy == 0 && System.Math.Abs(ix - fx) == 2 && !squareIsAttacked(0, 4)) 
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
    public static bool moveIsLegal(Move move)
    {
        int iy = move.iy;
        int ix = move.ix;
        int fy = move.fy;
        int fx = move.fx;

        if (moveIsPsuedoLegal(iy, ix, fy, fx))
        {
            makeMove(move);
            BoardScript.whitePsuedoLegalMoves = generatePsuedoLegalMoves(BoardScript.whitePieces);
            BoardScript.blackPsuedoLegalMoves = generatePsuedoLegalMoves(BoardScript.blackPieces);
            if (char.IsUpper(move.piece)) 
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
        char piece = move.piece;
        char capture = move.capture;

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

        //Tracking King Movement
        if (BoardScript.board[iy, ix] == 'K')
        {
            BoardScript.whiteKingPos = (fy, fx);
            if (BoardScript.whiteCanLongCastle)
            {
                move.whiteLongCastleRightsFlag = true;
                BoardScript.whiteCanLongCastle = false;
            }
            if (BoardScript.whiteCanShortCastle)
            {
                move.whiteShortCastleRightsFlag = true;
                BoardScript.whiteCanShortCastle = false;
            }
        }
        else if (BoardScript.board[iy, ix] == 'k')
        {
            if(BoardScript.blackCanLongCastle)
            {
                move.blackLongCastleRightsFlag = true;
                BoardScript.blackCanLongCastle = false;
            }
            if (BoardScript.blackCanShortCastle)
            {
                move.blackShortCastleRightsFlag = true;
                BoardScript.blackCanShortCastle = false;
            }
        }

        //Update Rook Castle Rights
        if((ix == 0 && iy == 0 && piece == 'r') || (fy == 0 && fx == 0 && capture == 'r'))
        {
            if (BoardScript.blackCanLongCastle)
            {
                move.blackLongCastleRightsFlag = true;
                BoardScript.blackCanLongCastle = false;
            }
        }
        if ((ix == 7 && iy == 0 && piece == 'r') || (fy == 0 && fx == 7 && capture == 'r'))
        {
            if (BoardScript.blackCanShortCastle)
            {
                move.blackShortCastleRightsFlag = true;
                BoardScript.blackCanShortCastle = false;
            }
        }
        if ((ix == 0 && iy == 7 && piece == 'R') || (fy == 7 && fx == 0 && capture == 'R'))
        {
            if (BoardScript.whiteCanLongCastle)
            {
                move.whiteLongCastleRightsFlag = true;
                BoardScript.whiteCanLongCastle = false;
            }
        }
        if ((ix == 7 && iy == 7 && piece == 'R') || (fy == 7 && fx == 7 && capture == 'R'))
        {
            if (BoardScript.whiteCanShortCastle)
            {
                move.whiteShortCastleRightsFlag = true;
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

        //Update Castle Rights
        if (move.whiteLongCastleRightsFlag)
        {
            BoardScript.whiteCanLongCastle = true;
        }
        if (move.whiteShortCastleRightsFlag)
        {
            BoardScript.whiteCanShortCastle = true;
        }
        if (move.blackLongCastleRightsFlag)
        {
            BoardScript.blackCanLongCastle = true;
        }
        if (move.blackShortCastleRightsFlag)
        {
            BoardScript.blackCanShortCastle = true;
        }


        //Check For King Moves
        if (char.ToLower(piece) == 'k')
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
        if(char.IsUpper(capture))
        {
            BoardScript.whitePieces.Add((fy, fx));
        }
        else if (char.IsLower(capture))
        {
            BoardScript.blackPieces.Add((fy, fx));
        }
    }
    static void setFalse(bool[] myArray)
    {
        for (int i = 0; i < myArray.Length; i++)
        {
            myArray[i] = false;
        }
    }
    public static bool squareIsAttacked(int fy, int fx)
    {
        //Check for attacked by black pieces
        if (BoardScript.whiteTurn)
        {
            //Check for pieces
            foreach(Move move in BoardScript.blackPsuedoLegalMoves)
            {
                if(move.capture == 'K')
                {
                    return true;
                }
            }
        }
        //Check for attacked by white pieces
        else
        {
            foreach (Move move in BoardScript.whitePsuedoLegalMoves)
            {
                if (move.capture == 'k')
                {
                    return true;
                }
            }
        }
        return false;
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
        UnityEngine.Debug.Log("All moves:");
        foreach (Move move in moves)
        {
            string moveStr = $"({move.iy}, {move.ix}) -> ({move.fy}, {move.fx}) | Piece: {move.piece}, Capture: {move.capture}";
            UnityEngine.Debug.Log(moveStr);
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

        //Space isnt covered already by own piece
        if (BoardScript.board[fy, fx] != '#' && (char.IsUpper(BoardScript.board[iy, ix]) == char.IsUpper(BoardScript.board[fy, fx])))
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
                    if (BoardScript.whiteCanLongCastle && fx == 2 && BoardScript.board[7, 3] == '#' && BoardScript.board[7, 2] == '#' && BoardScript.board[7, 1] == '#' && !squareIsAttacked(7, 3) && !squareIsAttacked(7, 2))
                    {
                        break;
                    }
                    if (BoardScript.whiteCanShortCastle && fx == 6 && BoardScript.board[7, 5] == '#' && BoardScript.board[7, 6] == '#' && !squareIsAttacked(7, 5) && !squareIsAttacked(7, 4))
                    {
                        break;
                    }
                }
                //Black Castles
                else if (char.IsLower(BoardScript.board[iy, ix]) && iy == 0 && fy == 0 && System.Math.Abs(ix - fx) == 2 && !squareIsAttacked(0, 4))
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
    public static int engineMove(int depth, bool maximizingPlayer, int alpha, int beta)
    {
        int eval;
        if (depth == 0) 
        {
            return evalPos();
        }
        if(maximizingPlayer)
        {
            int maxEval = -100000;
            BoardScript.whitePsuedoLegalMoves = orderMoves(generatePsuedoLegalMoves(BoardScript.whitePieces));
            //BoardScript.whitePsuedoLegalMoves = generatePsuedoLegalMoves(BoardScript.whitePieces);
            foreach (Move move in BoardScript.whitePsuedoLegalMoves)
            {
                if (moveIsLegalAndMake(move))
                {
                    eval = engineMove(depth - 1, false, alpha, beta);
                    nodesSearched++;
                    undoMove(move);
                    if (eval > maxEval)
                    {
                        maxEval = eval;
                        if (depth == engineDepth)
                        {
                            BoardScript.bestMove = move;
                        }
                    }
                    alpha = System.Math.Max(alpha, eval);
                    if (beta <= alpha)
                    {
                        break;
                    }    
                }
            }
            return maxEval;
        }
        else
        {
            int minEval = 100000;
            BoardScript.blackPsuedoLegalMoves = orderMoves(generatePsuedoLegalMoves(BoardScript.blackPieces));
            //BoardScript.blackPsuedoLegalMoves = generatePsuedoLegalMoves(BoardScript.blackPieces);
            foreach (Move move in BoardScript.blackPsuedoLegalMoves)
            {
                if (moveIsLegalAndMake(move))
                {
                    eval = engineMove(depth - 1, true, alpha, beta);
                    undoMove(move);
                    if(eval < minEval)
                    {
                        minEval = eval;
                        if(depth == engineDepth)
                        {
                            BoardScript.bestMove = move;
                        }    
                    }
                    beta = System.Math.Min(beta, eval);
                    if (beta <= alpha)
                    {
                        break;
                    }
                }
            }
            return minEval;
        }
    }
    public static int evalPos()
    {
        int total = 0;
        //Add White Piece Scores
        foreach((int row, int col) in BoardScript.whitePieces)
        {
            switch (BoardScript.board[row, col]) 
            {
                case 'P':
                    total += 100 + BoardScript.whitePawnBonus[row, col];
                    break;
                case 'N':
                    total += 300 + BoardScript.knightBonus[row, col];
                    break;
                case 'B':
                    total += 300 + BoardScript.bishopBonus[row, col];
                    break;
                case 'R':
                    total += 500;
                    break;
                case 'Q':
                    total += 900;
                    break;
                case 'K':
                    total += 10000 + BoardScript.kingEarlyBonus[row, col];
                    break;
                default:
                    break;
            }
        }
        //Add Black Piece Scores
        foreach ((int row, int col) in BoardScript.blackPieces)
        {
            switch (BoardScript.board[row, col])
            {
                case 'p':
                    total += (-100 - BoardScript.blackPawnBonus[row, col]);
                    break;
                case 'n':
                    total += (-300 - BoardScript.knightBonus[row, col]);
                    break;
                case 'b':
                    total += (-300 - BoardScript.bishopBonus[row, col]);
                    break;
                case 'r':
                    total += -500;
                    break;
                case 'q':
                    total += -900;
                    break;
                case 'k':
                    total += (-10000 - BoardScript.kingEarlyBonus[row, col]);
                    break;
                default:
                    break;
            }
        }

        return total;
    }
    public static bool moveIsLegalAndMake(Move move)
    {
        int iy = move.iy;
        int ix = move.ix;
        int fy = move.fy;
        int fx = move.fx;

        if (moveIsPsuedoLegal(iy, ix, fy, fx))
        {
            makeMove(move);
            BoardScript.whitePsuedoLegalMoves = generatePsuedoLegalMoves(BoardScript.whitePieces);
            BoardScript.blackPsuedoLegalMoves = generatePsuedoLegalMoves(BoardScript.blackPieces);
            if (char.IsUpper(move.piece))
            {
                if (squareIsAttacked(BoardScript.whiteKingPos.row, BoardScript.whiteKingPos.col))
                {
                    undoMove(move);
                    return false;
                }
                return true;
            }
            else
            {
                if (squareIsAttacked(BoardScript.blackKingPos.row, BoardScript.blackKingPos.col))
                {
                    undoMove(move);
                    return false;
                }
                return true;
            }
        }
        return false;
    }
    public static List<Move> orderMoves(List<Move> moves)
    {
        //Captures first
        foreach(Move move in moves)
        {
            if((char.ToLower(move.capture) == 'q'))
            {
                move.scoreGuess += 40;
            }
            if(char.ToLower(move.piece) == 'p' && move.capture != '#')
            {
                if (char.ToLower(move.piece) != 'p')
                {
                    move.scoreGuess += 20;
                }
                move.scoreGuess += 10;
            }
            else if(char.ToLower(move.piece) != 'q' && move.capture != '#')
            {
                move.scoreGuess += 1;
            }
        }
        List<Move> orderedMoves = moves.OrderByDescending(m => m.scoreGuess).ToList();
        return orderedMoves;
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
    public bool blackLongCastleRightsFlag;
    public bool blackShortCastleRightsFlag;
    public bool whiteLongCastleRightsFlag;
    public bool whiteShortCastleRightsFlag;
    public int scoreGuess;

    public Move(int iy, int ix, int fy, int fx, char piece, char capture) 
    {
        this.iy = iy;
        this.ix = ix;
        this.fy = fy;
        this.fx = fx;
        this.piece = piece;
        this.capture = capture;
        this.scoreGuess = 0;
        this.blackShortCastleRightsFlag = false;
        this.blackLongCastleRightsFlag = false;
        this.whiteShortCastleRightsFlag = false;
        this.whiteLongCastleRightsFlag = false;
    }
}
