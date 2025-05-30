// 좀비 매니저

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieManager : MonoBehaviour
{
    [SerializeField]
    float speed;
    [SerializeField]
    float jumpForce;

    Rigidbody2D rb;
    Animator animator;

    // 박스 닿아있는지 여부 
    bool isTouchingBox = false;
    // 현재 충돌체 개수
    int currCollisionNum = 0;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    void FixedUpdate()
    {
        rb.velocity = new Vector2(-speed, rb.velocity.y);
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Box"))
        {
            isTouchingBox = true;
            Attack();
        }
        if(collision.gameObject.CompareTag("Monster"))
        {
            if(isTouchingBox)
            {
                Vector2 contactPoint = collision.contacts[0].point;
                float yPos = transform.position.y;
                
                if (yPos < contactPoint.y)
                {
                    Push();
                }
            }
            else
            {
                currCollisionNum++;
            }
        }

        if(!isTouchingBox && currCollisionNum == 1)
            Jump();
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Box"))
            isTouchingBox = false;
        if (collision.gameObject.CompareTag("Monster"))
        {
            currCollisionNum--;
            if(currCollisionNum < 0)
                currCollisionNum = 0;
        }

    }
    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0); // 기존 Y속도 제거
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
    void Attack()
    {
        StartCoroutine(IAttack());
    }
    public void OnAttack()
    {
        
    }
    IEnumerator IAttack()
    {
        while(isTouchingBox)
        {
            animator.Play("Attack");
            yield return new WaitForSeconds(1f);
        }
    }
    // 밀려남 
    void Push()
    {
        StartCoroutine(IPush());
    }
    IEnumerator IPush()
    {
        float time = 0f;
        while(time < 1f)
        {
            time += Time.deltaTime;
            transform.position = Vector2.Lerp(transform.position, new Vector2(0f,transform.position.y), time);
            yield return null;
        }
    }
}
