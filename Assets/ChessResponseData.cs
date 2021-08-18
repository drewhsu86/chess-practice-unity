using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessResponseData 
{
    public int nextTurn; // 1 for white, 2 for black
    public string nextPhase; // for now, choices ("choose", "move")
    public List<int[]> nextLegal;

    public ChessResponseData(int nextTurn, string nextPhase, List<int[]> nextLegal = null) {
        this.nextTurn = nextTurn;
        this.nextPhase = nextPhase;
        if (nextLegal != null) this.nextLegal = nextLegal;
        else this.nextLegal = new List<int[]>();
    }
}
