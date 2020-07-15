using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraMonobehavior : GameObjectMonobehavior
{
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
}
