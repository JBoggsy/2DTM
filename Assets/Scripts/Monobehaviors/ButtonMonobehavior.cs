﻿using UnityEngine;

class ButtonMonobehavior : GameObjectMonobehavior
{
    public string ButtonName;

    public void HandleClick()
    {
        GM.HandleButton(ButtonName);
    }
}