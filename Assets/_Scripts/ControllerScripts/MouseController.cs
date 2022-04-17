using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour
{
    
    [SerializeField] private GameObject mouseCursorPrefab;
    private Vector2 _lastFrameMousePos;
    private Vector2 _dragStartPos;
    private Vector2 _currentFrameMousePos;

    private List<GameObject> _buildingHintList;
    private Camera _mainCamera;

    private void Awake()
    {
        _buildingHintList = new List<GameObject>();
        _mainCamera = Camera.main;
    }


    public Vector3 GetMousePosition() {
        return _currentFrameMousePos;
    }

    public Tile GetTileUnderMouse() {
        return WorldController.Instance.World.GetTileAt((int)_currentFrameMousePos.x, (int)_currentFrameMousePos.y);
    }
    private void Update()
    {
        _currentFrameMousePos = _mainCamera.ScreenToWorldPoint((Input.mousePosition));

        //MouseSnapToGrid();

        HandleMouseSelection();

        HandleMouseMovement();
        // Save it so we may use it if we moved the camera.
        _lastFrameMousePos = _mainCamera.ScreenToWorldPoint((Input.mousePosition));
    }

    private void HandleMouseSelection()
    {
        // Don't do if over UI element
        if (EventSystem.current.IsPointerOverGameObject()) { return; }


        // Start dragging left
        if (Input.GetMouseButtonDown(0))
        {
            _dragStartPos = _currentFrameMousePos;

        }

        var startX = Mathf.FloorToInt(_dragStartPos.x);
        var endX = Mathf.FloorToInt(_currentFrameMousePos.x);
        if (endX < startX) // swaps end and start if dragged the other way
        {
            (endX, startX) = (startX, endX);
        }
        var startY = Mathf.FloorToInt(_dragStartPos.y);
        var endY = Mathf.FloorToInt(_currentFrameMousePos.y);
        if (endY < startY)
        {
            (endY, startY) = (startY, endY);
        }

        // This makes it so our building hints resize as we move mouse
        while (_buildingHintList.Count > 0)
        {
            GameObject go = _buildingHintList[0];
            _buildingHintList.RemoveAt(0);
            EasyPooling.Despawn(go);
        }

        if (Input.GetMouseButton(0))
        {
            for (var x = startX; x <= endX; x++)
            {
                for (var y = startY; y <= endY; y++)
                {
                    Tile t = WorldController.Instance.World.GetTileAt(x, y);
                    if (t == null) continue;
                    
                    // Display the building hint on top of tile.
                    GameObject go = (GameObject)EasyPooling.Spawn(mouseCursorPrefab,
                        new Vector2(x, y), Quaternion.identity);
                    go.transform.SetParent(this.transform, true);
                    _buildingHintList.Add(go);
                }
            }
        }

        // Release dragging Left click
        if (Input.GetMouseButtonUp(0))
        {
            BuildModeController bmc = GameObject.FindObjectOfType<BuildModeController>();
            DesignationController dsc = GameObject.FindObjectOfType<DesignationController>();

            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    Tile t = WorldController.Instance.World.GetTileAt(x, y);

                    if (t != null)
                    {
                        // Call buildmode controller DoBuild(t).
                        if (bmc.areWeBuilding) {
                            bmc.DoBuild(t);
                        }
                        if (dsc.areWeDesignating) {
                            dsc.AddToDesignation(t);
                        }
                        

                    }
                }
            }
            if (dsc.areWeDesignating) {
                dsc.DoDesignate();
            }
            
        }
    }

    private void HandleMouseMovement()
    {
        // Handle the mouse movement.
        if (Input.GetMouseButton(2) || Input.GetMouseButton(1)) // right or middle mouse button
        {
            Vector2 diff = _lastFrameMousePos - _currentFrameMousePos;
            _mainCamera.transform.Translate(diff);
        }

        // Handle mouse zoom and pan ( multiplying with itself so it gives a way better feel)

        //TODO: These are expensive calls.
        
        _mainCamera.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * _mainCamera.orthographicSize;
        Mathf.Clamp(_mainCamera.orthographicSize, 3f, 20f);

    }

}
