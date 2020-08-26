using Constants;

namespace TuringMachines {
    /// <summary>
    /// The <c>Transition</c> class holds a triple <c>(int NEXT, TM_Direction DIR,
    /// TM_Symbol WRITE)</c> which describes the outcome of a particular transition.
    /// </summary>
    class Transition {
        /// <value><c>NEXT</c> indicates the state ID to transition to</value>
        public int NextState { get; set; }

        /// <value><c>DIR</c> indicates the direction for the r/w head to move</value>
        public TM_Direction Direction { get; set; }

        /// <value><c>WRITE</c> indicates the symbol to write to the grid</value>
        public TM_Symbol WriteSymbol { get; set; }

        /// <summary>
        /// Create a new <c>Transition</c> object for use in the transition table.
        /// </summary>
        public Transition() {
            NextState = -1;
            Direction = TM_Direction.STAY;
            WriteSymbol = 0;
        }

        /**
         * <summary>
         * Create a Turing machine transition to be placed in the transition table.
         * </summary>
         * <param name="nextState">The ID of the state to transition to. <see cref="NextState"/></param>
         * <param name="direction">The direction code for the head to move. <see cref="Direction"/></param>
         * <param name="writeSymbol">The symbol to write to the grid. <see cref="WriteSymbol"/></param>
         */
        public Transition(int nextState, TM_Direction direction, TM_Symbol writeSymbol) {
            NextState = nextState;
            Direction = direction;
            WriteSymbol = writeSymbol;
        }

        public string ToString() {
            return "(" + NextState.ToString() + ", " + Direction.ToString() + ", " + WriteSymbol.ToString() + ")";
        }
    }
}