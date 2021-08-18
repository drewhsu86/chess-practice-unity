using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessMoveList : MonoBehaviour
{
    public bool IsOnBoard(int[] coord) {
        // board goes from 0 to 7 inclusive
        if (coord[0] >= 0 && coord[0] < 8 && coord[1] >= 0 && coord[1] < 8 ) return true;
        return false;
    }

    private List<int []> SearchLinear(int[] coord, int[,] board, int owner, int slopeX, int slopeY) {
        // this pattern is for rooks, bishops, and queens - same capture pattern
        // but at different angles
        // owner is 1 for white and 2 for black 
        List<int []> legal = new List<int []>();

        // for a given slope (based on x and y), iterate through all 4 possible combinations
        // +/- for slopeX and +/i for slopeY
        foreach (int incX in new int[] {-slopeX, slopeX} ) {
            foreach (int incY in new int[] {-slopeY, slopeY}) {
                // iterate down the slope 1 by 1
                // we start one away from the coord 

                int[] newCoord = coord;
                while (IsOnBoard(newCoord)) {

                    // can also break this loop if non-legal move found
                    newCoord = new int[] {newCoord[0] + incX, newCoord[1] + incY};

                    // legal moves are to empty spaces, and spaces with enemy pieces
                    if (!IsOnBoard(newCoord)) break;
                    int otherPiece = board[newCoord[0], newCoord[1]];

                    if (otherPiece == 0 || (int)Mathf.Floor(otherPiece/10) != owner) {
                        // if the space is empty or owned by the other player
                        // then we can move there using this pattern
                        legal.Add(newCoord);
                        if (otherPiece != 0) {
                            // if an enemy piece is there, stop iterating
                            break;
                        }
                    } else {
                        // if it's your own piece, can't add or continue so break
                        break;
                    }

                }

                if (slopeY == 0) break;
            }
            if (slopeX == 0) break;
        }

        return legal;
    }

    public List<int[]> LegalMovesPawn(int[] coord, int[,] board, int owner) {
        List<int[]> legal = new List<int[]>();

        // colors: 2 = black, 1 = white
        int l = coord[0];
        int w = coord[1];

        // starting row numbers for pawns, because they can move up to 2 
        int startL = owner == 1 ? 6 : 1;

        // pawns cannot take directly, only diagonally, so check front and diagonals separately  
        // white starts at 6, negative movement to go forward
        // black starts at 1, positive movement to go forward 
        int forward = 1;
        if (owner == 1) forward = -1;

        // check forward for empty spot 
        // assume pawn cannot exit board, as they will transform when reaching the edge 
        if (board[l+forward,w] == 0) legal.Add(new int[] {l+forward, w});
        if (board[l+2*forward,w] == 0 && l == startL) legal.Add(new int[] {l+2*forward, w});

        // check diagonals. these might be off the edge.
        foreach (int diagW in new int[] {w-1, w+1}) {
            if (IsOnBoard(new int[] {l+forward, diagW})) {
                // if it's on the map, check if there's an enemy there
                int otherPiece = board[l+forward, diagW];
                int otherOwner = (int)Mathf.Floor(otherPiece/10);
                if (otherOwner != owner && otherPiece != 0) legal.Add(new int[] {l+forward, diagW});
            }
        }

        return legal;
    }

    public List<int[]> LegalMovesRook(int[] coord, int[,] board, int owner) {
        List<int[]> legal = new List<int[]>();

        List<int[]> vertical = SearchLinear(coord, board, owner, 1, 0);
        List<int[]> horizontal = SearchLinear(coord, board, owner, 0, 1);

        foreach(int[] move in vertical) { legal.Add(move); }
        foreach(int[] move in horizontal) { legal.Add(move); }
        
        return legal;
    }

    public List<int[]> LegalMovesBishop(int[] coord, int[,] board, int owner) {
        List<int[]> legal = SearchLinear(coord, board, owner, 1, 1);

        return legal;
    }

    public List<int[]> LegalMovesQueen(int[] coord, int[,] board, int owner) {
        List<int[]> legal = new List<int[]>();

        List<int[]> vertical = SearchLinear(coord, board, owner, 1, 0);
        List<int[]> horizontal = SearchLinear(coord, board, owner, 0, 1);
        List<int[]> diagonal = SearchLinear(coord, board, owner, 1, 1);

        foreach(int[] move in vertical) { legal.Add(move); }
        foreach(int[] move in horizontal) { legal.Add(move); }
        foreach(int[] move in diagonal) { legal.Add(move); }
        
        return legal;
    }

    public List<int[]> LegalMovesKnight(int[] coord, int[,] board, int owner) {
        // knight only checks 8 squares
        List<int[]> legal = new List<int[]>();

        int[] checkX = new int[] {1, 1, 2, 2, -1, -1, -2, -2};
        int[] checkY = new int[] {2, -2, 1, -1, 2, -2, 1, -1};

        // I coded i as magic number 8 because same reason the chessboard is using magic numbers
        // this is designed so that knights can't be easily customized
        for (int i = 0; i < 8; i ++) {
            int[] newCoord = new int[] {coord[0] + checkX[i], coord[1] + checkY[i]};

            if (IsOnBoard(newCoord)) {
                // check if each X, Y pair is on the board, then determine if it's open or an enemy
                int otherPiece = board[newCoord[0], newCoord[1]];
                int otherOwner = (int)Mathf.Floor(otherPiece/10);
                if (otherOwner != owner) legal.Add(newCoord);
            }
        }

        return legal;
    }

    public List<int[]> LegalMovesKing(int[] coord, int[,] board, int owner) {
        // king checks the adjacent 8 squares
        List<int[]> legal = new List<int[]>();

        int[] checkX = new int[] {0, 0, 1, -1, 1, 1, -1, -1};
        int[] checkY = new int[] {1, -1, 0, 0, 1, -1, 1, -1};

        // I coded i as magic number 8 because same reason the chessboard is using magic numbers
        // this is designed so that kings can't be easily customized
        for (int i = 0; i < 8; i ++) {
            int[] newCoord = new int[] {coord[0] + checkX[i], coord[1] + checkY[i]};

            if (IsOnBoard(newCoord)) {
                // check if each X, Y pair is on the board, then determine if it's open or an enemy
                int otherPiece = board[newCoord[0], newCoord[1]];
                int otherOwner = (int)Mathf.Floor(otherPiece/10);
                if (otherOwner != owner) legal.Add(newCoord);
            }
        }

        return legal;
    }
}
