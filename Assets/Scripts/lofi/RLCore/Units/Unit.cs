using lofi.RLCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Unit : MonoBehaviour
{
    public event EventHandler<OnUnitEnvironmentCollisionEventArgs> OnUnitEnvironmentCollision;
    public class OnUnitEnvironmentCollisionEventArgs : EventArgs
    {
        public int damage;
    }
    
    public event EventHandler<OnUnitToUnitCollisionEventArgs> OnUnitToUnitCollision;
    public class OnUnitToUnitCollisionEventArgs : EventArgs
    {
        public Unit otherUnit;
        public int otherDamage;
        public int damage;
    }
    
    public event EventHandler<OnUnitShotDamageEventArgs> OnUnitShotDamage;
    public class OnUnitShotDamageEventArgs : EventArgs
    {
        public Unit otherUnit;
        public int otherDamage;
    }
    
    public event EventHandler<OnUnitEnvironmentDamageEventArgs> OnUnitEnvironmentDamage;
    public class OnUnitEnvironmentDamageEventArgs : EventArgs
    {
        public Area area;
        public int damage;
    }

    public event EventHandler<OnUnitDestroyedEventArgs> OnUnitDestroyed;
    public class OnUnitDestroyedEventArgs : EventArgs
    {
        public Unit unit;
    }

    [SerializeField]
    public UnitData unitData;

    [SerializeField]
    protected UnitRuntimeSet unitSet;

    [SerializeField]
    public Explosion explosionPrefab;

    protected SpriteRenderer spriteRenderer;

    public int xPos { get; private set; }
    public int yPos { get; private set; }

    public int Health { get; set; }
    public int CurrentSpeed { get; protected set; }

    public int ShotgunAmmo { get; set; }

    public int UnitWidth { get; private set; }
    public int UnitHeight{ get; private set; }

    public Direction Direction { get; private set; }
    public Vector3 RotationOffset { get; private set; }
    public bool unitActive = true;

    protected Region Level;

    bool environmentDamageThisTurn = false;

    protected virtual void Start()
    {
        //Initialize();

    }

    public virtual bool Initialize(int startX, int startY)
    {
        xPos = startX;
        yPos = startY;
        ShotgunAmmo = 0;

        this.gameObject.layer = Constants.MAP_LAYER_ID;

        this.gameObject.name = unitData.name;

        spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = unitData.sprite;
        spriteRenderer.color = unitData.color;
        //spriteRenderer.color = Color.green;

        Level = unitData.gameState.Level;

        CurrentSpeed = unitData.maxSpeed;
        Health = unitData.maxHealth;
        unitActive = true;

        if (RotateTo(unitData.defaultDirection) && MoveTo(startX, startY))
            return true;

        return false;
    }

    public Vector3 GetRootPosition()
    {
        return new Vector3(xPos * Level.AreaWorldSizeX, yPos * Level.AreaWorldSizeY, Constants.UNIT_Z_POS) + Level.Origin;
    }

    protected virtual void Update()
    {

    }

    public bool ChangeSpeed(int value)
    {
        if (value >= 0 && value <= unitData.maxSpeed)
        {
            CurrentSpeed = value;
            return true;
        }
        return false;
    }

    public bool RotateTo(Direction dir)
    {
        if (!unitActive)
            return true;

        if (this.Direction == dir)
            return true;

        if ((this.Direction == Direction.North || this.Direction == Direction.South) &&
            (dir == Direction.East || dir == Direction.West))
        {
            UnitWidth = unitData.tileWidth;
            UnitHeight = unitData.tileHeight;

            if(!IsAreaOpenForUnit(xPos,yPos))
            {
                UnitWidth = unitData.tileHeight;
                UnitHeight = unitData.tileWidth;
                return false;
            }
        }
        else if ((this.Direction == Direction.East || this.Direction == Direction.West) &&
                (dir == Direction.North || dir == Direction.South))
        {
            UnitWidth = unitData.tileHeight;
            UnitHeight = unitData.tileWidth;

            if (!IsAreaOpenForUnit(xPos, yPos))
            {
                UnitWidth = unitData.tileWidth;
                UnitHeight = unitData.tileHeight;
                return false;
            }
        }

        switch (dir)
        {
            case Direction.East:
                RotationOffset = Vector3.zero;
                UnitWidth = unitData.tileWidth;
                UnitHeight = unitData.tileHeight;
                break;
            case Direction.North:
                RotationOffset = new Vector3(unitData.tileHeight * Level.AreaWorldSizeY,0, 0);
                UnitWidth = unitData.tileHeight;
                UnitHeight = unitData.tileWidth;
                break;
            case Direction.West:
                RotationOffset = new Vector3(unitData.tileWidth * Level.AreaWorldSizeX, unitData.tileHeight* Level.AreaWorldSizeY,0);
                UnitWidth = unitData.tileWidth;
                UnitHeight = unitData.tileHeight;
                break;
            case Direction.South:
                RotationOffset = new Vector3(0, unitData.tileWidth * Level.AreaWorldSizeX, 0);
                UnitWidth = unitData.tileHeight;
                UnitHeight = unitData.tileWidth;
                break;

            default:
                break;
        }

        Vector3 newRotation = new Vector3(0, 0, (float)((int)dir));
        transform.eulerAngles = newRotation;

        //this.transform.Rotate(0, 0, (float)((int)dir), Space.World);
        transform.position = GetRootPosition() + RotationOffset;
        this.Direction = dir;
        return true;
    }

    public bool MoveTo(int x, int y)
    {
        if (!unitActive)
            return true;

        if (Level == null)
            Level = unitData.gameState.Level;

        if (MoveUnitToArea(x, y))
        {
            xPos = x;
            yPos = y;
        }
        else
        {
            return false;
        }

        transform.position = GetRootPosition() + RotationOffset;

        return true;
    }
    
    public bool IsAreaOpenForUnit(int x, int y)
    {
        if (x < 0 || y < 0 || x >= (Level.Width - UnitWidth + 1) || y >= (Level.Height - UnitHeight + 1))
            return false;

        for (int areaX = x; areaX < x + UnitWidth; areaX++)
        {
            for (int areaY = y; areaY < y + UnitHeight; areaY++)
            {
                Area area = Level.GetAreaAtXY(areaX, areaY);

                if (area == null)
                    return false;

                if (!area.Data.isPassable)
                    return false;

                Unit unit = area?.UnitPresent;

                if (unit != null && unit != this)
                    return false;
            }
        }
        return true;
    }

    public bool CheckEnvironmentCollision(int x, int y)
    {
        if (!unitActive)
            return false;

        for (int areaX = x; areaX < x + UnitWidth; areaX++)
        {
            for (int areaY = y; areaY < y + UnitHeight; areaY++)
            {
                Area area = Level.GetAreaAtXY(areaX, areaY);

                if (area == null)
                    continue;

                if (!area.Data.isPassable)
                {
                    TakeDamage(DamageType.COLLISION, area.Data.damageAmount);
                    TriggerUnitEnvironmentCollision(area.Data.damageAmount);
                    return true;
                }
            }
        }

        return false;
    }

    public bool CheckUnitCollision(int x, int y)
    {
        if (!unitActive)
            return false;

        for (int areaX = x; areaX < x + UnitWidth; areaX++)
        {
            for (int areaY = y; areaY < y + UnitHeight; areaY++)
            {
                Area area = Level.GetAreaAtXY(areaX, areaY);

                if (area == null)
                    continue;

                Unit unit = area?.UnitPresent;

                if (unit != null && unit != this)
                {             
                    TakeDamage(DamageType.COLLISION, unit.unitData.crashDamage, unit);
                    unit.TakeDamage(DamageType.COLLISION, this.unitData.crashDamage, this);
                    TriggerUnitToUnitCollision(unit, this.unitData.crashDamage, unit.unitData.crashDamage);
                    return true;
                }
            }
        }

        return false;
    }

    public void TakeDamage(DamageType type, int amount, Unit source = null)
    {
        if (!unitActive)
            return;

        if (type == DamageType.BULLET && unitData.bulletproof)
            return;

        Health -= amount;

        if (type == DamageType.BULLET)
        {
            FlashColor(Color.red, 0.2f);
        }
        else
        {
            FlashColor(Color.red);
        }

        if (Health <= 0)
        {
            Health = 0;
            if (source != unitData.gameState.player)
                Die(false, false);
            else
                Die();
        }
    }

    public void AddHealth(int amount)
    {
        Health += amount;
        Health = Math.Min(Health, unitData.maxHealth);
    }
    public virtual void Die(bool quiet = false, bool score = true)
    {
        //Debug.Log("Unit died: " + name);
        unitActive = false;
        
        if (!quiet && score)
        { 
            unitData.gameState.gameScore += unitData.score;
            TriggerUnitDestroyed(this);
        }

        if (!quiet)
        {
            StartExplosion();
        }

        ClearUnitFromArea();
        gameObject.SetActive(false);
        GameObject.Destroy(this);
    }

    public bool MoveUnitToArea(int x, int y)
    {
        if (!unitActive)
            return true;

        if (!IsAreaOpenForUnit(x, y))
        {
            CheckEnvironmentCollision(x, y);
            CheckUnitCollision(x, y);
            return false;
        }

        ClearUnitFromArea();


        environmentDamageThisTurn = false;
        // add this unit to the area(s)
        for (int areaX = x; areaX < x + UnitWidth; areaX++)
        {
            for (int areaY = y; areaY < y + UnitHeight; areaY++)
            {
                Area area = Level.GetAreaAtXY(areaX, areaY);               

                if (area?.UnitPresent == null)
                    area.UnitPresent = this;

                if (!environmentDamageThisTurn && area.Data.isHazard)
                {
                    TakeDamage(DamageType.ENVIRONMENT, area.Data.damageAmount);
                    TriggerUnitEnvironmentDamage(area, area.Data.damageAmount);

                    if(this == unitData.gameState.player)
                        SoundManager.PlaySound(SoundManager.Sound.BUMPY);
                }
            }
        }
        return true;
    }

    public void ClearUnitFromArea()
    {
        // clear the area(s) of units
        for (int areaX = xPos; areaX < xPos + UnitWidth; areaX++)
        {
            for (int areaY = yPos; areaY < yPos + UnitHeight; areaY++)
            {
                //Unit unit = region.GetAreaAtXY(areaX, areaY).UnitPresent;

                if (Level.GetAreaAtXY(areaX, areaY)?.UnitPresent != null)
                    Level.GetAreaAtXY(areaX, areaY).UnitPresent = null;
            }
        }
    }
    public virtual bool Shoot()
    {
        if (!unitActive)
            return true;

        Unit unit = FindUnitAlongLine(Direction);

        if (unit != null && unit != this)
        {
            unit.TakeDamage(DamageType.BULLET, unitData.shotDamage, this);
            TriggerUnitShotDamage(unit, unitData.shotDamage);
            return true;
        }

        return false;
    }
    
    public virtual Vector2Int Shotgun()
    {
        if (!unitActive)
            return Vector2Int.zero;

        ShotgunAmmo--;
        Vector2Int fireLocation = new Vector2Int(xPos-1, yPos);

        List<Unit> units = FindUnitsAdjacent(Constants.SHOTGUN_RANGE);

        if (units.Count > 0)
        {
            Unit unit = units[Random.Range(0, units.Count)];
            fireLocation = new Vector2Int(unit.xPos, unit.yPos);
            unit.TakeDamage(DamageType.BULLET, Constants.SHOTGUN_DAMAGE, this);
            TriggerUnitShotDamage(unit, unitData.shotDamage);
        }

        return fireLocation;
    }

    public virtual List<Unit> FindUnitsAdjacent(int range)
    {
        List<Unit> units = new List<Unit>();

        for (int y = yPos - range; y < yPos + UnitHeight + range; y++)
        {
            for (int x = xPos - range; x < xPos + UnitWidth + range; x++)
            {
                var unit = Level.GetUnitAtXY(x, y);
                if (unit != null && unit != this && !unit.unitData.bulletproof)
                {
                    units.Add(unit);
                }
            }
        }
        return units;
    }

    public virtual Unit FindUnitAlongLine(Direction direction)
    {
        switch (direction)
        {
            case Direction.East:
                for (int x = xPos; x < Level.Width; x++)
                {
                    var unit = Level.GetUnitAtXY(x, yPos);
                    if (unit != null && unit != this && !unit.unitData.bulletproof)
                    {
                        return unit;
                    }
                }
                break;
            case Direction.North:
                for (int y = yPos; y < Math.Min(Level.Height, yPos + (Constants.ACTIVE_TILE_HEIGHT / 2)); y++)
                {
                    var unit = Level.GetUnitAtXY(xPos, y);
                    if (unit != null && unit != this && !unit.unitData.bulletproof)
                    {
                        return unit;
                    }
                }
                break;
            case Direction.West:
                for (int x = xPos; x >= 0; x--)
                {
                    var unit = Level.GetUnitAtXY(x, yPos);
                    if (unit != null && unit != this && !unit.unitData.bulletproof)
                    {
                        return unit;
                    }
                }
                break;
            case Direction.South:
                for (int y = yPos; y >= Math.Max(0,yPos-(Constants.ACTIVE_TILE_HEIGHT/2)); y--)
                {
                    var unit = Level.GetUnitAtXY(xPos, y);
                    if (unit != null && unit != this && !unit.unitData.bulletproof)
                    {
                        return unit;
                    }
                }
                break;

            default:
                break;
        }

        return null;
    }

    public bool MoveBy(int x, int y)
    {
        if (!unitActive)
            return true;

        return MoveTo(xPos + x, yPos + y);
    }

    public virtual bool MoveDirection(Direction direction)
    {
        if (!unitActive)
            return true;

        bool moved = false;

        switch (direction)
        {
            case Direction.East:
                moved = MoveBy(1, 0);
                break;
            case Direction.North:
                moved = MoveBy(0, 1);
                break;
            case Direction.West:
                moved = MoveBy(-1, 0);
                break;
            case Direction.South:
                moved = MoveBy(0, -1);
                break;

            default:
                break;
        }

        return moved;
    }

    public virtual bool TakeTurn(GameState gameState, GameplayManager manager)
    {
        return true;
    }

    protected virtual void OnEnable()
    {
        unitSet.Add(this);
    }

    protected virtual void OnDisable()
    {
        unitSet.Remove(this);
    }

    public void StartExplosion()
    {
        Explosion explosion = Instantiate(explosionPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity).GetComponent<Explosion>();
        explosion.transform.eulerAngles = transform.eulerAngles;
        SoundManager.PlaySound(SoundManager.Sound.EXPLOSION);
    }
    public void FlashColor(Color color, float delay = 0f)
    {
        if (spriteRenderer == null)
            return;

        StartCoroutine(FlashAndRestore(color, delay));
    }

    private IEnumerator FlashAndRestore(Color color, float delay = 0f)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);
        spriteRenderer.color = color;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = unitData.color;
    }


    public void TriggerUnitEnvironmentCollision(int damage)
    {
        if (OnUnitEnvironmentCollision != null)
        {
            OnUnitEnvironmentCollision(this, new OnUnitEnvironmentCollisionEventArgs { damage = damage });
        }
    }

    public void TriggerUnitToUnitCollision(Unit otherUnit, int otherDamage, int damage)
    {
        if (OnUnitToUnitCollision != null)
        {
            OnUnitToUnitCollision(this, new OnUnitToUnitCollisionEventArgs { otherUnit = otherUnit, otherDamage = otherDamage, damage = damage });
        }
    }
    
    public void TriggerUnitShotDamage(Unit otherUnit, int otherDamage)
    {
        if (OnUnitShotDamage != null)
        {
            OnUnitShotDamage(this, new OnUnitShotDamageEventArgs { otherUnit = otherUnit, otherDamage = otherDamage });
        }
    }

    public void TriggerUnitEnvironmentDamage(Area area, int damage)
    {
        if (OnUnitEnvironmentDamage != null)
        {
            OnUnitEnvironmentDamage(this, new OnUnitEnvironmentDamageEventArgs { area = area, damage = damage });
        }
    }

    public void TriggerUnitDestroyed(Unit unit)
    {
        if (OnUnitDestroyed != null)
        {
            OnUnitDestroyed(this, new OnUnitDestroyedEventArgs { unit = unit });
        }
    }
}
