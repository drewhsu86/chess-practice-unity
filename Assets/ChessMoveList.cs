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
                    } else {
                        // if it's your own piece, can't add or continue so break
                        break;
                    }

                }

                if (slopeY == 0) break;
            }
            if (slopeX == 0) break;
        }

        foreach (int[] move in legal) {
            print(move[0] + ", " + move[1]);
        }

        return legal;
    }

    public List<int[]> LegalMovesPawn(int[] coord, int[,] board, int owner) {
        // colors: 2 = black, 1 = white
        int l = coord[0];
        int w = coord[1];

        return new List<int[]>();
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
        // knight only checks 4 squares
        List<int[]> legal = new List<int[]>();

        return legal;
    }

}
