using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MouseOverInfoToolRoom : MonoBehaviour
{
    // Check to see what's under my mouse
    // update getcomponent text.text parameter

    Text myText;
    MouseController mouseController;

    private void Start() {
        myText = GetComponent<Text>();

        if(myText == null) {
            Debug.Log("No text component");
            this.enabled = false;
            return;
        }

        mouseController = GameObject.FindObjectOfType<MouseController>();
        if (mouseController == null) {
            Debug.Log("How tf do we not have instance of controller ??");
        }
    }
    private void Update()
    {
        Tile t = mouseController.GetTileUnderMouse();
        if( t == null)
        {
            return;
        }

        myText.text = "Room Index: " + t.World.Rooms.IndexOf(t.Room).ToString();



    }
}
