using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Settler : Unit
{
    void Update()
    {
        pathCordsDisplay = path.Select(p => p.x.ToString() + "," + p.y.ToString()).ToList();
        if (selected && cell.type == TerrainType.Grassland)
        {
            GameObject go = GameObject.Find("Canvas").transform.Find(type.ToString()).gameObject;
            go.SetActive(true);
            Button b = go.GetComponent<Button>();

        }
        else
        {
            GameObject go = GameObject.Find("Canvas").transform.Find(type.ToString()).gameObject;
            go.SetActive(false);
        }
    }
}
