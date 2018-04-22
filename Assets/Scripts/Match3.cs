using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match3 : MonoBehaviour {

    public enum State
    {
        Destroying,
        Moving,
        Idle
    }

    [SerializeField]
    private State _state;
    public static State CurrentState
    {
        get { return instance._state; }
    }

    public static Match3 instance;
       
    [SerializeField]
    private TileBase _holdingSlot;

    [SerializeField]
    private TileBase _potentialSlot;

    [SerializeField]
    private Vector2 _mouseOverPosition;
    public static Vector2 MouseOverPosition
    {
        get { return instance._mouseOverPosition; }
        set { instance._mouseOverPosition = value; }
    }

    [SerializeField]
    private bool isLocked;

    private Vector3 _mouseHoldStartPos;

    [SerializeField]
    private Vector2 _tileDelta;

    [SerializeField]
    private Vector2 _dragDirection;
    private Vector2 _prevDragDirection;

    [SerializeField]
    private Vector2 _targetPosition;

    [SerializeField]
    private GameObject _tilePrefab;

    [SerializeField]
    private GameObject _backgroundPrefab;

    [SerializeField]
    private TileType[] _types;

    [SerializeField]
    private Vector2 _gridSize;

    [SerializeField]
    private TileBase[,] _grid;
    public static TileBase[,] Grid 
    {
        get { return instance._grid; }
    }

    private SpriteRenderer[,] _backgroundGrid;

    public static Vector3 GetWorldPosition(Vector2 gridPos)
    {
       return GetWorldPosition((int)gridPos.x, (int)gridPos.y);
    }

    public static Vector3 GetWorldPosition(int x, int y)
    {
        return instance._backgroundGrid[x, y].gameObject.transform.localPosition;
    }

    public static void StartNewGame()
    {
        instance._grid = new TileBase[(int)instance._gridSize.x, (int)instance._gridSize.y];
        instance._backgroundGrid = new SpriteRenderer[(int)instance._gridSize.x, (int)instance._gridSize.y];
        for (int x = 0; x < instance._gridSize.x; x++)
            for (int y = 0; y < instance._gridSize.y; y++)
            {
                GameObject bckObj = Instantiate(instance._backgroundPrefab) as GameObject;
                bckObj.name = string.Format("Bck[{0},{1}]", x, y);
                bckObj.transform.parent = instance.transform;
                bckObj.transform.localScale = Vector3.one;
                bckObj.transform.localPosition = new Vector2((0 - instance._gridSize.x / 2) + (x), (0 - instance._gridSize.y / 2) + (y));
                instance._backgroundGrid[x, y] = bckObj.GetComponent<SpriteRenderer>();
                GameObject tileObj = Instantiate(instance._tilePrefab) as GameObject;
                tileObj.name = string.Format("Tile[{0},{1}]", x, y);
                tileObj.transform.parent = instance.transform;
                tileObj.transform.localScale = Vector3.one;
                tileObj.transform.localPosition = new Vector2((0 - instance._gridSize.x / 2) + (x), (0 - instance._gridSize.y / 2) + (y));
                TileBase tile = tileObj.GetComponent<TileBase>();
                tile.Setup(instance._types[Random.Range(0,instance._types.Length)]);
                tile.Spawn(new Vector2(x, y));
                //tile.gridLocation = new Vector2(x,y);
                //tile.target = GetWorldPosition(x, y);
              //  instance._grid[x, y] = tile;
            }
    }

    [SerializeField]
    private int ptsCount = 0;

    public static void OnTileDelete(Vector2 gridPosition)
    {
        int yPos = (int)gridPosition.y;
        Destroy(instance._grid[(int)gridPosition.x, (int)gridPosition.y].gameObject);

    }

    public void CheckForDeletion()
    {
        List<TileBase> tilesToRemove = new List<TileBase>();

        List<TileBase> removeList = new List<TileBase>();
        for(int x=0;x<_gridSize.x;x++)
        {
            TileType typeBuffer = null;
            int matchBuffer = 0;
            for(int y=0;y<_gridSize.y;y++)
            {                
                if(typeBuffer != _grid[x, y].tileType)
                {
                    typeBuffer = _grid[x, y].tileType;
                    matchBuffer = 1;
                }
                else
                {
                    matchBuffer++;
                    if (matchBuffer == 3)
                    {
                        if (!removeList.Contains(_grid[x, y])) removeList.Add(_grid[x, y]);
                        if (!removeList.Contains(_grid[x, y - 1])) removeList.Add(_grid[x, y - 1]);
                        if (!removeList.Contains(_grid[x, y - 2])) removeList.Add(_grid[x, y - 2]);
                    }
                    if(matchBuffer > 3)
                    {
                        if (!removeList.Contains(_grid[x, y])) removeList.Add(_grid[x, y]);
                    }
                }
            }
        }
        for (int y = 0; y < _gridSize.y; y++)
        {
            TileType typeBuffer = null;
            int matchBuffer = 0;
            for (int x = 0; x < _gridSize.x; x++)
            {
                if (typeBuffer != _grid[x, y].tileType)
                {
                    typeBuffer = _grid[x, y].tileType;
                    matchBuffer = 1;
                }
                else
                {
                    matchBuffer++;
                    if (matchBuffer == 3)
                    {
                        if (!removeList.Contains(_grid[x, y])) removeList.Add(_grid[x, y]);
                        if (!removeList.Contains(_grid[x - 1, y])) removeList.Add(_grid[x - 1, y]);
                        if (!removeList.Contains(_grid[x - 2, y])) removeList.Add(_grid[x - 2, y]);
                    }
                    if (matchBuffer > 3)
                    {
                        if (!removeList.Contains(_grid[x, y])) removeList.Add(_grid[x, y]);
                    }
                }
            }
        }

        for(int i=0;i<removeList.Count;i++)
        {
            Core.Ammo[removeList[i].tileType].AddValue(1);
            removeList[i].Destroy();            
        }
        instance.DropDownTiles();
    }

    void Awake()
    {
        instance = this;
    }
	// Use this for initialization
	void Start () {
        StartNewGame();
	}

    public static void Grab(TileBase tile)
    {
        instance._holdingSlot = tile;
        instance._holdingSlot.target = instance._holdingSlot.transform.localPosition;
        instance._mouseHoldStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        instance._dragDirection = Vector2.zero;
        instance._holdingSlot.renderer.transform.localScale = Vector3.one * 6f;
    }

    public static void Release()
    {
        if (instance._potentialSlot != null)
        {
            if (CheckIfMovePossible(instance._holdingSlot.gridLocation, instance._potentialSlot.gridLocation))
            {
                Vector2 tmpA = instance._holdingSlot.gridLocation;
                Vector2 tmpB = instance._potentialSlot.gridLocation;
                instance._holdingSlot.SetPosition(tmpB);
                instance._potentialSlot.SetPosition(tmpA);
            }
            else
            {
                instance._holdingSlot.transform.localPosition = GetWorldPosition(instance._holdingSlot.gridLocation);
                instance._potentialSlot.transform.localPosition = GetWorldPosition(instance._potentialSlot.gridLocation);
            }
        }
        instance._holdingSlot.renderer.transform.localScale = Vector3.one * 5f;
        instance._holdingSlot = null;
        instance._potentialSlot = null;
        instance._mouseHoldStartPos = Vector3.zero;
        instance.CheckForDeletion();
    }

    public void CreateNewTile(Vector2 gridPosition)
    {
        GameObject tileObj = Instantiate(instance._tilePrefab) as GameObject;
        tileObj.name = string.Format("Tile[{0},{1}]", gridPosition.x, gridPosition.y);
        tileObj.transform.parent = instance.transform;
        tileObj.transform.localScale = Vector3.one;
        tileObj.transform.localPosition = new Vector2((0 - instance._gridSize.x / 2) + (gridPosition.x), (0 - instance._gridSize.y / 2) + (gridPosition.y));
        TileBase tile = tileObj.GetComponent<TileBase>();
        tile.Setup(instance._types[Random.Range(0, instance._types.Length)]);
        tile.Spawn(gridPosition);
    }
    private TileBase GetClosestTile(int x,int y)
    {
        for (int i = y; i < _gridSize.y;i++ )
        {
            if (_grid[x, i] != null)
                return _grid[x, i];
        }
        return null;
    }

    public void DropDownTiles()
    {        
        for (int x = 0; x < _gridSize.x; x++)
            for (int y = 0; y < _gridSize.y; y++)
            {
                if(_grid[x,y] == null)
                {
                    TileBase closest = GetClosestTile(x, y);
                    if(closest != null)
                    {
                        RemoveTile(closest.gridLocation);
                        SetTile(x, y, closest);
                        closest.target = GetWorldPosition(x, y);
                    }
                    else
                    {
                        GameObject tileObj = Instantiate(instance._tilePrefab) as GameObject;
                        tileObj.name = string.Format("Tile[{0},{1}]", x, y);
                        tileObj.transform.parent = instance.transform;
                        tileObj.transform.localScale = Vector3.one;
                        tileObj.transform.localPosition = new Vector2((0 - instance._gridSize.x / 2) + (x), (0 - instance._gridSize.y / 2) + (y));
                        TileBase tile = tileObj.GetComponent<TileBase>();
                        tile.Setup(instance._types[Random.Range(0, instance._types.Length)]);
                        tile.Spawn(new Vector2(x, y));
                    }
                }
            }
        SanityCheck();
    }

    public void SanityCheck()
    {
        for (int x = 0; x < _gridSize.x; x++)
            for (int y = 0; y < _gridSize.y; y++)
            {
                _grid[x, y].gridLocation = new Vector2(x, y);
            }
    }

    //public void FillHoles()
    //{
    //    bool hadHoles = false;
    //    for (int x = 0; x < _gridSize.x; x++)
    //        for (int y = 0; y < _gridSize.y; y++)
    //        {
    //            if (_grid[x, y] == null)
    //            {

    //            }
    //        }
    //}
    
    public bool RecursiveCheck(Vector2 tile, Vector2 direction, TileType type, int step)
    {
        Vector2 newTile = tile + direction;
        if (newTile.x >= 0 && newTile.x < _gridSize.x && newTile.y >= 0 && newTile.y < _gridSize.y)
        {
            if (_grid[(int)newTile.x, (int)newTile.y].tileType == type)
            {
                if (step == 3) return true;
                else return RecursiveCheck(newTile,direction,type,step+1);
            }
            else return false;
        }
        else return false;
    }

    public static bool CheckIfMovePossible(Vector2 tileA, Vector2 tileB)
    {
        //if(instance.RecursiveCheck(tileB, new Vector2(-1,0), instance._grid[(int)tileA.x, (int)tileA.y].tileType, 1))
        //    return true;

        //if (instance.RecursiveCheck(tileB, new Vector2(1, 0), instance._grid[(int)tileA.x, (int)tileA.y].tileType, 1))
        //    return true;

        //if (instance.RecursiveCheck(tileB, new Vector2(0, -1), instance._grid[(int)tileA.x, (int)tileA.y].tileType, 1))
        //    return true;

        //if (instance.RecursiveCheck(tileB, new Vector2(0, 1), instance._grid[(int)tileA.x, (int)tileA.y].tileType, 1))
        //    return true;

        return true;
    }

    void UpdateState()
    {
        bool someoneMoving = false;
        bool someoneDestroying = false;

        for (int i = 0; i < _gridSize.x; i++)
            for (int j = 0; j < _gridSize.y; j++)
            {
                    if (_grid[i, j] != null)
                    {
                        if (_grid[i, j]._state == TileBase.State.Falling)
                            someoneMoving = true;
                        if (_grid[i, j]._state == TileBase.State.Dying)
                            someoneDestroying = true;
                    }
            }


        if(someoneDestroying)
        {
            _state = State.Destroying;
        }
        else if(!someoneDestroying && someoneMoving)
        {
            if(_state==State.Destroying)
            {
                DropDownTiles();
            }            
            _state = State.Moving;
        }
        else if(!someoneMoving && !someoneDestroying)
        {
            if (_state == State.Destroying)
            {
                DropDownTiles();
            }
            if (_state == State.Moving)
            {
                CheckForDeletion();
            }
            _state = State.Idle;
        }
    }

    public static void SetTile(Vector2 pos, TileBase tile)
    {
        SetTile((int)pos.x, (int)pos.y, tile);
    }

    public static void SetTile(int x, int y, TileBase tile)
    {
        instance._grid[x, y] = tile;
    }

    public static void RemoveTile(Vector2 pos)
    {
        RemoveTile((int)pos.x, (int)pos.y);
    }

    public static void RemoveTile(int x, int y)
    {
        instance._grid[x, y] = null;
    }

	// Update is called once per frame
    void Update()
    {
        UpdateState();
        if (_holdingSlot != null)
        {
            _tileDelta = _mouseHoldStartPos - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _dragDirection = Vector2.zero;
            if (_tileDelta.x < -0.5f)
            {
                _dragDirection.x = -1;
            }
            if (_tileDelta.x > 0.5f)
            {
                _dragDirection.x = 1;
            }
            if (_tileDelta.y < -0.5f)
            {
                _dragDirection.y = -1;
            }
            if (_tileDelta.y > 0.5f)
            {
                _dragDirection.y = 1;
            }

            if (_prevDragDirection != _dragDirection)
            {
                if (_dragDirection.x == 0 || _dragDirection.y == 0)
                {
                    Vector2 targetPos = new Vector2(_holdingSlot.gridLocation.x - _dragDirection.x, _holdingSlot.gridLocation.y - _dragDirection.y);
                    if (targetPos.x >= 0 && targetPos.x < _gridSize.x && targetPos.y >= 0 && targetPos.y < _gridSize.y)
                    {
                        _targetPosition = targetPos;
                        _holdingSlot.Animate(targetPos);
                        if (_potentialSlot != null)
                            _potentialSlot.Animate(_potentialSlot.gridLocation);
                        _potentialSlot = _grid[(int)targetPos.x, (int)targetPos.y];
                        _potentialSlot.Animate(_holdingSlot.gridLocation);
                    }
                }
                if (_dragDirection == Vector2.zero)
                {
                    Vector2 targetPos = new Vector2(_holdingSlot.gridLocation.x - _prevDragDirection.x, _holdingSlot.gridLocation.y - _prevDragDirection.y);
                    if (targetPos.x >= 0 && targetPos.x < _gridSize.x && targetPos.y >= 0 && targetPos.y < _gridSize.y)
                    {
                        _targetPosition = Vector2.zero;
                        _holdingSlot.Animate(_holdingSlot.gridLocation);
                        _potentialSlot = _grid[(int)targetPos.x, (int)targetPos.y];
                        _potentialSlot.Animate(_grid[(int)targetPos.x, (int)targetPos.y].gridLocation);
                        _potentialSlot = null;
                    }
                }
            }

            _prevDragDirection = _dragDirection;
        }
    }
}
