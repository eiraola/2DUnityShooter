using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    public float speed = 10.0f;
    private Vector3 direction = Vector3.zero;
    private bool initialized = false;
    void Update()
    {
        direction = transform.right;
        initialized = true;
        Travel();
    }
    public void Initialize(Vector3 newDirection)
    {
        direction = newDirection;
        initialized = true;
    }
    private void Travel()
    {
        if (!initialized)
        {
            return;
        }
        transform.position += direction.normalized * speed * Time.deltaTime;
    }
}
