using UnityEngine;
using UnityEngine.UI;

public enum Direction
{
    UpLeft,
    UpRight,
    Left,
    Right,
    DownLeft,
    DownRight
}

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
    [Header("人数")]
    public int person = 2;
    [Header("获胜提示")]
    public Text hint;
    
    private const int BoardSize = 18;
    private static readonly Transform[][] BoardGrid = new Transform[BoardSize][]; // 存储所有棋盘位置
    private static readonly Transform[][] Chesses = new Transform[BoardSize][]; // 存储所有棋盘位置
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
    [HideInInspector]
    public int count; // 计数
    [HideInInspector]
    public bool isGameOver; // 游戏结束

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
            Chesses[i] = new Transform[BoardSize];
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
                    Chesses[i][j] = chessObj.transform;
                }
            }
        }
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isGameOver)
        {
            RaycastHit2D hit = Physics2D.Raycast(_mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider)
            {
                Transform hitTransform = hit.transform;
                // 蓝棋先走
                if ((hitTransform.CompareTag("RedChess") && count % person == 1) || (hitTransform.CompareTag("BlueChess") && count % person == 0))
                {
                    CancelCanMove(); // 重置可移动状态
                    _selectedChess = hitTransform;
                    Chess chess = hitTransform.GetComponent<Chess>();
                    SetPosition(highLightObj, chess.posX, chess.posY);
                    _highLightSR.enabled = true;
                    Chesses[chess.posX][chess.posY] = null;
                    SetAllowPlace(chess.posX, chess.posY); // 计算周围可走位置，将可走位置标记为CanMove
                } 
                else if (_selectedChess && hitTransform.CompareTag("Position"))
                {
                    Position targetPos = hitTransform.GetComponent<Position>();
                    if (targetPos.canMove)
                    {
                        SetPosition(_selectedChess.gameObject, targetPos.posX, targetPos.posY);
                        _selectedChess.GetComponent<Chess>().SetIdx(targetPos.posX, targetPos.posY);
                        Chesses[targetPos.posX][targetPos.posY] = _selectedChess;
                        _highLightSR.enabled = false;
                        _selectedChess = null;
                        CancelCanMove();
                        
                        // 游戏结束判定
                        if (GameOver(redChessPrefab, 10, 13, 14, 17))
                        {
                            isGameOver = true;
                            hint.text = "红棋获胜！";
                        } else if (GameOver(blueChessPrefab, 5, 8, 1, 4))
                        {
                            isGameOver = true;
                            hint.text = "蓝棋获胜！";
                        }
                        
                        count++;
                    }
                }
            }
        }
    }

    private void CancelCanMove()
    {
        for (int i = 0; i < BoardGrid.Length; i++)
        {
            for (int j = 0; j < BoardGrid[i].Length; j++)
            {
                if (IsLegalPosition(i, j))
                    BoardGrid[i][j].GetComponent<Position>().canMove = false;
            }
        }
    }

    // 判断该位置是否有棋子
    private bool HasChess(int i, int j) => Chesses[i][j];

    // 获取相邻位置
    //
    //       B(X,Y+1)   C(X+1,Y+1)
    //
    //
    // D(X-1,Y)     A(X,Y)     E(X+1,Y)
    //
    //
    //       F(X-1,Y-1)  G(X,Y-1)
    //
    private Transform GetNeighbor(int i, int j, Direction dir)
    {
        int x = 0;
        int y = 0;
        switch (dir)
        {
            case Direction.UpLeft: x = i; y = j + 1;
                break;
            case Direction.UpRight: x = i + 1; y = j + 1;
                break;
            case Direction.Left: x = i - 1; y = j; 
                break;
            case Direction.Right: x = i + 1; y = j; 
                break;
            case Direction.DownLeft: x = i - 1; y = j - 1; 
                break;
            case Direction.DownRight: x = i; y = j - 1;
                break;
        }

        return IsLegalPosition(x, y) ? BoardGrid[x][y] : null;
    }

    // 判断第一圈周围的棋子可走
    private void FirstJudge(int i, int j, Direction dir)
    {
        Transform neighbor = GetNeighbor(i, j, dir);
        if (neighbor == null) 
            return;
        
        Position pos = neighbor.GetComponent<Position>();
        if (!HasChess(pos.posX, pos.posY))
            pos.canMove = true;
        else
            SecondJudge(pos.posX, pos.posY, dir);
    }
    
    // 判断第二圈周围的棋子可走
    private void SecondJudge(int i, int j, Direction dir)
    {
        Transform neighbor = GetNeighbor(i, j, dir);
        if (neighbor == null) 
            return;
        
        Position pos = neighbor.GetComponent<Position>();
        if (!HasChess(pos.posX, pos.posY))
        {
            if (!pos.canMove)
            {
                pos.canMove = true;
                AddChess(pos.posX, pos.posY, Direction.UpLeft);
                AddChess(pos.posX, pos.posY, Direction.UpRight);
                AddChess(pos.posX, pos.posY, Direction.Left);
                AddChess(pos.posX, pos.posY, Direction.Right);
                AddChess(pos.posX, pos.posY, Direction.DownLeft);
                AddChess(pos.posX, pos.posY, Direction.DownRight);
            }
        }
    }
    
    // 判断第三阶段
    private void AddChess(int i, int j, Direction dir)
    {
        Transform neighbor = GetNeighbor(i, j, dir);
        if (neighbor == null) 
            return;
        
        Position pos = neighbor.GetComponent<Position>();
        if (HasChess(pos.posX, pos.posY))
            SecondJudge(pos.posX, pos.posY, dir);
    }

    // 圈出可走位置
    private void SetAllowPlace(int i, int j)
    {
        FirstJudge(i, j, Direction.UpLeft);
        FirstJudge(i, j, Direction.UpRight);
        FirstJudge(i, j, Direction.Left);
        FirstJudge(i, j, Direction.Right);
        FirstJudge(i, j, Direction.DownLeft);
        FirstJudge(i, j, Direction.DownRight);
    }

    // 胜利判断
    private bool GameOver(GameObject chess, int iUpper, int iLower, int jUpper, int jLower)
    {
        for (int i = iUpper; i <= iLower; i++)
        {
            for (int j = jUpper; j <= jLower; j++)
            {
                if (IsLegalPosition(i, j) && Chesses[i][j])
                {
                    if (!Chesses[i][j].CompareTag(chess.tag))
                        return false;
                } else if (IsLegalPosition(i, j) && !Chesses[i][j])
                    return false;
            }
        }

        return true;
    }
}