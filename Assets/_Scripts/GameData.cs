
[System.Serializable]
public class GameData
{

    public float currency;
    public float respect;
    public int[] techTree;
    
    public int numOfWorkersConstruction;

    public GameData(GameManager manager)
    {
        currency = manager.Currency;
        respect = manager.Respect;
        
        numOfWorkersConstruction = manager.NumOfWorkersConstruction;

        techTree = new int[manager.TechCount];
        for (int i = 0; i < manager.TechCount; i++)
        {
            techTree[i] = manager.TechTree[i];
        }

    }
    
    
}
