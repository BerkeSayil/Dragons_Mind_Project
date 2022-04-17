using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DesignationController : MonoBehaviour
{
    private Designation.DesignationType _designationModeType = Designation.DesignationType.None;
    public bool areWeDesignating = false;

    private Dictionary<string, Designation.DesignationType> _designationTypes;
    private Action<Tile> _cbTileChangedDesignation;
    
    private World _world;
    private Dictionary<Designation, GameObject> _designationGameObjectMap = new Dictionary<Designation, GameObject>();

    private List<Tile> _tiles;
    private void Start() {
        LoadDesignations();
        _tiles = new List<Tile>();

        _world = WorldController.Instance.World;
        _world.RegisterFurnitureCreated(UpdateDesignationFurns);
        _world.RegisterDesignationChanged(UpdateDesignation);
    }

    public void UpdateDesignationFurns(Furniture furn) {

        foreach (Designation designation in _world.Designations) {

            designation.UpdateDesignationFurnitures();
            if (designation.IsFunctional()) {

            }
        }
    }
    private void MakeDesignationObject(Designation designation) {

        GameObject designationGo = new GameObject(" " + designation.Type);
        designationGo.transform.parent = gameObject.transform;

        designationGo.transform.position = new Vector2(designation.CenterX, designation.CenterY);
        
        
        foreach(Tile t in _tiles) {
            t.DesignationType = designation.Type;

            _cbTileChangedDesignation?.Invoke(t);
        }

        _designationGameObjectMap.Add(designation, designationGo);

        if (designation.IsFunctional()) return;

        designationGo.SetActive(false);



    }

    private void UpdateDesignation(Designation d) {

        if(_designationGameObjectMap.ContainsKey(d) == false) {
            Debug.Log("You are searching for a non existing designation but it wasn't removed from this dictionary probably ?");
            return;
        }
        if (_designationGameObjectMap[d].activeInHierarchy == false) {
            if (d.IsFunctional()) {
                _designationGameObjectMap[d].SetActive(true);
            }
        }
       
    }

    public void SetDesignationMode(string designationType) {

        if(_designationTypes.ContainsKey(designationType) == false) {
            Debug.Log("You are searching for a non existing designation type! ~ did you make a mistake on buttons? ");
            return;
        }
        _designationModeType = _designationTypes[designationType];

        _tiles.Clear();
    }

    private void LoadDesignations() {
        // Getting installed objects from the resources folder

        _designationTypes = new Dictionary<string, Designation.DesignationType>();

        foreach (Designation.DesignationType dt in (Designation.DesignationType[])Enum.GetValues(typeof(Designation.DesignationType))) {
            _designationTypes[dt.ToString()] = dt;
        }
    }

    public void DoDesignate() {

        Designation designation = new Designation(_tiles, _designationModeType) ;

        bool isDesignationValid = designation.IsValidDesignation(_tiles);

        if (isDesignationValid && designation.Type != Designation.DesignationType.None) {
            MakeDesignationObject(designation);
        }
        else {
            designation.DestroyDesignation();
        }

        _tiles.Clear(); // clear the list so designations don't get mixed up

    }
    public void RemoveDesignation() {
        //TODO:Get a way to remove
    }

    public void RegisterTileDesignationChangedCallback(Action<Tile> callback) {
        // the place where this gets called is subscribed to the cbTileDesignationChanged
        // so when cbTileDesignationChanged Action gets called in return this function callsback to
        // original script where its called.
        _cbTileChangedDesignation += callback;
    }
    public void UnRegisterTileTypeChangedCallback(Action<Tile> callback) {
        _cbTileChangedDesignation -= callback;
    }


    public void SetDesignationMode(bool isDesignating) {
        areWeDesignating = isDesignating;
        
    }

    internal void AddToDesignation(Tile t) {
        _tiles.Add(t);
       
    }
}
