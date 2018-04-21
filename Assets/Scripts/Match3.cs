using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match3 : MonoBehaviour {

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
                bckObj.transform.localPosition = new Vector2((0 - instance._gridSize.x / 2) + (x), (0 - instance._gridSize.y / 2) + (y));
                instance._backgroundGrid[x, y] = bckObj.GetComponent<SpriteRenderer>();
                GameObject tileObj = Instantiate(instance._tilePrefab) as GameObject;
                tileObj.name = string.Format("Tile[{0},{1}]", x, y);
                tileObj.transform.parent = instance.transform;
                tileObj.transform.localPosition = new Vector2((0 - instance._gridSize.x / 2) + (x), (0 - instance._gridSize.y / 2) + (y));
                TileBase tile = tileObj.GetComponent<TileBase>();
                tile.Setup(instance._types[Random.Range(0,instance._types.Length)]);
                tile.gridLocation = new Vector2(x,y);
                tile.target = GetWorldPosition(x, y);
                instance._grid[x, y] = tile;
            }
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
     //   instance._holdingSlot.transform.localPosition = GetWorldPosition(instance._holdingSlot.gridLocation);
        instance._holdingSlot.renderer.transform.localScale = Vector3.one * 5f;
        instance._holdingSlot = null;
        instance._potentialSlot = null;
        instance._mouseHoldStartPos = Vector3.zero;
    }

    public static bool CheckIfMovePossible(Vector2 tileA, Vector2 tileB)
    {
        return true;
    }

	// Update is called once per frame
	void Update () {
		if(_holdingSlot !=null)
        {
            _tileDelta = _mouseHoldStartPos - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _dragDirection = Vector2.zero;
            if(_tileDelta.x < -0.5f)
            {
                _dragDirection.x = -1;
            }
            if(_tileDelta.x > 0.5f)
            {
                _dragDirection.x = 1;
            }
            if(_tileDelta.y < -0.5f)
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
                if(_dragDirection == Vector2.zero)
                {
                    Vector2 targetPos = new Vector2(_holdingSlot.gridLocation.x - _prevDragDirection.x, _holdingSlot.gridLocation.y - _prevDragDirection.y);
                    _targetPosition = Vector2.zero;
                    _holdingSlot.Animate(_holdingSlot.gridLocation);
                    _potentialSlot = _grid[(int)targetPos.x, (int)targetPos.y];
                    _potentialSlot.Animate(_grid[(int)targetPos.x, (int)targetPos.y].gridLocation);
                    _potentialSlot = null;
                }
            }

            _prevDragDirection = _dragDirection;
        }
	}
}
