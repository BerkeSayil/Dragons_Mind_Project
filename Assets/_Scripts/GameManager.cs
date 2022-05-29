using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // TODO: This will upon completion inspect the build scene to check if min conditions are met with.

    public static GameManager Instance; //This is the only instance of game controller there ever will be.

    public float Currency { get; set; }
    public float Respect { get; protected set; }
    public int NumOfWorkersConstruction { get; private set; }
    public Technology[] Techs { get; set; } //array of techs

    public int TechCount = 18;
    public GameObject Mothership;

    public ContractCreator _currentContract { get; private set; }

    public Vector2 PlayableArea;

    private void Awake() {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        
        LoadGame();

    }

    private void CreateTechs()
    {
        //use constructor to create techs
        Techs = new Technology[TechCount];
        Techs[0] = new Technology(0, "WIP", "WIP", 1000, false);
        Techs[1] = new Technology(1, "WIP", "WIP", 1000, false);
        Techs[2] = new Technology(2, "WIP", "WIP", 1000, false);
        Techs[3] = new Technology(3, "WIP", "WIP", 1000, false);
        Techs[4] = new Technology(4, "WIP", "WIP", 1000, false);
        Techs[5] = new Technology(5, "WIP", "WIP", 1000, false);
        Techs[6] = new Technology(6, "WIP", "WIP", 1000, false);
        Techs[7] = new Technology(7, "WIP", "WIP", 1000, false);
        Techs[8] = new Technology(8, "WIP", "WIP", 1000, false);
        Techs[9] = new Technology(9, "WIP", "WIP", 1000, false);
        Techs[10] = new Technology(10, "WIP", "WIP", 1000, false);
        Techs[11] = new Technology(11, "WIP", "WIP", 1000, false);
        Techs[12] = new Technology(12, "WIP", "WIP", 1000, false);
        Techs[13] = new Technology(13, "WIP", "WIP", 1000, false);
        Techs[14] = new Technology(14, "WIP", "WIP", 1000, false);
        Techs[15] = new Technology(15, "WIP", "WIP", 1000, false);
        Techs[16] = new Technology(16, "WIP", "WIP", 1000, false);
        Techs[17] = new Technology(17, "WIP", "WIP", 1000, false);
        
    }
    private void LoadTechs(int[] TechIds, int[] TechCosts, bool[] TechUnlocked)
    {
        
        for (int i = 0; i < TechCount; i++)
        {
            Techs[i] = new Technology(TechIds[i], "WIP", "WIP", TechCosts[i], TechUnlocked[i]);
            if (Techs[i].IsTechBought)
            {
                TechBuy tb = Mothership.GetComponent<TechBuy>();
                tb.techShadows[i].SetActive(false);
                tb.techBuyButtons[i].SetActive(false);
                    
            }
        }

    }

    public void SaveGame()
    {
        SaveSystem.SaveGame(this);
    }

    public void LoadGame()
    {
        try {
            GameData data = SaveSystem.LoadGame();

            this.Currency = data.currency;
            this.Respect = data.respect;
            this.NumOfWorkersConstruction = data.numOfWorkersConstruction;

            this.Techs = new Technology[TechCount];
            LoadTechs(data.TechIds, data.TechCosts, data.TechUnlocked);
            
        }
        catch (Exception e) {
            print("first time loading game");
            //first time loading game
            Currency = 3500;
            Respect = 15;
            NumOfWorkersConstruction = 3;
            CreateTechs();
        }  
        
    }

    public void GoToBuild(ContractCreator cc)
    {

        _currentContract = cc;

        SetPlayableArea();
        
        SceneManager.LoadScene(1, LoadSceneMode.Single);

    }

    private void SetPlayableArea()
    {
        if(Techs[3].IsTechBought)
        {
            PlayableArea = new Vector2(40, 40);
        }else if (Techs[6].IsTechBought)
        {
            PlayableArea = new Vector2(60, 60);
        }else if (Techs[9].IsTechBought)
        {
            PlayableArea = new Vector2(80, 80);
        }else if (Techs[12].IsTechBought)
        {
            PlayableArea = new Vector2(100, 100);
        }else if (Techs[15].IsTechBought)
        {
            PlayableArea = new Vector2(125, 125);
        }
        else
        {
            //default
            PlayableArea = new Vector2(20, 20);
        }
    }


    // TODO: This place will hold all save-able and don't destroy on load information about the game

    
    //TODO: Our respect and trust that works with contracts
    //TODO: Company management menu information... [probably? stats, how we doing ?] [why not display them on main screen]
}
