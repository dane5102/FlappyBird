using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    public static GameHandler Instance;
    public delegate void GameDelegate();
    public static event GameDelegate OnGameStart;
    public static event GameDelegate OnGameEnd;

    public GameObject start;
    public GameObject gameOver;
    public GameObject countdown;
    public Text score;

    enum PageState
    {
        None,
        Start,
        GameOver,
        Countdown
    }

    int gameScore = 0;
    bool gameEnded = true;

    public bool GameEnded { get { return gameEnded; } }
    public int Score { get { return gameScore; } }

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        Countdown.OnCountdownFinished += OnCountdownFinished;
        PlayerController.OnPlayerDied += OnPlayerDied;
        PlayerController.OnPlayerScored += OnPlayerScored;

    }

    void OnDisable()
    {
        Countdown.OnCountdownFinished -= OnCountdownFinished;
        PlayerController.OnPlayerDied -= OnPlayerDied;
        PlayerController.OnPlayerScored -= OnPlayerScored;
    }

    void OnCountdownFinished()
    {
        SetPageState(PageState.None);
        OnGameStart();
        gameScore = 0;
        gameEnded = false;
    }

    void OnPlayerDied()
    {
        gameEnded = true;
        int savedScore = PlayerPrefs.GetInt("HighScore");
        if (gameScore > savedScore)
        {
            PlayerPrefs.SetInt("HighScore", gameScore);
        }
        SetPageState(PageState.GameOver);
    }

    void OnPlayerScored()
    {
        gameScore++;
        score.text = gameScore.ToString();
    }

    void SetPageState(PageState state)
    {
        switch (state)
        {
            case PageState.None:
                start.SetActive(false);
                gameOver.SetActive(false);
                countdown.SetActive(false);
                break;

            case PageState.Start:
                start.SetActive(true);
                gameOver.SetActive(false);
                countdown.SetActive(false);
                break;

            case PageState.GameOver:
                start.SetActive(false);
                gameOver.SetActive(true);
                countdown.SetActive(false);
                break;

            case PageState.Countdown:
                start.SetActive(false);
                gameOver.SetActive(false);
                countdown.SetActive(true);
                break;
        }
    }

    // when player hits restart
    public void ConfirmGameEnded()
    {
        OnGameEnd(); // event sent to Player Controller
        score.text = "0";
        SetPageState(PageState.Start);
    }

    // when player hits play
    public void StartGame()
    {
        SetPageState(PageState.Countdown);
    }
}
