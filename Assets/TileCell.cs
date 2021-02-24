using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileCell 
{
    public TileCoordinates coordinates;
    public Unit unit = null;
    public int x { get { return coordinates.X; } }
    public int y { get { return coordinates.Y; } }
    public TerrainType type;
    public TerrainFeature feature;
    public bool river = false;
    public List<TileCell> neighbors = new List<TileCell>();
    public int SearchHeuristic { get; set; }
    public int SearchPhase { get; set; }
    public TileCell NextWithSamePriority { get; set; }
    public GameObject Fog;
    public TileImprovments improvement = TileImprovments.None;
    public Zones zone = Zones.None;
    public GameObject cellUI;
    int distance;
    public bool fogged = true;
    public float tileFoodProduction
    {
        get
        {
            return 1 + (improvement == TileImprovments.Farm ? 2 : 0) +
                //if 2 neighbors have farms then +2 food
                (improvement == TileImprovments.Farm ? (NeighborsWithImprovement(TileImprovments.Farm) >= 2 ? 2 : 0) : 0) +
                (improvement == TileImprovments.Pasture ? 2 : 0) +
                (improvement == TileImprovments.Fisher ? 1 : 0) + 
                //commerce zone gives +2 food if on a river, +2 for each adj river, and +1 for each adjacent civic zone
                (zone == Zones.Commerce ? (river ? 2 : 0) + (NeighborsWithRiver() * 2) + NeighborsWithZone(Zones.Civic) : 0) +
                //warehouses give +1 food for each adj farm or pasture
                (zone == Zones.Warehouse ? NeighborsWithImprovement(TileImprovments.Farm) + NeighborsWithImprovement(TileImprovments.Pasture) : 0);
        }
    }
    public float tileProduction
    {
        get
        {
            return 0 + (improvement == TileImprovments.Mine ? 2 : 0) +
                //watermill provides +0.5 production per each adjacent tile and +.5 if on river
                (improvement == TileImprovments.Watermill ?((river ? 0.5f : 0) + Mathf.Min(2, NeighborsWithRiver() * 0.5f)) : 0) +
                (improvement == TileImprovments.WoodCutter ? 1 : 0) + 
                //+2 prod for each adj comm zone
                (zone == Zones.Commerce ? NeighborsWithZone(Zones.Commerce) * 2: 0) +
                (zone == Zones.Industrial ? NeighborsWithZone(Zones.Commerce) : 0) +
                (zone == Zones.Industrial ? NeighborsWithZone(Zones.Industrial) * 2 : 0) +
                (zone == Zones.Industrial ? NeighborsWithImprovement(TileImprovments.Mine) : 0) -
                (zone == Zones.Industrial ? NeighborsWithZone(Zones.Civic) : 0) +
                (zone == Zones.Warehouse ? NeighborsWithImprovement(TileImprovments.Mine) : 0);
        }
    }
    public int NeighborsWithZone(Zones zone)
    {
        return neighbors.Count(p => p.zone == zone);
    }
    public int NeighborsWithImprovement(TileImprovments improvment)
    {
        return neighbors.Count(p => p.improvement == improvement);
    }
    public int NeighborsWithRiver()
    {
        return neighbors.Count(p => p.river);
    }
    public int Distance
    {
        get
        {
            return distance;
        }
        set
        {
            distance = value;
        }
    }
    public int SearchPriority
    {
        get
        {
            return distance + SearchHeuristic;
        }
    }
    public void SetUnit(GameObject thisUnit)
    {
        unit = thisUnit.GetComponent<Unit>();
    }
    public void SetUnit(Unit thisUnit)
    {
        unit = thisUnit;
    }
}
public enum TerrainType
{
    Ocean,
    Grassland
}
public enum TerrainFeature
{
    Hills,
    Mountains,
    Coast,
    None
}
