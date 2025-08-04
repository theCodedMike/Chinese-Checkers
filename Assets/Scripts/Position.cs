using UnityEngine;

public class Position : MonoBehaviour
{
    // 把可移动位置设置为高亮黄色，不可移动位置设置为黑色
    [Header("正常贴图")]
    public Sprite normal;
    [Header("高亮贴图")]
    public Sprite highLight;
    
    
    public int posX; // 行
    public int posY; // 列
    public bool canMove;
    
    
    private SpriteRenderer _posRender;
    
    
    private void Start()
    {
        _posRender = GetComponent<SpriteRenderer>();
    }

    public void SetIdx(int x, int y)
    {
        posX = x;
        posY = y;
    }

    private void Update()
    {
        _posRender.sprite = canMove ? highLight : normal;
    }
}
