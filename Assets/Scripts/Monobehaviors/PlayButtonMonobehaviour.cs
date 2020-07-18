using UnityEngine;
using UnityEngine.UI;

public class PlayButtonMonobehaviour : MonoBehaviour
{
    public Texture2D PlayTexture;
    public Texture2D PauseTexture;

    private Sprite PlaySprite;
    private Sprite PauseSprite;

    public void Awake()
    {
        Rect rect = new Rect(0.0f, 0.0f, PlayTexture.width, PlayTexture.height);
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        PlaySprite = Sprite.Create(PlayTexture, rect, pivot);
        PauseSprite = Sprite.Create(PauseTexture, rect, pivot);

        gameObject.GetComponent<Image>().sprite = PlaySprite;
    }

    public void UpdateTexture()
    {
        if (GameMaster.Instance.Running)
        {
            gameObject.GetComponent<Image>().sprite = PauseSprite;
        }
        else
        {
            gameObject.GetComponent<Image>().sprite = PlaySprite;
        }
    }
}
