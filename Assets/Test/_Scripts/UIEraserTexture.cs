using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UIEraserTexture : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public RawImage image;
    public int brushScale = 4;

    Texture2D texRender;
    RectTransform mRectTransform;
    Canvas canvas;

    void Awake()
    {
        mRectTransform = GetComponent<RectTransform>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    void Start()
    {

        texRender = new Texture2D(image.mainTexture.width, image.mainTexture.height, TextureFormat.ARGB32, true);

        Reset();

    }

    bool isMove = false;

    public void OnPointerDown(PointerEventData data)
    {
        Debug.Log("OnPointerDown..." + data.position);
        start = ConvertSceneToUI(data.position);
        isMove = true;
    }

    public void OnPointerUp(PointerEventData data)
    {
        isMove = false;
        Debug.Log("OnPointerUp..." + data.position);
        OnMouseMove(data.position);
        start = Vector2.zero;
    }

    void Update()
    {
        if (isMove)
        {
            OnMouseMove(Input.mousePosition);
        }
    }

    Vector2 start = Vector2.zero;
    Vector2 end = Vector2.zero;

    Vector2 ConvertSceneToUI(Vector3 posi)
    {
        Vector2 postion;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(mRectTransform, posi, canvas.worldCamera, out postion))
        {
            return postion;
        }
        return Vector2.zero;
    }

    void OnMouseMove(Vector2 position)
    {

        end = ConvertSceneToUI(position);

        Draw(new Rect(end.x + texRender.width / 2, end.y + texRender.height / 2, brushScale, brushScale));

        if (start.Equals(Vector2.zero))
        {
            return;
        }

        Rect disract = new Rect((start + end).x / 2 + texRender.width / 2, (start + end).y / 2 + texRender.height / 2, Mathf.Abs(end.x - start.x), Mathf.Abs(end.y - start.y));

        for (int x = (int)disract.xMin; x < (int)disract.xMax; x++)
        {
            for (int y = (int)disract.yMin; y < (int)disract.yMax; y++)
            {
                Draw(new Rect(x, y, brushScale, brushScale));
            }
        }

        start = end;
    }

    void Reset()
    {

        for (int i = 0; i < texRender.width; i++)
        {

            for (int j = 0; j < texRender.height; j++)
            {

                Color color = texRender.GetPixel(i, j);
                color.a = 1;
                texRender.SetPixel(i, j, color);
            }
        }

        texRender.Apply();
        image.material.SetTexture("_RendTex", texRender);

    }

    void Draw(Rect rect)
    {

        for (int x = (int)rect.xMin; x < (int)rect.xMax; x++)
        {
            for (int y = (int)rect.yMin; y < (int)rect.yMax; y++)
            {
                if (x < 0 || x > texRender.width || y < 0 || y > texRender.height)
                {
                    return;
                }
                Color color = texRender.GetPixel(x, y);
                color.a = 0;
                texRender.SetPixel(x, y, color);
            }
        }

        texRender.Apply();
        image.material.SetTexture("_RendTex", texRender);
    }

}