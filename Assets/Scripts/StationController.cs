using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationChecker : MonoBehaviour
{
    //Check if what user builds is a station room
    /*
    What is a minimum room ?
    -> Enclosed with walls and doors.
    -> Has floor tiles in between enclosion
    -> Tiles in the room are pressurized and has oxygen
    */

    GameManager manager;

    private void Awake()
    {
        manager = gameObject.GetComponent<GameManager>();
    }


}
