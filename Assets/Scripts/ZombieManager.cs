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
    CapsuleCollider2D col;

    // 박스 닿아있는지 여부 
    bool isTouchingBox = false;

    Vector3 currCollisionPos;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        col = GetComponent<CapsuleCollider2D>();
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
                currCollisionPos = contactPoint;
                float topPos = transform.position.y + col.offset.y + col.size.y / 2f;
                // 머리 위에 몬스터가 타면 밀려남
                if (topPos <= contactPoint.y + 0.05f)
                {
                    Push();
                }
            }
            else
            {
                
                Vector2 normal = collision.contacts[0].normal;
                // 왼쪽이나 아래쪽에 몬스터가 닿으면 점프
                if (Vector2.Dot(normal, Vector2.up) > 0.5f || Vector2.Dot(normal, Vector2.right) > 0.5f)
                    Jump();
            }
        }

    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Box"))
            isTouchingBox = false;

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
        float duration = 2f;
        while(time < 2f)
        {
            time += Time.deltaTime;
            transform.position = Vector2.Lerp(transform.position, new Vector2(0f,transform.position.y), time / duration);
            yield return null;
        }
    }
    // 위치 확인 디버깅 기즈모
    private void OnDrawGizmos()
    {
        if (col == null) return;
        // 흰 원: collider 꼭대기 확인
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.position + (Vector3)col.offset + new Vector3(0f, col.size.y / 2f, 0f), 0.1f);
        // 노란 원: 충돌 위치 확인
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(currCollisionPos, 0.1f);
    }
}
