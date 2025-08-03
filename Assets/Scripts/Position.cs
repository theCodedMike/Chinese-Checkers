using UnityEngine;

public class Position : MonoBehaviour
{
    public int posX; // 行
    public int posY; // 列
    public bool canMove;

    public void SetIdx(int x, int y)
    {
        posX = x;
        posY = y;
    }
}
