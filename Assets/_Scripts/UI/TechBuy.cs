using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechBuy : MonoBehaviour
{
    public GameObject[] techShadows;
    public GameObject[] techBuyButtons;
    public void BuyTech(int techID)
    {
        if (GameManager.Instance.Techs[techID].TechCost <= GameManager.Instance.Currency)
        {
            GameManager.Instance.Currency -= GameManager.Instance.Techs[techID].TechCost;
            GameManager.Instance.Techs[techID].IsTechBought = true;
            
            techBuyButtons[techID].SetActive(false);
            techShadows[techID].SetActive(false);
        }
    }
}
