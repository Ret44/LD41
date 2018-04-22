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

    [SerializeField]
    private float _waveTimer;

    [SerializeField]
    private List<EnemyBase> _aliveEnemies;

    [SerializeField]
    private EnemyWave _currentWave;

    [SerializeField]
    private GameObject _enemyPrefab;

    public static void GenerateNewWave(int level)
    {
        instance._currentWave = new EnemyWave();
        instance._currentWave.enemyCount = level * 15;
        instance._currentWave.delayBetweenSpawn = 1f;
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
    }   
#endif
    public void Awake()
    {
        instance = this;
    }

    public static void AddEnemy(EnemyBase enemy)
    {
        instance._aliveEnemies.Add(enemy);
    }


    public static void AddEnemy()
    {
        GameObject enemyObj = Instantiate(instance._enemyPrefab) as GameObject;
        enemyObj.name = "Enemy";
        EnemyBase enemy = enemyObj.GetComponent<EnemyBase>();
        AddEnemy(enemy);
        enemy.Setup();
    }

    public static void RemoveEnemy(EnemyBase enemy)
    {
        instance._aliveEnemies.Remove(enemy);
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
    }
}
