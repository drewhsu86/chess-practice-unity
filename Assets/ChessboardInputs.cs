using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChessboardInputs : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] ChessController chessController;
    ChessPositionInput[,] boardPositionInputs = new ChessPositionInput[8,8];
 
    private Camera cam;
    int[] selected;
    List<int[]> legal;


    void Start() {
        cam = Camera.main;
        InitializePositions();
    }

    void Update() {
        MouseDetectSquare();
    }

    private void InitializePositions() {
        // create panels based on prefabs and assign them to the array
        // then run their initialization function with inputs
        for (int l = 0; l < 8; l++) {
            for (int w = 0; w < 8; w++) {
                boardPositionInputs[l,w] = Instantiate(panel,  transform).GetComponent<ChessPositionInput>();
                int pieceNum = chessController.board[l,w];
                boardPositionInputs[l,w].InitializeAtPosition(l,w,pieceNum);
            }
        }
    }

    public void ReflectBoard() {
        for (int l = 0; l < 8; l++) {
            for (int w = 0; w < 8; w++) {
                int pieceNum = chessController.board[l,w];
                boardPositionInputs[l,w].SetPiece(pieceNum);
            }
        }
    }

    private void MouseDetectSquare() {
        if (Input.GetMouseButtonDown(0)) {
            // shoot a raycast from the camera 
            // but make sure to get the mouse position
            Vector2 mousePos = new Vector2();

            // Get the mouse position from Event.
            // Note that the y position from Event is inverted.
            mousePos.x = Input.mousePosition.x;
            mousePos.y = Input.mousePosition.y;

            Vector3 point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));
            print("Point: " + point.x + ", " + point.y + ", " + point.z);

            // raycast
            Debug.DrawRay(point, new Vector3(0,0,20f), Color.yellow, 10f, true);
            RaycastHit2D hit = Physics2D.Raycast(point, new Vector3(0,0,1), 20f); 

            if (hit.collider != null) {
                print(hit.collider.name);
                ChessPositionInput squareInput = hit.collider.gameObject.GetComponent<ChessPositionInput>();

                chessController.PrintStatus();
                // process choose or move here
                ProcessClick(squareInput.GetCoord());
            }
        }
    }

    private void SetHighlights(int[] selected, List<int[]> legal, bool addColor) {
        // set the color of selected and legal squares
        // using ChessPositionInput.SetHighlight("selected" "legal" or "")
        if (selected == null) return;
        boardPositionInputs[selected[0], selected[1]].SetHighlight(addColor ? "selected" : "");
        foreach (int[] coord in legal) {
            boardPositionInputs[coord[0], coord[1]].SetHighlight(addColor ? "legal" : "");
        }
    }


    private void ProcessClick(int[] coord) {
        // 1. when clicked during choose phase, light up this square (only if at least one legal move)
        // and light up legal moves' squares
        // 2. when clicked during move phase, if this matches selected piece, cancel (unhighlight)
        // and go back to choose phase
        // 2a. when clicked during move phase, and if it belongs to legal moves, execute move and unhighlight
        // 2b. when clicked during move phase, and if not legal move or selected, do nothing
        // for now, actions that require changes: ("choose", "unselect", "move")
        // A. if we go back to choose state, we either moved or unselected (did no move). Move can be stored in List for ease/
        // executing A always unhighlights everything and resets stored selections
        // B. if we go to move state, we always add selected and legal to stored selections and then highlight them
        // chessController will control it's own internal state and then report it, so we take it as the source of truth

        // call function from chessController and ask it to return the next turn, phase, and a list of legal moves

        // if the clicked square belongs to the list of legal moves
        // we need to process this as a move execution
        if (isLegal(coord)) {
            // execute this move
            print("Execute order 66");
        } else {
            // determine what to highlight or to unhighlight
            ChessResponseData response = chessController.InquireMove(coord);
            int nextTurn = response.nextTurn;
            string nextPhase = response.nextPhase;
            List<int[]> nextLegal = response.nextLegal;

            if (nextPhase == "move" && nextLegal.Count > 0) {
                // going into move phase
                // that means selected and legal are filled in
                print("set highlights");
                selected = coord;
                legal = nextLegal;
                SetHighlights(selected, legal, true);
            } else {
                // the current actions cancel the previous stage
                // selected and legal are emptied out 
                print("remove highlights");
                SetHighlights(selected, legal, false);
                selected = null;
                legal = null;
            }
        }
    }

    private bool isLegal(int[] coord) {
        // check if this coordinate is within legal
        if (legal == null) return false;

        foreach(int[] legalCoord in legal) {
            if (legalCoord[0] == coord[0] && legalCoord[1] == coord[1]) return true;
        }
        return false;
    }

}

