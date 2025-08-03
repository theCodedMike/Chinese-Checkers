using UnityEngine;

public class Chess : MonoBehaviour
{
    public int posX;
    public int posY;

    public void SetIdx(int i, int j)
    {
        posX = i;
        posY = j;
    }
}
