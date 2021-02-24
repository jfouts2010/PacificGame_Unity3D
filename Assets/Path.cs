using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    public List<TileCell> path = new List<TileCell>();
    public List<GameObject> pathGameObjects = new List<GameObject>();
    public GameObject pathPrefab;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(path.Count < pathGameObjects.Count)
        {
            for(int i = path.Count; i < pathGameObjects.Count; i++)
            {
                Destroy(pathGameObjects[i]);
                pathGameObjects.RemoveAt(i);
            }
        }
        //check to see if pathgameobjects matchs path
        for (int i = 0; i < path.Count; i++)
        {
            //does go exist?
            if (pathGameObjects.Count - 1 >= i)
            {
                //check if they are the same continue, else move it
                if (!ComparePathAndPathGO(path[i], pathGameObjects[i]))
                {
                    pathGameObjects[i].transform.position = new Vector3(path[i].x, 0.1f, path[i].y);
                }
            }
            else
            {
                GameObject go = Instantiate(pathPrefab);
                go.transform.SetParent(this.transform);
                pathPrefab.transform.position = new Vector3(path[i].x, 0.1f, path[i].y);
                pathGameObjects.Add(go);
            }
        }
    }
    public bool ComparePathAndPathGO(TileCell cell, GameObject go)
    {
        return cell.x == go.transform.position.x && cell.y == go.transform.position.z;
    }
}
