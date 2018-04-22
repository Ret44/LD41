using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "towerType", menuName = "Tower type")]
public class TowerType : ScriptableObject {

    public TileType ammoType;

    public Sprite towerSprite;
    public int damage;

    [Range(0f,5f)]
    public float radius;

    public float shootingDelay;

    public GameObject bulletPrefab;
}
