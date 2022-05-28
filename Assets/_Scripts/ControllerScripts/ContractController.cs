using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ContractController : MonoBehaviour
{
    [SerializeField] private TMP_Text[] _contractText;
    
    private int _numberOfBedRequired;
    private int _numberOfStorageBoxRequired;
    private int _numberOfThrustersRequired;
    private int _numberOfEngineRequired;
    private int _numberOfFoodDispenserRequired;
    private int _numberOfMinimumShipHeight;
    private int _numberOfMaximumShipHeight;
    private int _numberOfMinimumShipWidth;
    private int _umberOfMaximumShipWidth;
    private int _numberBudget;
    
    private void Awake()
    {
        ContractCreator cc = GameManager.Instance._currentContract;
        
        _numberOfBedRequired = cc.randomNumberOfBedRequired;
        _numberOfStorageBoxRequired = cc.randomNumberOfStorageBoxRequired;
        _numberOfThrustersRequired = cc.randomNumberOfThrustersRequired;
        _numberOfEngineRequired = cc.randomNumberOfEngineRequired;
        _numberOfFoodDispenserRequired = cc.randomNumberOfFoodDispenserRequired;
        _numberOfMinimumShipHeight = cc.randomNumberOfMinimumShipHeight;
        _numberOfMaximumShipHeight = cc.randomNumberOfMaximumShipHeight;
        _numberOfMinimumShipWidth = cc.randomNumberOfMinimumShipWidth;
        _umberOfMaximumShipWidth = cc.randomNumberOfMaximumShipWidth;
        _numberBudget = cc.randomNumberBudget;
        
    }
    
    private void Start()
    {
        
        if (_numberOfBedRequired > 0)
        {
            _contractText[0].text = "Bed Required : " + _numberOfBedRequired;
        }
        if (_numberOfStorageBoxRequired > 0)
        {
            _contractText[1].text = "Storage Box Required : " + _numberOfStorageBoxRequired;
        }
        if (_numberOfThrustersRequired > 0)
        {
            _contractText[2].text = "Thrusters Required : " + _numberOfThrustersRequired ;
        }
        if (_numberOfEngineRequired > 0)
        {
            _contractText[3].text = "Engine Required : " + _numberOfEngineRequired ;
        }
        if (_numberOfFoodDispenserRequired > 0)
        {
            _contractText[4].text = "Food Dispenser Required : " + _numberOfFoodDispenserRequired ;
        }
        if (_numberOfMinimumShipHeight > 0)
        {
            _contractText[5].text = "Minimum Ship Height : " + _numberOfMinimumShipHeight;
        }
        if (_numberOfMaximumShipHeight > 0)
        {
            _contractText[6].text = "Maximum Ship Height : " + _numberOfMaximumShipHeight;
        }
        if (_numberOfMinimumShipWidth > 0)
        {
            _contractText[7].text = "Minimum Ship Width : " + _numberOfMinimumShipWidth ;
        }
        if ( _umberOfMaximumShipWidth > 0)
        {
            _contractText[8].text = "Maximum Ship Width : " + _umberOfMaximumShipWidth;
        }
        if (_numberBudget > 0) 
        {
            _contractText[9].text += "Budget : " + _numberBudget;
        }

        foreach (TMP_Text requirement in _contractText)
        {
            if(requirement.text == "Option A")
            {
                requirement.transform.parent.gameObject.SetActive(false);
            }
        }
        
        
        
    }
    
    
    
}
