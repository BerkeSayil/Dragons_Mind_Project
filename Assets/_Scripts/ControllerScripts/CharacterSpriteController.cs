using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpriteController : MonoBehaviour
{
    List<GameObject> characterGameObjectList;
    Dictionary<string, Sprite> characterSprites;

    Action<GameObject> cbOnCharacterReadyForAI;

    public string jobType { get; protected set; }

    World world {
        get { return WorldController.Instance.world; }
    }

    private void Start() {
        LoadSprites();

        characterGameObjectList = new List<GameObject>();

        // This basically registers a call back so when the
        // callback we gave to register gets called it actually on character created.
        world.RegisterCharacterCreated(OnCharacterCreated);

    }

    public void SpawnCharacter(string jobType) { //TODO: Make this also understand what job a character should be and do so accordingly

        Character c = world.CreateCharacter(world.GetTileAt((world.width / 2), (world.height / 2)));

    }

    private void LoadSprites() {
        // Getting installed objects from the resources folder

        characterSprites = new Dictionary<string, Sprite>();

        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprite");
        foreach (Sprite s in sprites) {
            characterSprites[s.name] = s;
        }
    }

    public void OnCharacterCreated(GameObject character) {
        

        characterGameObjectList.Add(character);
        Character characterScript = character.GetComponent<Character>();

        character.name = "Character";
        character.transform.position = new Vector2(characterScript.CurrTile.x + 0.5f, characterScript.CurrTile.y +0.5f);
        character.transform.SetParent(this.transform, true);

        // TODO: change sprite based on occupation
        character.AddComponent<SpriteRenderer>().sprite = characterSprites["character_0003"];

        character.GetComponent<SpriteRenderer>().sortingLayerName = "Characters";

        CircleCollider2D collider = character.AddComponent<CircleCollider2D>();

        collider.radius = 0.45f; // to ensure its just enough smaller than a tile.
                                 // Floor sorting laye ensures our character is on top.


        // This signals to AstarController that character is crated and ready for the AI parts.
        if (cbOnCharacterReadyForAI != null) {
            cbOnCharacterReadyForAI(character);
        }

    }
    

    public void RegisterCharacterReadyForAI(Action<GameObject> callbackFunc) {
        cbOnCharacterReadyForAI += callbackFunc;
    }
    public void UnregisterCharacterReadyForAI(Action<GameObject> callbackFunc) {
        cbOnCharacterReadyForAI -= callbackFunc;
    }

}
