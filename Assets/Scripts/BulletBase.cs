using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBase : MonoBehaviour {

    public float velocity;

    public TowerBase owner;
    public EnemyBase target;

    [SerializeField]
    private SpriteRenderer _renderer;

    public void OnTriggerEnter(Collider other)
    {
        EnemyBase enemy = other.GetComponent<EnemyBase>();
        if (enemy == target)
        {
            owner.RemoveNearbyEnemy(enemy);
            enemy.Hit(owner.damage);
            Destroy(this.gameObject);
        }
    }

    public void Update()
    {
        if(target!=null)
        {
            transform.up = (target.transform.position - transform.position).normalized;
            transform.Translate(transform.up * velocity * Time.deltaTime);
        }
        _renderer.sortingOrder = (int)(transform.position.y * 1000);
    }
}
