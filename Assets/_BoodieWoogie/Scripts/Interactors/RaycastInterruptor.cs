using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastInterruptor : MonoBehaviour
{
    public LayerMask layers;
    public float length = 20.0f;
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D ray;
        ray = Physics2D.Raycast(transform.position, Vector2.right, length, layers);
        if (ray.collider == null)
        {
            Debug.DrawLine(transform.position, Vector3.right * length + transform.position, Color.green);
            return;
        }
        Debug.DrawLine(transform.position, ray.point, Color.red);
    }
}
