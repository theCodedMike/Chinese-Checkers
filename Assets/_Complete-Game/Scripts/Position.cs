using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position : MonoBehaviour {

    public int Position_X;
    public int Position_Y;
    public bool CanMove;
    public Sprite Normal;
    public Sprite HeighLight;

    private SpriteRenderer positionRenderer;
    // Use this for initialization
    void Start()
    {
        positionRenderer = this.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CanMove)
        {
            positionRenderer.sprite = HeighLight;
        }
        else
        {
            positionRenderer.sprite = Normal;
        }
    }

    public void setIndex(int i, int j)
    {
        Position_X = i;
        Position_Y = j;
    }
}
