using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;

public class MainGameManager : Singleton<MainGameManager>
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
    float spawnTime = 1.6f;

    // 좀비 줄
    const int MAX_LINE = 3;

    Vector2 spawnPos = new Vector2(7f, -3f);

    // 가장 앞에 있는 좀비 위치 
    public Transform firstZombieTrs;
    List<Transform> zombieTrs = new List<Transform>();

    
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
            zombieTrs.Add(SpawnZombie().transform);
        }
        if(zombieTrs.Count > 0) 
            firstZombieTrs = zombieTrs
                .OrderBy(t => t.position.x)
                .First();
    }

    // 좀비 스폰 
    private GameObject SpawnZombie()
    {
        GameObject currZombie = Instantiate(zombiePrf, zombiePrt);
        
        int randomNum = Random.Range(0, MAX_LINE);

        currZombie.layer = LayerMask.NameToLayer("Monster" + (randomNum + 1).ToString());
        currZombie.GetComponent<SortingGroup>().sortingOrder = MAX_LINE - randomNum;
        currZombie.transform.position = spawnPos;
        return currZombie;
    }
    public void DestroyZombie(Transform zombie)
    {
        Debug.Log("DetroyZombie");
        zombieTrs.Remove(zombie);
        Debug.Log(zombieTrs.Count);
    }
}

