using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PeoplePlacementButtonScript : MonoBehaviour
{
    Button button;
    public TileCell cell;
    public City city;
    public void Start()
    {
        button = GetComponent<Button>();
    }
    public void PersonButtonClick()
    {
        foreach(var cell in city.peoplePlaceableCells)
        {
            cell.cellUI.transform.Find("Button").GetComponent<Image>().color = Color.white;
        }
        if (city.workedTiles.Contains(cell))
        {
            city.workedTiles.Remove(cell);
        }
        else
        {
            city.workedTiles.Add(cell);
            if (city.workedTiles.Count > city.population)
            {
                city.workedTiles.RemoveAt(0);
            }
        }
        foreach(var cell in city.workedTiles)
        {
            cell.cellUI.transform.Find("Button").GetComponent<Image>().color = Color.blue;
        }
    }
}
