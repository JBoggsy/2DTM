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
    private TuringMachine[] TuringMachines;

    // MONOBEHAVIOR REFERENCES
    private CameraMonobehavior CameraMonobehavior;

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
    public bool Running { get{return RunSimulation;} }


    /// <summary>
    /// Initializes the <see cref="GameMaster"/> singleton. This is called by
    /// the <see cref="GameMasterMonobehavior"/> in its <c>Awake()</c> method.
    /// </summary>
    public void Initialize() {
        if (UseCustomSeed) {
            Random.InitState(RandomSeed);
        } else {
            RandomSeed = Random.Range(int.MinValue, int.MaxValue);
            print("SEED: " + RandomSeed.ToString());
            Random.InitState(RandomSeed);
        }

        GridData = new GridData();
        TuringMachines = new TuringMachine[NumberOfTuringMachines];
        for (int machineID=0; machineID<NumberOfTuringMachines; machineID++) {
            TuringMachine newMachine = new TuringMachine(machineID, NumberStatesPerMachine);
            if (RandomStartingTransitions) { newMachine.GenerateRandomTransitions(); }
            TuringMachines[machineID] = newMachine;
        }

        RunSimulation = false;
        IEnumerator simulationUpdater = TuringMachineUpdateClock();
        StartCoroutine(simulationUpdater);
    }

    /**********************
     * SIMULATION METHODS *
     **********************/
    /**
     * <summary>
     * Simulate a single step of the "game" by iterating through each Turing machine.
     * </summary>
     */
    public void SimulateOneStep() {
        foreach (TuringMachine tm in TuringMachines) {
            tm.PrepareSimulationStep(GridData.GetCellSymbol(tm.position));
        }
        foreach (TuringMachine tm in TuringMachines) {
            tm.ApplySimulationStep();
        }
    }

    /// <summary>
    /// Initialize a simulation/game of 2DTMs by clearing the whole board and creating
    /// new Turing machines. 
    /// </summary>
    public void ResetGame() {
        Random.InitState(RandomSeed);
    }

    /// <summary>
    /// Writes the given symbol to the given location on the grid.
    /// </summary>
    public void WriteSymbolToGrid(Vector3Int loc, TM_Symbol symbol) {
        GridData.SetCellSymbol(loc, symbol);
    }


    /***********************************
     * GAMEOBJECT INTERACTION HANDLING *
     ***********************************/
    public void HandleTuringMachineHeadClick(int machineID) {
        print("Turing machine clicked...");
    }

    /*****************************
     * KEYPRESS HANDLING METHODS *
     *****************************/
    public void HandleKeyPresses(List<KeyCode> keysPressed) {
        foreach (KeyCode key in keysPressed) {
            switch (key) {
                case KeyCode.W: _HandleKeyW(); break;
                case KeyCode.A: _HandleKeyA(); break;
                case KeyCode.S: _HandleKeyS(); break;
                case KeyCode.D: _HandleKeyD(); break;
                case KeyCode.Space: _HandleKeySpace(); break;
            }
        }
    }

    private void _HandleKeyW() { CameraMonobehavior.MoveInDirection(TM_Direction.UP); }
    private void _HandleKeyA() { CameraMonobehavior.MoveInDirection(TM_Direction.LEFT); }
    private void _HandleKeyS() { CameraMonobehavior.MoveInDirection(TM_Direction.DOWN); }
    private void _HandleKeyD() { CameraMonobehavior.MoveInDirection(TM_Direction.RIGHT); }
    private void _HandleKeySpace() { _ToggleSimulation(); }

    /***************************
     * BUTTON HANDLING METHODS *
     ***************************/
    /// <summary>
    /// Handle when the mouse wheel scrolls in a direction.
    /// </summary>
    /// <param name="direction">An integer either -1 or 1.</param>
    public void HandleMouseWheelScroll(int direction) {
        CameraMonobehavior.ZoomCamera(direction);
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

    public void SetSeed(int value) {
        RandomSeed = Mathf.Clamp(value, int.MinValue, int.MaxValue);
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

    /******************************
     * SIMULATION CONTROL METHODS *
     ******************************/
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

    /*******************
     * UTILITY METHODS *
     *******************/
    /// <summary>
    /// Register the given <see cref="CameraMonobehavior"/> as the camera the
    /// <see cref="GameMaster"/> should pay attention to and interact with.
    /// </summary>
    public void RegisterCameraMonobehavior(CameraMonobehavior newCamera) {
        if (CameraMonobehavior == null) {
            CameraMonobehavior = newCamera;
        }
    }
}
