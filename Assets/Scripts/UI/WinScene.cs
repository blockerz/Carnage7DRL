using lofi.RLCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class WinScene : BaseScene
{
    [SerializeField]
    public GameState gameState;

    [SerializeField]
    public TextMeshProUGUI scoreText;

    [SerializeField]
    public TextMeshProUGUI highScoreText;

    float time;

    protected override void Start()
    {
        base.Start();
        time = 0;
        scoreText.text = "Score: " + gameState.gameScore;
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
            ExitButton();
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            ExitButton();
        }

    }

    public void ExitButton()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

}
