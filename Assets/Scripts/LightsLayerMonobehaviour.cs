using UnityEngine;

using Constants;

public class LightsLayerMonobehaviour : MonoBehaviour
{
    private const int ScreenWidth = 1920;
    private const int ScreenHeight = 1080;
    private const int HorizontalCount = 128;
    private const int VerticalCount = 72;

    public Camera Camera;
    public Shader Shader;

    public Color OnColor;
    public Color OffColor;
    public Color ErrorColor;

    private Texture2D StateTexture;

    public void Awake()
    {
        StateTexture = new Texture2D(HorizontalCount, VerticalCount) {
            filterMode = FilterMode.Point
        };
        Texture2D texture = new Texture2D(ScreenWidth, ScreenHeight);
        Rect rect = new Rect(0, 0, ScreenWidth, ScreenHeight);
        Material material = new Material(Shader);

        gameObject.GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), ScreenHeight);
        gameObject.GetComponent<SpriteRenderer>().material = material;
        gameObject.GetComponent<SpriteRenderer>().material.SetTexture("_StateTexture", StateTexture);

    }

    private void LateUpdate()
    {
        gameObject.transform.position = (Vector2)Camera.transform.position;
        float size = Camera.orthographicSize;
        gameObject.transform.localScale = new Vector3(size * 2, size * 2, 1);

        UpdateShader();
    }

    private void UpdateShader()
    {
        int x_offset = Mathf.FloorToInt(Camera.transform.position.x) - (HorizontalCount / 2);
        int y_offset = Mathf.FloorToInt(Camera.transform.position.y) - (VerticalCount / 2);

        for (int y = 0; y < VerticalCount; y++)
        {
            for (int x = 0; x < HorizontalCount; x++)
            {
                Color color = GetCellColor(x + x_offset, y + y_offset);
                StateTexture.SetPixel(x, y, color);
            }
        }
        StateTexture.Apply();
        gameObject.GetComponent<SpriteRenderer>().material.SetInt("x_offset", x_offset);
        gameObject.GetComponent<SpriteRenderer>().material.SetInt("y_offset", y_offset);
    }

    private Color GetCellColor(int x, int y)
    {
        Vector3Int tile_loc = new Vector3Int(x, y, 0);
        TM_Symbol cellSymbol = GameMaster.Instance.GridData.GetCellSymbol(tile_loc);
        switch (cellSymbol)
        {
            case TM_Symbol.OFF:
                return OffColor;
            case TM_Symbol.ON:
                return OnColor;
            case TM_Symbol.NUMBER:
                return ErrorColor;
            default:
                return ErrorColor;
        }
    }
}
