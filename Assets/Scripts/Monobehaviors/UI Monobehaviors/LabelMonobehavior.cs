using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabelMonobehavior : GameObjectMonobehavior
{
    public void SetLabel(string newLabel)
    {
        gameObject.GetComponent<Text>().text = newLabel;
    }
    public void SetLabel(int newLabel)
    {
        gameObject.GetComponent<Text>().text = newLabel.ToString();
    }
    public void SetLabel(float newLabel)
    {
        gameObject.GetComponent<Text>().text = newLabel.ToString();
    }
    public void SetLabel(bool newLabel)
    {
        gameObject.GetComponent<Text>().text = newLabel.ToString();
    }
    public string GetLabel()
    {
        return gameObject.GetComponent<Text>().text;
    }
    
}
