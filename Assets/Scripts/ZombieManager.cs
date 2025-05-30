// 좀비 매니저

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ZombieManager : MonoBehaviour
{
    // 이동 속도
    [SerializeField]
    float speed;
    
    // 맨 뒤 몬스터가 올라가는 힘
    [SerializeField]
    float climbForce;

    Rigidbody2D rb;
    Animator animator;
    CapsuleCollider2D col;

    // 박스 닿아있는지 여부 
    bool isTouchingBox = false;
    // 줄 중간에 있는지 여부
    bool isMiddle = false;
    // 현재 닿은 위치 
    Vector3 contactPoint;
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
            Vector2 normal = collision.contacts[0].normal;
            if (isTouchingBox)
            {
                contactPoint = collision.contacts[0].point;
                float topPos = transform.position.y + col.offset.y + col.size.y / 2f;
                // 머리 위에 몬스터가 타면 밀려남
                if (topPos <= contactPoint.y + 0.05f)
                {
                    Push();
                }
            }
            else
            {
                // 오른쪽에 몬스터가 닿아있으면 중간에 있다고 판단
                if (Vector2.Dot(normal, Vector2.left) > 0.5f)
                    isMiddle = true;
            }
        }

    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Box"))
            isTouchingBox = true;
        if(collision.gameObject.CompareTag("Monster"))
        {
            Vector2 normal = collision.contacts[0].normal;
            // 오른쪽에 몬스터가 닿아있으면 중간에 있다고 판단
            if (Vector2.Dot(normal, Vector2.left) > 0.5f)
                isMiddle = true;
            if(!isTouchingBox && !isMiddle)
            {
                if (normal.y >= 0f && normal.y < 0.8f)
                    rb.velocity = new Vector2(-speed, climbForce * (1 - normal.y));
                else
                    rb.velocity = new Vector2(-speed, 0f);
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Box"))
            isTouchingBox = false;
        if (collision.gameObject.CompareTag("Monster"))
            isMiddle = false;

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
        Gizmos.DrawSphere(contactPoint, 0.1f);
    }
}
