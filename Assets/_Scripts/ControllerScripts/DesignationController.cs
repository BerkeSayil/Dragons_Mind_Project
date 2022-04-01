using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DesignationController : MonoBehaviour
{
    Designation.DesignationType designationModeType = Designation.DesignationType.None;
    public bool areWeDesignating = false;

    Dictionary<string, Designation.DesignationType> designationTypes;
    Action<Tile> cbTileChangedDesignation;
    
    World world;
    Dictionary<Designation, GameObject> designationGameObjectMap = new Dictionary<Designation, GameObject>();

    List<Tile> tiles;
    private void Start() {
        LoadDesignations();
        tiles = new List<Tile>();

        world = WorldController.Instance.world;
        world.RegisterFurnitureCreated(UpdateDesignationFurns);
        world.RegisterDesignationChanged(UpdateDesignation);
    }

    public void UpdateDesignationFurns(Furniture furn) {

        foreach (Designation designation in world.designations) {

            designation.UpdateDesignationFurnitures();
            if (designation.IsFunctional()) {

            }
        }
    }
    private void MakeDesignationObject(Designation designation) {

        GameObject designationGO = new GameObject(" " + designation.Type);
        designationGO.transform.parent = gameObject.transform;

        designationGO.transform.position = new Vector2(designation.centerX, designation.centerY);
        
        
        foreach(Tile t in tiles) {
            t.designationType = designation.Type;

            if (cbTileChangedDesignation != null)
                cbTileChangedDesignation(t);
        }

        designationGameObjectMap.Add(designation, designationGO);

        if (designation.IsFunctional()) return;

        designationGO.SetActive(false);



    }

    private void UpdateDesignation(Designation d) {

        if(designationGameObjectMap.ContainsKey(d) == false) {
            Debug.Log("You are searching for a non existing designation but it wasn't removed from this dictionary probably ?");
            return;
        }
        if (designationGameObjectMap[d].activeInHierarchy == false) {
            if (d.IsFunctional()) {
                designationGameObjectMap[d].SetActive(true);
            }
        }
       
    }

    public void SetDesignationMode(string designationType) {

        if(designationTypes.ContainsKey(designationType) == false) {
            Debug.Log("You are searching for a non existing designation type! ~ did you make a mistake on buttons? ");
            return;
        }
        designationModeType = designationTypes[designationType];

        tiles.Clear();
    }

    private void LoadDesignations() {
        // Getting installed objects from the resources folder

        designationTypes = new Dictionary<string, Designation.DesignationType>();

        foreach (Designation.DesignationType dt in (Designation.DesignationType[])Enum.GetValues(typeof(Designation.DesignationType))) {
            designationTypes[dt.ToString()] = dt;
        }
    }

    public void DoDesignate() {

        Designation designation = new Designation(tiles, designationModeType) ;

        bool isDesignationValid = designation.IsValidDesignation(tiles);

        if (isDesignationValid && designation.Type != Designation.DesignationType.None) {
            MakeDesignationObject(designation);
        }
        else {
            designation.DestroyDesignation();
        }

        tiles.Clear(); // clear the list so designations don't get mixed up

    }
    public void RemoveDesignation() {
        //TODO:Get a way to remove
    }

    public void RegisterTileDesignationChangedCallback(Action<Tile> callback) {
        // the place where this gets called is subscribed to the cbTileDesignationChanged
        // so when cbTileDesignationChanged Action gets called in return this function callsback to
        // original script where its called.
        cbTileChangedDesignation += callback;
    }
    public void UnRegisterTileTypeChangedCallback(Action<Tile> callback) {
        cbTileChangedDesignation -= callback;
    }


    public void SetDesignationMode(bool isDesignating) {
        areWeDesignating = isDesignating;
        
    }

    internal void AddToDesignation(Tile t) {
        tiles.Add(t);
       
    }
}
