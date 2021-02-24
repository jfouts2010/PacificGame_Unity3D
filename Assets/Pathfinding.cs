using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    List<PathNode> openList;
    List<PathNode> closedList;
    public TileGrid grid;
    // Start is called before the first frame update
    void Start()
    {
        grid = GameObject.Find("Grid").GetComponent<TileGrid>();
    }

   public List<TileCell> FindPath(TileCell start, TileCell end, bool ocean)
    {
        PathNode startNode = new PathNode() { cell = start };
        PathNode endNode = new PathNode() { cell = end };
        openList = new List<PathNode>() { startNode};
        closedList = new List<PathNode>();
        List<PathNode> allNodes = new List<PathNode>();
        allNodes.Add(startNode);
        allNodes.Add(endNode);
        foreach (TileCell cell in grid.cells)
        {
            if (cell == start || cell == end)
                continue;
            PathNode pathNode = new PathNode() { cell = cell, G = 999999999 };
            allNodes.Add(pathNode);
        }
        startNode.G = 0;
        startNode.H = TravelCost(start, end);
        endNode.G = 999999999;

        while (openList.Count > 0)
        {
            PathNode currentNode = openList.OrderBy(p => p.F).First();
            if (currentNode.cell == end)
                return CreateFinalPath(endNode, startNode);
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach(TileCell neighborCell in currentNode.cell.neighbors)
            {
               /* if (ocean && neighborCell.type != TerrainType.Ocean)
                    continue;
                else if (!ocean && neighborCell.type == TerrainType.Ocean)
                    continue;*/
                PathNode neighborNode = allNodes.First(p => p.cell == neighborCell);
                if (closedList.Any(p => p.cell == neighborCell))
                    continue;
                int G = currentNode.G + TravelCost(currentNode.cell, neighborCell);
                if (currentNode.cell.type != neighborCell.type && neighborCell.Fog == null)
                    G += 40;
                if(G < neighborNode.G)
                {
                    neighborNode.prevNode = currentNode;
                    neighborNode.G = G;
                    neighborNode.H = TravelCost(neighborCell, end);

                    if (!openList.Contains(neighborNode))
                        openList.Add(neighborNode);
                }
            }
        }
        return null;
    }
    public List<TileCell> CreateFinalPath(PathNode endNode, PathNode startNode)
    {
        List<TileCell> path = new List<TileCell>();
        path.Add(endNode.cell);
        PathNode currentNode = endNode;
        while (currentNode.prevNode != null &&currentNode.prevNode.cell != startNode.cell)
        {
            path.Add(currentNode.prevNode.cell);
            currentNode = currentNode.prevNode;
        }
        path.Reverse();
        return path;
    }
    public int TravelCost(TileCell a, TileCell b)
    {
        int x = Mathf.Abs(a.x - b.x);
        int y = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(x - y);
        return 14 * Mathf.Min(x, y) + 10 * remaining;
    }
}
public class PathNode
{
    public TileCell cell;
    public int H;
    public int G;
    public int F { get { return H + G; } }
    public PathNode prevNode;
}
