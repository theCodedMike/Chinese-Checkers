using UnityEngine;

public class ChessBoard : MonoBehaviour
{
    [Header("棋盘位置")]
    public GameObject posPrefab;
    [Header("红棋")]
    public GameObject redChessPrefab;
    [Header("蓝棋")]
    public GameObject blueChessPrefab;
    [Header("高亮圈")]
    public GameObject highLightObj;
    
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



    private Transform _selectedChess; // 被选中的棋子
    private SpriteRenderer _highLightSR;
    private Camera _mainCamera;

    private void Start()
    {
        CreatePositions();
        
        CreateChesses(redChessPrefab, 5, 8,  1, 4);
        CreateChesses(blueChessPrefab, 10, 13, 14, 17);
        
        _mainCamera = Camera.main;
        _highLightSR = highLightObj.GetComponent<SpriteRenderer>();
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
                    posObj.GetComponent<Position>().SetIdx(i, j);
                    SetPosition(posObj, i, j);
                    BoardGrid[i][j] = posObj.transform;
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

    private void CreateChesses(GameObject chess, int iUpper, int iLower, int jUpper, int jLower)
    {
        for (int i = iUpper; i <= iLower; i++)
        {
            for (int j = jUpper; j <= jLower; j++)
            {
                if (IsLegalPosition(i, j))
                {
                    GameObject chessObj = Instantiate(chess, transform, true);
                    chessObj.GetComponent<Chess>().SetIdx(i, j);
                    SetPosition(chessObj, i, j);
                    BoardGrid[i][j] = chessObj.transform;
                }
            }
        }
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(_mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider)
            {
                Transform hitTransform = hit.transform;
                if (hitTransform.CompareTag("RedChess") || hitTransform.CompareTag("BlueChess"))
                {
                    _selectedChess = hitTransform;
                    Chess chess = hitTransform.GetComponent<Chess>();
                    //print($"Chess: {chess.posX} {chess.posY}");
                    SetPosition(highLightObj, chess.posX, chess.posY);
                    _highLightSR.enabled = true;
                    BoardGrid[chess.posX][chess.posY] = null;
                } 
                else if (_selectedChess && hitTransform.CompareTag("Position"))
                {
                    Position targetPos = hitTransform.GetComponent<Position>();
                    //print($"Position: {clickPos.posX} {clickPos.posY}");
                    SetPosition(_selectedChess.gameObject, targetPos.posX, targetPos.posY);
                    _selectedChess.GetComponent<Chess>().SetIdx(targetPos.posX, targetPos.posY);
                    BoardGrid[targetPos.posX][targetPos.posY] = _selectedChess;
                    _highLightSR.enabled = false;
                    _selectedChess = null;
                }
            }
        }
    }
}