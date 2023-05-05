using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class ProjectileBase : UsingOnUpdateBase
{
    public bool bCanBePickedUp = false;
    // Start is called before the first frame update
    private ProjectileMovementComponent m_projectileMovementComponent;
    private Rigidbody2D m_rigidbody;
    
    [SerializeField]
    float m_maxVelocity = 20f;
    [SerializeField]
    AudioClip m_arrowGrassHit;
    [SerializeField]
    AudioClip m_playerHit;
    GameObject m_owner;
    public void FireAtDirection(Vector2 direction, float velocity, GameObject owner)
    {
        m_owner = owner;
        direction.Normalize();
        m_rigidbody = GetComponent<Rigidbody2D>(); 
        m_projectileMovementComponent = GetComponent<ProjectileMovementComponent>();

        m_projectileMovementComponent.SetupComponent(m_rigidbody, owner, 1f,0.1f);
        m_projectileMovementComponent.FireAtDirection(direction, Mathf.Clamp(velocity, 0f, m_maxVelocity), AtTargetHit);
    }
    void AtTargetHit(Collision2D col)
    {
        if (col.gameObject.tag == "Arrow")
        {
            PlayAudio(m_arrowGrassHit);
            bCanBePickedUp = true;
            m_rigidbody.velocity = new Vector2(0, 0);
            return;
        }
        if (!bCanBePickedUp)
        {
            var killable = col.gameObject.GetComponent<IKillable>();
            if(killable != null)
            {
                PlayAudio(m_playerHit);
                bCanBePickedUp = true;
                killable.OnHitted();
                return;

            }
        }
        PlayAudio(m_arrowGrassHit);
        m_rigidbody.freezeRotation = true;
        m_rigidbody.isKinematic = true;
        bCanBePickedUp = true;
        GetComponent<Collider2D>().isTrigger = true;

    }
    private void PlayAudio(AudioClip audio)
    {
        if(audio != null)
        {
            AudioSource.PlayClipAtPoint(audio, new Vector3(-m_rigidbody.position.x, 0, m_rigidbody.position.y));
        }

    }
}
