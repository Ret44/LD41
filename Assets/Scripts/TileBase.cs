using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TileBase : MonoBehaviour {

    public TileType tileType;
    public Vector2 gridLocation;
    public Vector3 target;

    public Tweener tweener;

    public SpriteRenderer renderer;

    public void OnMouseDrag()
    {
        
    }

    public void OnMouseOver()
    {
        Match3.MouseOverPosition = gridLocation;
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
	
    public void Animate(Vector2 position)
    {
        target = Match3.GetWorldPosition(position);
    }

    public void SetPosition(Vector2 position)
    {
        gridLocation = position;
        transform.localPosition = Match3.GetWorldPosition(gridLocation);
        target = transform.localPosition;
        Match3.Grid[(int)gridLocation.x, (int)gridLocation.y] = this;
    }

    public void Setup(TileType def)
    {
        tileType = def;
        renderer.sprite = def.sprite;
        renderer.color = def.color;
    }

    public void Update()
    {
        if(transform.localPosition != target)
        {
            transform.localPosition = new Vector2(Mathf.Lerp(transform.localPosition.x, target.x, Time.deltaTime * 10), Mathf.Lerp(transform.localPosition.y, target.y, Time.deltaTime * 10));
        }
    }
}
