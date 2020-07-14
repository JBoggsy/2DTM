using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeedLabelController : MonoBehaviour
{
    public void SetLabelToSeed(int seed)
    {
        gameObject.GetComponent<Text>().text = seed.ToString();
    }
}
