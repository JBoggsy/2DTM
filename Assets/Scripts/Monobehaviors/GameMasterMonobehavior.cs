using System.Collections.Generic;
using UnityEngine;

using GameCore;

namespace MonoBehaviours {
    class GameMasterMonobehavior : GameObjectMonobehavior {
        void Awake() {
            GameMaster.Instance.Initialize();
            GameObjectMonobehavior.SetGameMaster(GameMaster.Instance);
        }

        void Update() {
            // Dispatch all key presses to GameMaster
            List<KeyCode> keysPressed = new List<KeyCode>();
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode))) {
                if (Input.GetKey(key)) {
                    keysPressed.Add(key);
                }
            }
            GM.HandleKeyPresses(keysPressed);

            float mouseWheelInput = Input.mouseScrollDelta.y;
            if (mouseWheelInput > 0) {
                GM.HandleMouseWheelScroll(1);
            } else if (mouseWheelInput < 0) {
                GM.HandleMouseWheelScroll(-1);
            }
        }
    }
}