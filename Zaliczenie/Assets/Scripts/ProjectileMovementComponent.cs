using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovementComponent : UsingOnUpdateBase
{
    // Start is called before the first frame update
    private Rigidbody2D m_projectileRB;
    private GameObject m_owner;
    public void SetupComponent(Rigidbody2D projectileRB, GameObject owner = null, float mass = 1, float gravityScale = 1)
    {
        m_owner = owner;
        m_projectileRB = projectileRB;
        m_projectileRB.mass = mass;
        m_projectileRB.gravityScale = gravityScale;
        Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), m_owner.GetComponent<Collider2D>());
    }
    public void FireAtDirection(Vector2 direction, float velocity)
    {
        m_projectileRB.velocity = direction * velocity;
        AddActionOnFixedUpdate(UpdateRotation);
    }
    void UpdateRotation()
    {
        var direction = m_projectileRB.velocity;
        direction.Normalize();
        m_projectileRB.transform.up = direction;
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        RemoveActionFromFixedUpdate(UpdateRotation);
        m_projectileRB.isKinematic = true;
        m_projectileRB.freezeRotation = true;
        m_projectileRB.velocity = new Vector2(0f,0f);
    }

}
