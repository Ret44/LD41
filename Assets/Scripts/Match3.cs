using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match3 : MonoBehaviour {

    public static Match3 instance;
       
    [SerializeField]
    private TileBase _holdingSlot;

 //   private Vector3

    private Vector3 _mouseHoldStartPos;

    [SerializeField]
    private Vector2 _tileDelta;

    [SerializeField]
    private GameObject _tilePrefab;

    [SerializeField]
    private TileType[] _types;

    [SerializeField]
    private Vector2 _gridSize;

    private TileBase[,] _grid;

    public static void StartNewGame()
    {
        instance._grid = new TileBase[(int)instance._gridSize.x, (int)instance._gridSize.y];
        for (int x = 0; x < instance._gridSize.x; x++)
            for (int y = 0; y < instance._gridSize.y; y++)
            {
                GameObject tileObj = Instantiate(instance._tilePrefab) as GameObject;
                tileObj.name = string.Format("Tile[{0},{1}]", x, y);
                tileObj.transform.parent = instance.transform;
                tileObj.transform.localPosition = new Vector2((0 - instance._gridSize.x / 2) + (x), (0 - instance._gridSize.y / 2) + (y));
                TileBase tile = tileObj.GetComponent<TileBase>();
                tile.Setup(instance._types[Random.Range(0,instance._types.Length)]);
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
        instance._mouseHoldStartPos = Input.mousePosition;
    }

    public static void Release()
    {
        instance._holdingSlot = null;
        instance._mouseHoldStartPos = Vector3.zero;
    }


	// Update is called once per frame
	void Update () {
		if(_holdingSlot !=null)
        {
            _tileDelta = _mouseHoldStartPos - Input.mousePosition;
            _holdingSlot.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
	}
}
