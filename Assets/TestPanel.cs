using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestPanel : MonoBehaviour
{
    [SerializeField] ChessController chessController;
    [SerializeField] ChessboardInputs chessboardInputs;
    [SerializeField] InputField inputSrcX;
    [SerializeField] InputField inputSrcY;
    [SerializeField] InputField inputDestX;
    [SerializeField] InputField inputDestY;

    bool canRun = false;
    void Start()
    {
        if (chessController != null) canRun = true;
    }

    public void RunTest() {
        print("testing");
        if (canRun) {
            int srcX = int.Parse(inputSrcX.text);
            int srcY = int.Parse(inputSrcY.text);
            int destX = int.Parse(inputDestX.text);
            int destY = int.Parse(inputDestY.text);

            chessController.TestMovePiece(srcX, srcY, destX, destY);
            if (chessboardInputs != null) chessboardInputs.ReflectBoard();
        }
    }

}
