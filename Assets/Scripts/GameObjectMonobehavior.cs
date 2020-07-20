using UnityEngine;
using System.Collections;

/// <summary>
/// A parent class for all other Monobehaviors in 2DTM which provides them with
/// a private <c>GM</c> field giving easy access to the <c><see cref="GameMaster"/></c>
/// instance.
/// </summary>
public class GameObjectMonobehavior : MonoBehaviour
{
    /// <summary>The singleton <c><see cref="GameMaster"/></c> instance.</summary>
    protected static GameMaster GM;

    virtual protected void Start() {
        print("Starting a monobehavior...");
    }

    public static void SetGameMaster(GameMaster newGM) {
        GM = newGM;
    }
}
