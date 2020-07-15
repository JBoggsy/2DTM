using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraMonobehavior : GameObjectMonobehavior
{
    void Start() {
        base.Start();
        GM.CurrentViewRect = _GetWorldViewRect();
    }

    void Update() {
        GM.CurrentViewRect = _GetWorldViewRect();
    }

    private Rect _GetWorldViewRect() {
        Rect returnRect;
        Vector3 bottomLeft = gameObject.GetComponent<Camera>().ViewportToWorldPoint(Vector3.zero);
        Vector3 topRight = gameObject.GetComponent<Camera>().ViewportToWorldPoint(Vector3.one);
        float xMin = bottomLeft.x;
        float yMin = bottomLeft.y;
        float xMax = topRight.x;
        float yMax = topRight.y;
        float width = xMax - xMin;
        float height = yMax - yMin;
        returnRect = new Rect(xMin, yMin, width, height);
        return returnRect;
    }
}
