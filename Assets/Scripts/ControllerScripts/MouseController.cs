using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour
{
    
    [SerializeField] GameObject mouseCursorPrefab;
    Vector2 lastFrameMousePos;
    Vector2 dragStartPos;
    Vector2 currentFrameMousePos;

    List<GameObject> buildingHintList;

    Tile.TileType buildModeTile = Tile.TileType.Floor;
    string buildModeObjectType;
    bool buildModeIsObjects = false;


    private void Start()
    {
        buildingHintList = new List<GameObject>();
    }

    private void Update()
    {
        currentFrameMousePos = Camera.main.ScreenToWorldPoint((Input.mousePosition));

        //MouseSnapToGrid();

        HandleMouseSelection();

        HandleMouseMovement();
        // Save it so we may use it if we moved the camera.
        lastFrameMousePos = Camera.main.ScreenToWorldPoint((Input.mousePosition));
    }

    private void HandleMouseSelection()
    {
        // Don't do if over UI element
        if (EventSystem.current.IsPointerOverGameObject()) { return; }


        // Start dragging left
        if (Input.GetMouseButtonDown(0))
        {
            dragStartPos = currentFrameMousePos;
        }

        int startX = Mathf.FloorToInt(dragStartPos.x);
        int endX = Mathf.FloorToInt(currentFrameMousePos.x);
        if (endX < startX) // swaps end and start if dragged the other way
        {
            int temp = endX;
            endX = startX;
            startX = temp;
        }
        int startY = Mathf.FloorToInt(dragStartPos.y);
        int endY = Mathf.FloorToInt(currentFrameMousePos.y);
        if (endY < startY)
        {
            int temp = endY;
            endY = startY;
            startY = temp;
        }
        // This makes it so our building hints resize as we move mouse
        while (buildingHintList.Count > 0)
        {
            GameObject go = buildingHintList[0];
            buildingHintList.RemoveAt(0);
            EasyPooling.Despawn(go);
        }


        if (Input.GetMouseButton(0))
        {
            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    Tile t = WorldController.Instance.world.GetTileAt(x, y);
                    if (t != null)
                    {
                        // Display the building hint on top of tile.
                        GameObject go = (GameObject)EasyPooling.Spawn(mouseCursorPrefab,
                            new Vector2(x, y), Quaternion.identity);
                        go.transform.SetParent(this.transform, true);
                        buildingHintList.Add(go);
                    }
                }
            }
        }

        // Release dragging Left click
        if (Input.GetMouseButtonUp(0))
        {
            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    Tile t = WorldController.Instance.world.GetTileAt(x, y);
                    if (t != null)
                    {
                        if (buildModeIsObjects)
                        {
                            // Create the installed objects on the tile given. And assign.

                            // Furniture type here exist in the scope so it prevents furniture changing mid build
                            // you remember that problem... yeah this is the fix.

                            string furnitureType = buildModeObjectType;

                            // We check if it's a valid position and if so
                            // create a job and queue it for something else to come and do.
                            if (WorldController.Instance.world.IsFurniturePlacementValid(furnitureType, t)
                                && t.pendingFurnitureJob == null

                                ){
                                Job j = new Job(t, (theJob) =>
                                {
                                    WorldController.Instance.world.
                                    PlaceFurnitureAt(furnitureType, theJob.tile);
                                    t.pendingFurnitureJob = null;
                                });

                                // TODO: This being this way very easy to clear or forget make it automated in
                                // some other way possible
                                t.pendingFurnitureJob = j;
                                j.RegisterJobCancelCallback((theJob) => { theJob.tile.pendingFurnitureJob = null; });

                                WorldController.Instance.world.jobQueue.Enqueue(j);
                            } 
                        } 
                        else
                        {
                            t.Type = buildModeTile;
                        }
                    }
                }
            }
        }
    }

    private void HandleMouseMovement()
    {
        // Handle the mouse movement.
        if (Input.GetMouseButton(2) || Input.GetMouseButton(1)) // right or middle mouse button
        {
            Vector2 diff = lastFrameMousePos - currentFrameMousePos;
            Camera.main.transform.Translate(diff);
        }

        // Handle mouse zoom and pan ( multiplying with itself so it gives a way better feel)
        Camera.main.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * Camera.main.orthographicSize;
        Mathf.Clamp(Camera.main.orthographicSize, 3f, 20f);

    }

    public void SetModeBuildFloor()
    {
        buildModeIsObjects = false;
        buildModeTile = Tile.TileType.Floor;
    }
    public void SetModeMineMode()
    {
        buildModeIsObjects = false;
        buildModeTile = Tile.TileType.Empty;

    }
    public void SetModeBuildInstalledObject( string objectType)
    {
        buildModeIsObjects = true;
        buildModeObjectType = objectType;

    }

}
