using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelManager : MonoBehaviour
{
    float speed = 100;
    float currZ;
    private void Start()
    {
        currZ = transform.rotation.z;
    }
    void Update()
    {
        currZ -= speed * Time.deltaTime;
        if (currZ < 0f) currZ = 360f + currZ;
        transform.rotation = Quaternion.Euler(0f, 0f, currZ);
    }
}
