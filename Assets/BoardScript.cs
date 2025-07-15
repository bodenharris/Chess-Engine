using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardScript : MonoBehaviour
{
    public GameObject boardSquare;
    public GameObject piece;
    public Sprite blackRookSprite;
    public Sprite blackKnightSprite;
    public Sprite blackBishopSprite;
    public Sprite blackQueenSprite;
    public Sprite blackKingSprite;
    public Sprite blackPawnSprite;
    public Sprite whiteRookSprite;
    public Sprite whiteKnightSprite;
    public Sprite whiteBishopSprite;
    public Sprite whiteQueenSprite;
    public Sprite whiteKingSprite;
    public Sprite whitePawnSprite;
    public static string startingPos = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
    public static HashSet<(int row, int col)> whitePieces = new HashSet<(int, int)>();
    public static HashSet<(int row, int col)> blackPieces = new HashSet<(int, int)>();
    public static bool[] whitePawnsEP = { false, false, false, false, false, false, false, false };
    public static bool[] blackPawnsEP = { false, false, false, false, false, false, false, false };
    public static bool whiteCanLongCastle = true;
    public static bool whiteCanShortCastle = true;
    public static bool blackCanLongCastle = true;
    public static bool blackCanShortCastle = true;
    public static bool whiteCanCastle = true;
    public static bool blackCanCastle = true;
    public static (int row, int col) whiteKingPos = (7, 4);
    public static (int row, int col) blackKingPos = (0, 4);
    public static char[,] board = new char[8, 8];
    public static bool whiteTurn = true;
    public static bool gameOn = true;
    public static GameObject[,] UnityBoard = new GameObject[8, 8];
    public static List<Move> whitePsuedoLegalMoves;
    public static List<Move> blackPsuedoLegalMoves;
    public static List<Move> whiteLegalMoves;
    public static List<Move> blackLegalMoves;

    public static void PrintCharArray(char[,] array)
    {
        int rows = array.GetLength(0);
        int cols = array.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            string line = "";
            for (int j = 0; j < cols; j++)
            {
                line += array[i, j] + " ";
            }
            Debug.Log(line);
        }
    }
    public void DisplayBoard(char[,] array)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Vector3 position = new Vector3(j, i, 1);
                switch (char.ToLower(array[i, j]))
                {
                    case 'r':
                        GameObject rook = Instantiate(piece, position, Quaternion.Euler(0, 180, 180));
                        SpriteRenderer spriteRenderer1 = rook.GetComponent<SpriteRenderer>();
                        spriteRenderer1.sprite = char.IsUpper(array[i, j]) ? whiteRookSprite : blackRookSprite;
                        UnityBoard[i, j] = rook;
                        rook.GetComponent<Collider2D>().enabled = true;
                        //rook.GetComponent<PieceScript>().enabled = true;
                        rook.name = "Rook";
                        break;

                    case 'n':
                        GameObject knight = Instantiate(piece, position, Quaternion.Euler(0, 180, 180));
                        SpriteRenderer spriteRenderer2 = knight.GetComponent<SpriteRenderer>();
                        spriteRenderer2.sprite = char.IsUpper(array[i, j]) ? whiteKnightSprite : blackKnightSprite;
                        UnityBoard[i, j] = knight;
                        knight.GetComponent<Collider2D>().enabled = true;
                        //knight.GetComponent<PieceScript>().enabled = true;
                        knight.name = "Knight";
                        break;

                    case 'b':
                        GameObject bishop = Instantiate(piece, position, Quaternion.Euler(0, 180, 180));
                        SpriteRenderer spriteRenderer3 = bishop.GetComponent<SpriteRenderer>();
                        spriteRenderer3.sprite = char.IsUpper(array[i, j]) ? whiteBishopSprite : blackBishopSprite;
                        UnityBoard[i, j] = bishop;
                        bishop.GetComponent<Collider2D>().enabled = true;
                        //bishop.GetComponent<PieceScript>().enabled = true;
                        bishop.name = "Bishop";
                        break;

                    case 'q':
                        GameObject queen = Instantiate(piece, position, Quaternion.Euler(0, 180, 180));
                        SpriteRenderer spriteRenderer4 = queen.GetComponent<SpriteRenderer>();
                        spriteRenderer4.sprite = char.IsUpper(array[i, j]) ? whiteQueenSprite : blackQueenSprite;
                        UnityBoard[i, j] = queen;
                        queen.GetComponent<Collider2D>().enabled = true;
                        //queen.GetComponent<PieceScript>().enabled = true;
                        queen.name = "Queen";
                        break;

                    case 'k':
                        GameObject king = Instantiate(piece, position, Quaternion.Euler(0, 180, 180));
                        SpriteRenderer spriteRenderer5 = king.GetComponent<SpriteRenderer>();
                        spriteRenderer5.sprite = char.IsUpper(array[i, j]) ? whiteKingSprite : blackKingSprite;
                        UnityBoard[i, j] = king;
                        king.GetComponent<Collider2D>().enabled = true;
                        // king.GetComponent<PieceScript>().enabled = true;
                        king.name = "King";
                        break;

                    case 'p':
                        GameObject pawn = Instantiate(piece, position, Quaternion.Euler(0, 180, 180));
                        SpriteRenderer spriteRenderer6 = pawn.GetComponent<SpriteRenderer>();
                        spriteRenderer6.sprite = char.IsUpper(array[i, j]) ? whitePawnSprite : blackPawnSprite;
                        UnityBoard[i, j] = pawn;
                        pawn.GetComponent<Collider2D>().enabled = true;
                        //pawn.GetComponent<PieceScript>().enabled = true;
                        pawn.name = "Pawn";
                        break;

                    default:
                        break;
                }
            }
        }
    }
    public void updateDisplayBoard()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                UnityEngine.Object.Destroy(UnityBoard[i, j]);
                UnityBoard[i, j] = null;
            }
        }
        DisplayBoard(board);
    }
    public void boardSetUp()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                //background
                Vector3 position = new Vector3(i, j, 0);
                GameObject square = Instantiate(boardSquare, position, Quaternion.identity);
                Renderer rend = square.GetComponent<Renderer>();
                Color32 boardColor1 = new Color32(238, 238, 210, 255);
                Color32 boardColor2 = new Color32(118, 150, 86, 255);
                rend.material.color = ((i + j) % 2 == 0) ? boardColor1 : boardColor2;
                board[i, j] = '#';
            }
        }


        int row = 0;
        int col = 0;
        for (int index = 0; index < startingPos.Length; index++)
        {
            if (startingPos[index] == '/')
            {
                row++;
                col = 0;
            }
            else if (char.IsLetter(startingPos[index]))
            {
                board[row, col] = startingPos[index];
                if (char.IsUpper(startingPos[index]))
                {
                    whitePieces.Add((row, col));
                }
                else
                {
                    blackPieces.Add((row, col));
                }
                col++;
            }
            else
            {
                col += (startingPos[index] - '0');
            }
        }
        DisplayBoard(board);
        whitePsuedoLegalMoves = new List<Move>();
        blackPsuedoLegalMoves = new List<Move>();
        whiteLegalMoves = new List<Move>();
        blackLegalMoves = new List<Move>();
    }
    public static void PrintPieceLocations(HashSet<(int row, int col)> pieces)
    {
        Debug.Log("Pieces at: ");
        foreach ((int row, int col) in pieces)
        {
            Debug.Log($"Piece at: ({row}, {col})");
        }
    }

}
