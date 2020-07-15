using UnityEngine;

class GameMasterMonobehavior : GameObjectMonobehavior
{
    void Start()
    {
        GM.Start();
    }
    void Update()
    {
        GM.Update();
    }
}
