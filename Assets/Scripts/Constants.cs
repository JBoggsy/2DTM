using UnityEngine;

namespace Constants {
    public enum TM_Symbol : int {
        /// <summary>
        /// Symbol representing the off state of tiles
        /// </summary>
        OFF,
        /// <summary>
        /// Symbol representing the on state of tiles
        /// </summary>
        ON,
        /// <summary>
        /// The number of different symbols in the alphabet
        /// </summary>
        NUMBER
    }

    public enum TM_Direction : int {
        /// <summary>
        /// The upward (positive y) direction
        /// </summary>
        UP,
        /// <summary>
        /// The downward (negative y) direction
        /// </summary>
        DOWN,
        /// <summary>
        /// The leftward (negative x) direction
        /// </summary>
        LEFT,
        /// <summary>
        /// The rightward  (positive x) direction
        /// </summary>
        RIGHT,
        /// <summary>
        /// Don't move in any direction.
        /// </summary>
        STAY,
        /// <summary>
        /// The number of directions a TM can go
        /// </summary>
        NUMBER
    }
}

