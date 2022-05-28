using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Technology
{
    // has an id to represent the technology
    public int TechId;
    // has a name to represent the technology
    public string TechName;
    // has a description to represent the technology
    public string TechDescription;
    // has a cost to represent the technology
    public int TechCost;
    // has a bool to represent if the technology is bought
    public bool IsTechBought;
    
    //has a constructor to create a new technology
    public Technology(int id, string name, string description, int cost, bool bought)
    {
        TechId = id;
        TechName = name;
        TechDescription = description;
        TechCost = cost;
        IsTechBought = bought;
    }
    
}