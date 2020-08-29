using UnityEngine;

using Constants;

namespace MonoBehaviours {

    /// <summary>
    /// Monobehaviour for the Turing machine heads being rendered in the Unity
    /// scene. This controls the actual movement of the sprites in Unity. A
    /// <see cref="TuringMachines.TuringMachine"/> instance manages a
    /// corresponding <see cref="TuringMachineHeadMonobehavior"/> instance.
    /// </summary>
    /// <seealso cref="TuringMachines.TuringMachine"/>
    public class TuringMachineHeadMonobehavior : GameObjectMonobehavior {
        /// <value>
        /// A unique identifier for this Truing machine head.
        /// </value>
        public int id;

        /// <summary>
        /// Handles mouse down events on the Turing machine head. Calls the
        /// <see cref="GameCore.GameMaster.HandleTuringMachineHeadClick(int)"/>'
        /// method.
        /// </summary>
        void OnMouseDown() {
            GM.HandleTuringMachineHeadClick(id);
        }

        /// <summary>
        /// Indicates where the Turing machine head is located.
        /// </summary>
        /// <returns>
        /// The position of the Turing machine head in world space (not in grid
        /// coordinates).
        /// </returns>
        public Vector3 position {
            get { return gameObject.transform.position; }
        }

        /// <summary>
        /// Moves the Turing machine head 1 unit in the given direction.
        /// </summary>
        /// <param name="dir">The direction in which to move.</param>
        public void MoveHeadInDirection(TM_Direction dir) {
            Vector3 move_vector = Vector3.zero;
            switch (dir) {
                case TM_Direction.UP:
                    move_vector = new Vector3(0, 1);
                    break;
                case TM_Direction.DOWN:
                    move_vector = new Vector3(0, -1);
                    break;
                case TM_Direction.LEFT:
                    move_vector = new Vector3(-1, 0);
                    break;
                case TM_Direction.RIGHT:
                    move_vector = new Vector3(1, 0);
                    break;
                case TM_Direction.STAY:
                    move_vector = Vector3.zero;
                    break;
            }

            gameObject.transform.Translate(move_vector);
        }

        /// <summary>
        /// Moves the hard from wherever it is to the center of the grid, (0, 0).
        /// </summary>
        public void MoveHeadToCenter() {
            MoveHeadTo(Vector3Int.zero);
        }

        /// <summary>
        /// Moves the Turing machine head to an arbitrary grid location.
        /// </summary>
        /// <param name="newLocation">
        /// The (row, col) location the Turing machine head should be moved to.
        /// </param>
        public void MoveHeadTo(Vector3Int newLocation) {
            gameObject.transform.position = newLocation;
        }
    }
}