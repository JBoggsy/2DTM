using System;
using System.Collections.Generic;
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
    public int ID { get; private set; }
    public int NumberOfStates { get; private set; }
    public int CurrentState { get; private set; }

    private int NextState;
    private GameObject headObject;
    private TuringMachineHeadMonobehavior headMonobehavior;
    private Dictionary<(int, TM_Symbol), Transition> TransitionTable;

    /// <summary>
    /// TODO: DOC
    /// </summary>
    /// <param name="id"></param>
    public TuringMachine(int id, int numberOfStates) {
        ID = id;
        NumberOfStates = numberOfStates;
        CurrentState = 0;
        NextState = 0;
        TransitionTable = new Dictionary<(int, TM_Symbol), Transition>();
        headObject = GameObject.Instantiate((GameObject)Resources.Load("Turing Machine Head"), Vector3.zero, Quaternion.identity);
        headMonobehavior = headObject.GetComponent<TuringMachineHeadMonobehavior>();
    }

    /******************************
     * TRANSITION TABLE FUNCTIONS *
     ******************************/
    /// <summary>
    /// Fills the <c>TransitionTable</c> with randomly-generated valid transitions.
    /// <para>
    /// This method guarantees a non-halting Turing machine.
    /// </para>
    /// </summary>
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
     * <br></br>
     * Insert the given <c>transition</c> 'transition' into the transition
     * table at the specified pair of input 'symbol' and current 'state'. This
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

    public void AddTransition(TM_Symbol input, int state, Transition transition) {
        TransitionTable[(state, input)] = transition;
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
