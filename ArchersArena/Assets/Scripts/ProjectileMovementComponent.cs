using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovementComponent : UsingOnUpdateBase
{
    // Start is called before the first frame update
    private Rigidbody2D m_projectileRB;
    private GameObject m_owner;
    private Action<Collision2D> m_onTargetHitDelegate;
    public void SetupComponent(Rigidbody2D projectileRB, GameObject owner = null, float mass = 1, float gravityScale = 1)
    {
        m_owner = owner;
        m_projectileRB = projectileRB;
        m_projectileRB.mass = mass;
        m_projectileRB.gravityScale = gravityScale;
        Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), m_owner.GetComponent<Collider2D>());
    }
    public void FireAtDirection(Vector2 direction, float velocity, Action<Collision2D> onTargetHitDelegate = null)
    {
        m_onTargetHitDelegate = onTargetHitDelegate;
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
        if(col.gameObject.tag ==  "Arrow")
        {
            m_onTargetHitDelegate.Invoke(col);
            return;
        }
        RemoveActionFromFixedUpdate(UpdateRotation);
        m_projectileRB.velocity = new Vector2(0f,0f);
        if (m_owner != null)
        {
            Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), m_owner.GetComponent<Collider2D>(), false);
        }

        m_onTargetHitDelegate.Invoke(col);
    }

}
