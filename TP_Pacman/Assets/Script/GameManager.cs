using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance = null;

    public Player player;
    public List<Ghost> ghosts;

    public int RemainingBalls {
        get; set;
    }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        /*for (int i=0; i<4; i++) {
            Ghost ghost = Instantiate(prefabGhost)
            ghosts.Add()
        }*/
        foreach(Ghost g in ghosts) {
            g.target = player;
            Debug.Log("Ghosts targets assigned!");
        }
    }
}
