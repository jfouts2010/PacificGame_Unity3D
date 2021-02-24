using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class GameEngine : MonoBehaviour
{
    TileGrid grid;
    List<Unit> units = new List<Unit>();
    public City city;
    public GameObject unitPrefab;
    public Unit selectedUnit;
    Pathfinding pathfinder;
    public GameObject PathGameObject;
    public GameObject PathGOPrefab;
    public GameObject cityCenterPrefab;
    // Start is called before the first frame update
    void Start()
    {
        city = GameObject.Find("City").GetComponent<City>();
        pathfinder = GameObject.Find("Pathfinder").GetComponent<Pathfinding>();
        grid = GameObject.Find("Grid").GetComponent<TileGrid>();
        GameObject goUnit = Instantiate(unitPrefab);
        goUnit.transform.position = new Vector3(0, 0, 0);
        TileCell unitsCell = grid.cells.First(p => p.x == 0 && p.y == 0);
        unitsCell.SetUnit(goUnit);
        goUnit.GetComponent<Unit>().SetTileCell(unitsCell);
        units.Add(goUnit.GetComponent<Unit>());
        grid.DiscoverArea(unitsCell, 2);
        grid.RenderTileUI();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            HandleInputLeft();
        }
        if (Input.GetMouseButton(1) && selectedUnit != null)
        {
            HandleInputRight();
        }
        else if (Input.GetMouseButtonUp(1) && selectedUnit != null && PathGameObject != null && PathGameObject.GetComponent<Path>().path != null && PathGameObject.GetComponent<Path>().path.Count > 0)
        {
            selectedUnit.path = PathGameObject.GetComponent<Path>().path;
            selectedUnit.MaxUnitMove();
        }
        else if (PathGameObject != null)
        {
            Destroy(PathGameObject);
            PathGameObject = null;
        }
    }
    void HandleInputRight()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            {
                hit.point = transform.InverseTransformPoint(hit.point);
                TileCell currentCell = grid.GetCell(hit.transform.position);
                List<TileCell> path = pathfinder.FindPath(selectedUnit.cell, currentCell, true);//cells[Random.Range(0, cells.Count)], true);
                if (PathGameObject == null)
                {
                    PathGameObject = Instantiate(PathGOPrefab);
                }
                PathGameObject.GetComponent<Path>().path = path;
            }
        }
    }
    public void CityStatsClick()
    {
        Debug.Log("test");
    }
    void HandleInputLeft()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            hit.point = transform.InverseTransformPoint(hit.point);
            TileCell currentCell = grid.GetCell(hit.transform.position);
            if (currentCell.unit != null)
                SelectUnit(currentCell.unit);
        }
    }
    public void Settle()
    {
        if (selectedUnit != null && selectedUnit.type == Unit.UnitTypes.Settler)
        {
            TileCell cell = selectedUnit.cell;
            GameObject go = Instantiate(cityCenterPrefab);
            go.transform.position = new Vector3(cell.x, 0, cell.y);
            grid.DiscoverArea(cell, 3, false);
            DeleteUnit(selectedUnit);
            city.ActivateCity();
            GameObject.Find("Canvas").transform.Find("Settler").gameObject.SetActive(false);
            city.cell = cell;
            city.PlaceZone(cell, Zones.Civic);
        }
    }
    public void SelectUnit(Unit unit)
    {
        if (selectedUnit != null)
            selectedUnit.selected = false;
        selectedUnit = unit;
        selectedUnit.selected = true;
    }
    public void DeleteUnit(Unit unit)
    {
        units.Remove(unit);
        Destroy(selectedUnit.gameObject);
        selectedUnit = null;
    }
    public void EndTurn()
    {
        MoveUnits();
        if (city != null)
        {
            city.EndTurn();
        }
    }
    public void MoveUnits()
    {
        foreach (Unit unit in units)
        {
            //for now assume each tile is 1 cost
            unit.MaxUnitMove();
            unit.movementLeft = unit.MAX_MOVEMENT;
        }
    }
}
