using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StationController : MonoBehaviour
{

    //get min x min y coordinates of designation
    //get max x max y coordinates of designation
    //check for that designation type's minimum requirements
    //some examples would be
    /* enclosed with walls and doors (airtight)
     * is pressurized (has gas resource exhaust)
     * A x B minimum size (ex:2x3)
     * if it's for a specialized job (captain, trade hub, storage) that job's specialized equipment must be present
     * left and drag to create designation, right and drag to remove designation, has the name of designation written
     * if minimum not met it's in gray tones if met get the color it's supposed to have
     * 
     */


    // high level dependencies
    private GameManager manager;
    private PerlinBasedGridCreator gridCreator;

    // script related
    private Vector2 startPos;
    private int selectedDesignationType = 0; // 0 = null , 1 = personal room , 2 = storage, 3 = rec room, 4 = cafeteria, 5 = kitchen
    private List<GameObject> selectedTiles = new List<GameObject>();
    private List<GameObject> floorTiles = new List<GameObject>();


    // get from outside
    [SerializeField] GameObject designationSelector;
    
    private RectTransform selectionBox;
    private Image selectionImage;
    private void Awake()
    {
        manager = gameObject.GetComponent<GameManager>();
        gridCreator = manager.gridCreator;
        selectionBox = designationSelector.GetComponent<RectTransform>();
        selectionImage = designationSelector.GetComponent<Image>();
    }
    private void Update()
    {
        //designation selector bir designation secildikten sonra secim yapmamizi saglayacak olan Ui elementi.
        //bu elementin widht height start pos gibi pozisyonlarinin dunya pozisyonlari int olarak alindiginda
        //tile gridde olmasi gereken yeri verecektir.
        //konudan bagimsiz hangi designationun secildigi infosu lazim ki ona gore islemler yapilsin
        //(isim yazma minimum gereksinimleri belirleme ve kontrol etmek

        //this shit will work in screen positions not particularly enjoying that
        if (designationSelector.activeInHierarchy)
        {
                if (Input.GetMouseButtonDown(0)) // left click
                {
                    startPos = Input.mousePosition;
                }
                if (Input.GetMouseButtonUp(0)) // left release
                {
                    ReleaseSelectionBox();
                }
                if (Input.GetMouseButton(0)) // left hold down
                {
                    UpdateSelectionBox(Input.mousePosition);
                }
        }
    }

    void UpdateSelectionBox(Vector2 currentPos)
    {
            if (designationSelector.activeInHierarchy) //mine button activates and if active does the size calculations.
            {
                selectionImage.enabled = true;
                float width = currentPos.x - startPos.x;
                float height = currentPos.y - startPos.y;

                selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));

                selectionBox.anchoredPosition = startPos + new Vector2(width / 2, height / 2);

            }
    }
        
    void ReleaseSelectionBox()
    {

        floorTiles = manager.floorTiles;

        selectionImage.enabled = false;

        Vector2 min = selectionBox.anchoredPosition - (selectionBox.sizeDelta / 2);
        Vector2 max = selectionBox.anchoredPosition + (selectionBox.sizeDelta / 2);

        foreach (GameObject tiles in floorTiles)
        {
            if (tiles != null)
            {
                Vector2 screenPos = Camera.main.WorldToScreenPoint(tiles.transform.position);

                if (screenPos.x > min.x && screenPos.x < max.x && screenPos.y > min.y && screenPos.y < max.y)
                {
                    selectedTiles.Add(tiles);
                    Debug.Log("A");
                }
            }
        }
        SetTileDesignation(selectedDesignationType, selectedTiles);

    }

    private void SetTileDesignation(int selectedDesignationType, List<GameObject> selectedTiles)
    {
        foreach ( GameObject tile in selectedTiles)
        {
            tile.GetComponentInChildren<SpriteRenderer>().color = Color.cyan;
            Debug.Log(tile.transform.position);
        }

    }
    public void DesignationSet(int designationValue)
    {
        selectedDesignationType = designationValue;
    }

    



}
