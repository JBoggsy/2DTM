using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Camera_Controller : MonoBehaviour
{
    public float CAM_SPEED;
    public bool moved { get; set; }
    public Rect world_view_rect { 
        get {
            Rect return_rect;
            Vector3 bot_left = gameObject.GetComponent<Camera>().ViewportToWorldPoint(Vector3.zero);
            Vector3 top_rite = gameObject.GetComponent<Camera>().ViewportToWorldPoint(Vector3.one);
            float xMin = bot_left.x;
            float yMin = bot_left.y;
            float xMax = top_rite.x;
            float yMax = top_rite.y;
            float width = xMax - xMin;
            float height = yMax - yMin;
            return_rect = new Rect(xMin, yMin, width, height);
            return return_rect;
        } 
    }

    // Start is called before the first frame update
    void Start()
    {
        moved = true;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 translation_vector = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) {
            translation_vector += Vector3.up * CAM_SPEED * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S)) {
            translation_vector += Vector3.down * CAM_SPEED * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A)) {
            translation_vector += Vector3.left * CAM_SPEED * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D)) {
            translation_vector += Vector3.right * CAM_SPEED * Time.deltaTime;
        }
        gameObject.transform.Translate(translation_vector);

        float scrollDeltaY = Input.mouseScrollDelta.y;
        if (scrollDeltaY > 0) {
            gameObject.GetComponent<Camera>().orthographicSize /= 1.2f;
            CAM_SPEED /= 1.2f;
        } else if (scrollDeltaY < 0) {
            gameObject.GetComponent<Camera>().orthographicSize *= 1.2f;
            CAM_SPEED *= 1.2f;
        }

        if (translation_vector != Vector3.zero) {
            moved = true;
        }
    }
}
