using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class candy : MonoBehaviour
{
    public float x, y;
    public float speed = 3.0f; // Hýz deðeri, bu deðeri istediðiniz gibi deðiþtirebilirsiniz

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(x, y, 0), Time.deltaTime * speed);
    }
}
