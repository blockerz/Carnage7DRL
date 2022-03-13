using lofi.RLCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuScene : BaseScene
{
    [SerializeField]
    public GameState gameState;

    [SerializeField]
    public TMP_Dropdown stageSelect;
    
    [SerializeField]
    public TextMeshProUGUI highScoreText;

    int stage;
    float time;

    protected override void Start()
    {
        base.Start();
        stage = 0;
        time = 0;
        gameState.highScore = PlayerPrefs.GetInt(Constants.HIGH_SCORE_KEY);
        //stageSelect.AddOptions(new List<string>() { "0", "1", "2", "3", "4" });
    }

    protected override void Update()
    {
        base.Update();

        time += Time.deltaTime;

        if (Mathf.RoundToInt(time) % 2 == 1)
        {
            highScoreText.text = "";
        }
        else
        {
            highScoreText.text = "High Score: " + gameState.highScore;
        }

        if (Input.GetKey(KeyCode.Return))
        {
            OnStartButtonClick();
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            ExitButton();
        }

        if (Input.GetKey(KeyCode.Question) || Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.F1))
        {
            HelpButton();
        }
    }

    public void OnStartButtonClick()
    {
        //gameState.gameStage = 0;
        gameState.gameStage = stage;
        SceneManager.LoadScene("GameScene");

    }

    public void OnStageSelect()
    {
        stage = stageSelect.value;
    }

    public void HelpButton()
    {
        SceneManager.LoadScene("HelpScene");
    }

    public void ExitButton()
    {
        Application.Quit();
    }

}
