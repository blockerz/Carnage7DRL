using lofi.RLCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class HelpScene : BaseScene
{

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();        

        if (Input.GetKey(KeyCode.Return))
        {
            MainScene();
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            MainScene();
        }
    }


    public void MainScene()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

}
