using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBase : MonoBehaviour {

    [SerializeField]
    private TowerType _towerType;

    [SerializeField]
    private SpriteRenderer _renderer;

    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private List<EnemyBase> _nearbyEnemies;

    private float shootTimer;

    [SerializeField]
    private EnemyBase _target;
    public EnemyBase target
    {
        get { return _target; }
    }


    public float radius
    {
        get { 
            if(_towerType!=null)
            return _towerType.radius; 
            else return 0;
        }
    }

    public int damage
    {
        get
        {
            if (_towerType != null)
                return _towerType.damage;
            else return 0;
        }
    }

    public void Setup(TowerType type, Vector2 position)
    {
        _towerType = type;
        _renderer.sprite = type.towerSprite;
        
    }
    
    public void OnTriggerEnter(Collider other)
    {
        EnemyBase enemy = other.GetComponent<EnemyBase>();
        if(enemy!=null)
        {
            _nearbyEnemies.Add(enemy);
        }
    }

    public void RemoveNearbyEnemy(EnemyBase enemy)
    {
        _nearbyEnemies.Remove(enemy);
    }

    public void OnTriggerExit(Collider other)
    {
        EnemyBase enemy = other.GetComponent<EnemyBase>();
        if(enemy!=null)
        {
            _nearbyEnemies.Remove(enemy);
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
    public void SetNearestTarget()
    {
        EnemyBase nearest = null;
        float bestDistance = float.MaxValue;
        for(int i=0;i<_nearbyEnemies.Count;i++)
        {
            if (!_nearbyEnemies[i].isDying)
            {
                float distance = Vector3.Distance(_nearbyEnemies[i].transform.position, transform.position);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    nearest = _nearbyEnemies[i];
                }
            }
        }
        _target = nearest;
    }

    public void Fire()
    {
        if (_target != null)
        {
            GameObject bulletObj = Instantiate(_towerType.bulletPrefab, transform.position, Quaternion.identity) as GameObject;
            BulletBase bullet = bulletObj.GetComponent<BulletBase>();
            bullet.target = _target;
            bullet.owner = this;
        }
    }

	// Update is called once per frame
	void Update () {
        if (TowerDef.instance.phase == TowerDef.Phase.Defending)
        {
            if(_towerType!=null)
            {
                  shootTimer -= Time.deltaTime;
                  if(shootTimer <= 0)
                  {
                      shootTimer = _towerType.shootingDelay;
                      SetNearestTarget();
                      if(_target!=null) 
                         _animator.SetTrigger("fire");
                  }
            }
        }
        _renderer.sortingOrder = (int)(transform.position.y * -1000);
	}
}
