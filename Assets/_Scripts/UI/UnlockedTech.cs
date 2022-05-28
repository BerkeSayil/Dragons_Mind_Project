using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockedTech : MonoBehaviour
{
    public bool unlocked;
    [SerializeField] private GameObject techShadow;
    
    public void UnlockTech(int techId)
    {
        unlocked = true;
        techShadow.SetActive(false);
        
    }
    
    
}
