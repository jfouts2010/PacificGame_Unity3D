using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            Camera.main.orthographicSize += -Input.GetAxis("Mouse ScrollWheel") * 5;
        }
        float movementspeed = 0.5f;
        if (Input.GetKey(KeyCode.A))
            transform.position += new Vector3(-movementspeed, 0, 0);
        if (Input.GetKey(KeyCode.D))
            transform.position += new Vector3(movementspeed, 0, 0);
        if (Input.GetKey(KeyCode.W))
            transform.position += new Vector3(0, 0, movementspeed);
        if (Input.GetKey(KeyCode.S))
            transform.position += new Vector3(0, 0, -movementspeed);
    }
}
