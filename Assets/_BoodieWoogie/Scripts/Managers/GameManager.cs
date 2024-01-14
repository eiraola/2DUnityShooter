using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    public PlayerInput playerInput;
    public static GameManager Instance { get; private set; }
    private bool isStoped = false;
    public bool IsStoped { get => isStoped;}
   

    private void Awake()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 90;
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    private void OnEnable()
    {
        if (playerInput)
        {
            //playerInput.OnStartEvent += HandleStop;
        }
    }
    private void OnDisable()
    {
        if (playerInput)
        {
            //playerInput.OnStartEvent -= HandleStop;
        }
    }
    public void HandleStop()
    {
        if (IsStoped)
        {
            RunGame();
            return;
        }
        StopGame();
    }
    public void StopGame()
    {
        isStoped = true;
        Time.timeScale = 0.0f;
    }
    public void RunGame()
    {
        isStoped = false;
        Time.timeScale = 1.0f;
    }
}
