using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;


public class ChangeTexturePixel : MonoBehaviour
{
    private RawImage mUITex;
    private Texture2D MyTex;
    public int Radius = 10;
    public Color Col = new Color(0, 0, 0, 0);
    public Color Col2 =new Color(0,0,0,0);
    private int[][] pixelArray;
    private Dictionary<int,TexturePixel> texPixelDic=new Dictionary<int, TexturePixel>(); 
    void Awake()
    {
        mUITex = GetComponent<RawImage>();
        var tex = mUITex.texture as Texture2D;
        
        MyTex =new Texture2D(tex.width, tex.height, TextureFormat.ARGB32, false); // new Texture2D(tex.width, tex.height, tex.format, false);
        
        MyTex.SetPixels(tex.GetPixels());
        //MyTex.SetPixels(tex.GetPixels());
        MyTex.Apply();
        mUITex.texture = MyTex;

        //---
        int value = 0;
        pixelArray=new int[MyTex.width][];
        for (int i = 0; i < pixelArray.Length; i++)
        {
            pixelArray[i]=new int[MyTex.height];
            for (int j = 0; j < MyTex.height; j++)
            {
                pixelArray[i][j] = value;

                texPixelDic.Add(value,new TexturePixel(MyTex,i,j));
                value++;
            }
        }
        //Debug.Log(MyTex ==texPixelDic[pixelArray[10][10]].myTex);
        Debug.Log(value);
        for (int i = 0; i < 200; i++)
        {
            for (int j = 0; j < 100; j++)
            {
                value = pixelArray[i][j];
                //Debug.Log(value);
                texPixelDic[pixelArray[i][j]].Scratch(Col);
                
            }
        }
        MyTex.Apply();
    }

    void ChangePixelColorByCircle(int x, int y, int radius, Color col)
    {
        for (int i = -Radius; i < Radius; i++)
        {
            var py = y + i;
            if (py < 0 || py >= MyTex.height)
            {
                continue;
            }

            for (int j = -Radius; j < Radius; j++)
            {
                var px = x + j;
                if (px < 0 || px >= MyTex.width)
                {
                    continue;
                }
                if (new Vector2(px - x, py - y).magnitude > Radius)
                {
                    continue;
                }
                Profiler.BeginSample("text1");
                TexturePixel tp ;//= texPixelDic[pixelArray[MyTex.width - 1][py]];
                
                if (px==0)
                {
                    tp = texPixelDic[pixelArray[MyTex.width - 1][py]];
                    // px = MyTex.width - 1;
                    //MyTex.SetPixel(MyTex.width-1,py,Col);

                    if (tp.GetScratchedTime()<1)
                    {
                        tp.Scratch(Col);
                    }
                    else
                    {
                        tp.Scratch(Col2);
                    }
                }
                //MyTex.SetPixel(px, py, Col);
                tp = texPixelDic[pixelArray [px][py]];
                if (tp.GetScratchedTime() < 1)
                {
                    tp.Scratch(Col);
                }
                else
                {
                    tp.Scratch(Col2);
                }
                Profiler.EndSample();
            }
        }
        Profiler.BeginSample("text2");
        MyTex.Apply();
        Profiler.EndSample();
        Profiler.BeginSample("text3");


        int radiusBigger =(int)( Radius*2.0f);
        for (int i = -radiusBigger; i < radiusBigger; i++)
        {
            var py = y + i;
            if (py < 0 || py >= MyTex.height)
            {
                continue;
            }

            for (int j = -radiusBigger; j < radiusBigger; j++)
            {
                var px = x + j;
                if (px < 0 || px >= MyTex.width)
                {
                    continue;
                }
                if (new Vector2(px - x, py - y).magnitude > radiusBigger)
                {
                    continue;
                }
               
                TexturePixel tp;//= texPixelDic[pixelArray[MyTex.width - 1][py]];

                if (px == 0)
                {
                    tp = texPixelDic[pixelArray[MyTex.width - 1][py]];
                    // px = MyTex.width - 1;
                    //MyTex.SetPixel(MyTex.width-1,py,Col);

                    tp.SetScratchedTime();
                }
                //MyTex.SetPixel(px, py, Col);
                tp = texPixelDic[pixelArray[px][py]];
                tp.SetScratchedTime();
                
            }
        }


        //float iMax= MyTex.width;
        //int jMax = MyTex.height;
        //for (int i = 0; i < iMax; i++)
        //{
        //    for (int j = 0; j < jMax; j++)
        //    {
        //        texPixelDic[pixelArray[i][j]].SetScratchedTime();
        //    }
        //}
        Profiler.EndSample();
    }

    int[] WorldPos2Pix(Vector3 worldPos)
    {
        var temp = transform.InverseTransformPoint(worldPos);
        var pos = new Vector2(temp.x + mUITex.rectTransform.sizeDelta.x / 2, temp.y + mUITex.rectTransform.sizeDelta.y / 2);
        float rateX = mUITex.rectTransform.sizeDelta.x / (float)MyTex.width;
        float rateY = mUITex.rectTransform.sizeDelta.y / (float)MyTex.height;

        return new[]
        {
            (int)(pos.x / rateX), (int)(pos.y/rateY)
        };

    }

    Vector2 ScreenPoint2Pixel(Vector2 mousePos)
    {
        float imageWidth = mUITex.rectTransform.sizeDelta.x;
        float imageHeight = mUITex.rectTransform.sizeDelta.y;
        Vector3 imagePos = mUITex.rectTransform.anchoredPosition3D;
        //求鼠标在image上的位置
        float HorizontalPercent = (mousePos.x - (Screen.width/2 + imagePos.x - imageWidth/2))/imageWidth; //鼠标在Image 水平上的位置  %
        float verticalPercent = (mousePos.y - (Screen.height/2 + imagePos.y - imageHeight/2))/imageHeight;//鼠标在Image 垂直上的位置  %
        float x = HorizontalPercent*MyTex.width;
        float y = verticalPercent*MyTex.height;
        return new Vector2(x,y);
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            //Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,10)); //Camera.main .ScreenToWorldPoint(Input.mousePosition);
            //Debug.Log(worldPos);
            //var posA = WorldPos2Pix(worldPos);
            //Debug.Log(posA[0] +"  "+posA[1]);
            var posA= ScreenPoint2Pixel(Input.mousePosition);
            ChangePixelColorByCircle((int)posA.x, (int)posA.y, Radius, Col);
            //Debug.Log(Input.mousePosition);
        }
        //if (Input.GetMouseButtonDown(1))
        //{
        //    for (int i = 0; i < MyTex.height; i++)
        //    {
        //        for (int j = 0; j < MyTex.width; j++)
        //        {
        //            if (MyTex.GetPixel(j,i).r!=0)
        //            {
        //                Debug.Log("x:"+j+"  y:"+i);
        //            }
        //        }
        //    }
        //}

        if (Input.GetMouseButtonDown(2))
        {
            for (int i = 0; i < MyTex.height; i++)
            {
                MyTex.SetPixel(MyTex.width-1, i, Col);
            }
            MyTex.Apply();
        }
    }
}