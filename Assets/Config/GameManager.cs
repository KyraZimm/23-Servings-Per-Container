using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private ConfigFile config;

    private void Awake() {
        //singleton initializtion
        if (Instance != null){
            Debug.LogWarning($"A later instance of {nameof(GameManager)} on {gameObject.name} was destroyed to preserve an earlier instance on {Instance.gameObject.name}.");
            DestroyImmediate(this);
            return;
        }
        Instance = this;
    }

    private void Start() {

        //initialize static classes using current config file
        AudioManager.Init(config);
    }

    private void Update() {
        AudioManager.Update();
    }
}
