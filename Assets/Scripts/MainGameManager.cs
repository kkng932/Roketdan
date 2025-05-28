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
    float spawnTime = 2f;

    // 좀비 줄
    const int MAX_LINE = 3;

    Vector2[] spawnPos = { new Vector2(7f, -3.52f), new Vector2(7f, -3.32f), new Vector2(7f, -3.12f) };
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
        
        int randomNum = Random.Range(0, MAX_LINE);

        currZombie.layer = LayerMask.NameToLayer("Monster" + (randomNum + 1).ToString());
        currZombie.transform.position = spawnPos[randomNum];
    }
}

