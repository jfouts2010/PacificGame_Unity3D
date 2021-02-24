using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
public class TileGrid : MonoBehaviour
{
    float border = .75f;
    public int MapWidth = 20;
    public int MapHeight = 20;
    HexCellPriorityQueue searchFrontier;
    GameEngine ge;
    int searchFrontierPhase;
    public List<TileCell> cells = new List<TileCell>();
    public List<TileCell> landCells = new List<TileCell>();
    public List<TileCell> hillCells = new List<TileCell>();
    public List<TileCell> mtnCells = new List<TileCell>();
    public GameObject landPrefab;
    public GameObject waterPrefab;
    public GameObject hillPrefab;
    public GameObject mtnPrefab;
    public GameObject fogPrefab;
    public GameObject cellUIPrefab;
    [Range(0f, 0.5f)]
    public float jitterProbability = 0.25f;
    [Range(1, 100)]
    public int chunkSizeMin = 30;

    [Range(1, 100)]
    public int chunkSizeMax = 100;
    [Range(5, 95)]
    public int landPercentage = 12;
    [Range(5, 95)]
    public int hillPercentage = 35;
    [Range(5, 95)]
    public int mountainPercentage = 15;
    void Awake()
    {
        ge = GameObject.Find("GameEngine").GetComponent<GameEngine>();
        GenerateMap();
    }

    public TileCell GetCell(Vector3 hit)
    {
        return cells.First(p => p.coordinates.X == hit.x && p.coordinates.Y == hit.z);
    }
    public void GenerateMap()
    {
        if (searchFrontier == null)
        {
            searchFrontier = new HexCellPriorityQueue();
        }
        for (int x = -MapWidth / 2; x <= MapWidth / 2; x++)
            for (int y = -MapHeight / 2; y <= MapHeight / 2; y++)
            {
                CreateCell(x, y);
            }
        FindNeighbors();
        CreateLand();
        CreateHillsAndMountains();
        RenderLand();
    }
    public void RenderTileUI()
    {
        foreach(TileCell cell in cells)
        {
            GameObject cellInfo = Instantiate(cellUIPrefab);
            cellInfo.gameObject.SetActive(false);
            cellInfo.transform.SetParent(GameObject.Find("WorldSpaceCanvas").transform.Find("TileUI").transform);
            cellInfo.transform.Find("Button").GetComponent<PeoplePlacementButtonScript>().cell = cell;
            cellInfo.transform.Find("Button").GetComponent<PeoplePlacementButtonScript>().city = ge.city;
            cellInfo.transform.position = new Vector3(cell.x, 0.5f, cell.y);
            Camera.main.WorldToScreenPoint(new Vector3(cell.x, 0.5f, cell.y));
            cell.cellUI = cellInfo;
        }
    }
    public void DiscoverArea(TileCell cell, int radius, bool lookForCity = true)
    {
        var cellsInRadius = CellsInRadius(cell, radius);
        foreach (var discoverCell in cellsInRadius)
        {
            Destroy(discoverCell.Fog);
            discoverCell.fogged = false;
        }
        Destroy(cell.Fog);
        cell.fogged = false;

    }
    public List<TileCell> CellsInRadius(TileCell cell, int radius)
    {
        return cells.Where(p => p.x >= cell.x - radius && p.x <= cell.x + radius && p.y >= cell.y - radius && p.y <= cell.y + radius).ToList();
    }
    void CreateLand()
    {
        int cellCount = MapWidth * MapWidth;
        //start with large chuck, every chunk after builds off central
        LandChunkGen(Random.Range(25, 31 + 1), true);
        float currentLandPerc = ((float)landCells.Count / (float)cells.Count) * 100;
        int maxRuns = 20;
        int runs = 0;
        while (currentLandPerc < landPercentage)
        {
            LandChunkGen(
                Random.Range(chunkSizeMin, chunkSizeMax + 1), false
            );
            currentLandPerc = ((float)landCells.Count / (float)cells.Count) * 100;
            if (runs++ > maxRuns)
                break;
        }

        //get coastalCells
        /*foreach (HexCell cell in landcells)
            foreach (HexCell neigh in cell.neighbors)
            {
                if (neigh.landType == LandType.Water && !coastalcells.Contains(neigh))
                    coastalcells.Add(neigh);
            }*/
    }
    public void RenderLand()
    {
        foreach (TileCell cell in cells)
        {
            GameObject go2 = Instantiate(fogPrefab);
            go2.transform.position = new Vector3(cell.x, 0.1f, cell.y);
            go2.transform.SetParent(this.transform);
            cell.Fog = go2;
            if (cell.type == TerrainType.Grassland)
            {
                if (cell.feature == TerrainFeature.Hills)
                {
                    GameObject go = Instantiate(hillPrefab);
                    go.transform.position = new Vector3(cell.x, 0, cell.y);
                    go.transform.SetParent(this.transform);
                }
                else if (cell.feature == TerrainFeature.Mountains)
                {
                    GameObject go = Instantiate(mtnPrefab);
                    go.transform.position = new Vector3(cell.x, 0, cell.y);
                    go.transform.SetParent(this.transform);
                }
                else
                {
                    GameObject go = Instantiate(landPrefab);
                    go.transform.position = new Vector3(cell.x, 0, cell.y);
                    go.transform.SetParent(this.transform);
                }
            }
            if (cell.type == TerrainType.Ocean)
            {
                GameObject go = Instantiate(waterPrefab);
                go.transform.position = new Vector3(cell.x, 0, cell.y);
                go.transform.SetParent(this.transform);
            }
           
        }
    }
    public void LandChunkGen(int chunkSize, bool firstChunk)
    {
        searchFrontierPhase += 1;
        TileCell firstCell = firstChunk ? FindRandom(.75f) : GetWaterCellWithinChunk();
        firstCell.SearchPhase = searchFrontierPhase;
        firstCell.Distance = 0;
        firstCell.SearchHeuristic = 0;
        searchFrontier.Enqueue(firstCell);
        TileCoordinates center = firstCell.coordinates;
        int size = 0;
        while (size < chunkSize && searchFrontier.Count > 0)
        {
            TileCell current = searchFrontier.Dequeue();
            if (current.type == TerrainType.Ocean)
            {
                landCells.Add(current);
                current.type = TerrainType.Grassland;
                size += 1;
                foreach (TileCell neighbor in current.neighbors)
                {
                    if (neighbor.SearchPhase < searchFrontierPhase)
                    {
                        neighbor.SearchPhase = searchFrontierPhase;
                        neighbor.Distance = (int)neighbor.coordinates.DistanceTo(center);
                        neighbor.SearchHeuristic =
                            Random.value < jitterProbability ? 1 : 0;
                        searchFrontier.Enqueue(neighbor);
                    }
                }
            }
        }
        searchFrontier.Clear();
    }
    void CreateHillsAndMountains()
    {
        int cellCount = MapWidth * MapHeight;
        float currentHillPerc = ((float)hillCells.Count / (float)cells.Count) * 100;
        int maxRuns = 20;
        int runs = 0;
        while (currentHillPerc < hillPercentage)
        {
            HillMtnChunk(
                Random.Range(2, 5 + 1), TerrainFeature.Hills
            );
            currentHillPerc = ((float)hillCells.Count / (float)landCells.Count) * 100;
            if (runs++ > maxRuns)
                break;
        }
        float currentMtnPerc = ((float)mtnCells.Count / (float)landCells.Count) * 100;
        maxRuns = 20;
        runs = 0;
        while (currentMtnPerc < mountainPercentage)
        {
            HillMtnChunk(
                Random.Range(1, 3 + 1), TerrainFeature.Mountains
            );
            currentMtnPerc = ((float)mtnCells.Count / (float)landCells.Count) * 100;
            if (runs++ > maxRuns)
                break;
        }
    }
    public void HillMtnChunk(int chunkSize, TerrainFeature feature)
    {
        searchFrontierPhase += 1;
        TileCell firstCell = GetLandCellOfType((feature == TerrainFeature.Hills ? TerrainFeature.None : TerrainFeature.Hills));
        firstCell.SearchPhase = searchFrontierPhase;
        firstCell.Distance = 0;
        firstCell.SearchHeuristic = 0;
        searchFrontier.Enqueue(firstCell);
        TileCoordinates center = firstCell.coordinates;
        int size = 0;
        while (size < chunkSize && searchFrontier.Count > 0)
        {
            TileCell current = searchFrontier.Dequeue();
            if (current.feature == (feature == TerrainFeature.Hills ? TerrainFeature.None : TerrainFeature.Hills))
            {
                if (feature == TerrainFeature.Hills)
                    hillCells.Add(current);
                else
                {
                    mtnCells.Add(current);
                    hillCells.Remove(current);
                }
                //current.color = type == LandType.Hills ? Color.yellow : Color.grey;
                current.feature = feature;
                size += 1;

                foreach (TileCell neighbor in current.neighbors)
                {
                    if (neighbor.SearchPhase < searchFrontierPhase)
                    {
                        neighbor.SearchPhase = searchFrontierPhase;
                        neighbor.Distance = (int)neighbor.coordinates.DistanceTo(center);
                        neighbor.SearchHeuristic =
                            Random.value < jitterProbability ? 1 : 0;
                        searchFrontier.Enqueue(neighbor);
                    }
                }
            }
        }
        searchFrontier.Clear();
    }
    TileCell GetLandCellOfType(TerrainFeature feature = TerrainFeature.None)
    {
        var availableCells = landCells.Where(p => p.feature == feature).ToList();
        return availableCells[Random.Range(0, availableCells.Count())];
    }
    public TileCell FindRandom(float borderPerc)
    {
        int x = (int)Random.Range((-MapWidth / 2) * borderPerc, (MapWidth / 2) * borderPerc);
        int y = (int)Random.Range((-MapWidth / 2) * borderPerc, (MapWidth / 2) * borderPerc);
        return cells.First(p => p.x == x && p.y == y);
    }
    TileCell GetWaterCellWithinChunk()
    {
        //get cells with water neighbors
        var availableCells = landCells.Where(p => p.neighbors.Any(p2 => p2.type == TerrainType.Ocean) && p.x > -(MapWidth / 2) * border && p.x < (MapWidth / 2) * border && p.y > -(MapHeight / 2) * border && p.y < (MapHeight / 2) * border).ToList();
        TileCell choosenCell = availableCells[Random.Range(0, availableCells.Count())];
        TileCell neighborWaterCell = choosenCell.neighbors.First(p2 => p2.type == TerrainType.Ocean);
        return neighborWaterCell;
    }
    public void FindNeighbors()
    {
        foreach (TileCell cell in cells)
        {
            for (int x = -1; x <= 1; x++)
                for (int y = -1; y <= 1; y++)
                {
                    TileCell neighborCell = cells.FirstOrDefault(p => p.x == cell.x + x && p.y == cell.y + y);
                    if (neighborCell != null && neighborCell != cell)
                        cell.neighbors.Add(neighborCell);
                }
        }
    }
    public void CreateCell(int x, int y)
    {
        TileCell newCell = new TileCell()
        {
            coordinates = new TileCoordinates() { X = x, Y = y },
            type = TerrainType.Ocean,
            feature = TerrainFeature.None
        };
        cells.Add(newCell);
    }
}
