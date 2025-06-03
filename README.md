# 로켓단게임즈 과제전형
Unity 2D를 사용한 몬스터 충돌·점프 구현 과제   
* 설명을 위한 코드의 일부만 정리하였습니다. 
<br><br><br>
   
   
## 핵심 구현 기능

- **FSM 기반 몬스터 행동 제어**
  - 걷기, 공격, 오르기, 사망
- **충돌 방향 감지**
- **쿼리 연산자로 최전방 몬스터 판단**, 총알 발사
- **몬스터 박스 공격** 시 애니메이션 및 박스 **셰이더 제어**
- 배경 스크롤, 바퀴 회전 등 추가 연출
<br><br>

### FSM 기반 몬스터 행동 제어
각 행동(걷기, 공격 등)을 ZombieState를 상속한 개별 상태 클래스로 분리함으로써 상태 전이의 명확성과 코드 유지보수성을 높였습니다.  
또한 Unity의 Update, OnCollisionEnter2D 등 주요 이벤트를 ZombieState 내에 가상 메서드로 정의함으로써, 각 상태가 필요한 이벤트만 오버라이딩하여 처리할 수 있도록 구성했습니다.  
```C#
public abstract class ZombieState
{
    protected ZombieManager zombieMgr;
    public ZombieState(ZombieManager zombieMgr)
    {
        this.zombieMgr = zombieMgr;
    }
    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void OnCollisionEnter2D(Collision2D collision) { }
    public virtual void OnCollisionStay2D(Collision2D collision) { }
    public virtual void OnCollisionExit2D(Collision2D collision) { }
}
```

박스에 닿은 몬스터는 공격을, 줄 마지막에 있는 몬스터는 점프를, 머리 위에 몬스터가 있으면 밀려나는 등의 액션을 취했습니다.  
<br><br>
### 충돌 방향 감지
몬스터의 현재 상태를 ZombieMonster 내에서 판단하기 위해 현재 상태를 어느 위치에 어떤 물체와 충돌했느냐로 판단했습니다.  

```C#
bool isLast = false;
public override void OnCollisionEnter2D(Collision2D collision)
{
    if(collision.gameObject.CompareTag("Monster"))
    {
        // 왼쪽에 접촉했을 때 
        Vector2 normal = collision.contacts[0].normal;
        if (normal.x >= 0.9f)
        {
            isLast = true;
        }
    }
}
```
```C#
// 현재 시간
float currTime = 0f;
// 타고 올라가기 시작할 시간 
float lastTime = 0.3f;
public override void Update()
{
    if(isLast)
    {
        currTime += Time.deltaTime;
        if (currTime > lastTime)
        {
            zombieMgr.ChangeState(new ZombieClimbState(zombieMgr));
        }
    }
    
}
```
<br><br>
### 쿼리 연산자로 최전방 몬스터 판단 
가독성 향상과 최적화를 위해 쿼리 연산자를 사용해 최전방 몬스터를 판단하고, 이를 참조해 총알을 발사하였습니다. 
```C#
// 가장 앞에 있는 좀비 위치 
public Transform firstZombieTrs;
List<Transform> zombieTrs = new List<Transform>();
...
private void Update()
{
     if(zombieTrs.Count > 0) 
         firstZombieTrs = zombieTrs
             .OrderBy(t => t.position.x)
             .First();
}
```
```C#
private void Shoot()
{
    GameObject currBullet = Instantiate(bulletPrf);
    currBullet.GetComponent<BulletManager>().Shoot(gunTrs.position, MainGameManager.Instance.firstZombieTrs.position);
}
```
<br><br>
### 박스 셰이더 제어
커스텀 셰이더를 사용하여 Sprite가 피격될 때 일시적으로 흰색으로 반짝이는 효과를 구현했습니다.
```hlsl
fixed4 frag (v2f i) : SV_Target
{
    fixed4 col = tex2D(_MainTex, i.uv);
    fixed3 falshCol = lerp(col.rgb, float3(1.0, 1.0, 1.0), flashAmount);
    return fixed4(falshCol, col.a);
}
```
무작위로 들어오는 피격을 코루틴을 활용해 비동기적 흐름 제어로 자연스럽게 구현했습니다. 
```C#
// 반짝이는 시간
float flashTime = 1f;
private void Flash()
{
    if (flashClr != null) StopCoroutine(flashClr);
    flashClr = StartCoroutine(IEFlash());
}
IEnumerator IEFlash()
{
    float t = 0f;
    while (t < flashTime)
    {
        float amount = Mathf.Lerp(1f, 0f, t / flashTime);
        flashMat.SetFloat("flashAmount", amount);
        t += Time.deltaTime;
        yield return null;        
    }
    flashMat.SetFloat("flashAmount", 0f);
}
```

### 감사합니다. 
