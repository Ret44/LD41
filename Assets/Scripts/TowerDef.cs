using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerDef : MonoBehaviour {

    public enum Phase
    {
        Building,
        Defending
    }
    public enum DefState
    {
        WaveActive
    }

    public static TowerDef instance;

    public Phase phase;
    public DefState defState;

    public FieldBase pathStart;
    public FieldBase pathEnd;

    public TowerBase nowBuilding = null;

    public Vector2 mouseGridPosition;

    [SerializeField]
    private float _waveTimer;

    [SerializeField]
    private List<EnemyBase> _aliveEnemies;

    [SerializeField]
    private EnemyWave _currentWave;

    [SerializeField]
    private GameObject _enemyPrefab;

    [SerializeField]
    private GameObject _towerPrefab;

    [SerializeField]
    private FieldBase[,] _grid;

    [SerializeField]
    private List<TowerBase> _towers;


    [SerializeField]
    private TileType _buildCurrency;

    public static int Cash
    {
        get
        {
            return Core.Ammo[instance._buildCurrency].value;
        }
        set
        {
            Core.Ammo[instance._buildCurrency].SetValue(value);
        }
    }


    public static Vector3 GetWorldPosition(Vector2 gridLocation)
    {
        return GetWorldPosition((int)gridLocation.x, (int)gridLocation.y);
    }

    public static Vector3 GetWorldPosition(int x, int y)
    {
        return instance._grid[x, y].transform.position;
    }
    public static void RegisterField(FieldBase field)
    {
        instance._grid[(int)field.gridLocation.x, (int)field.gridLocation.y] = field;
    }

    public static void GenerateNewWave(int level)
    {
        instance._currentWave = new EnemyWave();
        instance._currentWave.enemyCount = level * 15;
        instance._currentWave.delayBetweenSpawn = 4f;
    }

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        FieldBase pathTmp = pathStart;
        Gizmos.color = Color.red;
        while(pathTmp.nextPath!=null)
        {
            Gizmos.DrawLine(pathTmp.transform.position, pathTmp.nextPath.transform.position);
            pathTmp = pathTmp.nextPath;
        }
        Gizmos.color = Color.yellow;
        if (_aliveEnemies != null)
        {
            for (int i = 0; i < _aliveEnemies.Count; i++)
            {
                Gizmos.DrawLine(_aliveEnemies[i].transform.position, _aliveEnemies[i].transform.position - _aliveEnemies[i].movementOffset);
            }
        }
        if(_towers != null)
        {        
            for(int i = 0; i < _towers.Count; i++)
            {
                if (_towers[i].target != null)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(_towers[i].transform.position, _towers[i].target.transform.position);
                }
                Gizmos.color = new Color(1f, 1f, 1f, 0.25f);
                Gizmos.DrawSphere(transform.position, _towers[i].radius);
            }
        }
    }   
#endif
    public void Awake()
    {
        instance = this;
        _grid = new FieldBase[8, 8];
    }

    public static void AddEnemy(EnemyBase enemy)
    {
        instance._aliveEnemies.Add(enemy);
    }


    public static void AddEnemy()
    {
        GameObject enemyObj = Instantiate(instance._enemyPrefab) as GameObject;
        enemyObj.name = "Enemy";
        enemyObj.transform.parent = instance.transform;
        enemyObj.transform.localScale = Vector3.one;
        EnemyBase enemy = enemyObj.GetComponent<EnemyBase>();
        AddEnemy(enemy);
        enemy.Setup();
    }

    public static void RemoveEnemy(EnemyBase enemy)
    {
        instance._aliveEnemies.Remove(enemy);
    }

    public void BuildNewTower(TowerType type)
    {
        if (nowBuilding == null)
        {
            if (Cash >= 25)
            {
                Cash -= 25;
                GameObject towerObj = Instantiate(instance._towerPrefab);
                towerObj.transform.parent = transform;
                towerObj.transform.localScale = Vector3.one;
                TowerBase tower = towerObj.GetComponent<TowerBase>();
                tower.Setup(type);
                nowBuilding = tower;
            }
        }
    }

    public void PlaceNewTower()
    {
        if(nowBuilding!=null)
        {
            nowBuilding.spriteRenderer.color = Color.white;
            nowBuilding.animator.SetTrigger("build");
            nowBuilding.isBuilding = false;
            _grid[(int)mouseGridPosition.x, (int)mouseGridPosition.y].fieldType = FieldBase.FieldType.NotBuildable;            
            nowBuilding = null;
        }
    }

    public void Update()
    {
        if (phase == Phase.Defending)
        {
            if (_currentWave.enemyCount == 0)
                GenerateNewWave(1);
            
            _waveTimer -= Time.deltaTime;
            
            if(_waveTimer<=0)
            {
                _waveTimer = _currentWave.delayBetweenSpawn - Random.Range(0f, 0.5f);
                _currentWave.enemyCount--;
                AddEnemy();
            }
        }

        if(nowBuilding!=null)
        {
            nowBuilding.transform.position = GetWorldPosition(mouseGridPosition);
            if(_grid[(int)mouseGridPosition.x, (int)mouseGridPosition.y].fieldType == FieldBase.FieldType.Buildable)
            {
                nowBuilding.spriteRenderer.color = Color.green;
                if(Input.GetMouseButton(0))
                {
                    PlaceNewTower();
                }               
            }
            else
            {
                nowBuilding.spriteRenderer.color = Color.red;
            }
            if (Input.GetMouseButton(1))
            {
                Destroy(nowBuilding.gameObject);
                Cash += 25;
            }
        }
    }
}
