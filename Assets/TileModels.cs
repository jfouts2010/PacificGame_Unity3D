using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCoordinates
{
    public int X { get; set; }
    public int Y { get; set; }
    public float DistanceTo(TileCoordinates cord)
    {
        return Mathf.Sqrt(Mathf.Pow(cord.X - X, 2) + Mathf.Pow(cord.Y - Y, 2));
    }
}
public class HexCellPriorityQueue
{

    List<TileCell> list = new List<TileCell>();

    int count = 0;
    int minimum = int.MaxValue;

    public int Count
    {
        get
        {
            return count;
        }
    }

    public void Enqueue(TileCell cell)
    {
        count += 1;
        int priority = cell.SearchPriority;
        if (priority < minimum)
        {
            minimum = priority;
        }
        while (priority >= list.Count)
        {
            list.Add(null);
        }
        cell.NextWithSamePriority = list[priority];
        list[priority] = cell;
    }

    public TileCell Dequeue()
    {
        count -= 1;
        for (; minimum < list.Count; minimum++)
        {
            TileCell cell = list[minimum];
            if (cell != null)
            {
                list[minimum] = cell.NextWithSamePriority;
                return cell;
            }
        }
        return null;
    }

    public void Change(TileCell cell, int oldPriority)
    {
        TileCell current = list[oldPriority];
        TileCell next = current.NextWithSamePriority;
        if (current == cell)
        {
            list[oldPriority] = next;
        }
        else
        {
            while (next != cell)
            {
                current = next;
                next = current.NextWithSamePriority;
            }
            current.NextWithSamePriority = cell.NextWithSamePriority;
        }
        Enqueue(cell);
        count -= 1;
    }

    public void Clear()
    {
        list.Clear();
        count = 0;
        minimum = int.MaxValue;
    }
}
