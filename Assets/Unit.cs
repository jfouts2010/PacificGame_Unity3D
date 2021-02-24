using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public TileCell cell;
    TileGrid grid;
    public List<TileCell> path = new List<TileCell>();
    public List<string> pathCordsDisplay = new List<string>();
    public int MAX_MOVEMENT = 4;
    public int VIEW_RADIUS = 2;
    public int movementLeft;
    public bool selected = false;
    public UnitTypes type;
    // Start is called before the first frame update
    void Start()
    {
        grid = GameObject.Find("Grid").GetComponent<TileGrid>();
        movementLeft = MAX_MOVEMENT;
    }

    // Update is called once per frame
    void Update()
    {
        pathCordsDisplay = path.Select(p => p.x.ToString() + "," + p.y.ToString()).ToList();
    }
    public void MoveUnit(TileCell thisCell)
    {
        cell = thisCell;
        this.gameObject.transform.position = new Vector3(thisCell.x, 0, thisCell.y);
    }
    public void MoveToNextTileInPath()
    {
        MoveUnit(path.First());
        movementLeft--;
        grid.DiscoverArea(path.First(),2);
        path.RemoveAt(0);

    }
    public void MaxUnitMove()
    {
        while (movementLeft > 0 && path.Count > 0)
        {
            //try to move
            MoveToNextTileInPath();
        }
    }
    public void SetTileCell(TileCell thisCell)
    {
        cell = thisCell;
    }
    public enum UnitTypes
    {
        Worker,
        Settler,
        Explorer
    }
}
