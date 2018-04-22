using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAnimationsEvent : MonoBehaviour {

    public TowerBase tower;

    public void Awake()
    {
        tower = GetComponentInParent<TowerBase>();
    }

   public void TriggerFire()
    {
        tower.Fire();
    }
}
