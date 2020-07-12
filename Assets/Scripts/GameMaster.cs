using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class GameMaster : MonoBehaviour {

    public Tilemap GridTilemap;
    public Camera MainCamera;
    public GameObject TuringMachineHeadPrefab;
    public int NumberOfTuringMachines;
    public bool RandomBoardGeneration;

    private Tilemap_Controller TilemapController;
    private Camera_Controller MainCamController;
    private GameObject[] TuringMachineHeads;
    private TuringMachineHeadController[] TuringMachineHeadControllers;
    private TuringMachine[] TuringMachines;

    private bool RunSimulation;
    private IEnumerator TuringMachineUpdateClock(float waitTime) {
        while (RunSimulation) {
            SimulateOneStep();
            yield return new WaitForSeconds(waitTime);
        }
    }

    // Start is called before the first frame update
    void Start() {
        TuringMachineHeads = new GameObject[NumberOfTuringMachines];
        TuringMachineHeadControllers = new TuringMachineHeadController[NumberOfTuringMachines];
        TuringMachines = new TuringMachine[NumberOfTuringMachines];
        for (int i = 0; i < NumberOfTuringMachines; i++) {
            TuringMachineHeads[i] = Instantiate(TuringMachineHeadPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            TuringMachineHeadControllers[i] = TuringMachineHeads[i].GetComponent<TuringMachineHeadController>();
            TuringMachines[i] = new TuringMachine(100);
        }

        TilemapController = GridTilemap.GetComponent<Tilemap_Controller>();
        MainCamController = MainCamera.GetComponent<Camera_Controller>();
        foreach (TuringMachine tm in TuringMachines) {
            tm.InitWithRandomTransitions();
            print(tm.ToString());
        }

        RunSimulation = true;
        IEnumerator simulationUpdater = TuringMachineUpdateClock(0.1f);
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

        if (Input.GetMouseButtonUp((int)MouseButton.LeftMouse)) {
            Vector3 mouse_click_pos = Input.mousePosition;
            Vector3 world_pos = MainCamera.ScreenToWorldPoint(mouse_click_pos);
            TilemapController.FlipTileAtWorldPos(world_pos);
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
            print("TM " + tm_id.ToString() + " state " + TuringMachines[tm_id].CurrentState.ToString() + " output " + tm_output_list[tm_id].ToString());
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
}
