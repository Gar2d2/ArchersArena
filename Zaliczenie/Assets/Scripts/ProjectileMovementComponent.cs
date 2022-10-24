using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovementComponent : UsingOnUpdateBase
{
    // Start is called before the first frame update
    private Rigidbody2D m_projectileRB;
    public void SetupComponent(Rigidbody2D projectileRB, float mass = 1, float gravityScale = 1)
    {
        m_projectileRB = projectileRB;
        m_projectileRB.mass = mass;
        m_projectileRB.gravityScale = gravityScale;
    }
    public void FireAtDirection(Vector2 direction, float velocity)
    {
        m_projectileRB.velocity = direction * velocity;
        //AddActionOnFixedUpdate(UpdateMovement);
    }
    //void UpdateMovement()
    //{
    //    Vector2 velocity = m_projectileRB.velocity;
    //    velocity.x = 5;
    //    m_projectileRB.velocity = velocity;

    //}

    void Start()
    {
        
    }

}
