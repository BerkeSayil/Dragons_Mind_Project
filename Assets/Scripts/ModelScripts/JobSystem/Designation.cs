using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Designation {

    // Designation system will require to be in a valid room that's not
    // room index 0 unless canInSpace is true;

    public Tile tile { get; protected set; } // The tiles that's designated
    public enum DesignationType { // Tells what designation color to render.

        None,             // default tile
        PersonalCrewRoom, // where they sleep n stuff
        Kitchen,          // responsible for cooking
        Cafeteria,        // where they eat n drink
        Engine,           // making sure no problems on electricty side
        LifeSupport,      // responsible for not making everyone die (athmos balance)
        TradeGoods        // where we receive bought goods.


    }
    Action<Designation> cbDesignationChanged;


    DesignationType type = DesignationType.None;

    public DesignationType Type {
        get {
            return type;
        }
        set {
            DesignationType oldType = type;
            type = value;
            // call callback to let things know we changed this
            if (cbDesignationChanged != null && oldType != type)
                cbDesignationChanged(this);
        }
    }

    public bool canInSpace { get; protected set; } // determines if this designation can be on empty tiles
                                                   //examples would be trade ship docking designation ?

    // how big is this designation
    int width;
    int height;






    public void RegisterDesignationTypeChangedCallback(Action<Designation> callback) {
        // the place where this gets called is subscribed to the cbTDesignationTypeChanged
        // so when cbTDesignationTypeChanged Action gets called
        // in return this function callsback to
        // original script where its called.
        cbDesignationChanged += callback;
    }
    public void UnRegisterDesignationTypeChangedCallback(Action<Designation> callback) {
        cbDesignationChanged -= callback;
    }
    



}
