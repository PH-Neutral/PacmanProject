using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {
    public ItemType type = ItemType.Ball;
    public int pointValue = 1;
}

public enum ItemType {
    Ball, Bonus, PowerUp
}
