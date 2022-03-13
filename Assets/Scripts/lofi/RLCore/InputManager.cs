using lofi.RLCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public event EventHandler<OnPlayerMoveCommandEventArgs> OnPlayerMoveCommand;
    public class OnPlayerMoveCommandEventArgs : EventArgs
    {
        public Direction moveDirection;
    }
    
    public event EventHandler OnPlayerBrakeCommand;
    public event EventHandler OnPlayerShiftCommand;

    public event EventHandler OnPlayerShootCommand;
    public event EventHandler OnPlayerShotgunCommand;

    [SerializeField]
    public GameState gameState;

    private float inputDelay;
    //private Player player;

    void Start()
    {
        inputDelay = Constants.KEYBOARD_INPUT_DELAY;
        //player = gameState.player;
    }

    void Update()
    {
        inputDelay -= Time.deltaTime;


        if (inputDelay < 0f)
        {
            if (CheckKeyboardInput())
                inputDelay = Constants.KEYBOARD_INPUT_DELAY;
        }
    }

    private bool CheckKeyboardInput()
    {

        if (Input.GetKey(KeyCode.A))
        {
            TriggerPlayerMoveCommand(Direction.West);
            return true;
        }

        else if (Input.GetKey(KeyCode.D))
        {
            TriggerPlayerMoveCommand(Direction.East);
            return true;
        }

        else if (Input.GetKey(KeyCode.W))
        {
            TriggerPlayerMoveCommand(Direction.North);
            return true;
        }

        else if (Input.GetKey(KeyCode.S))
        {
            TriggerPlayerMoveCommand(Direction.South);
            return true;
        }

        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            TriggerPlayerMoveCommand(Direction.West);
            return true;
        }

        else if (Input.GetKey(KeyCode.RightArrow))
        {
            TriggerPlayerMoveCommand(Direction.East);
            return true;
        }

        else if (Input.GetKey(KeyCode.UpArrow))
        {
            TriggerPlayerMoveCommand(Direction.North);
            return true;
        }

        //else if (Input.GetKey(KeyCode.DownArrow))
        //{
        //    TriggerPlayerMoveCommand(Direction.South);
        //    return true;
        //}

        else if (Input.GetKey(KeyCode.DownArrow))
        {
            OnPlayerBrakeCommand?.Invoke(this, EventArgs.Empty);
            return true;
        }
        
        else if (Input.GetKey(KeyCode.E))
        {
            OnPlayerShiftCommand?.Invoke(this, EventArgs.Empty);
            return true;
        }

        else if (Input.GetKey(KeyCode.Space))
        {
            OnPlayerShootCommand?.Invoke(this, EventArgs.Empty);
            return true;
        }

        else if (Input.GetKey(KeyCode.G))
        {
            OnPlayerShotgunCommand?.Invoke(this, EventArgs.Empty);
            return true;
        }

        return false;
    }

    public void TriggerPlayerMoveCommand(Direction direction)
    {
        if (OnPlayerMoveCommand != null)
        {
            OnPlayerMoveCommand(this, new OnPlayerMoveCommandEventArgs { moveDirection = direction });
        }
    }

    //public void TriggerPlayerBrakeCommand()
    //{
    //    if (OnPlayerBrakeCommand != null)
    //    {
    //        OnPlayerBrakeCommand(this, EventArgs.Empty);
    //    }
    //}

    //public void TriggerPlayerShootCommand()
    //{
    //    if (OnPlayerShootCommand != null)
    //    {
    //        OnPlayerShootCommand(this, EventArgs.Empty);
    //    }
    //}
}
