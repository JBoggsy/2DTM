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

    private Tilemap_Controller tmap_controller;
    private Camera_Controller mcam_controller;
    private GameObject[] tm_heads;
    private TuringMachineHeadController[] tm_head_controllers;
    private TuringMachine[] turing_machines;

    private bool run_simulation;
    private IEnumerator TuringMachineUpdateClock(float waitTime) {
        while (run_simulation) {
            SimulateOneStep();
            yield return new WaitForSeconds(waitTime);
        }
    }

    // Start is called before the first frame update
    void Start() {
        tm_heads = new GameObject[NumberOfTuringMachines];
        tm_head_controllers = new TuringMachineHeadController[NumberOfTuringMachines];
        turing_machines = new TuringMachine[NumberOfTuringMachines];
        for (int i = 0; i < NumberOfTuringMachines; i++) {
            tm_heads[i] = Instantiate(TuringMachineHeadPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            tm_head_controllers[i] = tm_heads[i].GetComponent<TuringMachineHeadController>();
            turing_machines[i] = new TuringMachine(10);
        }

        tmap_controller = GridTilemap.GetComponent<Tilemap_Controller>();
        mcam_controller = MainCamera.GetComponent<Camera_Controller>();
        foreach (TuringMachine tm in turing_machines) {
            tm.InitWithRandomTransitions();
            print(tm.ToString());
        }

        run_simulation = true;
        IEnumerator simulationUpdater = TuringMachineUpdateClock(1.0f);
        StartCoroutine(simulationUpdater);
    }

    // Update is called once per frame
    void Update() {
        if (mcam_controller.moved) {
            if (RandomBoardGeneration) {
                tmap_controller.RandomPopulateGridAtView(mcam_controller.world_view_rect);
            } else {
                tmap_controller.PopulateGridAtView(mcam_controller.world_view_rect);
            }
            mcam_controller.moved = false;
        }

        if (Input.GetMouseButtonUp((int)MouseButton.LeftMouse)) {
            Vector3 mouse_click_pos = Input.mousePosition;
            Vector3 world_pos = MainCamera.ScreenToWorldPoint(mouse_click_pos);
            tmap_controller.FlipTileAtWorldPos(world_pos);
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
            Vector3 tm_world_loc = tm_heads[tm_id].transform.position;
            Vector3Int tm_tile_loc = GridTilemap.WorldToCell(tm_world_loc);
            TM_Symbol tm_input_symbol = tmap_controller.GetTileSymbol(tm_tile_loc);

            tm_output_list[tm_id] = turing_machines[tm_id].HandleInput(tm_input_symbol);
            print("TM " + tm_id.ToString() + " state " + turing_machines[tm_id].CurrentState.ToString() + " output " + tm_output_list[tm_id].ToString());
        }

        // TODO: Resolve any conflicts

        for (int tm_id = 0; tm_id < NumberOfTuringMachines; tm_id++) {
            TuringMachineHeadController tm_head_controller = tm_head_controllers[tm_id];
            (TM_Direction, TM_Symbol) tm_output = tm_output_list[tm_id];
            TuringMachine turing_machine = turing_machines[tm_id];

            Vector3 current_head_world_loc = tm_head_controller.position;
            Vector3Int current_head_grid_loc = GridTilemap.WorldToCell(current_head_world_loc);
            TM_Symbol write_symbol = tm_output.Item2;
            TM_Direction move_direction = tm_output.Item1;

            tmap_controller.SetTileSymbol(current_head_grid_loc, write_symbol);
            tm_head_controller.MoveHeadInDirection(move_direction);
            turing_machine.UpdateState();
        }
    }
}
