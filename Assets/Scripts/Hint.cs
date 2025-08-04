using UnityEngine;
using UnityEngine.UI;

public class Hint : MonoBehaviour
{
    [Header("提示")]
    public Text hint;
    
    private ChessBoard _chessBoard;

    private void Start()
    {
        _chessBoard = FindFirstObjectByType<ChessBoard>();
    }

    private void Update()
    {
        if (_chessBoard.isGameOver)
            return;
        
        if (_chessBoard.count % _chessBoard.person == 1)
        {
            hint.text = "请红方移动";
            hint.color = Color.red;
        }
        else
        {
            hint.text = "请蓝方移动";
            hint.color = Color.blue;
        }
    }
}
