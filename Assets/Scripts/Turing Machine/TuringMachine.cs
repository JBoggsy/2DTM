using System;
using System.Linq;
using Constants;
using UnityEngine;

/// <summary>
/// The <c>TuringMachine</c> class represents the internal mathmatical Turing
/// machine model of a Turing machine in the game. 
/// </summary>
/// <remarks>
/// <para>
/// A <c>TuringMachine</c> instance contains three pieces of internal state data
/// as well as accessory data and methods which allow the Turing machine to be
/// run. In particular, an instance contains an integer array <c>States</c>, an
/// integer <c>CurrentState</c>, and a look-up table <c>TransitionTable</c>
/// serving as a function defined as <c>f(symbol, state)->(state, direction, symbol)</c>.
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
class TuringMachine { 
    // INTERNAL STATE
    public int NumStates { get; private set; }
    public int[] States { get; private set; }
    public int CurrentState { get; private set; }

    private int NextState;
    private Transition[,] TransitionTable;

    /** 
     * <summary>
     * Instantiate a Turing machine with 'n' possible States. The
     * transition function defaults to completely undefined, meaning every
     * state will immediately halt on any input.
     * </summary>
     */
    public TuringMachine(int n) {
        n = Math.Max(n, 1);

        NumStates = n;
        States = new int[NumStates];
        for (int stateID = 0; stateID < NumStates; stateID++) {
            States[stateID] = stateID;
        }
        CurrentState = 0;
        NextState = 0;
        TransitionTable = new Transition[(int)TM_Symbol.NUMBER, NumStates];
    }

    /**
     * <summary>
     * Instantiate the transition table with random valid transitions.
     * <br></br>
     * There is no guarantee this will create anything interesting and it is
     * guaranteed to never halt.
     * </summary>
     */
    public void InitWithRandomTransitions() {
        for (int input = 0; input < (int)TM_Symbol.NUMBER; input++) {
            for (int state=0; state < NumStates; state++) {
                //int next = UnityEngine.Random.Range(0, NumStates);
                //TM_Direction dir = RandomDirection.Get();
                //TM_Symbol write = RandomSymbol.Get();
                //Transition transition = new Transition(next, dir, write);
                //TransitionTable[input, state] = transition;
            }
        }
    }

    /**
     * <summary>Add a new transition to the transition table. 
     * <br></br>
     * Insert the given <c>transition</c> 'transition' into the transition
     * table at the specified pair of input 'symbol' and current 'state'. This
     * partially defines the transition function <c>d</c> for <c>d(symbol, 
     * state)</c></summary>.
     * <param name="symbol">The input symbol the transition requires</param>
     * <param name="state">The state the TM should be in for the transition</param>
     * <param name="transition">The <c>Transition</c> object to be used</param>
     */
    public void AddTransition(int symbol, int state, Transition transition) {
        TransitionTable[symbol, state] = transition;
    }

    /**
     * <summary>Add a new transition to the transition table. 
     * <br></br>
     * First creates a <c>Transition</c> based on the last three parameters, then
     * inserts the <c>Transition</c> in the table at the specified pair of input 
     * 'symbol' and current 'state'. This partially defines the transition function
     * <c>d</c> for <c>d(symbol, state)</c></summary>.
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

    /**
     * <summary>Handle a given input given the current state.</summary>
     * <param name="input">The current symbol the TM head is over</param>
     * <returns>A pair of <c>int</c>s <c>(direction, write)</c> indicating
     * the movement of the head and the symbol to write.</returns>
     */
    public (TM_Direction, TM_Symbol) HandleInput(TM_Symbol input) {
        Transition transition = TransitionTable[(int)input, CurrentState];
        NextState = transition.NextState;
        return (transition.Direction, transition.WriteSymbol);
    }

    override public string ToString() {
        String returnString = String.Format("Current State: {0}\n", CurrentState);
        returnString = String.Concat(returnString, "Transition Table:\n");
        for (int sym=0; sym<(int)TM_Symbol.NUMBER; sym++) {
            for (int state=0; state<NumStates; state++) {
                Transition transition = TransitionTable[sym, state];
                returnString = String.Concat(returnString, String.Format("\t({0},{1})=>{2}\n", sym, state, transition.ToString()));
            }
        }
        return returnString;
    }

    /**
     * <summary>Update the current state with the next state.</summary>
     */
    public void UpdateState() {
        CurrentState = NextState;
        NextState = 0;
    }
}
