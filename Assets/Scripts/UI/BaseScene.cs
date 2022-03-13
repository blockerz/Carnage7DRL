using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseScene : MonoBehaviour
{

    protected virtual void Start()
    {
        Application.targetFrameRate = 60;
    }

    protected virtual void Update()
    {
        
    }
}
