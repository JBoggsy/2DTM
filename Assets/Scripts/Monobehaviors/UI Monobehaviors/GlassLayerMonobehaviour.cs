using UnityEngine;

namespace MonoBehaviours {
    namespace UI {
        public class GlassLayerMonobehaviour : MonoBehaviour {
            private int width = 1920;
            private int height = 1080;
            public Camera Camera;
            public Shader Shader;

            public void Awake() {
                Texture2D texture = new Texture2D(width, height);
                Rect rect = new Rect(0, 0, width, height);
                Material material = new Material(Shader);

                gameObject.GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), height);
                gameObject.GetComponent<SpriteRenderer>().material = material;
            }

            private void LateUpdate() {
                gameObject.transform.position = (Vector2)Camera.transform.position;
                float size = Camera.orthographicSize;
                gameObject.transform.localScale = new Vector3(size * 2, size * 2, 1);
            }
        }
    }
}