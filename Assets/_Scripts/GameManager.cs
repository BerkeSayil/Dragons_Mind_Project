using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // TODO: This will be a class that doesn't get destroyed in change of scenes
    // TODO: This will upon complation inspect the build scene to check if min conditions are met with.

    public static GameManager instance; //This is the only instance of gamecontroller there ever will be.

    public int numOfWorkersConstruction { get; protected set; }


    private void Awake() {
        instance = this;

        DontDestroyOnLoad(this.gameObject);

        numOfWorkersConstruction = 5;  //DEBUG
    }


    public void GoToBuild() {


        SceneManager.LoadScene(1, LoadSceneMode.Single);

    }


    // TODO: This place will hold all save-able and don't destroy on load information about the game

    //TODO: Amount on employees based on occupations
    //TODO: Types of upgrades we got
    //TODO: Our respect and trust that works with contracts
    //TODO: Company management menu information... [probly? stats, how we doin ?] [why not display them on main screen]
}
