using UnityEngine;
using UnityEngine.UI;


public class ChangeTexturePixel : MonoBehaviour
{
    private RawImage mUITex;
    private Texture2D MyTex;
    public int Radius = 10;
    public Color Col = new Color(0, 0, 0, 0);
    void Awake()
    {
        mUITex = GetComponent<RawImage>();
        var tex = mUITex.texture as Texture2D;
        
        MyTex =new Texture2D(tex.width, tex.height, TextureFormat.ARGB32, false); // new Texture2D(tex.width, tex.height, tex.format, false);
        
        MyTex.SetPixels(tex.GetPixels());
        //MyTex.SetPixels(tex.GetPixels());
        MyTex.Apply();
        mUITex.texture = MyTex;
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
                MyTex.SetPixel(px, py, Col);
                //Debug.Log("px:"+px);
                //Debug.Log("py:" + py);
            }
        }
        MyTex.Apply();
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
    }
}