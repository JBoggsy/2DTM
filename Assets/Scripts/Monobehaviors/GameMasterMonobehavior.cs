using UnityEngine;

class GameMasterMonobehavior : GameObjectMonobehavior
{
    override protected void Start()
    {
        base.Start();
        GM.Start();
    }
    void Update()
    {
        GM.Update();
    }
}
