using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// LooseObjects are things that are lying on the floor/stockpiles, like a bunch
// of metal bars or potentially a non-installed copy of a furniture.
public class Inventory {
    string objectType; // like "Steel Plate"
    int maxStackSize;  // how much could be in there
    int stackSize; // how much is there

}