using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroManager : MonoBehaviour
{
    [SerializeField]
    Transform gunTrs;

    // 총알 프리팹
    [SerializeField]
    GameObject bulletPrf;

    float currTime = 0f;
    // 총알 쿨타임
    [SerializeField]
    float bulletCoolTime;

    private void Update()
    {
        
        if(MainGameManager.Instance.firstZombieTrs != null)
        {
            // 총이 첫번째 좀비를 추적함 
            Vector3 direction = MainGameManager.Instance.firstZombieTrs.position - gunTrs.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            gunTrs.rotation = Quaternion.Euler(0, 0, angle);

            currTime += Time.deltaTime;
            if(currTime > bulletCoolTime)
            {
                currTime = 0f;
                Shoot();
            }
        }
    }
    private void Shoot()
    {
        GameObject currBullet = Instantiate(bulletPrf);
        currBullet.GetComponent<BulletManager>().Shoot(gunTrs.position, MainGameManager.Instance.firstZombieTrs.position);

    }
}
