using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // TODO: This will upon completion inspect the build scene to check if min conditions are met with.

    public static GameManager Instance; //This is the only instance of game controller there ever will be.

    public float Currency { get; protected set; }
    public float Respect { get; protected set; }
    
    public int[] TechTree { get; protected set; } // if we unlocked 0 th tech it's 1 if not it's 0
    public int TechCount { get; protected set; } //amount of technologies there are
    public int NumOfWorkersConstruction { get; private set; }

    private void Awake() {
        Instance = this;

        TechCount = 10; //TODO: Change this depending on the final size of tech tree.

        DontDestroyOnLoad(this.gameObject);

    }

    public void SaveGame()
    {
        SaveSystem.SaveGame(this);
    }

    public void LoadGame()
    {
        GameData data = SaveSystem.LoadGame();

        this.Currency = data.currency;
        this.Respect = data.respect;
        this.NumOfWorkersConstruction = data.numOfWorkersConstruction;

        this.TechTree = new int[TechCount];
        for (int i = 0; i < TechCount; i++)
        {
            this.TechTree[i] = data.techTree[i];
        }

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
