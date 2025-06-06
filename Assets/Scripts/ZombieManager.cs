// 좀비 매니저

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ZombieManager : MonoBehaviour
{
    ZombieState currState;
    // 이동 속도
    public float speed;
    
    // 맨 뒤 몬스터가 올라가는 힘
    public float climbForce;

    public Rigidbody2D rb;
    public Animator animator;
    CapsuleCollider2D col;

    // 현재 닿은 위치 
    Vector3 contactPoint;
    // 마지막으로 닿은 박스
    public BoxManager lastTouchBox;
    public float attackDamage;

    public float hp = 100;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        col = GetComponent<CapsuleCollider2D>();
        ChangeState(new ZombieWalkState(this));
        
    }
    public void ChangeState(ZombieState newState)
    {
        //Debug.Log("ChangeState: " + newState.ToString());
        currState?.Exit();
        currState = newState;
        currState.Enter();
    }
    private void Update()
    {
        currState?.Update();
    }
    private void FixedUpdate()
    {
        currState?.FixedUpdate();
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        currState.OnCollisionEnter2D(collision);
        
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        currState.OnCollisionStay2D(collision);
      
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        currState.OnCollisionExit2D(collision);
    }
    
    public void OnAttack()
    {
        if (lastTouchBox == null) return;
        lastTouchBox.TakeDamage(attackDamage);
    }

    // 맞음
    public void TakeDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0) 
            Die();
    }
    private void Die()
    {
        ChangeState(new ZombieDieState(this));
    }
    public void OnDie()
    {
        MainGameManager.Instance.DestroyZombie(transform);
        Destroy(gameObject);
    }
    
    // 위치 확인 디버깅 기즈모
    private void OnDrawGizmos()
    {
        if (col == null) return;
        // 흰 원: collider 꼭대기 확인
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.position + (Vector3)col.offset + new Vector3(0f, col.size.y / 2f, 0f), 0.1f);
        // 노란 원: 충돌 위치 확인
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(contactPoint, 0.1f);
    }
}
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

public class ZombieWalkState : ZombieState
{
    public ZombieWalkState(ZombieManager zombieMgr) : base(zombieMgr) { }
    bool isLast = false;
    // 타고 올라가기 전 시간 
    float currTime = 0f;
    // 타고 올라가기 시작할 시간 
    float lastTime = 0.3f;
    public override void FixedUpdate()
    {
        zombieMgr.rb.velocity = new Vector2(-zombieMgr.speed, zombieMgr.rb.velocity.y);
    }
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
    public override void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Box"))
        {
            zombieMgr.lastTouchBox = collision.gameObject.GetComponentInParent<BoxManager>();
            zombieMgr.ChangeState(new ZombieAttackState(zombieMgr));
        }
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
    public override void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Monster"))
        {
            // 오른쪽에 접촉했을 때 
            Vector2 normal = collision.contacts[0].normal;
            if (normal.x <= -0.9f)
            {
                isLast = false;
            }
        }
    }
}

public class ZombieClimbState : ZombieState
{
    public ZombieClimbState(ZombieManager zombieMgr) : base(zombieMgr) { }
    public override void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Box"))
            zombieMgr.ChangeState(new ZombieAttackState(zombieMgr));
        if (collision.gameObject.CompareTag("Monster"))
        {
            // 닿은 좀비를 타고 앞으로 전진
            Vector2 normal = collision.contacts[0].normal;
            zombieMgr.rb.velocity = new Vector2(-zombieMgr.speed, zombieMgr.climbForce * (1 - normal.y));
        }
        
        
    }
}
public class ZombieAttackState : ZombieState
{
    bool isPushing = false;
    Coroutine attackCrt;
    public ZombieAttackState(ZombieManager zombieMgr) : base(zombieMgr) { }
    public override void Enter()
    {
        attackCrt = zombieMgr.StartCoroutine(IEAttack());
    }
    public override void Exit()
    {
        if(attackCrt != null)
            zombieMgr.StopCoroutine(attackCrt);
    }
    IEnumerator IEAttack()
    {
        while(true)
        {
            zombieMgr.animator.Play("Attack");
            yield return new WaitForSeconds(1f);
        }
    }
    
    public override void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Box"))
        {
            if(collision.gameObject.TryGetComponent<BoxManager>(out BoxManager currBox))
            {
                if(currBox == null || currBox != zombieMgr.lastTouchBox)
                    zombieMgr.lastTouchBox = currBox;
            }
        }
        if(collision.gameObject.CompareTag("Monster"))
        {
            Vector2 normal = collision.contacts[0].normal;
            if (normal.y <= -0.95f)
                Push();
        }
    }
    // 밀려남 
    void Push()
    {
        if (isPushing) return;
        zombieMgr.StartCoroutine(IPush());
    }
    IEnumerator IPush()
    {
        isPushing = true;
        float time = 0f;
        float duration = 1f;
        while (time < 1f)
        {
            time += Time.deltaTime;
            zombieMgr.transform.position = Vector2.Lerp(zombieMgr.transform.position, new Vector2(0f, zombieMgr.transform.position.y), time / duration);
            yield return null;
        }
        isPushing = false;
        zombieMgr.ChangeState(new ZombieWalkState(zombieMgr));
    }
}
public class ZombieDieState : ZombieState
{
    public ZombieDieState(ZombieManager zombieMgr) : base(zombieMgr) { }
    public override void Enter()
    {
        zombieMgr.animator.Play("Die");
    }
}