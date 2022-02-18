using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Texture2D gameCursor;
    [SerializeField] Canvas UiCanvas;

    bool onMineMode = false;
    bool onBuildMode = false;

    [SerializeField] PerlinBasedGridCreator gridCreator;

    

    void Start()
    {
        Cursor.SetCursor(gameCursor, Vector2.zero, CursorMode.Auto);

    }

    private void Update()
    {
        if (onMineMode)
        {

            //do mine mode stuff
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                if (hit.collider == null)
                {
                    Debug.Log("Null collider");

                } else if (hit.collider.gameObject.CompareTag(PerlinBasedGridCreator.BUILDRESTILE))
                {
                    
                    Instantiate(gridCreator.spaceVoidTile, hit.collider.transform.position , Quaternion.identity, gridCreator.transform);
                    hit.collider.gameObject.SetActive(false);
                }

            }
        }else if (onBuildMode)
        {


            //do build mode stuff


        }
        
    }

    public void MineTileMode()
    {
        onMineMode = true;
    }
    public void BuildTileMode()
    {
        onBuildMode = true;
    }
    public void CancelModes()
    {
        onBuildMode = false;
        onMineMode = false;
    }
}
