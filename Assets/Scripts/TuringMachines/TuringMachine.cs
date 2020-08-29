using System;
using System.Collections.Generic;
using UnityEngine;

using GameCore;
using MonoBehaviours;
using Constants;

namespace TuringMachines {
    /// <summary>
    /// The <see cref="TuringMachine"/> class represents the internal mathmatical Turing
    /// machine model of a Turing machine in the game. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// A <see cref="TuringMachine"/> instance contains three pieces of internal state data
    /// as well as accessory data and methods which allow the Turing machine to be
    /// run. In particular, it has a number of states represented as integer IDs,
    /// maintains the ID of the <see cref="CurrentState"/>, and a 
    /// <see cref="TransitionType"/> serving as a function defined as 
    /// <c>d(symbol, state)->(state, direction, symbol)</c>.
    /// </para>
    /// <para>
    /// A formal, mathematical Turing machine is defined as a 7-tuple which includes
    /// <list type="bullet">
    ///     <item>
    ///         <description>A set of States</description>
    ///     </item>
    ///     <item>
    ///         <description>An alphabet of symbols</description>
    ///     </item>
    ///     <item>
    ///         <description>An initial state</description>
    ///     </item>
    ///     <item>
    ///         <description>A set of accepting States</description>
    ///     </item>
    ///     <item>
    ///         <description>A transition function</description>
    ///     </item>
    /// </list>
    /// as well as a couple others. To learn more about mathematical Turing machines,
    /// see https://en.wikipedia.org/wiki/Turing_machine.
    /// </para>
    /// </remarks>
    public class TuringMachine {
        /// <value>
        /// A unique ID assigned to the Turing machine. The corresponding Turing
        /// machine head sprite in the Unity view has the same ID.
        /// </value>
        public int ID { get; private set; }
        /// <value>
        /// The number of states in the Turing machine's FSM.
        /// </value>
        public int NumberOfStates { get; private set; }
        /// <value>
        /// The FSM state the Turing machine is currently in.
        /// </value>
        public int CurrentState { get; private set; }

        /// <value>
        /// The <see cref="Transition"/> instance which will be applied at the
        /// end of the next simulation update.
        /// </value>
        protected Transition nextTransition;
        /// <value>
        /// The <see cref="GameObject"/> which corresponds to the Turing machine's
        /// sprite in the board visualization.
        /// </value>
        protected GameObject headObject;
        /// <value>
        /// The monobehaviour of the Turing machine's sprite in the simulation
        /// visualization.
        /// </value>
        protected TuringMachineHeadMonobehavior headMonobehavior;
        /// <value>
        /// The Turing machine's transition table. Maps (state, input) pairs
        /// to corresponding <see cref="Transition"/> instances. Each unique
        /// (state, input) pair maps to a unique <see cref="Transition"/>.
        /// </value>
        protected Dictionary<(int, TM_Symbol), Transition> TransitionTable;
        /// <value>
        /// The <see cref="GameMaster"/> signleton instance.
        /// </value>
        protected GameMaster GM;

        /// <summary>
        /// Create a new Turing machine instance with the given ID and number of
        /// states.
        /// 
        /// This method initializes the <see cref="TransitionTable"/>, 
        /// <see cref="headObject"/>, and <see cref="headMonobehavior"/> attributes.
        /// </summary>
        /// <param name="id">A unique integer identifying this Turing machine.</param>
        public TuringMachine(int id, int numberOfStates) {
            GM = GameMaster.Instance;
            ID = id;
            NumberOfStates = numberOfStates;
            CurrentState = 0;
            TransitionTable = new Dictionary<(int, TM_Symbol), Transition>();
            headObject = GameObject.Instantiate(GM.TuringMachineHeadPrefab, Vector3.zero, Quaternion.identity);
            headMonobehavior = headObject.GetComponent<TuringMachineHeadMonobehavior>();
            headMonobehavior.id = ID;
        }

        /**********************
         * SIMULATION METHODS *
         **********************/
        /// <summary>
        /// Performs the first step of simulating this Turing machine's next step
        /// in simulation.
        /// 
        /// Since there may be multiple Turing machines which share the same 
        /// grid which they all write to, there is a significant difference
        /// between simulating the machines in parallel and serially. If the
        /// machines are simulated in serial, the order in which they are 
        /// simulated matters a lot. In order to avoid this, we want to simulate
        /// them in parallel, and therefor every Turing machine must first 
        /// determine what it will do at the next simulation step before any
        /// Turing can apply those changes. 
        /// 
        /// This method is the first step of that process, determining what state
        /// changes the Turing machine will make based on the current state of
        /// the grid. It sets the <see cref="nextTransition"/> attribute to the
        /// <see cref="Transition"/> instance returned by querying the
        /// <see cref="GetTransition(int, TM_Symbol)"/> method with the current
        /// input and state.
        /// </summary>
        /// <param name="inputSymbol">The current state and current input (the
        /// symbol in the cell the Turing machine is occupying).
        /// </param>
        public void PrepareSimulationStep(TM_Symbol inputSymbol) {
            nextTransition = GetTransition(CurrentState, inputSymbol);
        }

        /// <summary>
        /// Performs the first step of simulating this Turing machine's next step
        /// in simulation.
        /// 
        /// Since there may be multiple Turing machines which share the same 
        /// grid which they all write to, there is a significant difference
        /// between simulating the machines in parallel and serially. If the
        /// machines are simulated in serial, the order in which they are 
        /// simulated matters a lot. In order to avoid this, we want to simulate
        /// them in parallel, and therefor every Turing machine must first 
        /// determine what it will do at the next simulation step before any
        /// Turing can apply those changes
        /// 
        /// This method is the second step of that process, applying the predicted
        /// changes to the board. It references the current value of the 
        /// <see cref="nextTransition"/> to write a symbol to the grid, move
        /// the Turing Machine's head, and change the current state.
        /// </summary>
        /// <remarks>
        /// This method <b>DOES NOT</b> handle conflict resolution in terms of
        /// movement collision or simultaneous writing to the same cell.
        /// </remarks>
        public void ApplySimulationStep() {
            if (nextTransition == null) { return; }
            GM.WriteSymbolToGrid(position, nextTransition.WriteSymbol);
            headMonobehavior.MoveHeadInDirection(nextTransition.Direction);
            CurrentState = nextTransition.NextState;
            nextTransition = null;
        }

        /// <summary>
        /// Returns the coordinates of the read/write head on the grid.
        /// </summary>
        public Vector3Int position {
            get {
                Vector3 rawHeadPosition = headMonobehavior.position;
                Vector3Int headPosition = new Vector3Int((int)rawHeadPosition.x, (int)rawHeadPosition.y, 0);
                return headPosition;
            }
        }

        /// <summary>
        /// Reset the Turing machine by returning it to its initial FSM state
        /// and moving it to the center of the board.
        /// </summary>
        public void Reset() {
            CurrentState = 0;
            headMonobehavior.MoveHeadToCenter();
        }

        /******************************
         * TRANSITION TABLE FUNCTIONS *
         ******************************/
        /// <summary>
        /// Fills the <see cref="TransitionTable"/> with randomly-generated valid
        /// transitions. The algorithm simply iterates through each possible
        /// state-input pair and chooses a random next state, symbol to write,
        /// and direction to move.
        /// </summary>
        /// <remarks>
        /// This method guarantees a non-halting Turing machine.
        /// </remarks>
        public void GenerateRandomTransitions() {
            for (int state = 0; state < NumberOfStates; state++) {
                for (int input = 0; input < (int)TM_Symbol.NUMBER; input++) {
                    int nextState = UnityEngine.Random.Range(0, NumberOfStates);
                    int writeSymbol = UnityEngine.Random.Range(0, (int)TM_Symbol.NUMBER);
                    int direction = UnityEngine.Random.Range(0, (int)TM_Direction.NUMBER);
                    AddTransition(input, state, nextState, direction, writeSymbol);
                }
            }
        }

        /**
         * <summary>Add a new transition to the transition table. 
         * 
         * Insert the given <see cref="Transition"/> into the transition
         * table at the specified pair of input symbol and current state. This
         * partially defines the transition function <c>d</c> for <c>d(symbol, 
         * state)</c></summary>.
         * <param name="input">The input symbol the transition requires</param>
         * <param name="state">The state the TM should be in for the transition</param>
         * <param name="transition">The <c>Transition</c> object to be used</param>
         */
        public void AddTransition(int input, int state, Transition transition) {
            TM_Symbol inputSymbol = (TM_Symbol)input;
            TransitionTable[(state, inputSymbol)] = transition;
        }

        /**
         * <summary>Add a new <see cref="Transition"/> to the transition table. 
         * <br></br>
         * Insert the given <see cref="Transition"/> into the <see cref="TransitionTable"/>
         * at the specified pair of input symbol and current state. This
         * partially defines the transition function <c>d</c> for <c>d(symbol, 
         * state)</c>
         * </summary>.
         * <param name="input">The input symbol the transition requires</param>
         * <param name="state">The state the TM should be in for the transition</param>
         * <param name="transition">The <c>Transition</c> object to be used</param>
         */
        public void AddTransition(TM_Symbol input, int state, Transition transition) {
            TransitionTable[(state, input)] = transition;
        }

        /**
         * <summary>Add a new transition to the transition table. 
         * <br></br>
         * First creates a <see cref="Transition"/> based on the last three 
         * parameters, then inserts the <see cref="Transition"/> into the 
         * <see cref="TransitionTable"/> at the specified pair of input symbol
         * and current state. This partially defines the transition function
         * <c>d</c> for <c>d(symbol, state)</c>
         * </summary>.
         * <param name="symbol">The input symbol the transition requires</param>
         * <param name="state">The state the TM should be in for the transition</param>
         * <param name="nextState">The ID of the state to transition to</param>
         * <param name="direction">The direction code for the head to move</param>
         * <param name="writeSymbol">The symbol to write to the grid</param>
         */
        public void AddTransition(int symbol, int state,
                                  int nextState, int direction, int writeSymbol) {
            TM_Direction dir = (TM_Direction)direction;
            TM_Symbol write = (TM_Symbol)writeSymbol;
            Transition transition = new Transition(nextState, dir, write);
            AddTransition(symbol, state, transition);
        }

        /// <summary>
        /// Get the <see cref="Transition"/> produced by the transition function.
        /// <para>
        /// Given the current state and the symbol read in by the Turing machine 
        /// head, produce a transition triple (nextState, moveDirection, 
        /// writeSymbol). If the (currentState, inputSymbol) pair doesn't have a
        /// defined transition, it halts the machine.
        /// </para>
        /// </summary>
        /// <param name="currentState">The current state the machine is in</param>
        /// <param name="inputSymbol">The symbol the read/write head read in</param>
        /// <returns>The result of calling the transition function <c>d</c> on
        /// <c>currentState</c> and <c>inputSymbol</c>, represented as a 
        /// <see cref="Transition"/> object.</returns>
        public Transition GetTransition(int currentState, TM_Symbol inputSymbol) {
            Transition returnTransition = new Transition(-1, TM_Direction.STAY, inputSymbol);
            (int, TM_Symbol) transitionKey = (currentState, inputSymbol);
            if (TransitionTable.ContainsKey(transitionKey)) {
                returnTransition = TransitionTable[transitionKey];
            }
            return returnTransition;
        }

        /// <summary>
        /// Remove the transition given be <c>d(state, input)</c>. The previous
        /// value for that input is replaced with null, which indicates a halting
        /// state.
        /// </summary>
        /// <param name="currentState">
        /// An integer between 0 and <see cref="NumberOfStates"/>. Part of the
        /// (state, input) pair.
        /// </param>
        /// <param name="inputSymbol">
        /// A valid <see cref="TM_Symbol"/>. Part of the (state, inpit pair).
        /// </param>
        public void RemoveTransition(int currentState, TM_Symbol inputSymbol) {
            (int, TM_Symbol) transitionKey = (currentState, inputSymbol);
            TransitionTable.Remove(transitionKey);
            TransitionTable[transitionKey] = null;
        }

        /*******************
         * UTILITY METHODS *
         *******************/
        /// <summary>
        /// Get a human-readable string representation of this Turing machine.
        /// 
        /// String contains the current state of the Turing machine and its
        /// <see cref="TransitionTable"/>.
        /// </summary>
        override public string ToString() {
            String returnString = String.Format("Current State: {0}\n", CurrentState);
            returnString = String.Concat(returnString, "Transition Table:\n");
            for (int state = 0; state < NumberOfStates; state++) {
                for (int sym = 0; sym < (int)TM_Symbol.NUMBER; sym++) {
                    Transition transition = TransitionTable[(state, (TM_Symbol)sym)];
                    returnString = String.Concat(returnString, String.Format("\t({0},{1})=>{2}\n", sym, state, transition.ToString()));
                }
            }
            return returnString;
        }
    }
}