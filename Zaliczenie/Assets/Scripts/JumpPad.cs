using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class JumpPad : MonoBehaviour
{
 
    private bool bCanBeUsedAgain = true;
    private Animator m_Animator;
    private Collider2D m_collider2D;
    [SerializeField]
    private AudioClip m_ejectSound;
    Vector2 m_ejectDirection = new Vector2 (0f, 0f);
    // Start is called before the first frame update
    [SerializeField]
    private float m_jumpForce = 20f;
    void Start()
    {
        m_collider2D = GetComponent<Collider2D>();
        m_ejectDirection = m_collider2D.gameObject.transform.up;
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
            PlayAudio(m_ejectSound);
            if(player!=null)
            {
                player.OnEject();
            }
        }
    }
    public void OnAnimationEjectEnd()
    {
        m_Animator.SetTrigger("tEndEject");
        bCanBeUsedAgain = true;
    }
    private void PlayAudio(AudioClip audio)
    {
        if (audio != null)
        {
            AudioSource.PlayClipAtPoint(audio, new Vector3(-m_collider2D.transform.position.x, 0, m_collider2D.transform.position.y));
        }

    }
}
