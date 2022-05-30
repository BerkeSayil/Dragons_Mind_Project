using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BudgetMoneyInfo : MonoBehaviour
{
    // Get the budget money info text element
    public TMP_Text budgetMoneyInfoText;
    private float _budgetMoney;
    private const float Tolerance = 100f;
    
    private void Start()
    {
        // Get the budget money from the game manager
        _budgetMoney = GameManager.Instance.Currency;
        
        // Update the budget money info text
        UpdateBudgetMoneyInfoText();
    }
    
    private void UpdateBudgetMoneyInfoText()
    {
        // Update the budget money info text
        _budgetMoney = GameManager.Instance.Currency;
        budgetMoneyInfoText.text = "Remaining Money: " + _budgetMoney;
    }

    private void FixedUpdate()
    {
        if (Math.Abs(_budgetMoney - GameManager.Instance.Currency) > Tolerance)
        {
            UpdateBudgetMoneyInfoText();
        }
    }

}
