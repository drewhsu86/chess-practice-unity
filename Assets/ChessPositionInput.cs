using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChessPositionInput : MonoBehaviour
{
    [SerializeField] int x;
    [SerializeField] int y;
    [SerializeField] float sideLength = 1;
    ChessController chessController;
    GameObject selected;
    GameObject legal;
    TextMesh displayText;
    SpriteRenderer pieceSprite;
    Dictionary<int, string> pieceDictionary;
    Sprite[] spriteList;
    int[] spriteIndices;


    public void SetTextDisplay(bool showDisplay) {
        if (!showDisplay) displayText.text = "";
    }

    private void ShowSprite(int pieceNum) {
        // find index in spriteIndices and reflect the sprite in spriteList
        if (pieceNum == 0) { 
            pieceSprite.sprite = null;
            return;
        }
        int spriteIndex = Array.IndexOf(spriteIndices, pieceNum);
        pieceSprite.sprite = spriteList[spriteIndex];
    }

    public void InitializeAtPosition(int l, int w, int pieceNum, bool textDisplay) {
        selected = transform.Find("Selected").gameObject;
        legal = transform.Find("Legal").gameObject;

        chessController = GameObject.Find("ChessController").GetComponent<ChessController>();
        pieceSprite = transform.Find("Piece").GetComponent<SpriteRenderer>();

        pieceDictionary = chessController.pieceDictionary;
        spriteList = chessController.spriteList;
        spriteIndices = chessController.spriteIndices;

        // l and w go from 0 to 7
        // the chess board (empty object) is the upper left corner
        // the transform's corners should be set according to the square sideLength
        x = l;
        y = w;
        transform.localScale = new Vector2(sideLength, sideLength);
        transform.localPosition = new Vector2(w*sideLength,-l*sideLength);

        displayText = transform.Find("TextMesh").GetComponent<TextMesh>();
        
        if ((l+w)%2 != 0) {
            // if this square's position is even (0,0 ... 0,2 ... 2,2) etc.
            // it should be white (alternating ones are black)
            displayText.color = Color.white;
            GetComponent<SpriteRenderer>().color = Color.black;
        }

        gameObject.name = "ChessboardSquare-"+l+","+w;
        if (textDisplay) displayText.text = pieceDictionary[pieceNum];
        ShowSprite(pieceNum);
    }

    public void SetPiece(int pieceNum) {
        displayText.text = pieceDictionary[pieceNum];
        ShowSprite(pieceNum);
    }

    public int[] GetCoord() {
        return new int[] {x,y};
    }

    public void SetHighlight(string highlight) {
        switch (highlight)
        {
            case "selected":
                selected.SetActive(true);
                legal.SetActive(false);
                break;
            case "legal":
                selected.SetActive(false);
                legal.SetActive(true);
                break;
            default:
                selected.SetActive(false);
                legal.SetActive(false);
                break;
        }
    }

}
