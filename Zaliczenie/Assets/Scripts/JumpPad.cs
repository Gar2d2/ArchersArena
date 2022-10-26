using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
 
    private bool bCanBeUsedAgain = true;
    private Animator m_Animator;
    Vector2 m_ejectDirection = new Vector2 (0f, 0f);
    // Start is called before the first frame update
    [SerializeField]
    private float m_jumpForce = 20f;
    void Start()
    {
        var collider = GetComponent<Collider2D>();
        m_ejectDirection = collider.gameObject.transform.up;
        m_ejectDirection.Normalize();

        m_Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D col)
    {
        if(!bCanBeUsedAgain)
        {
            return;
        }
        var rb = col.gameObject.GetComponent<Rigidbody2D>();
        if(rb!=null)
        {
            
            bCanBeUsedAgain = false;
            m_Animator.SetTrigger("tEject");
            var velo = rb.velocity;
            velo.y = Mathf.Abs(velo.y);
            velo += m_ejectDirection * m_jumpForce;
            rb.velocity = velo;
            var player = col.gameObject.GetComponent<PlayerMove>();
            if(player!=null)
            {
                player.m_bIsEjected = true;
            }
        }
    }
    public void OnAnimationEjectEnd()
    {
        m_Animator.SetTrigger("tEndEject");
        bCanBeUsedAgain = true;
    }
}
