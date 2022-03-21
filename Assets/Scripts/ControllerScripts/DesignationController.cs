using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesignationController : MonoBehaviour
{
    Designation.DesignationType designationModeType = Designation.DesignationType.None;


    public void SetDesignationMode(Designation.DesignationType desig) {
        designationModeType = desig;
    }
    
    
    public void DoBuild(Tile t) {



    }

        

}
