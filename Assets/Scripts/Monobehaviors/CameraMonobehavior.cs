using UnityEngine;

using Constants;
using System;

namespace MonoBehaviours {
    /// <summary>
    /// Monobehaviour for controlling the cmaera.
    /// 
    /// This monobehaviour handles moving or zooming the camera in the Unity
    /// simulation. It is managed by the <see cref="GameCore.GameMaster"/>
    /// singleton instance.
    /// </summary>
    public class CameraMonobehavior : GameObjectMonobehavior {
        private const float ZOOM_INCREMENT = 1.2f;
        private const float CAMERA_SPEED = 3;
        private const float MAX_ZOOM = 15.0f;
        private const float MIN_ZOOM = 0.5f;

        /// <summary>
        /// When Unity starts, register this monobehaviour with the
        /// <see cref="GameCore.GameMaster"/> set the viewing rectangle.
        /// </summary>
        override protected void Start() {
            base.Start();
            GM.RegisterCameraMonobehavior(this);
            GM.CurrentViewRect = _GetWorldViewRect();
        }

        /// <summary>
        /// Every update, make sure the <see cref="GameCore.GameMaster"/>
        /// instance has the correct viewing rectangle.
        /// </summary>
        void Update() {
            GM.CurrentViewRect = _GetWorldViewRect();
        }

        /// <summary>
        /// Zooms the camera in or out by changing its
        /// <see cref="Camera.orthographicSize"/> property. This seems to be
        /// equivalent in 2D Unity to moving the camera backwards.
        /// </summary>
        /// <param name="direction">
        /// An integer either -1 or 1, where:
        /// <list type="bullet">
        ///     <item>
        ///         <term>-1</term>
        ///         <description>Zooms out</description>
        ///     </item>
        ///     <item>
        ///         <term>1</term>
        ///         <description>Zooms in</description>
        ///     </item>
        /// </list>
        /// </param>
        public void ZoomCamera(int direction) {
            if (direction > 0) {
                gameObject.GetComponent<Camera>().orthographicSize /= ZOOM_INCREMENT;
            } else if (direction < 0) {
                gameObject.GetComponent<Camera>().orthographicSize *= ZOOM_INCREMENT;
            }
            gameObject.GetComponent<Camera>().orthographicSize = Mathf.Clamp(gameObject.GetComponent<Camera>().orthographicSize, MIN_ZOOM, MAX_ZOOM);
        }

        /// <summary>
        /// Move the camera in the given direction.
        /// </summary>
        /// <param name="dir">
        /// The direction to move the camera. One of:
        /// <list type="bullet">
        ///     <item>
        ///     <description><see cref="TM_Direction.UP"/></description>
        ///     </item>
        ///     <item>
        ///     <description><see cref="TM_Direction.DOWN"/></description>
        ///     </item>
        ///     <item>
        ///     <description><see cref="TM_Direction.LEFT"/></description>
        ///     </item>
        ///     <item>
        ///     <description><see cref="TM_Direction.RIGHT"/></description>
        ///     </item>
        /// </list>
        /// </param>
        public void MoveInDirection(TM_Direction dir) {
            float moveDistance = CAMERA_SPEED * gameObject.GetComponent<Camera>().orthographicSize * Time.deltaTime;
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
                case TM_Direction.STAY:
                    gameObject.transform.Translate(Vector3.zero);
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
}