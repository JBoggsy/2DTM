using UnityEngine;
using GameCore;

/// <summary>
/// A parent class for all other Monobehaviors in 2DTM which provides them with
/// a private <c>GM</c> field giving easy access to the <c><see cref="GameMaster"/></c>
/// instance.
/// </summary>
namespace MonoBehaviours {
    public class GameObjectMonobehavior : MonoBehaviour {
        /// <value>The <c><see cref="GameMaster"/></c> singleton instance.</value>
        protected static GameMaster GM;

        virtual protected void Start() {
            print("Starting a monobehavior...");
        }

        /// <summary>
        /// Sets the value of <see cref="GM"/> to the <see cref="GameMaster"/>
        /// singleton instance.
        /// </summary>
        /// <param name="newGM">The new <see cref="GameMaster"/> instance.</param>
        public static void SetGameMaster(GameMaster newGM) {
            GM = newGM;
        }
    }
}