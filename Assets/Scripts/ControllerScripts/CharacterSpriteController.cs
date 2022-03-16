using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpriteController : MonoBehaviour
{
    List<GameObject> characterGameObjectList;
    Dictionary<string, Sprite> characterSprites;

    Action<GameObject> cbOnCharacterReadyForAI;

    World world {
        get { return WorldController.Instance.world; }
    }

    private void Start() {
        LoadSprites();

        characterGameObjectList = new List<GameObject>();

        // This basically registers a call back so when the
        // callback we gave to register gets called it actually on character created.
        world.RegisterCharacterCreated(OnCharacterCreated);

        //TODO: Fix place like why would we want it in the middle of the world ?
        Character c = world.CreateCharacter(world.GetTileAt((world.width / 2),(world.height / 2)));
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
        character.transform.position = new Vector2(characterScript.currTile.x + 0.5f, characterScript.currTile.y +0.5f);
        character.transform.SetParent(this.transform, true);

        // TODO: Better sprite?
        character.AddComponent<SpriteRenderer>().sprite = characterSprites["character_0003"];

        character.GetComponent<SpriteRenderer>().sortingLayerName = "Characters";

        // TODO: We might want to move this to elsewhere ?
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
