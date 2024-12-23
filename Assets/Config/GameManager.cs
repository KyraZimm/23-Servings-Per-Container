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
    private GameObject terrainRoot;
    private GameObject playerRoot;

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

        //init necessary prefabs
        terrainRoot = GameObject.Instantiate(config.Settings.TerrainPrefab, Vector3.zero, Quaternion.identity);
        playerRoot = GameObject.Instantiate(config.Settings.PlayerPrefab, config.Settings.PlayerSpawn, Quaternion.identity);

        //init player on reliant scripts
        //NOTE: once player mechanics are settled, this will prob be resolved by having a universal static ref for the player
        CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
        cam.SetTarget(playerRoot.transform);

        thwomp[] thwomps = GameObject.FindObjectsOfType<thwomp>();
        foreach (thwomp t in thwomps) t.SetPlayer(playerRoot.transform);
    }

    private void Update() {
        AudioManager.Update();

        //disabled during prototyping phase. re-enable during dev phase
        /*if (tutorialMode) {
            CheckForTutorialEnd();
        }*/
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
