using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionController : MonoBehaviour
{
    [SerializeField]GameManager manager;

    RectTransform selectionBox;
    private Vector2 startPos;

    List<GameObject> selectedTiles = new List<GameObject>();
    List<GameObject> minableTiles = new List<GameObject>();

    Image selectionImage;
    

    private void Start()
    {
        minableTiles = manager.minableTiles;
        selectionBox = gameObject.GetComponent<RectTransform>();
        selectionImage = gameObject.GetComponent<Image>();
    }
    private void Update()
    {

        //this shit will work in screen positions not particularly enjoying that
        if (selectionBox.gameObject.activeInHierarchy)
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
        if (selectionBox.gameObject.activeInHierarchy) //mine button activates and if active does the size calculations.
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
        selectionBox.gameObject.SetActive(false);
        selectionImage.enabled = false;

        Vector2 min = selectionBox.anchoredPosition - (selectionBox.sizeDelta / 2);
        Vector2 max = selectionBox.anchoredPosition + (selectionBox.sizeDelta / 2);

        foreach  (GameObject tiles in minableTiles)
        {
            if(tiles != null)
            {
                Vector2 screenPos = Camera.main.WorldToScreenPoint(tiles.transform.position);

                if (screenPos.x > min.x && screenPos.x < max.x && screenPos.y > min.y && screenPos.y < max.y)
                {
                    selectedTiles.Add(tiles);
                }
            }
            

        }
        MineSelectedTiles(selectedTiles);
        foreach (GameObject tile in selectedTiles)
        {
            minableTiles.Remove(tile);
            
        }

        

    }

    private void MineSelectedTiles(List<GameObject> selectedTiles)
    {
        manager.MineTiles(selectedTiles);
        //clear selected tiles list

    }
    
}
