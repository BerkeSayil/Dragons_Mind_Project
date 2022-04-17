using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // TODO: This will be a class that doesn't get destroyed in change of scenes
    // TODO: This will upon completion inspect the build scene to check if min conditions are met with.

    public static GameManager Instance; //This is the only instance of game controller there ever will be.

    public float Currency { get; protected set; }
    public int NumOfWorkersConstruction { get; private set; }

    //TODO: Game state machine to run it maybe first look it up ?

    private void Awake() {
        Instance = this;

        DontDestroyOnLoad(this.gameObject);

        NumOfWorkersConstruction = 5;  //DEBUG
        Currency = 10000f;
    }


    public void GoToBuild() {


        SceneManager.LoadScene(1, LoadSceneMode.Single);

    }


    // TODO: This place will hold all save-able and don't destroy on load information about the game

    //TODO: Amount on employees based on occupations
    //TODO: Types of upgrades we got
    //TODO: Our respect and trust that works with contracts
    //TODO: Company management menu information... [probably? stats, how we doing ?] [why not display them on main screen]
}
