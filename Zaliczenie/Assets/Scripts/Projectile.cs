using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : UsingOnUpdateBase
{
    // Start is called before the first frame update
    private ProjectileMovementComponent m_projectileMovementComponent;
    private Rigidbody2D m_rigidbody;
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>(); 
        if(m_rigidbody == null)
        {
            return;
        }
        m_projectileMovementComponent = GetComponent<ProjectileMovementComponent>();
        m_projectileMovementComponent.SetupComponent(m_rigidbody);
        m_projectileMovementComponent.FireAtDirection(new Vector2(1f, 0f), 5);
    }
}
