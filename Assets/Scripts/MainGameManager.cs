using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameManager : MonoBehaviour
{
    // 좀비 프리팹
    [SerializeField]
    GameObject zombiePrf;
    // 좀비 담을 곳
    [SerializeField]
    Transform zombiePrt;

    // 현재 지난 시간
    float currTime;
    // 스폰 시간
    float spawnTime = 1f;

    Vector2 spawnPos = new Vector2(7f, -3.22f);
    private void Start()
    {
        currTime = 0f;
    }
    private void Update()
    {
        currTime += Time.deltaTime;
        if (currTime > spawnTime)
        {
            currTime = 0f;
            SpawnZombie();
        }
    }

    // 좀비 스폰 
    private void SpawnZombie()
    {
        GameObject currZombie = Instantiate(zombiePrf, zombiePrt);
        currZombie.transform.position = spawnPos;
    }
}

