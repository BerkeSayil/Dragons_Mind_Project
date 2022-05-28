using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ContractCreator : MonoBehaviour
{
    // Get Chapter Button component of this gameobject
    [SerializeField] private TMP_Text _descriptionText;
    private String _contractRequirements;

    public int randomNumberOfBedRequired;
    public int randomNumberOfBathRequired;
    public int randomNumberOfKitchenRequired;
    public int randomNumberOfLivingRoomRequired;
    public int randomNumberOfDiningRoomRequired;
    public int randomNumberOfBathRoomRequired;
    public int randomNumberOfGarageRequired;
    public int randomNumberOfStorageBoxRequired;
    public int randomNumberOfThrustersRequired;
    public int randomNumberOfLightsRequired;
    public int randomNumberOfEngineRequired;
    public int randomNumberOfCockpitRequired;
    public int randomNumberOfCargoBayRequired;
    public int randomNumberOfFoodDispenserRequired;
    public int randomNumberOfMinimumShipHeight;
    public int randomNumberOfMaximumShipHeight;
    public int randomNumberOfMinimumShipWidth;
    public int randomNumberOfMaximumShipWidth;
    public int randomNumberBudget;
    
    private void Start()
    {
        
        // procedurally generate the contract requirements
        _contractRequirements = GenerateContractRequirements();

        _descriptionText.text = _contractRequirements;
    }


    private string GenerateContractRequirements()
    {
        // Generate a random number between -10 and 10
        randomNumberOfBedRequired = UnityEngine.Random.Range(-10, 10);
        // Generate a random number between -10 and 10
        randomNumberOfStorageBoxRequired = UnityEngine.Random.Range(-10, 10);
        // Generate a random number between -5 and 5
        randomNumberOfThrustersRequired = UnityEngine.Random.Range(-5, 5);
        // Generate a random number between -2 and 2
        randomNumberOfEngineRequired = UnityEngine.Random.Range(-2, 2);
        // Generate a random number between -3 and 3
        randomNumberOfFoodDispenserRequired = UnityEngine.Random.Range(-3, 3);
        // Generate a random number between -15 and 15
        randomNumberOfMinimumShipHeight = UnityEngine.Random.Range(-15, 15);
        // Generate a random number between -30 and 30
        randomNumberOfMaximumShipHeight = UnityEngine.Random.Range(-30, 30);
        // Generate a random number between -10 and 10
        randomNumberOfMinimumShipWidth= UnityEngine.Random.Range(-10, 10);
        // Generate a random number between -20 and 20
        randomNumberOfMaximumShipWidth = UnityEngine.Random.Range(-20, 20);
        // Generate a random number between 15000 and 20000
        randomNumberBudget = UnityEngine.Random.Range(15000, 20000);

        String finalRequirements = "";

        if (randomNumberOfBedRequired > 0)
        {
            finalRequirements += "Bed Required : " + randomNumberOfBedRequired + "\n";
        }

        if (randomNumberOfStorageBoxRequired > 0)
        {
            finalRequirements += "Storage Box Required : " + randomNumberOfStorageBoxRequired + "\n";
        }

        if (randomNumberOfThrustersRequired > 0)
        {
            finalRequirements += "Thrusters Required : " + randomNumberOfThrustersRequired + "\n";
        }

        if (randomNumberOfEngineRequired > 0)
        {
            finalRequirements += "Engine Required : " + randomNumberOfEngineRequired + "\n";
        }

        if (randomNumberOfFoodDispenserRequired > 0)
        {
            finalRequirements += "Food Dispenser Required : " + randomNumberOfFoodDispenserRequired + "\n";
        }

        if (randomNumberOfMinimumShipHeight > 0)
        {
            finalRequirements += "Minimum Ship Height : " + randomNumberOfMinimumShipHeight + "\n";
        }

        if (randomNumberOfMaximumShipHeight > 0)
        {
            finalRequirements += "Maximum Ship Height : " + randomNumberOfMaximumShipHeight + "\n";
        }

        if (randomNumberOfMinimumShipWidth > 0)
        {
            finalRequirements += "Minimum Ship Width : " + randomNumberOfMinimumShipWidth + "\n";
        }

        if ( randomNumberOfMaximumShipWidth > 0)
        {
            finalRequirements += "Maximum Ship Width : " + randomNumberOfMaximumShipWidth + "\n";
        }
        if (randomNumberBudget > 0) 
        {
            finalRequirements += "Budget : " + randomNumberBudget + "\n";
        }
        

        return finalRequirements;


    }
}
