using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
public class ChessBoard : MonoBehaviour {

    public GameObject RedChess;
    public GameObject BlueChess;
    public GameObject posPrefab;
    public GameObject HeighLight;
    public GameObject SelectChess;
    public AudioSource Audio_Win;
    public AudioSource Audio_Alarm;
    public Text Hint;//文本
    public int count = 1;//回合数
    public bool IsOver = false;//游戏结束标记

    public enum Director
    {
        UpLeft,
        UpRight,
        Left,
        Right,
        DownLeft,
        DownRight
    }
    private SpriteRenderer heighLightRenderer;
    private AudioSource moveChess;
    public GameObject[,] chesses = new GameObject[18, 18];
    private GameObject[,] positions = new GameObject[18, 18];
    private static int[,] pos = {
        { 5,5},//a为1，b的上限是5，下限是5
        { 5,6},//a为2，b的上限是5，下限是6
        { 5,7},//a为3，b的上限是5，下限是7
        { 5,8},//a为4
        { 1,13},//5
        { 2,13},//6
        { 3,13},//7
        { 4,13},//8
        { 5,13},//9
        { 5,14},//10
        { 5,15},//11
        { 5,16},//12
        { 5,17},//13
        { 10,13},//14
        { 11,13},//15
        { 12,13},//16
        { 13,13},//17
    };
    // Use this for initialization
    void Start () {
        moveChess = this.GetComponent<AudioSource>();
        heighLightRenderer = HeighLight.GetComponent<SpriteRenderer>();
        heighLightRenderer.enabled = false;
        CreatePositions();
        CreateChesses(RedChess,5,8,1,4);
        CreateChesses(BlueChess,10,13,14,17);
    }

    // Update is called once per frame
    void Update()
    {
        //当鼠标点击时  
        if (Input.GetMouseButtonDown(0)&&!IsOver)
        {
            //摄像机到点击位置的射线  
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                if (hit.transform.tag == "RedChess"&& count%2==1 || hit.transform.tag == "BlueChess"&&count%2==0)
                {
                    CancelCanMove();
                    //hit.transform.GetComponent<Chess>().IsSelect = true;
                    SelectChess = hit.transform.gameObject;

                    //选中棋子，设置高亮，播放音效
                    SetPosition(HeighLight, hit.transform.GetComponent<Chess>().ChessPosition_X, hit.transform.GetComponent<Chess>().ChessPosition_Y);
                    heighLightRenderer.enabled = true;
                    moveChess.Play();

                    //计算周围可走位置，将可走位置标记为CanMove
                    SetAllowPlace(hit.transform.GetComponent<Chess>().ChessPosition_X, hit.transform.GetComponent<Chess>().ChessPosition_Y);
                }
                else if (hit.transform.tag == "Position" && SelectChess != null)
                {
                    bool canMove = hit.transform.GetComponent<Position>().CanMove;
                    if (canMove)
                    {
                        int Click_X = hit.transform.GetComponent<Position>().Position_X;
                        int Click_Y = hit.transform.GetComponent<Position>().Position_Y;
                        SetPosition(SelectChess, Click_X, Click_Y);

                        chesses[SelectChess.GetComponent<Chess>().ChessPosition_X, SelectChess.GetComponent<Chess>().ChessPosition_Y] = null;//移除该位置储存的棋子
                        SelectChess.GetComponent<Chess>().setIndex(Click_X,Click_Y);//改变选中棋子的位置编号
                        chesses[Click_X, Click_Y] = SelectChess;//在点击位置加入选中的棋子

                        //落子，取消高亮，播放音效，记一回合，将所有位置标记为不可移动
                        SelectChess = null;
                        heighLightRenderer.enabled = false;
                        moveChess.Play();
                        count++;
                        CancelCanMove();
                        
                        //红方胜利
                        if(GameOver(RedChess,10, 13, 14, 17))
                        {
                            IsOver = true;
                            Hint.text = "红方获胜！";
                            Audio_Win.Play();
                        }
                        //蓝方胜
                        if(GameOver(BlueChess, 5, 8, 1, 4))
                        {
                            IsOver = true;
                            Hint.text = "蓝方获胜！";
                            Audio_Win.Play();
                        }
                    }
                    else
                    {
                        Audio_Alarm.Play();//移动选择错误提示音
                    }
                }
            }
        }
    }

    //判断是否是合法位置
    public static bool IsLegalPosition(int X, int Y)
    {
        if (X < 1 || X > 17)
        {
            return false;
        }
        if (Y < pos[X - 1, 0] || Y > pos[X - 1, 1])
        {
            return false;
        }
        return true;
    }

    //根据位置信息设置物体坐标
    public static void SetPosition(GameObject go, int i, int j)
    {
        if (go.tag == "Position")
        {
            go.transform.position = new Vector3(2 * i - j - 9, Mathf.Sqrt(3) * (j - 9), 0);
        }
        else
        {
            go.transform.position = new Vector3(2 * i - j - 9, Mathf.Sqrt(3) * (j - 9), -1);
        }
    }

    //创建棋盘位置
    void CreatePositions()
    {
        for (int i = 0; i <= 17; i++)
        {
            for (int j = 0; j <= 17; j++)
            {
                if (IsLegalPosition(i, j))
                {
                    GameObject go = Instantiate(posPrefab);
                    positions[i, j] = go;
                    go.GetComponent<Position>().setIndex(i, j);//给位置标记
                    SetPosition(go, i, j);//根据位置，设置坐标
                }
            }
        }
    }

    //创建棋子
    public void CreateChesses(GameObject chess,int i_upper,int i_lower,int j_upper,int j_lower)
    {
        int index = 1;
        for(int i = i_upper; i<= i_lower; i++)
        {
            for(int j = j_upper; j <= j_lower; j++)
            {
                if (IsLegalPosition(i, j))
                {
                    GameObject go = Instantiate(chess);
                    chesses[i, j] = go;
                    Chess thisChess = go.GetComponent<Chess>();
                    thisChess.chessIndex = index;
                    thisChess.setIndex(i, j);                  
                    SetPosition(go,i,j);
                    index++;
                }
            }
        }
    }

    //判断该位置是否有棋子
    public bool IsThereChess(int i,int j)
    {
        if (chesses[i, j] == null)
        {
            return false;
        }
        return true;
    }

    //取消可移动状态
    public void CancelCanMove()
    {
        for (int i = 0; i <= 17; i++)
        {
            for (int j = 0; j <= 17; j++)
            {
                if (positions[i , j])
                {
                    positions[i, j].GetComponent<Position>().CanMove = false;
                }
            }
        }
    }

    //获得相邻的位置
    public GameObject GetJoint(int X,int Y,Director director)
    {
        int x = 0;
        int y = 0;
        if (director == Director.UpLeft)
        {
            x = X;
            y = Y + 1;
        }
        else if (director == Director.UpRight)
        {
            x =X + 1;
            y =Y + 1;
        }
        else if (director == Director.Left)
        {
            x =X - 1;
            y =Y;
        }
        else if (director == Director.Right)
        {
            x =X + 1;
            y =Y;
        }
        else if (director == Director.DownLeft)
        {
            x =X - 1;
            y =Y - 1;
        }
        else if (director == Director.DownRight)
        {
            x =X;
            y =Y - 1;
        }
        if (IsLegalPosition(x,y))
        {
            return positions[x,y];
        }
        else
        {
            return null;
        }
    }

    //判断六个方向
    public void SetAllowPlace(int X,int Y)
    {
        FirstJudgment(X,Y,Director.UpLeft);
        FirstJudgment(X, Y, Director.UpRight);
        FirstJudgment(X, Y, Director.Left);
        FirstJudgment(X, Y, Director.Right);
        FirstJudgment(X, Y, Director.DownLeft);
        FirstJudgment(X, Y, Director.DownRight);
    }

    //初次判断
    public void FirstJudgment(int X, int Y, Director director)
    {
        if (GetJoint(X, Y, director))
        {

            if (!IsThereChess(GetJoint(X, Y, director).GetComponent<Position>().Position_X,
            GetJoint(X, Y, director).GetComponent<Position>().Position_Y))
            {
                if (GetJoint(X, Y, director))
                    GetJoint(X, Y, director).GetComponent<Position>().CanMove = true;
                return;
            }
            else
            {
                SecondJudgment(GetJoint(X, Y, director).GetComponent<Position>().Position_X,
                GetJoint(X, Y, director).GetComponent<Position>().Position_Y,director);
            }
        }
    }

    //第二次判断
    public void SecondJudgment(int X, int Y,Director director)
    {
        if (GetJoint(X, Y, director))
        {
            if (!IsThereChess(GetJoint(X, Y, director).GetComponent<Position>().Position_X,
        GetJoint(X, Y, director).GetComponent<Position>().Position_Y))
            {
                if (GetJoint(X, Y, director)&&!GetJoint(X, Y, director).GetComponent<Position>().CanMove)
                {
                    GetJoint(X, Y, director).GetComponent<Position>().CanMove = true;
                    AddChess(GetJoint(X, Y, director).GetComponent<Position>().Position_X,
                        GetJoint(X, Y, director).GetComponent<Position>().Position_Y,Director.UpLeft);
                    AddChess(GetJoint(X, Y, director).GetComponent<Position>().Position_X,
                        GetJoint(X, Y, director).GetComponent<Position>().Position_Y,Director.UpRight);
                    AddChess(GetJoint(X, Y, director).GetComponent<Position>().Position_X,
                        GetJoint(X, Y, director).GetComponent<Position>().Position_Y,Director.Left);
                    AddChess(GetJoint(X, Y, director).GetComponent<Position>().Position_X,
                        GetJoint(X, Y, director).GetComponent<Position>().Position_Y,Director.Right);
                    AddChess(GetJoint(X, Y, director).GetComponent<Position>().Position_X,
                        GetJoint(X, Y, director).GetComponent<Position>().Position_Y,Director.DownLeft);
                    AddChess(GetJoint(X, Y, director).GetComponent<Position>().Position_X,
                        GetJoint(X, Y, director).GetComponent<Position>().Position_Y,Director.DownRight);
                }
            }
            else
            {
                //return;
            }
        }
    }

    //递归算法
    public void AddChess(int X, int Y,Director director)
    {
        if (GetJoint(X, Y, director))
        {
            if (IsThereChess(GetJoint(X, Y, director).GetComponent<Position>().Position_X,
          GetJoint(X, Y, director).GetComponent<Position>().Position_Y))
            {
                SecondJudgment(GetJoint(X, Y, director).GetComponent<Position>().Position_X,
                    GetJoint(X, Y, director).GetComponent<Position>().Position_Y, director);
            }
            else
            {
                return;
            }
        }       
    }

    //判定是否全部移动到对面
    public bool GameOver(GameObject chess, int i_upper, int i_lower, int j_upper, int j_lower)
    {
        for (int i = i_upper; i <= i_lower; i++)
        {
            for (int j = j_upper; j <= j_lower; j++)
            {
                if (IsLegalPosition(i, j)&&chesses[i,j]!=null)
                {
                    if (chesses[i, j].tag != chess.tag)
                    {
                        return false;
                    }
                }
                else if(IsLegalPosition(i, j) && chesses[i, j] == null)
                {
                    return false;
                }
            }
        }
        return true;
    }

}
*/