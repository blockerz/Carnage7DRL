using lofi.RLCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{

    [SerializeField]
    private EnemyRuntimeSet enemySet;

    EnemyAI enemyAI;

    public Enemy()
    {
        enemyAI = new EnemyAI(this);
    }

    public void SetEnemyAI(EnemyAI enemyAI)
    {
        this.enemyAI = enemyAI;
    }

    public override bool TakeTurn(GameState gameState, GameplayManager manager)
    {
        if (!unitActive)
            return true;

        if (enemyAI == null)
            return false;

        if (yPos >= Mathf.Min(Level.Height - 2, gameState.player.yPos + (Constants.ACTIVE_TILE_HEIGHT/2) - 2) ||
            yPos < Mathf.Max(0, gameState.player.yPos - (Constants.ACTIVE_TILE_HEIGHT/2)))
        {
            if (unitData.enemyType != EnemyType.SEMI_TRUCK)
                Die(true);
        }

        bool moved = false;
        for (int s = 0; s < CurrentSpeed; s++)
        {
            moved = enemyAI.TakeTurn(gameState, manager);
        }

        if (CurrentSpeed == 0 && !moved)
        {
            moved = enemyAI.TakeTurn(gameState, manager);
        }
        return moved;
    }

    protected override void Update()
    {
        base.Update();

    }

    public override void Die(bool quiet = false, bool score = true)
    {
        base.Die(quiet, score);
        
        if (unitData.enemyType == EnemyType.SEMI_TRUCK)
        {
            unitData.gameState.gameWon = true;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        enemySet.Add(this);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        enemySet.Remove(this);
    }
}
