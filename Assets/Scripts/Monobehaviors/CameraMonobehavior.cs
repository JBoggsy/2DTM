using Constants;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraMonobehavior : GameObjectMonobehavior
{
    public int CameraSpeed;

    override protected void Start() {
        base.Start();
        GM.RegisterCameraMonobehavior(this);
        GM.CurrentViewRect = _GetWorldViewRect();
    }

    void Update() {
        GM.CurrentViewRect = _GetWorldViewRect();
    }

    /// <summary>
    /// Move the camera in the given direction. The actual view from the camera
    /// will only be updated once the <c>Update()</c> method of the 
    /// <see cref="TilemapMonobehavior"/> is called.
    /// </summary>
    /// <param name="dir">The direction to move the camera</param>
    public void MoveInDirection(TM_Direction dir) {
        float moveDistance = CameraSpeed * Time.deltaTime;
        switch (dir) {
            case TM_Direction.UP:
                gameObject.transform.Translate(Vector3.up * moveDistance);
                break;
            case TM_Direction.LEFT:
                gameObject.transform.Translate(Vector3.left * moveDistance);
                break;
            case TM_Direction.DOWN:
                gameObject.transform.Translate(Vector3.down * moveDistance);
                break;
            case TM_Direction.RIGHT:
                gameObject.transform.Translate(Vector3.right * moveDistance);
                break;
        }
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
