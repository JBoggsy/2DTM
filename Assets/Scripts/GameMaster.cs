using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class GameMaster : Singleton<GameMaster> {
    // CORE DATA REFERENCES
    public GridData GridData; 
    public Rect CurrentViewRect;

    // SETTINGS
    public int NumberOfTuringMachines { get; private set; } = 25;
    public int NumberStatesPerMachine { get; private set; } = 3;
    public int RandomSeed { get; private set; } = 0;
    public bool UseCustomSeed { get; private set; } = false;
    public bool RandomBoardGeneration { get; private set; } = false;
    public bool RandomStartingTransitions { get; private set; } = true;
    public float SimulationSpeedSetting { get; private set; } = 1.0f;

    // SIMULATION STATE
    private float SimulationSpeed = 0.25f;
    private bool RunSimulation;
    private IEnumerator TuringMachineUpdateClock() {
        while (true) {
            if (RunSimulation) {
                SimulateOneStep();
            }
            yield return new WaitForSeconds(SimulationSpeed);
        }
    }

    /// <summary>
    /// Initializes the <see cref="GameMaster"/> singleton. This is called by
    /// the <see cref="GameMasterMonobehavior"/> in its <c>Awake()</c> method.
    /// </summary>
    public void Initialize() {
        // Set initial seed based on random vs. custom
        if (UseCustomSeed) {
            Random.InitState(RandomSeed);
        } else {
            RandomSeed = Random.Range(int.MinValue, int.MaxValue);
            print("SEED: " + RandomSeed.ToString());
            Random.InitState(RandomSeed);
        }

        // Initialize empty grid
        GridData = new GridData();

        // Set up simulation co-routine
        RunSimulation = false;
        IEnumerator simulationUpdater = TuringMachineUpdateClock();
        StartCoroutine(simulationUpdater);
    }

    /// <summary>
    /// Called at every frame by the GameMasterMonobehavior object
    /// </summary>
    public void Update() {

    }

    /// <summary>
    /// Initialize a simulation/game of 2DTMs by clearing the whole board and creating
    /// new Turing machines. 
    /// </summary>
    public void ResetGame() {
        Random.InitState(RandomSeed);
    }

    /**
     * <summary>
     * Simulate a single step of the "game" by iterating through each Turing machine.
     * </summary>
     */
    public void SimulateOneStep() {
    }

    public void HandleButton(string buttonName) {
        switch (buttonName) {
            case "playPause":
                HandlePlayPauseButton();
                break;
            case "reset":
                HandleResetButton();
                break;
            case "changeSeed":
                HandleChangeSeedButton();
                break;
            case "step":
                HandleStepButton();
                break;
            case "simSpeedUp":
                HandleSimSpeedIncreaseButton();
                break;
            case "simSpeedDown":
                HandleSimSpeedDecreaseButton();
                break;
        }
    }

    public void HandlePlayPauseButton() {
        _ToggleSimulation();
    }

    public void HandleResetButton() {
        ResetGame();
        _PauseSimulation();
    }

    public void HandleChangeSeedButton() {
        RandomSeed = Random.Range(int.MinValue, int.MaxValue);
    }

    public void HandleStepButton() {
        _PauseSimulation();
        SimulateOneStep();
    }

    public void HandleSimSpeedIncreaseButton() {
        SimulationSpeedSetting += 0.25f;
        _UpdateSimulationSpeed();
    }

    public void HandleSimSpeedDecreaseButton() {
        SimulationSpeedSetting -= 0.25f;
        _UpdateSimulationSpeed();
    }

    private void _ToggleSimulation() {
        if (RunSimulation) { _PauseSimulation(); } else { _ResumeSimulation(); }
    }

    private void _PauseSimulation() {
        RunSimulation = false;
    }

    private void _ResumeSimulation() {
        RunSimulation = true;
    }

    private void _UpdateSimulationSpeed() {
        SimulationSpeed = 1.0f / SimulationSpeedSetting;
    }
}
