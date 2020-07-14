using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class GameMaster : MonoBehaviour {

    public Tilemap GridTilemap;
    public Camera MainCamera;
    public GameObject TuringMachineHeadPrefab;
    public GameObject SeedLabelObject;

    private int NumberOfTuringMachines = 25;
    private int NumberStatesPerMachine = 3;

    private int RandomSeed = 0;
    private bool UseCustomSeed = false;

    private bool RandomBoardGeneration = false;
    private bool RandomStartingTransitions = true;

    private float SimulationSpeedSetting = 1.0f;
    private float SimulationSpeed = 0.25f;

    private SeedLabelController SeedLabelController;
    private Tilemap_Controller TilemapController;
    private Camera_Controller MainCamController;
    private GameObject[] TuringMachineHeads;
    private TuringMachineHeadController[] TuringMachineHeadControllers;
    private TuringMachine[] TuringMachines;

    private bool RunSimulation;
    private IEnumerator TuringMachineUpdateClock() {
        while (true) {
            if (RunSimulation)
            {
                SimulateOneStep();
            }
            yield return new WaitForSeconds(SimulationSpeed);
        }
    }

    // Start is called before the first frame update
    void Start() {
        // Get controllers for various GameObjects
        SeedLabelController = SeedLabelObject.GetComponent<SeedLabelController>();
        TilemapController = GridTilemap.GetComponent<Tilemap_Controller>();
        MainCamController = MainCamera.GetComponent<Camera_Controller>();

        // Set initial seed based on random vs. custom
        if (UseCustomSeed) {
            Random.InitState(RandomSeed);
        } else
        {
            RandomSeed = Random.Range(int.MinValue, int.MaxValue);
            print("SEED: " + RandomSeed.ToString());
            Random.InitState(RandomSeed);
        }
        SeedLabelController.SetLabelToSeed(RandomSeed);

        // Create Turing machines
        TuringMachineHeads = new GameObject[NumberOfTuringMachines];
        TuringMachineHeadControllers = new TuringMachineHeadController[NumberOfTuringMachines];
        TuringMachines = new TuringMachine[NumberOfTuringMachines];
        _CreateTuringMachines();

        // Randomly create transition functions if requested
        if (RandomStartingTransitions)
        {
            foreach (TuringMachine tm in TuringMachines)
            {
                tm.InitWithRandomTransitions();
                print(tm.ToString());
            }
        }

        RunSimulation = false;
        IEnumerator simulationUpdater = TuringMachineUpdateClock();
        StartCoroutine(simulationUpdater);
    }

    // Update is called once per frame
    void Update() {
        if (MainCamController.moved) {
            if (RandomBoardGeneration) {
                TilemapController.RandomPopulateGridAtView(MainCamController.world_view_rect);
            } else {
                TilemapController.PopulateGridAtView(MainCamController.world_view_rect);
            }
            MainCamController.moved = false;
        }

        if (Input.GetMouseButtonUp((int)MouseButton.LeftMouse) && !EventSystem.current.IsPointerOverGameObject()) {
            Vector3 mouse_click_pos = Input.mousePosition;
            Vector3 world_pos = MainCamera.ScreenToWorldPoint(mouse_click_pos);
            TilemapController.FlipTileAtWorldPos(world_pos);
        }
    }

    /// <summary>
    /// Initialize a simulation/game of 2DTMs by clearing the whole board and creating
    /// new Turing machines. 
    /// </summary>
    public void ResetGame()
    {
        // Reset the symbols written on the grid cells
        TilemapController.ResetBoard();
        // Reset to the original random seed. 
        Random.InitState(RandomSeed);

        // Move all the old Turing machines back to the center
        foreach (GameObject tmHead in TuringMachineHeads)
        {
            Destroy(tmHead);
        }

        // Create new Turing machines
        _CreateTuringMachines();

        // Randomly create transition functions if requested
        if (RandomStartingTransitions)
        {
            foreach (TuringMachine tm in TuringMachines)
            {
                tm.InitWithRandomTransitions();
                print(tm.ToString());
            }
        }
    }

    /**
     * <summary>
     * Simulate a single step of the "game" by iterating through each Turing machine.
     * </summary>
     */
    public void SimulateOneStep() {
        (TM_Direction, TM_Symbol)[] tm_output_list = new (TM_Direction, TM_Symbol)[NumberOfTuringMachines];
        for (int tm_id=0; tm_id < NumberOfTuringMachines; tm_id++) {
            Vector3 tm_world_loc = TuringMachineHeads[tm_id].transform.position;
            Vector3Int tm_tile_loc = GridTilemap.WorldToCell(tm_world_loc);
            TM_Symbol tm_input_symbol = TilemapController.GetTileSymbol(tm_tile_loc);

            tm_output_list[tm_id] = TuringMachines[tm_id].HandleInput(tm_input_symbol);
            //print("TM " + tm_id.ToString() + " state " + TuringMachines[tm_id].CurrentState.ToString() + " output " + tm_output_list[tm_id].ToString());
        }

        // TODO: Resolve any conflicts

        for (int tm_id = 0; tm_id < NumberOfTuringMachines; tm_id++) {
            TuringMachineHeadController tm_head_controller = TuringMachineHeadControllers[tm_id];
            (TM_Direction, TM_Symbol) tm_output = tm_output_list[tm_id];
            TuringMachine turing_machine = TuringMachines[tm_id];

            Vector3 current_head_world_loc = tm_head_controller.position;
            Vector3Int current_head_grid_loc = GridTilemap.WorldToCell(current_head_world_loc);
            TM_Symbol write_symbol = tm_output.Item2;
            TM_Direction move_direction = tm_output.Item1;

            TilemapController.SetTileSymbol(current_head_grid_loc, write_symbol);
            tm_head_controller.MoveHeadInDirection(move_direction);
            turing_machine.UpdateState();
        }
    }

    public void HandlePlayPauseButton()
    {
        _ToggleSimulation();
    }

    public void HandleResetButton()
    {
        ResetGame();
        _PauseSimulation();
    }

    public void HandleChangeSeedButton()
    {
        RandomSeed = Random.Range(int.MinValue, int.MaxValue);
        SeedLabelController.SetLabelToSeed(RandomSeed);
    }

    public void HandleStepButton()
    {
        _PauseSimulation();
        SimulateOneStep();
    }

    public void HandleSimSpeedIncreaseButton()
    {
        SimulationSpeedSetting += 0.25f;
        _UpdateSimulationSpeed();
    }

    public void HandleSimSpeedDecreaseButton()
    {
        SimulationSpeedSetting -= 0.25f;
        _UpdateSimulationSpeed();
    }

    private void _CreateTuringMachines()
    {
        for (int i = 0; i < NumberOfTuringMachines; i++)
        {
            TuringMachineHeads[i] = Instantiate(TuringMachineHeadPrefab, Vector3.zero, Quaternion.identity);
            TuringMachineHeadControllers[i] = TuringMachineHeads[i].GetComponent<TuringMachineHeadController>();
            TuringMachines[i] = new TuringMachine(NumberStatesPerMachine);
        }
    }

    private void _ToggleSimulation()
    {
        if (RunSimulation) { _PauseSimulation(); }
        else { _ResumeSimulation(); }
    }

    private void _PauseSimulation()
    {
        RunSimulation = false;
    }

    private void _ResumeSimulation()
    {
        RunSimulation = true;
    }

    private void _UpdateSimulationSpeed()
    {
        SimulationSpeed = 1.0f / SimulationSpeedSetting;
    }
}
