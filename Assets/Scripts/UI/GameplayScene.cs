using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using lofi.RLCore;
using UnityEngine.UI;

public class GameplayScene : BaseScene
{
    [SerializeField]
    public TextMeshProUGUI stageText;
    
    [SerializeField]
    public TextMeshProUGUI scoreText;
    
    [SerializeField]
    public TextMeshProUGUI highScoreText;
    
    [SerializeField]
    public TextMeshProUGUI healthText;
    
    [SerializeField]
    public TextMeshProUGUI gearText;    

    [SerializeField]
    public TextMeshProUGUI shotgunText;

    [SerializeField]
    public TextMeshProUGUI gameOverText;

    [SerializeField]
    public Image shifterStick;

    [SerializeField]
    public GameState gameState;

    private bool highGear;

    protected override void Start()
    {
        base.Start();

        stageText.text = "Stage: " + (gameState.gameStage + 1);

        if (gameState.player != null && gameState.player.CurrentSpeed == 2)
        {
            highGear = true;
        }
        else
        {
            highGear = false;
        }

    }

    protected override void Update()
    {
        base.Update();

        if (gameState.gameScore > gameState.highScore)
        {
            gameState.highScore = gameState.gameScore;
            PlayerPrefs.SetInt(Constants.HIGH_SCORE_KEY, gameState.highScore);
        }

        scoreText.text = "Score: " + (gameState.gameScore);
        highScoreText.text = "High Score: " + (gameState.highScore);

        if (gameState.player != null)
        {

            healthText.text = "X " + gameState.player.Health;
            gearText.text = "Gear: " + ((gameState.player.CurrentSpeed == 2) ? "High" : "Low");
            shotgunText.text = "X " + gameState.player.ShotgunAmmo;

            if (highGear && gameState.player.CurrentSpeed == 1)
            {
                shifterStick.transform.eulerAngles = new Vector3(0, 0, 180);
                highGear = false;
            }
            else if (!highGear && gameState.player.CurrentSpeed == 2)
            {
                shifterStick.transform.eulerAngles = new Vector3(0, 0, 0);
                highGear = true;
            }
        }

        if (gameState.gameOver)
        {
            gameOverText.gameObject.SetActive(true);
            StartCoroutine(DelayedSceneChange("MainMenuScene", 3f));
        }

        if (gameState.gameWon)
        {
            StartCoroutine(DelayedSceneChange("WinScene", 1f));
        }

        
        //if (Input.GetKey(KeyCode.Return))
        //{
        //    OnStartButtonClick();
        //}

        if (Input.GetKey(KeyCode.Escape))
        {
            ExitButton();
        }

        if (Input.GetKey(KeyCode.Question) || Input.GetKey(KeyCode.F1))
        {
            HelpButton();
        }
    }

    private IEnumerator DelayedSceneChange(string scene, float delay = 0f)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        SceneManager.LoadScene(scene);
    }

    //public void OnStartButtonClick()
    //{
    //    SceneManager.LoadScene("GameScene");

    //}

    public void HelpButton()
    {
        SceneManager.LoadScene("HelpScene");
    }

    public void ExitButton()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

}