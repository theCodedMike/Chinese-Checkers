using UnityEngine;

public class ChessBoard : MonoBehaviour
{
    public GameObject posPrefab;

    private const int BoardSize = 18;
    private static readonly Transform[][] BoardGrid = new Transform[BoardSize][]; // 存储所有棋盘位置

    private static readonly int[][] Pos =
    {
        new[] { 5, 5 }, // X为1，Y的上限为5，下限为5
        new[] { 5, 6 }, // X为2
        new[] { 5, 7 }, // X为3
        new[] { 5, 8 }, // X为4
        new[] { 1, 13 }, // X为5
        new[] { 2, 13 }, // X为6
        new[] { 3, 13 }, // X为7
        new[] { 4, 13 }, // X为8
        new[] { 5, 13 }, // X为9
        new[] { 5, 14 }, // X为10
        new[] { 5, 15 }, // X为11
        new[] { 5, 16 }, // X为12
        new[] { 5, 17 }, // X为13
        new[] { 10, 13 }, // X为14
        new[] { 11, 13 }, // X为15
        new[] { 12, 13 }, // X为16
        new[] { 13, 13 }, // X为17
    };

    private static readonly float Sqrt3 = Mathf.Sqrt(3);

    private void Start()
    {
        CreatePositions();
    }

    // 生成棋盘位置
    private void CreatePositions()
    {
        for (int i = 0; i < BoardSize; i++)
        {
            BoardGrid[i] = new Transform[BoardSize];
            for (int j = 0; j < BoardSize; j++)
            {
                if (IsLegalPosition(i, j))
                {
                    GameObject posObj = Instantiate(posPrefab, transform, true);
                    BoardGrid[i][j] = posObj.transform;
                    posObj.GetComponent<ChessPosition>().SetIdx(i, j);
                    SetPosition(posObj, i, j);
                }
            }
        }
    }

    private static void SetPosition(GameObject go, int i, int j)
    {
        go.transform.position =
            new Vector3(2 * i - j - 9, Sqrt3 * (j - 9), go.CompareTag("Position") ? 0 : -1);
    }

    private static bool IsLegalPosition(int x, int y)
    {
        if (x < 1 || x > 17)
            return false;
        if (y < Pos[x - 1][0] || y > Pos[x - 1][1])
            return false;

        return true;
    }
}