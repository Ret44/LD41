using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBase : MonoBehaviour {

    public TileType tileType;

    [SerializeField]
    private SpriteRenderer renderer;

    public void OnMouseDrag()
    {
        
    }

    public void OnMouseDown()
    {
        Debug.LogFormat("Picked {0}", gameObject.name);
        Match3.Grab(this);
    }

    public void OnMouseUp()
    {
        Debug.Log("Released");
        Match3.Release();
    }
	

    public void Setup(TileType def)
    {
        tileType = def;
        renderer.sprite = def.sprite;
        renderer.color = def.color;
    }
}
