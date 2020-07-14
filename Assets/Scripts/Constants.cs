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
    public static class RandomSymbol {
        /// <summary>
        /// Produces a random symbol from the alphabet.
        /// </summary>
        /// <returns>
        /// An <c>TM_Symbol</c> <c>s</c> such that <c>0 <= s < TM_Symbol.NUMBER</c>
        /// </returns>
        public static TM_Symbol Get() {
            return (TM_Symbol) UnityEngine.Random.Range(0, (int)TM_Symbol.NUMBER);
        }
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
        /// The number of directions a TM can go
        /// </summary>
        NUMBER
    }
    public static class RandomDirection{

        /// <summary>
        /// Produces a random direction from the possible directions.
        /// </summary>
        /// <returns>
        /// An <c>TM_Direction</c> <c>d</c> such that <c>0 <= d < TM_Direction.NUMBER</c>
        /// </returns>
        public static TM_Direction Get() {
            return (TM_Direction)UnityEngine.Random.Range(0, (int)TM_Direction.NUMBER);
        }
    }

}

