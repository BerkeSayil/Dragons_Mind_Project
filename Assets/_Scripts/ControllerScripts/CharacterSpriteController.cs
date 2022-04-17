using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpriteController : MonoBehaviour
{
    private List<GameObject> _characterGameObjectList;
    private Dictionary<string, Sprite> _characterSprites;

    private Action<GameObject> _cbOnCharacterReadyForAI;
    
    private static World World => WorldController.Instance.World;

    private void Start() {
        LoadSprites();

        _characterGameObjectList = new List<GameObject>();

        // This basically registers a call back so when the
        // callback we gave to register gets called it actually on character created.
        World.RegisterCharacterCreated(OnCharacterCreated);

    }

    public void SpawnCharacter(int jobType) { 

        Character c = null;

        switch (jobType) {
            case 0 : // construction worker
                if (World.Workers.Count < GameManager.Instance.NumOfWorkersConstruction) {
                    c = World.CreateCharacter(0, World.GetTileAt((int)(World.ShipTilePos.x), (int)(World.ShipTilePos.y)));
                }
                

                break;
            case 1: // engineer
                c = World.CreateCharacter(1, World.GetTileAt((int)(World.ShipTilePos.x), (int)(World.ShipTilePos.y)));

                break;
           
        }
        
    }

    private void LoadSprites() {
        // Getting installed objects from the resources folder

        _characterSprites = new Dictionary<string, Sprite>();

        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprite");
        foreach (Sprite s in sprites) {
            _characterSprites[s.name] = s;
        }
    }

    public void OnCharacterCreated(GameObject character) {
        

        _characterGameObjectList.Add(character);
        Character characterScript = character.GetComponent<Character>();

        character.name = "Character";
        character.transform.position = new Vector2(characterScript.CurrTile.x + 0.5f, characterScript.CurrTile.y +0.5f);
        character.transform.SetParent(this.transform, true);

        // TODO: change sprite based on occupation
        switch (character.tag) {
            case "Worker": // construction worker
                character.AddComponent<SpriteRenderer>().sprite = _characterSprites["workerCharacter"];

                break;
            case "Visitor": // visitor
                character.AddComponent<SpriteRenderer>().sprite = _characterSprites["visitorCharacter"];

                break;

        }
        

        character.GetComponent<SpriteRenderer>().sortingLayerName = "Characters";

        CircleCollider2D collider = character.AddComponent<CircleCollider2D>();

        collider.radius = 0.45f; // to ensure its just enough smaller than a tile.
                                 // Floor sorting layer ensures our character is on top.
                                 

        // This signals to A starController that character is crated and ready for the AI parts.
        _cbOnCharacterReadyForAI?.Invoke(character);

    }
    

    public void RegisterCharacterReadyForAI(Action<GameObject> callbackFunc) {
        _cbOnCharacterReadyForAI += callbackFunc;
    }
    public void UnregisterCharacterReadyForAI(Action<GameObject> callbackFunc) {
        _cbOnCharacterReadyForAI -= callbackFunc;
    }

}
