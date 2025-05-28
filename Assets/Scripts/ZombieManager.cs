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
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Jump();
        }
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

        if (!isTouchingBox && collision.gameObject.CompareTag("Monster"))
        {
            currCollisionNum++;
        }
        if(currCollisionNum == 1)
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
        Debug.Log("어택");
    }
    IEnumerator IAttack()
    {
        while(isTouchingBox)
        {
            animator.Play("Attack");
            yield return new WaitForSeconds(1f);
        }
    }
}
