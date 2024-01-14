using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurret : EnemyBase
{
    [SerializeField]
    private Projectile projectile;
    [SerializeField]
    private Transform firePoint;
    [SerializeField]
    private float timeBetweenShoots = 1.0f;
    private Transform target;
    private float lastTimeShooting = 0.0f;

    void Start()
    {
        
    }

    void Update()
    {
        if (Time.time - lastTimeShooting > timeBetweenShoots)
        {
            lastTimeShooting = Time.time;
            Attack();
        }
    }
    protected override void Attack()
    {
        if (!target)
        {
            //return;
        }
        if (!projectile || !firePoint)
        {
            return;
        }
        Projectile projectileData = Instantiate(projectile, firePoint.position, firePoint.rotation);
        projectileData.Initialize(transform.forward);

    }
    
}
