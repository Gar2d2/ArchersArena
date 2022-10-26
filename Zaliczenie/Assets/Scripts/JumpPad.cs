using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    private bool bCanBeUsedAgain = true;
    private Animator m_Animator;
    // Start is called before the first frame update
    [SerializeField]
    private float m_jumpForce = 20f;
    void Start()
    {
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
            velo.y = Mathf.Abs(velo.y) + m_jumpForce;
            rb.velocity = velo;
        }
    }
    public void OnAnimationEjectEnd()
    {
        m_Animator.SetTrigger("tEndEject");
        bCanBeUsedAgain = true;
    }
}
