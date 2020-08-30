using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MonoBehaviours;
using TuringMachines;
using Constants;
using MonoBehaviours.UI;

namespace GameCore {
    /// <summary>
    /// The <see cref="GameMaster"/> class is the central authority in charge
    /// of running the game.
    /// 
    /// The <see cref="GameMaster"/> singleton acts as a communication and
    /// control hub which components of the game communicate to and through.
    /// It maintains references to most or all core components of the backend
    /// or simulation side of the game, including the <see cref="GridData"/>,
    /// a list of <see cref="TuringMachine"/> instances, and the
    /// <see cref="CameraMonobehavior"/>. Upon instantiation, it also grabs
    /// references to a couple prefabs needed for controlling the Turing 
    /// machine sprites.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///     <item><description>
    ///     There should (and can) only be one GameMaster instance, a singleton
    ///     which every object should have access to.
    ///     </description></item>
    ///     <item><description>
    ///     The <see cref="GameMaster"/> instance must be initialized using the
    ///     <see cref="GameMaster.Initialize"/> method after contruction.
    ///     </description></item>
    /// </list>
    /// </remarks>
    public class GameMaster : Singleton<GameMaster> {
        // CORE DATA REFERENCES
        /// <value>
        /// Maintains the state of the grid which acts as a shared, two-dimensional
        /// tape for the Turing machines.
        /// </value>
        public GridData GridData;
        /// <value>
        /// A simple <see cref="Rect"/> which indicates which grid cells are
        /// currently in the viewport given the camera's location.
        /// </value>
        public Rect CurrentViewRect;
        /// <value>
        /// Maintans a list of the Turing machines in the game.
        /// </value>
        protected TuringMachine[] TuringMachines;

        // MONOBEHAVIOR REFERENCES
        /// <value>
        /// Provides the <see cref="GameMaster"/> instance a reference to the
        /// camera's monobehavior to faciliate camera control.
        /// </value>
        protected CameraMonobehavior CameraMonobehavior;

        // PREFAB REFERENCES
        /// <value>
        /// Provides a reference to the Unity prefab for creating a Turing machine
        /// editor panel.
        /// </value>
        public TuringMachineEditorMonobehaviour TuringMachineEditor;
        /// <value>
        /// Provides access to the Unity prefab for creating TUring machine sprites.
        /// </value>
        public GameObject TuringMachineHeadPrefab;

        // SETTINGS
        /// <value>
        /// Number of Turing machines in the simulation.
        /// </value>
        public int NumberOfTuringMachines { get; private set; } = 25;
        /// <value>
        /// Maximum number of states in any Turing machine.
        /// </value>
        public int NumberStatesPerMachine { get; private set; } = 3;
        /// <value>
        /// Seed used for the random generation of transition tables.
        /// </value>
        public int RandomSeed { get; private set; } = 0;
        /// <value>
        /// Whether the custom seed value be used.
        /// </value>
        public bool UseCustomSeed { get; private set; } = false;
        /// <value>
        /// Whether the cell states of the board should be randomized.
        /// </value>
        /// <remarks>
        /// <b>DEPRECATED</b>
        /// </remarks>
        public bool RandomBoardGeneration { get; private set; } = false;
        /// <value>
        /// Whether new Turing machines should have randomly generated
        /// transition tables.
        /// </value>
        public bool RandomStartingTransitions { get; private set; } = true;
        /// <value>
        /// Multiplier for the simulation speed.
        /// </value>
        public float SimulationSpeedSetting { get; private set; } = 1.0f;

        // SIMULATION STATE
        /// <value>
        /// Delay between simulation updates, in seconds.
        /// </value>
        protected float SimulationSpeed = 0.25f;
        /// <value>
        /// Whether the simulation is running.
        /// </value>
        protected bool RunSimulation;
        private IEnumerator TuringMachineUpdateClock() {
            while (true) {
                if (RunSimulation) {
                    SimulateOneStep();
                }
                yield return new WaitForSeconds(SimulationSpeed);
            }
        }
        /// <value>
        /// Return whether the simulation is running.
        /// </value>
        public bool Running { get { return RunSimulation; } }


        /// <summary>
        /// Initializes the <see cref="GameMaster"/> singleton. This is called in
        /// the <see cref="GameMasterMonobehavior.Awake"/> method so that all
        /// <see cref="GameMasterMonobehavior"/> sub-classes can access the
        /// <see cref="GameMaster"/> instance.
        /// 
        /// Grabs Unity prefab resources for UI and sprites, instantiates the
        /// random seed, the grid, and the Turing machines, and then begins
        /// the simulation update co-routine.
        /// </summary>
        public void Initialize() {
            if (UseCustomSeed) {
                Random.InitState(RandomSeed);
            } else {
                RandomSeed = Random.Range(int.MinValue, int.MaxValue);
                print("SEED: " + RandomSeed.ToString());
                Random.InitState(RandomSeed);
            }

            TuringMachineHeadPrefab = (GameObject)Resources.Load("Turing Machine Head");
            TuringMachineEditor = GameObject.Find("TuringMachineEditorCanvas").GetComponent<TuringMachineEditorMonobehaviour>();

            GridData = new GridData();
            TuringMachines = new TuringMachine[NumberOfTuringMachines];
            for (int machineID = 0; machineID < NumberOfTuringMachines; machineID++) {
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
         * Simulate a single step of the "game" by iterating through each
         * <see cref="TuringMachine"/> twice over. First it calls the 
         * <see cref="TuringMachine.PrepareSimulationStep(TM_Symbol)"/> 
         * method of each <see cref="TuringMachine"/>, then it calls the
         * <see cref="TuringMachine.ApplySimulationStep"/> method.
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
        /// Reset the simulation/game of 2DTMs by clearing the whole board and
        /// reseting the existing Turing machines. 
        /// </summary>
        /// <seealso cref="GameMaster.HandleResetButton"/>
        public void ResetGame() {
            Random.InitState(RandomSeed);
            GridData.ResetGrid();
            foreach (TuringMachine tm in TuringMachines) {
                tm.Reset();
            }
        }

        /// <summary>
        /// Writes the given symbol to the given location on the grid.
        /// </summary>
        /// <param name="loc">Grid location to write the symbol to.</param>
        /// <param name="symbol">The symbol to write to the grid.</param>
        public void WriteSymbolToGrid(Vector3Int loc, TM_Symbol symbol) {
            GridData.SetCellSymbol(loc, symbol);
        }


        /***********************************
         * GAMEOBJECT INTERACTION HANDLING *
         ***********************************/
        /// <summary>
        /// Triggered when a <see cref="TuringMachine"/>'s sprite it clicked.
        /// </summary>
        /// <param name="machineID">
        /// The ID of the <see cref="TuringMachine"/> whose sprite was clicked.
        /// </param>
        public void HandleTuringMachineHeadClick(int machineID) {
            print($"Turing machine clicked: {machineID}");
            _PauseSimulation();
            TuringMachineEditor.SetMachine(TuringMachines[machineID]);
        }

        /*****************************
         * KEYPRESS HANDLING METHODS *
         *****************************/
        /// <summary>
        /// Triggered when a keyboard key is pressed. Allows the <see cref="GameMaster"/>
        /// instance to handle user input from the keyboard.
        /// </summary>
        /// <param name="keysPressed">
        /// A list of keys which were pressed since the last update.
        /// </param>
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

        /// <summary>
        /// Triggered by UI buttons which are pressed. Allows the <see cref="GameMaster"/>
        /// instance to do the processing required for whatever button was pressed.
        /// </summary>
        /// <param name="buttonName">The name of the button which was pressed.</param>
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

        /// <summary>
        /// Play or pause the simulation.
        /// </summary>
        public void HandlePlayPauseButton() {
            _ToggleSimulation();
        }

        /// <summary>
        /// Reset the simulation.
        /// 
        /// This clears the whole board, reseting each cell to the <see cref="TM_Symbol.OFF"/>
        /// state. Then it resets all the <see cref="TuringMachine"/> instances,
        /// setting their <see cref="TuringMachine.CurrentState"/> values to the 
        /// initial state and moving them to the (0, 0) cell. Finally, it pauses
        /// the simulation.
        /// </summary>
        /// <remarks>
        /// Under the hood, this just calls <see cref="GameMaster.ResetGame"/>
        /// and then pauses the simulation.
        /// </remarks>
        /// <seealso cref="GameMaster.ResetGame"/>
        public void HandleResetButton() {
            ResetGame();
            _PauseSimulation();
        }

        /// <summary>
        /// Change the random seed used for Turing machine generation.
        /// </summary>
        public void HandleChangeSeedButton() {
            RandomSeed = Random.Range(int.MinValue, int.MaxValue);
        }

        /// <summary>
        /// Sets the seed used for random generation of Turing machines.
        /// </summary>
        /// <param name="value">The new value of the random seed.</param>
        public void SetSeed(int value) {
            RandomSeed = Mathf.Clamp(value, int.MinValue, int.MaxValue);
        }

        /// <summary>
        /// Perform a single step and then pause the simulation
        /// </summary>
        /// <seealso cref="GameMaster.SimulateOneStep"/>
        public void HandleStepButton() {
            _PauseSimulation();
            SimulateOneStep();
        }

        /// <summary>
        /// Increase the speed of the simulation in terms of wall time.
        /// </summary>
        public void HandleSimSpeedIncreaseButton() {
            SimulationSpeedSetting += 0.25f;
            _UpdateSimulationSpeed();
        }

        /// <summary>
        /// Decrease the speed of the simulation in terms of wall time.
        /// </summary>
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
        /// <param name="newCamera"> The <see cref="CameraMonobehavior"/> the
        /// <see cref="GameMaster"/> will control.
        /// </param>
        public void RegisterCameraMonobehavior(CameraMonobehavior newCamera) {
            if (CameraMonobehavior == null) {
                CameraMonobehavior = newCamera;
            }
        }
    }
}