using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] private int currentStage;
    public int GetCurrentStage() => currentStage;
    private GameObject currentMap;
    private bool haveAnomaly;
    public bool GetHaveAnomaly() => haveAnomaly;
    private GameObject player;

    private MapController mc;
    private PlayerController pc;
    private CameraController cc;

    private PlayerInformation pi;

    private LandscapeManager landscapeManager;

    private bool Test => mc.test;

    private void Awake()
    {
        InitializeVariables();
    }

    private void InitializeVariables()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        pc = player.GetComponent<PlayerController>();
        cc = FindObjectOfType<CameraController>().GetComponent<CameraController>();
        mc = FindObjectOfType<MapController>().GetComponent<MapController>();
        pi = player.GetComponent<PlayerInformation>();
        currentMap = GameObject.FindGameObjectWithTag("Map");
        landscapeManager = FindObjectOfType<LandscapeManager>().GetComponent<LandscapeManager>();
    }

    public void GameStart()
    {
        currentStage = 0;
        pc.Initialize();
        cc.Initialize();
        mc.FillAnomaly();
        GameManager.GetInstance().Play();
        InitializeStage(currentStage);
    }

    public void InitializeStage(int stage)
    {
        if (stage == 7)
        {
            GameClear();
            return;
        }
        this.currentStage = stage;

        if (!Test && (stage == 0 || Random.Range(0f, 1f) > 0.5))
            haveAnomaly = false;
        else
            haveAnomaly = true;

        // Reset UI
        GameManager.GetInstance().um.HideEverything();
        // Reset Player position and Information
        player.transform.position = new Vector3(-19.5f, 1.2f, -5.45f);
        pi.Initialize();
        // Create new stage map and inform player about it is hard anomaly or not
        bool hard = mc.GenerateMap(haveAnomaly, stage);
        pc.SetAnomalyType(hard);
        // Set time
        ToggleActionAvailability(true);
        landscapeManager.ChangeLandscape(stage);

        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ToggleActionAvailability(bool available)
    {
        pc.SetSleep(available);
        pc.SetMove(available);
        cc.SetInteract(available);
    }
    private void GameClear()
    {
        //DisableControllers();
        GameManager.GetInstance().Clear();
        GameManager.GetInstance().um.ShowStateUI(GameState.GameClear);
    }

    public void GameOver() {
        GameManager.GetInstance().GameOver();
        GameManager.GetInstance().um.ShowStateUI(GameState.GameOver);
    }
    public void HandleSleepOutcome(bool sleep)
    {
        if (sleep ^ haveAnomaly)
            Succeed();
        else
            Fail();
    }

    private void Succeed()
    {
        // TODO: Animation
        InitializeStage(++currentStage);
    }

    private void Fail()
    {
        // TODO: Animation
        if (currentStage > 1) currentStage--;
        InitializeStage(currentStage);
    }
}
