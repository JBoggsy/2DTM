using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TuringMachineHeadMonobehavior : GameObjectMonobehavior
{
    public Vector3 position {
        get { return gameObject.transform.position; }
    }
    public void MoveHeadInDirection(TM_Direction dir) {
        Vector3 move_vector = Vector3.zero;
        switch (dir) {
            case TM_Direction.UP:
                move_vector = new Vector3(0, 1);
                break;
            case TM_Direction.DOWN:
                move_vector = new Vector3(0, -1);
                break;
            case TM_Direction.LEFT:
                move_vector = new Vector3(-1, 0);
                break;
            case TM_Direction.RIGHT:
                move_vector = new Vector3(1, 0);
                break;
        }

        gameObject.transform.Translate(move_vector);
    }

    public void MoveHeadToCenter()
    {
        MoveHeadTo(Vector3Int.zero);
    }

    public void MoveHeadTo(Vector3Int newLocation)
    {
        gameObject.transform.position = newLocation;
    }
}
