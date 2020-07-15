using System.Collections.Generic;
using UnityEngine;

class GameMasterMonobehavior : GameObjectMonobehavior
{
    void Awake() {
        GameMaster.Instance.Initialize();
    }

    void Update()
    {
        List<KeyCode> keysPressed = new List<KeyCode>();
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode))) {
            if (Input.GetKey(key)) {
                keysPressed.Add(key);
            }
        }

        GM.HandleKeyPresses(keysPressed);
    }
}
