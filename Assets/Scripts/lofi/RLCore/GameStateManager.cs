using lofi.RLCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    [SerializeField]
    public GameState GameState;
    
    [SerializeField]
    public GameStateType thisState;

    void Start()
    {
        //GameState.lastState = GameState.currentState;
        GameState.currentState = thisState;
    }

    void Update()
    {
        
    }
}
