using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TexturePixel
{
    public Texture2D myTex;
    //float alpha = 1;        //当前透明度
    int scratchedTime = 0;//被刮的次数
    private int x;      //像素坐标X
    private int y;      //像素坐标Y
    private bool scratcedPrevious = false;
    private bool scratcedCurrent = false;
    public TexturePixel(Texture2D tex,int x,int y)
    {
        myTex = tex;
        this.x = x;
        this.y = y;
    }

    public void Scratch( Color targetCol)
    {
        myTex.SetPixel(x,y,targetCol);
        scratcedCurrent = true;
        //Debug.Log("x:"+x+"  y:"+y+"  a "+ targetCol.a);
    }

    public int GetScratchedTime()
    {
        return scratchedTime;
    }

    public void SetScratchedTime()
    {
        if (scratcedPrevious &&!scratcedCurrent)
        {
            scratchedTime++;
        }
        scratcedPrevious = scratcedCurrent;
        scratcedCurrent = false;

    }
}
