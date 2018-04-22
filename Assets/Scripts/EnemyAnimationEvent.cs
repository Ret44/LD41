using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEvent : MonoBehaviour {

    public EnemyBase enemy;

    public void Awake()
    {
        enemy = GetComponentInParent<EnemyBase>();
    }

    public void TriggerDead()
    {
        enemy.Die();
    }
}
