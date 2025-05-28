using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieManager : MonoBehaviour
{
    [SerializeField]
    float speed;
    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);
    }
}
