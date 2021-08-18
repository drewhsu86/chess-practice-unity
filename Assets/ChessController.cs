using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChessController : MonoBehaviour
{
    private enum Phases {choose, move};
    private Phases phase = Phases.choose;
    private int turn = 1; // 1 for white, 2 for black
    public List<int> blackCaptures = new List<int>();
    public List<int> whiteCaptures = new List<int>();

    public int[,] board = new int[8,8] {
        {25,26,27,28,29,27,26,25},
        {21,21,21,21,21,21,21,21},
        {0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0},
        {11,11,11,11,11,11,11,11},
        {15,16,17,18,19,17,16,15}
    };
    ChessMoveList moveList;
    public Dictionary<int, string> pieceDictionary = new Dictionary<int, string> {
        {0, "    "},
        {25, "B-R"},
        {26, "B-N"},
        {27, "B-B"},
        {28, "B-Q"},
        {29, "B-K"},
        {21, "b-p"},
        {15, "W-R"},
        {16, "W-N"},
        {17, "W-B"},
        {18, "W-Q"},
        {19, "W-K"},
        {11, "w-p"},
    };

    public Sprite[] spriteList;
    public int[] spriteIndices = new int[] {11, 15, 16, 17, 18, 19, 21, 25, 26, 27, 28, 29};

    [SerializeField] Text statusText;

    // Methods start here
    void Start() {
        moveList = GetComponent<ChessMoveList>();
        PrintBoard();

        ListLegalMoves(new int[]{1,7});
    }

    private string PieceName(int pieceNum) {
        return pieceDictionary[pieceNum];
    }

    private int BoardPiece(int[] coord) {
        return board[coord[0],coord[1]];
    }

    private void PrintBoard() {
        // prints to unity console, does not do anything in game
        for (int l = 0; l < 8; l++) {
            string printLine = "";
            for (int w = 0; w < 8; w++) {
                printLine += " | " + PieceName(board[l,w]) + " | ";
            }
            print(printLine);
        }
    }

    private List<int[]> ListLegalMoves(int[] coord) {
        int l = coord[0];
        int w = coord[1];
        if (l < 0 || w < 0 || l > 7 || w > 7) {
            return new List<int[]>();
        }

        print("Checking legal moves for: " + l + ", " + w);
        print("This piece is a: " + PieceName(board[l,w]));
        
        // container for legal moves
        List<int[]> legalMoves = new List<int[]>();
        
        // run method for checking legal moves for this piece

        return legalMoves;
    }

    private void MovePiece(int[] src, int[] dest) {
        // move the piece from src to dest if both are on the board
        int piece  = board[src[0], src[1]];
        if (moveList.IsOnBoard(src) && moveList.IsOnBoard(src) && piece != 0) {
            int takenPiece = board[dest[0], dest[1]];
            board[src[0], src[1]] = 0;
            board[dest[0], dest[1]] = piece;

            // record taken piece
            // TODO
        }
    }

    // for testing private methods
    public void TestMovePiece(int srcX, int srcY, int destX, int destY) {
        int[] src = new int[] {srcX, srcY};
        int[] dest = new int[] {destX, destY};
        MovePiece(src, dest);
        PrintBoard();
    }

    public void PrintStatus() {
        // shows the turn and phase on the textbox
        string turnStr = turn == 1 ? "white" : "black";
        string phaseStr = phase == Phases.choose ? "Choose a piece to move." : "Finalize move.";
        if (statusText != null) {
            statusText.text = "Currently " + turnStr + "'s turn to move.\n" + phaseStr;
        }
    }

    public void ProcessPhase(bool changeTurn, bool changePhase) {
        if (changeTurn) turn = turn == 1 ? 2 : 1;
        if (changePhase) phase = phase == Phases.choose ? Phases.move : Phases.choose;
        print("New turn/phase: " + turn + " / " + phase);
    }

    public ChessResponseData InquireMove(int[] coord) {
        // hey, coord was just clicked, what should the next turn and phase be?
        
        string nextPhase = phase == Phases.move ? "move" : "choose"; // by default, no phase change
        List<int[]> nextLegal = new List<int[]>();

        // describe the piece
        int pieceNum = board[coord[0], coord[1]];
        int owner = (int)Mathf.Floor(pieceNum/10);
        int rank = pieceNum%10;
    

        bool ownersTurn = BoardPiece(coord) != 0 && owner == turn; 

        if (phase == Phases.choose && ownersTurn) {
            // if it's this piece's owner's turn and they chose a piece
            switch (rank) {
                case 9: 
                    nextLegal = moveList.LegalMovesKing(coord, board, owner);
                    break;
                case 8: 
                    nextLegal = moveList.LegalMovesQueen(coord, board, owner);
                    break;
                case 7: 
                    nextLegal = moveList.LegalMovesBishop(coord, board, owner);
                    break;
                case 6: 
                    nextLegal = moveList.LegalMovesKnight(coord, board, owner);
                    break;
                case 5: 
                    nextLegal = moveList.LegalMovesRook(coord, board, owner);
                    break;
                case 1:
                    nextLegal = moveList.LegalMovesPawn(coord, board, owner);
                    break;
            }
        }

        if (ownersTurn) {
            
            if (phase == Phases.choose) {              
                nextPhase = "move";
            } else {
                nextPhase = "choose";
            }
            ProcessPhase(false, true); // iterates  phase, but turn only iterates on successful move
            
        } else {
            // if empty space is clicked, always go to choose phase
            phase = Phases.choose;
            nextPhase = "choose";
        }

        print("Next Phase:" + nextPhase);
        print("Piece:" + BoardPiece(coord));

        return new ChessResponseData(1, nextPhase, nextLegal);
    }

    public void ExecuteMove(int[] selected, int[] dest) {
        // this is only when a click is confirmed a move
        // aka a clicked piece is on the legal moves list in ChessboardInputs 

        int destPieceNum = board[dest[0], dest[1]];
        if (destPieceNum != 0) {
            // if it's an enemy piece, determine where to store it and then store it 
            int owner = (int)Mathf.Floor(destPieceNum/10);
            int rank = destPieceNum%10;

            if (owner == 1) whiteCaptures.Add(destPieceNum);
            else blackCaptures.Add(destPieceNum);

            // if king, end the game 
            if (rank == 9) Endgame(owner == 1 ? 2 : 1);
        }

        // actually move the thing 
        int pieceNum = board[selected[0], selected[1]];
        board[selected[0], selected[1]] = 0; // old space empty
        board[dest[0], dest[1]] = pieceNum; // moved to new space

        // change turn and phase upon execution 
        ProcessPhase(true, true);
    }

    private void Endgame(int winner) {
        string winScene = winner == 1 ? "WhiteWin" : "BlackWin";
        SceneManager.LoadScene(winScene);
    }
}


// two turns, 1 and 2 (white and black)
// two phases per turn - choose and move
// during choose phase, any square can be clicked, but only squares that return legal moves go to move phase
// during move phase, one of the legal squares is clicked. If the clicked square is in the legal moves list, execute move
// clicking other squares stays in move phase. clicking original piece goes back to choose phase 
// (can light up legal moves if necessary)