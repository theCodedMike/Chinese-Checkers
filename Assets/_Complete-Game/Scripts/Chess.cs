using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chess : MonoBehaviour {

    public int chessIndex;
    public int ChessPosition_X;
    public int ChessPosition_Y;
    //public bool IsSelect;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void setIndex(int i, int j)
    {
        ChessPosition_X = i;
        ChessPosition_Y = j;
    }
}
