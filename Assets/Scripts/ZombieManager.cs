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

    bool isTouchingBox = false;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
            isTouchingBox = true;
        if (!isTouchingBox && collision.gameObject.CompareTag("Monster"))
            Jump();
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
}
