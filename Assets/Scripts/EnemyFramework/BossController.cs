using UnityEngine;

public class BossController : GenericEnemyController
{
    protected Transform targetPlayer;
    public ProjectileBehaviour projectilePrefab;
    public Transform launchOffSet;
    public int bulletCount;
    public float fireInterval;
    public float fireCooldownDuration;
    public float meleeRadius;

    bool fireCooldown = true;
    bool doMelee = false;

    private Vector3 offset = new(0, -0.5f, 0);

    protected new void Start()
    {
        base.Start();
        targetPlayer = GameObject.FindWithTag("Player").transform;
        StartCoroutine(Helpers.SetTimer(fireCooldownDuration, ResetCooldown));
    }
    protected override void AttackSequence()
    {
        attackingDirection = (targetPlayer.position + offset
                             - transform.position).normalized;
        currentState = EnemyState.attacking;
        if (doMelee)
        {
            doMelee = false;
            MeleeAttack();
        }
        else
        {
            RangedAttack();
        }
    }

    protected void RangedAttack()
    {
        fireCooldown = true;
        enemyAnimator.SetBool("fire", true);
        StartCoroutine(Helpers.RepeatWithDelay(bulletCount, fireInterval, Fire));
        StartCoroutine(Helpers.SetTimer(bulletCount * fireInterval, ResetAttacking));
        StartCoroutine(Helpers.SetTimer(bulletCount * fireInterval + fireCooldownDuration, ResetCooldown));
    }

    protected void MeleeAttack()
    {
        enemyAnimator.SetBool("melee", true);
        StartCoroutine(Helpers.SetTimer(1.683f, ResetAttacking));

    }

    protected override bool ConditionIsSatisfied()
    {
        return currentState != EnemyState.attacking &&
            (!fireCooldown || MeleeCondition());
    }

    private bool MeleeCondition()
    {
        if ((targetPlayer.position - 
            transform.position).magnitude < meleeRadius)
            doMelee = true;
        return doMelee;
    }

    protected override void IdleBehaviour()
    {
        if (currentState == EnemyState.idle || currentState == EnemyState.moving)
        {
            movementDirection = (targetPlayer.position
                                 - transform.position).normalized;
            currentState = EnemyState.moving;
            enemyRigidbody.MovePosition(
                    transform.position + speed * Time.deltaTime * movementDirection);
        }
    }

    protected void Fire()
    {
        attackingDirection = (targetPlayer.position + offset
                                      - transform.position).normalized;
        ProjectileBehaviour newBullet = Instantiate(projectilePrefab, launchOffSet.position, Quaternion.identity);
        newBullet.transform.right = attackingDirection;
    }
    protected void ResetCooldown()
    {
        fireCooldown = false;
    }
    protected void ResetAttacking()
    {
        currentState = EnemyState.idle;
        enemyAnimator.SetBool("melee", false);
        enemyAnimator.SetBool("fire", false);
    }
    public override void OnHit(OnHitPayload payload)
    {
        return;
    }
}
