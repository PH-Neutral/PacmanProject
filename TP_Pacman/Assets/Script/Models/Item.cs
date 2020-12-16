using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {
    public ItemType type = ItemType.PacGum;
    public int pointValue = 1;
}

public enum ItemType {
    PacGum, Fruit, SuperPacGum
}
