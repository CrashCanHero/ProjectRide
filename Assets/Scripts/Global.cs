using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour {
    public static Global Instance;
    public List<PlayerManager> Players = new List<PlayerManager>();
    public List<WarpStar> Stars = new List<WarpStar>();
    public float Gravity;
    public float GlobalStarWeight;
    public float StarGetOffWaitTime;

    public void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            DestroyImmediate(gameObject);
        }
    }

    public void OnEnable() {
        GameEvents.OnStarSpawned += StarSpawned;
        GameEvents.OnStarDespawned += StarDespawned;
    }

    public void OnDisable() {
        GameEvents.OnStarSpawned -= StarSpawned;
        GameEvents.OnStarDespawned -= StarDespawned;
    }

    public void StarSpawned(WarpStar star, Collider col) {
        Stars.Add(star);
    }

    public void StarDespawned(WarpStar star, Collider col) {
        Stars.Remove(star);
    }
}

public class GameEvents {
    public delegate void OnStarSpawnedEvent(WarpStar star, Collider col);
    public delegate void OnStarDespawnedEvent(WarpStar star, Collider col);

    public static event OnStarSpawnedEvent OnStarSpawned;
    public static event OnStarDespawnedEvent OnStarDespawned;

    public static void StarSpawned(WarpStar star, Collider col) {
        OnStarSpawned?.Invoke(star, col);
    }

    public static void StarDespawned(WarpStar star, Collider col) {
        OnStarDespawned?.Invoke(star, col);
    }

}