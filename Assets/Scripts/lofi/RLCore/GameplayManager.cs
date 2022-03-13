using Cinemachine;
using lofi.RLCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameplayManager : MonoBehaviour
{

    public event EventHandler OnStageComplete;    

    [SerializeField]
    public GameState gameState; 

    [SerializeField]
    public GameObject playerPrefab;

    [SerializeField]
    public GameObject enemyPrefab;

    [SerializeField]
    public GameEvent[] gameEvents;

    [SerializeField]
    public CinemachineVirtualCamera gameCamera;

    [SerializeField]
    public GameObject projectilePrefab;

    [SerializeField]
    public GameObject smokePrefab;

    [SerializeField]
    public GameObject explosionPrefab;

    [SerializeField]
    public LevelGeneratorData[] levelGeneratorData;

    [SerializeField]
    public AudioSource backgroundMusic;

    public Player Player { get; private set; }
    public Region Level { get; private set; }

    public bool PlayerTurn { get; private set; }
    public bool Paused { get; set; }
    public bool GeneratingNewStage { get; set; }

    private InputManager inputManager;
    private GameMap gameMap;
    List<Unit> activeUnits;
    EnemyFactory enemyFactory;
    ItemFactory itemFactory;

    // Start is called before the first frame update
    void Start()
    {
        gameMap = GameObject.Find("GameMap").GetComponent<GameMap>();
        activeUnits = new List<Unit>();
        enemyFactory = GetComponent<EnemyFactory>();
        itemFactory = GetComponent<ItemFactory>();
        

        inputManager = GetComponentInChildren<InputManager>();
        inputManager.OnPlayerMoveCommand += PlayerMoveCommand;
        inputManager.OnPlayerBrakeCommand += PlayerBrakeCommand;
        inputManager.OnPlayerShiftCommand += PlayerShiftCommand;
        inputManager.OnPlayerShootCommand += PlayerShootCommand;
        inputManager.OnPlayerShotgunCommand += PlayerShotgunCommand;

        OnStageComplete += StageComplete;

        GeneratingNewStage = true;
        //if (gameState.gameStage == 0 || gameState.player == null)
        //{
        //    NewStage(0);
        //}
        //else
        //{
            
        //}


    }


    public bool GenerateLevel()
    {
        if (gameState.gameStage < 0 || gameState.gameStage >= levelGeneratorData.Length)
            return false;

        LevelGenerator levelGenerator = new LevelGenerator(levelGeneratorData[gameState.gameStage]);
        levelGenerator.PaveRoad();
        Level = levelGenerator.Level;
        Level.generatorData = levelGenerator.Data;
        //pathfinding = new Pathfinding(level);

        return true;
    }

    private List<Unit> GetActiveUnits()
    {
        activeUnits.Clear();

        int activeBottom = Math.Max(Player.yPos - (Constants.ACTIVE_TILE_HEIGHT / 2), 0);
        int activeTop = Math.Min(Player.yPos + (Constants.ACTIVE_TILE_HEIGHT / 2), Level.Height-1);

        for (int y = activeBottom; y <= activeTop; y++)
        {
            for (int x = 0; x < Level.Width; x++)
            {
                Unit unit = Level.GetUnitAtXY(x, y);

                if (unit != null)
                {
                    if (!activeUnits.Contains(unit) && unit != Player)
                    {
                        //Debug.Log("Adding Unit: " + unit.name);
                        activeUnits.Add(unit);
                    }
                }
            }
        }

        return activeUnits;
    }

    private void PlayerMoveCommand(object sender, InputManager.OnPlayerMoveCommandEventArgs args)
    {
        if (PlayerTurn && !Paused)
        {
            PlayerTurnComplete();
            Player.MoveDirection(args.moveDirection);            
            ProcessEnemyTurns();
            PlayerTurnStart();
        }
    }

    private void PlayerBrakeCommand(object sender, EventArgs e)
    {
        if (PlayerTurn && !Paused && Player.unitActive)
        {
            PlayerTurnComplete();
            ProcessEnemyTurns();
            PlayerTurnStart();
        }
    }

    private void PlayerShiftCommand(object sender, EventArgs e)
    {
        if (PlayerTurn && !Paused && Player.unitActive)
        {
            if (Player.CurrentSpeed == 1)
            {
                Player.ChangeSpeed(2);
                SoundManager.PlaySound(SoundManager.Sound.HIGH_GEAR);
            }
            else if (Player.CurrentSpeed == 2)
            {
                Player.ChangeSpeed(1);
                SoundManager.PlaySound(SoundManager.Sound.LOW_GEAR);
            }
        }

    }

    private void PlayerShootCommand(object sender, EventArgs e)
    {
        if (PlayerTurn && !Paused && Player.unitActive)
        {
            ShootProjectile(new Vector3(0, 1, 0), Player.xPos, Player.yPos);
            SoundManager.PlaySound(SoundManager.Sound.SHOOT);

            PlayerTurnComplete();
            Player.Shoot();

            Player.MoveDirection(Player.Direction);
            ProcessEnemyTurns();
            PlayerTurnStart();


        }
    }

    private void PlayerShotgunCommand(object sender, EventArgs e)
    {
        if (PlayerTurn && !Paused && Player.unitActive)
        {
            //ShootProjectile(new Vector3(0, 1, 0), Player.xPos, Player.yPos);
            if (Player.ShotgunAmmo <= 0)
            {
                SoundManager.PlaySound(SoundManager.Sound.SHOTGUN_EMPTY);
                return;
            }

            PlayerTurnComplete();
            Vector2Int firePosition = Player.Shotgun();
            CreateSmokeParticle(firePosition.x, firePosition.y);

            Player.MoveDirection(Player.Direction);
            ProcessEnemyTurns();
            PlayerTurnStart();


        }
    }

    private void PlayerTurnComplete()
    {
        PlayerTurn = false;
        gameState.stageTurns++;
        gameState.gameTurns++;
    }
    
    private void PlayerTurnStart()
    {
        SpawnEnemies();

        PlayerTurn = true;
    }

    public void SpawnEnemies()
    {
        if (Random.Range(0, 100) < Level.generatorData.enemyProbabilityBehindPerTurn)
        {
            //Debug.Log("Spawning Enemy behind Player");
            Enemy enemy = enemyFactory.CreateEnemyBehindPlayer(enemyFactory.enemyDatabase[EnemyType.STREET_BIKE]);
            enemy.OnUnitDestroyed += EnemyUnitDestroyed;
        }

        if (Random.Range(0, 100) < Level.generatorData.enemyProbabilityAheadPerTurn)
        {
            //Debug.Log("Spawning Enemy ahead of Player");

            Enemy enemy = enemyFactory.CreateEnemyAheadOfPlayer(enemyFactory.GetRandomEnemyData());
            enemy.OnUnitDestroyed += EnemyUnitDestroyed;
        }

        if (gameState.gameStage == 4 && gameState.stageTurns ==2)
        {
            SpawnSemiUnit(Player.xPos, Constants.SPAWN_BEHIND_RANGE - 2);
        }
    }

    public void SpawnBombUnit(int x, int y)
    {
        Enemy enemy = enemyFactory.CreateBombAtXY(x, y);
    }

    public void SpawnSemiUnit(int x, int y)
    {
        Enemy enemy = enemyFactory.CreateSemiAtXY(x, y);
    }

    private void EnemyUnitDestroyed(object sender, Unit.OnUnitDestroyedEventArgs args)
    {
        if (Random.Range(0,100) < Constants.LOOT_CHANCE)
        {
            int index = Random.Range(0, itemFactory.itemDatabase.Count);
            itemFactory.SpawnItemOnLevel((ItemType)index, 5, args.unit.xPos, args.unit.yPos);
        }
    }

    public void ShootProjectile(Vector3 direction, int x, int y)
    {
        Projectile projectile = Instantiate(projectilePrefab).GetComponent<Projectile>();
        projectile.transform.position = Level.GetAreaWorldPosition(x, y) + new Vector3(5f,5f,-0.9f);
        //projectile.transform.position = new Vector3(projectile.transform.position.x, projectile.transform.position.y, -1);
        projectile.region = Level;
        projectile.unit = Player;
        projectile.direction = direction;
    }
    
    public void CreateSmokeParticle(int x, int y)
    {
        SmokeParticle smoke = Instantiate(smokePrefab).GetComponent<SmokeParticle>();
        smoke.transform.position = Level.GetAreaWorldPosition(x, y) + new Vector3(5f,5f,-1.1f);

        //smoke.region = Level;
        //smoke.unit = Player;
        //projectile.direction = direction;
    }

    private void ProcessEnemyTurns()
    {
        GetActiveUnits();

        foreach (var unit in activeUnits)
        {
            if (unit != null)
                unit.TakeTurn(gameState, this);
        }

    }

    public void TriggerGameEvent(GameEvent[] gameEvents, string eventName)
    {
        foreach(GameEvent e in gameEvents)
        {
            if (e.name.Equals(eventName))
            {
                e.Raise();
                break;
            }
        }

        return;
    }

    // Update is called once per frame
    void Update()
    {
        if (GeneratingNewStage)
        {
            NewStage(gameState.gameStage);
        }
        if(!Paused && IsStageComplete())
        {
            OnStageComplete?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool IsStageComplete()
    {
        if (Player.yPos >= Level.Height - Constants.STAGE_COMPLETE_RANGE)
        {
            return true;
        }

        return false;
    }

    public void StageComplete(object sender, EventArgs e)
    {

        Debug.Log("Stage Complete");
        Paused = true;

        if (gameState.gameStage >= levelGeneratorData.Length-1)
        {
            GameOver();
        }
        else 
        {
            gameState.gameStage = gameState.gameStage + 1;
            gameState.playerHealth = Player.Health;
            gameState.shotgunAmmo = Player.ShotgunAmmo;

            SceneManager.LoadScene("GameScene");
        }

    }

    public void NewStage(int stage)
    {
        //Debug.Log("New Stage Starting: " + stage);



        gameState.stageTurns = 0;
        GeneratingNewStage = true;
        Paused = true;
        
        activeUnits.Clear();

        CreateStage(stage);

        CreatePlayer();

        CreateEnemies();


        Player = gameState.player;
        Level = gameState.Level;

        itemFactory.Initialize();

        if (stage == 0)
        {
            gameState.gameOver = false;
            gameState.gameWon = false;
            gameState.gameScore = 0;
            gameState.gameTurns = 0;
            gameState.playerHealth = Player.Health;
            gameState.shotgunAmmo = Constants.SHOTGUN_START_AMOUNT;
        }

        if (stage == 4)
        {
            backgroundMusic.clip = SoundManager.GetAudioClip(SoundManager.Sound.BOSS_MUSIC);
            backgroundMusic.Play();
        }

        Player.Health = gameState.playerHealth;
        Player.ShotgunAmmo = gameState.shotgunAmmo;

        GeneratingNewStage = false;
        Paused = false;


        PlayerTurnStart();
    }
    
    public void CreateStage(int stage)
    {
        gameState.gameStage = stage;
        GenerateLevel();
        gameState.Level = Level;
        gameMap.CreateMapMesh(gameState);
    }

    public void CreatePlayer()
    {
        gameState.player = Instantiate(playerPrefab).GetComponent<Player>();
        gameState.player.Initialize(levelGeneratorData[gameState.gameStage].startPlayerX, levelGeneratorData[gameState.gameStage].startPlayerY);
        gameCamera.Follow = gameState.player.transform;

        gameState.player.OnPlayerDestroyed += PlayerDied;
    }

    private void PlayerDied(object sender, EventArgs e)
    {
        Paused = true;
        Debug.Log("Player Died");
        GameOver();
    }

    public void CreateEnemies()
    {
        enemyFactory = GetComponent<EnemyFactory>();
        enemyFactory.Initialize();
        //factory.PopulateInitialEnemies();

        //Enemy enemy = Instantiate(enemyPrefab).GetComponent<Enemy>();
        //enemy.Initialize(10, 1);
        //enemy.SetEnemyAI(new TravelFacingDirectionEnemyAI(enemy));

        //enemy = Instantiate(enemyPrefab).GetComponent<Enemy>();
        //enemy.Initialize(10, 20);
        //enemy.SetEnemyAI(new TravelFacingDirectionEnemyAI(enemy));
        //enemy.RotateTo(Direction.South);

    }
    public void GameOver()
    {
        Debug.Log("Game Over Man!");
        Paused = true;

        if (gameState.gameScore > gameState.highScore)
        {
            gameState.highScore = gameState.gameScore;
            PlayerPrefs.SetInt(Constants.HIGH_SCORE_KEY, gameState.highScore);
        }

        gameState.gameOver = true;

        
        
    }

}
