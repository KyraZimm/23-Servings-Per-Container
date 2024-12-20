using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private ConfigFile config;
    [SerializeField] private float startZoneLeftBound;
    [SerializeField] private float startZoneRightBound;
    [SerializeField] private AudioClip popGoesTheWeasel;

    private bool tutorialMode = true;

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
        if (tutorialMode) {
            CheckForTutorialEnd();
        }
    }

    private void CheckForTutorialEnd() {
        float playerX = Player.Instance.transform.position.x;
        float playerY = Player.Instance.transform.position.y;
        if (playerX > startZoneRightBound || playerY < startZoneLeftBound){
            tutorialMode = false;
            EndTutorial();
        }
    }

    private void EndTutorial(){
        CameraFollow mainCamera = Camera.main.GetComponent<CameraFollow>();
        if (mainCamera == null) {
            Debug.Log("Fuck");
        }
        mainCamera.SetToStationary(true);
        AudioManager.PlaySFX(popGoesTheWeasel);
    }
}
