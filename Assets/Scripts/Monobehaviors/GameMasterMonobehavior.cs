using UnityEngine;

class GameMasterMonobehavior : GameObjectMonobehavior
{
    void Awake() {
        GameMaster.Instance.Initialize();
    }

    void Update()
    {
        GM.Update();
    }
}
