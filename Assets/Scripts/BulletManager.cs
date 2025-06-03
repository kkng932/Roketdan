using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BulletManager : MonoBehaviour
{
    [SerializeField]
    float speed;
    [SerializeField]
    float damage;

    Vector2 direction;
    
    void Update()
    {
        if (direction != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, direction, speed * Time.deltaTime);

            float distance = Vector2.Distance(transform.position, direction);
            if (distance < 0.1f)
            {
                Destroy(gameObject); 
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Monster"))
        {
            collision.gameObject.GetComponent<ZombieManager>().TakeDamage(damage);
            Destroy(gameObject);
        }
        
    }
    public void Shoot(Vector2 start, Vector2 dir)
    {
        transform.position = start;
        direction = dir;
    }
}
