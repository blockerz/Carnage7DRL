using lofi.RLCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
    public event EventHandler<OnPlayerDestroyedEventArgs> OnPlayerDestroyed;
    public class OnPlayerDestroyedEventArgs : EventArgs
    {
        public Player player;
    }

    [SerializeField]
    private PlayerRuntimeSet playerSet;

    protected override void Start()
    {
        base.Start();
        CurrentSpeed = 1;
    }

    protected override void Update()
    {
        base.Update();
    }
    
    
    public override bool MoveDirection(Direction direction)
    {
        bool moved = false;

        switch (direction)
        {
            case Direction.East:
                moved = MoveBy(1, 1);
                CheckLoot();
                if (CurrentSpeed == 2)
                {
                    moved = MoveBy(0, 1);
                    CheckLoot();
                }
                break;
            case Direction.North:
                moved = MoveBy(0, 1);
                CheckLoot();
                if (CurrentSpeed == 2)
                {
                    moved = MoveBy(0, 1);
                    CheckLoot();
                }
                break;
            case Direction.West:
                moved = MoveBy(-1, 1);
                CheckLoot();
                if (CurrentSpeed == 2)
                {
                    moved = MoveBy(0, 1);
                    CheckLoot();
                }
                break;
            case Direction.South:
                //CurrentSpeed = 1;
                //moved = MoveBy(0, 1);
                //CheckLoot();
                break;

            default:
                break;
        }

        return moved;
    }

    public override void Die(bool quiet = false, bool score = true)
    {
        base.Die(quiet, score);
        TriggerPlayerDestroyed(this);
    }

    public void CheckLoot()
    {
        for (int x = xPos; x < xPos + UnitWidth; x++)
        {
            for (int y = yPos; y < yPos + UnitHeight; y++)
            {
                Item item = Level.GetItemAtXY(x, y);

                if (item != null)
                {
                    switch(item.itemData.Type)
                    {
                        case ItemType.SHOTGUN_AMMO:
                            ShotgunAmmo += item.amount;
                            break;
                        case ItemType.WRENCH:
                            AddHealth(item.itemData.amount);
                            break;
                        default:
                            break;
                    }
                    Level.AddItemAtArea(null, x, y);
                    item.DestroyItem();
                    SoundManager.PlaySound(SoundManager.Sound.PICKUP);
                }
            }
        }
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        playerSet.Add(this);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        playerSet.Remove(this);
    }

    public void TriggerPlayerDestroyed(Player player)
    {
        if (OnPlayerDestroyed != null)
        {
            OnPlayerDestroyed(this, new OnPlayerDestroyedEventArgs { player = player });
        }
    }
}
