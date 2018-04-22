using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TileBase : MonoBehaviour {

    public enum State
    {
        Idle,
        Falling,
        Dying
    }

    public State _state;
    public TileType tileType;
    public Vector2 gridLocation;
    public Vector3 target;
    public bool isFalling = false;
    public Tweener tweener;

    public SpriteRenderer renderer;

    public void Destroy()
    {
        _state = State.Dying;
        transform.DOScale(Vector3.zero, 0.5f).OnComplete(() => 
        {
            Match3.RemoveTile((int)gridLocation.x, (int)gridLocation.y);
            Destroy(this.gameObject); 
        });
    }

    public void Spawn(Vector2 gridPosition)
    {
        _state = State.Falling;
        SetPosition(gridPosition);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + Screen.height, transform.localPosition.z);
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
        Match3.SetTile(gridLocation, this);
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
            transform.localPosition = Vector2.Lerp(transform.localPosition, target, Time.deltaTime * 5);
            //new Vector2(Mathf.Lerp(transform.localPosition.x, target.x, Time.deltaTime * 10), 
              //                                    Mathf.Lerp(transform.localPosition.y, target.y, Time.deltaTime * 10));
        }
        else
        {
            if (_state == State.Falling)
                _state = State.Idle;
        }
    }
}
