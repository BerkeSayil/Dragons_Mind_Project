
[System.Serializable]
public class GameData
{

    public float currency;
    public float respect;
    
    // Technology variables need to deconstructed
    public int[] TechIds;
    public int[] TechCosts;
    public bool[] TechUnlocked;

    public int numOfWorkersConstruction;

    public void TechnologyToPrimaries()
    {
        GameManager manager = GameManager.Instance;
        TechIds = new int[manager.TechCount];
        TechCosts = new int[manager.TechCount];
        TechUnlocked = new bool[manager.TechCount];
        
        for (int i = 0; i < manager.TechCount; i++)
        {
            TechIds[i] = i;
            TechCosts[i] = manager.Techs[i].TechCost;
            TechUnlocked[i] = manager.Techs[i].IsTechBought;
        }
        
    }
    public GameData(GameManager manager)
    {
        currency = manager.Currency;
        respect = manager.Respect;
        
        numOfWorkersConstruction = manager.NumOfWorkersConstruction;

        TechnologyToPrimaries();

    }
    
    
}
