using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyTypes : Humanoid
{
    public int attackPatternCount;
    public int attackCount;
    public GameObject[] Weapons;
    public GameObject currentWeapon;
    public EnemyMovement enemyMovement;
    public IEnemyStates enemyState;
    public float LastTimeDeflectedCounter;
    public GameObject selectedThrowable;
    public Collider DefendHitbox;

    private void Awake()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        animator = GetComponent<Animator>();
        currentWeapon = Weapons[0];
        enemyState = new EnemyIdle(this);
        LastTimeDeflectedCounter = 100f;
        selectedThrowable = GameManager.instance.ThrowablePrefabs[0];
        EnemyCombat.ChangeWeapon(this, 1);
        armorType = ArmorType.LightArmor;

        MaxArmor = 100f;
        Armor = MaxArmor;

        MaxHealth = 100f;
        Health = MaxHealth;

        Stamina = 100f;
        PoiseBrokenTime = 0.75f;
        poiseMultiplier = 3f;

        HurtTime = StateMethods.FindAnimationLenght(animator, "Hurt");
        RollTime = StateMethods.FindAnimationLenght(animator, "Roll");
        DodgeTime = StateMethods.FindAnimationLenght(animator, "Dodge");
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.isPaused) return;

        base.Update();
        LastTimeDeflectedCounter += Time.deltaTime;
        enemyState = enemyState.CheckForStateChange(this);
        enemyState.StateUpdate(this);
    }
    public void ChangeThrowable()
    {
        //if(isThereAny) then selected=new
    }
}

public static class EnemyCombat
{
    

    public static void ChangeWeapon(EnemyTypes enemy, int number)
    {
        enemy.currentWeapon = enemy.Weapons[number - 1];
        //change positions and rotations
    }

    public static void FirstAttack(EnemyTypes enemy, bool isUpAttack)
    {
        enemy.attackCount = 1;
        if (isUpAttack)
        {
            enemy.attackCount = 0;
            enemy.currentWeapon.GetComponent<MeleeWeapon>().Attack("UpAttack");
        }
        if (enemy.LastTimeDeflectedCounter <= 1f)//if deflected last
        {
            enemy.currentWeapon.GetComponent<MeleeWeapon>().Attack("DeflectAttack");
        }
        else if (enemy.enemyState is EnemyWalk || enemy.enemyState is EnemyIdle)
        {
            enemy.currentWeapon.GetComponent<MeleeWeapon>().Attack("WalkAttack");
        }
        else if (enemy.enemyState is EnemyRun)
        {
            enemy.currentWeapon.GetComponent<MeleeWeapon>().Attack("RunAttack");
        }
        else if (enemy.enemyState is EnemyDefend)
        {
            enemy.currentWeapon.GetComponent<MeleeWeapon>().Attack("DefendAttack");
        }
    }
    public static void UpAttack(EnemyTypes enemy)
    {
        enemy.attackCount = 0;
        enemy.currentWeapon.GetComponent<MeleeWeapon>().Attack("UpAttack");
    }
    public static void AttackContinue(EnemyTypes enemy)
    {
        enemy.attackCount += 1;
        enemy.currentWeapon.GetComponent<MeleeWeapon>().Attack("Attack" + enemy.attackCount % enemy.attackPatternCount);//normal baþlangýç anim dahil 3 anim
    }
    public static void Throw(EnemyTypes enemy)
    {
        enemy.animator.CrossFade("Throw", 0.1f);
        GameObject.Instantiate(enemy.selectedThrowable, enemy.transform.position, Quaternion.identity);
    }
    
    public static void Deflect(EnemyTypes enemy)
    {
        var touchingObjects = enemy.DefendHitbox.GetComponent<DefendTrigger>().TouchingObjects;
        foreach (var item in touchingObjects)
        {
            if (!item.CompareTag("MeleeWeapon") || (item.GetComponent<MeleeWeapon>().isUpAttacking))
            {
                touchingObjects.Remove(item);
            }
        }
        if (touchingObjects.Count > 0)
        {
            enemy.LastTimeDeflectedCounter = 0f;
            enemy.currentWeapon.GetComponent<MeleeWeapon>().Deflect();
            foreach (var item in touchingObjects)
            {
                item.GetComponent<MeleeWeapon>().Owner.GetComponent<Humanoid>().DecreaseStamina(enemy.currentWeapon.GetComponent<MeleeWeapon>().weight * enemy.poiseMultiplier);
                if (item.GetComponent<MeleeWeapon>().Owner.GetComponent<Humanoid>().Stamina == 0)
                {
                    if (item.GetComponent<MeleeWeapon>().Owner.CompareTag("Player"))
                    {
                        item.GetComponent<MeleeWeapon>().Owner.GetComponent<Player>().playerState = new PoiseBroken(enemy.currentWeapon.GetComponent<ICanDamage>());
                        return;
                    }
                }
            }
        }
    }


}

public  class EnemyMovement:MonoBehaviour
{
    public Rigidbody rb;
    public Animator animator;
    public EnemyTypes enemy;
    
    private Vector3 direction;

    #region StaminaNeeds
    public float NeedForRun { get; private set; }
    public float RunConsume { get; private set; }
    public float NeedForAttack { get; private set; }
    public float NeedForDeflect { get; private set; }
    public float NeedForDodge { get; private set; }
    public float NeedForRoll { get; private set; }

    #endregion

    public float WalkSpeed { get; private set; }
    public float RunSpeed { get; private set; }
    public float AttackMoveSpeed { get; private set; }
    public float DefendMoveSpeed { get; private set; }
    public float ThrowMoveSpeed { get; private set; }
    public float DodgeMoveSpeed { get; private set; }
    public float RollMoveSpeed { get; private set; }
    public float HurtMoveSpeed { get; private set; }
    public bool isLerpingIdle;
    public bool isLockedToPlayer;
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        enemy = GetComponent<EnemyTypes>();
        WalkSpeed = 3f;
        RunSpeed = 5f;
        AttackMoveSpeed = 1.5f;
        DefendMoveSpeed = 2.5f;
        ThrowMoveSpeed = 3f;
        DodgeMoveSpeed = 10f;
        RollMoveSpeed = 7f;
        HurtMoveSpeed = 1f;

        NeedForRun = 8f;
        RunConsume = 10f;
        NeedForAttack = 6f;
        NeedForDeflect = 12f;
        NeedForDodge = 6f;
        NeedForRoll = 20f;

    }
    private void Update()
    {
        if (GameManager.isPaused) return;

        if (isLerpingIdle)
        {
            LerpIdle();
        }
    }
    public void Idle()
    {
        //GetDirection();
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
    }
    public void Walk()
    {
        GetDirection(true);
        var speed = direction * WalkSpeed;
        speed.y = rb.velocity.y;
        rb.velocity = speed;
    }
    public void Run()
    {
        GetDirection(true);
        var speed = direction * RunSpeed;
        speed.y = rb.velocity.y;
        rb.velocity = speed;
    }
    private void ArrangeMoveAnimations()
    {
        //only when upper body animation plays(not while roll hurt dodge happening)
        Vector3 newDir = transform.InverseTransformDirection(rb.velocity.normalized);
        direction = new Vector3(newDir.x, 0, newDir.z).normalized;
        if (direction.magnitude < 0.1f)
        {
            if (animator.GetCurrentAnimatorClipInfo(0).Length > 0 && !animator.GetCurrentAnimatorClipInfo(0)[0].clip.ToString().StartsWith("Idle"))//0 = normal layer
                animator.CrossFade("Idle", 0.1f);
        }
        else
        {
            if (animator.GetCurrentAnimatorClipInfo(0).Length > 0 && !animator.GetCurrentAnimatorClipInfo(0)[0].clip.ToString().StartsWith("Walk"))//0 = normal layer
                animator.CrossFade("Walk", 0.1f);
        }
    }
    public void AttackMoveArrange()
    {
        //ArrangeMoveAnimations();
        GetDirection(false);
        var speed = direction * AttackMoveSpeed;
        speed.y = rb.velocity.y;
        rb.velocity = speed;
    }
    public void DefendMove()
    {
        GetDirection(true);
        ArrangeMoveAnimations();
        var speed = direction * DefendMoveSpeed;
        speed.y = rb.velocity.y;
        rb.velocity = speed;
    }
    public void ThrowMove()
    {
        GetDirection(true);
        ArrangeMoveAnimations();
        var speed = direction * ThrowMoveSpeed;
        speed.y = rb.velocity.y;
        rb.velocity = speed;
    }
    public void DodgeMoveArrange()
    {
        GetDirection(false);
        var speed = direction * DodgeMoveSpeed;
        speed.y = rb.velocity.y;
        rb.velocity = speed;
    }
    public void RollMoveArrange()
    {
        GetDirection(false);
        var speed = direction * RollMoveSpeed;
        speed.y = rb.velocity.y;
        rb.velocity = speed;
    }
    public void HurtMoveArrange()
    {
        GetDirection(false);
        var speed = direction * HurtMoveSpeed;
        speed.y = rb.velocity.y;
        rb.velocity = speed;
    }
    public void PoiseBrokenMoveArrange(float speed)
    {
        GetDirection(false);
        var newSpeed = direction * speed;
        newSpeed.y = rb.velocity.y;
        rb.velocity = newSpeed;
        isLerpingIdle = true;
    }
    public void LerpIdle()
    {
        rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(0, rb.velocity.y, 0), Time.deltaTime * 5);
    }
    public void DyingMoveArrange(float speed)
    {
        PoiseBrokenMoveArrange(speed);
        GetComponent<EnemyTypes>().MakeRagdoll();
    }
    
    private void GetDirection(bool isLerping)
    {
        if (isLerping)
        {
            direction = EnemyStateMethods.FindDirectionAI(enemy);
            Vector3 lookDirection = rb.velocity;
            lookDirection = new Vector3(lookDirection.x, 0, lookDirection.z).normalized;
            LookArrange(lookDirection, isLerping);
        }
        else
        {
            Vector3 lookDirection = rb.velocity;
            lookDirection = new Vector3(lookDirection.x, 0, lookDirection.z).normalized;
            LookArrange(lookDirection, isLerping);
            direction = lookDirection;
        }
        
    }
    private void LookArrange(Vector3 dir, bool isLerping)
    {
        animator.SetFloat("xVel", transform.InverseTransformDirection(rb.velocity.normalized).x);
        animator.SetFloat("zVel", transform.InverseTransformDirection(rb.velocity.normalized).z);
        if (isLockedToPlayer)
        {
            LookToPlayer(isLerping);
        }
        else
        {
            LookToDirection(dir, isLerping);
        }

    }
    private void LookToPlayer(bool isLerping)
    {
        Vector3 dir = Player.instance.transform.position - transform.position;
        dir.y = 0;
        dir = dir.normalized;
        LookToDirection(dir, isLerping);
    }
    private void LookToDirection(Vector3 dir, bool isLerping)
    {
        dir.y = 0;
        if (dir != Vector3.zero && isLerping)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 0.03f);
        else if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);
    }
    
}


public interface IEnemyStates
{
    IEnemyStates CheckForStateChange(EnemyTypes enemy);
    void StateUpdate(EnemyTypes enemy);
}
public static class EnemyStateMethods
{
    public static bool CanRotationRun(EnemyTypes enemy)
    {
        if (enemy.transform.InverseTransformDirection(enemy.enemyMovement.rb.velocity.normalized).z > 0.03f)
        {
            return true;
        }
        return false;
    }
    public static Vector3 FindDirectionAI(EnemyTypes enemy)
    {
        //do it!
        Vector3 direction = Vector3.zero;
        return direction;
    }
    public static void LockToPlayer(EnemyTypes enemy)
    {
        enemy.enemyMovement.isLockedToPlayer = true;
    }
    public static void UnlockToPlayer(EnemyTypes enemy)
    {
        enemy.enemyMovement.isLockedToPlayer = false;
    }
    #region AICheckStates
    public static bool CheckForUpAttack(EnemyTypes enemy)
    {
        return false;
    }
    public static bool CheckForAttack(EnemyTypes enemy)
    {
        return false;
    }
    public static bool CheckForDodge(EnemyTypes enemy)
    {
        //check if defend
        return false;
    }
    public static bool CheckForRoll(EnemyTypes enemy)
    {
        //check if defend
        return false;
    }
    public static bool CheckForDefend(EnemyTypes enemy)
    {
        return false;
    }
    public static bool CheckForThrow(EnemyTypes enemy)//throwing state
    {
        //check if defend
        return false;
    }
    public static bool CheckForThrowOne(EnemyTypes enemy)//throw one thing
    {
        //if true call Combat method of throw
        return false;
    }
    public static bool CheckForRun(EnemyTypes enemy)
    {
        return false;
    }
    public static bool CheckForWalk(EnemyTypes enemy)
    {
        return false;
    }
    public static bool CheckForIdle(EnemyTypes enemy)
    {
        return false;
    }
    public static bool CheckForAttackBuffer(EnemyTypes enemy)
    {
        return false;
    }
    public static bool CheckForUpAttackBuffer(EnemyTypes enemy)
    {
        return false;
    }
    public static bool CheckForDeflect(EnemyTypes enemy)
    {
        //if true then call the Combat method
        return false;
    }
    public static bool CheckForExitDefend(EnemyTypes enemy)
    {
        return false;
    }
    public static bool CheckForExitThrow(EnemyTypes enemy)
    {
        return false;
    }
    #endregion
}
public class EnemyIdle : IEnemyStates
{
    public IEnemyStates CheckForStateChange(EnemyTypes enemy)
    {
        if (EnemyStateMethods.CheckForUpAttack(enemy))
        {
            EnemyCombat.FirstAttack(enemy, true);
            return new EnemyAttack(enemy);
        }
        if (EnemyStateMethods.CheckForAttack(enemy))
        {
            EnemyCombat.FirstAttack(enemy, false);
            return new EnemyAttack(enemy);
        }
        
        if (EnemyStateMethods.CheckForDodge(enemy))
        {
            return new EnemyDodge(enemy);
        }
        if (EnemyStateMethods.CheckForRoll(enemy))
        {
            return new EnemyRoll(enemy);
        }
        if (EnemyStateMethods.CheckForDefend(enemy))
        {
            return new EnemyDefend(enemy);
        }
        
        if (EnemyStateMethods.CheckForThrow(enemy))
        {
            return new EnemyThrow(enemy);
        }
        if (EnemyStateMethods.CheckForRun(enemy))
        {
            return new EnemyRun(enemy);
        }
        if (EnemyStateMethods.CheckForWalk(enemy))
        {
            return new EnemyWalk(enemy);
        }

        return this;
    }
    public void StateUpdate(EnemyTypes enemy)
    {
        enemy.IncreaseStamina(Time.deltaTime * 25f);
        enemy.enemyMovement.Idle();
    }

    public EnemyIdle(EnemyTypes enemy)
    {
        enemy.attackCount = 0;
        enemy.enemyMovement.animator.CrossFade("Idle", 0.2f);
        enemy.animator.CrossFade("Empty", 0.2f);
    }
}
public class EnemyWalk : IEnemyStates
{
    public IEnemyStates CheckForStateChange(EnemyTypes enemy)
    {
        if (EnemyStateMethods.CheckForUpAttack(enemy))
        {
            EnemyCombat.FirstAttack(enemy, true);
            return new EnemyAttack(enemy);
        }
        if (EnemyStateMethods.CheckForAttack(enemy))
        {
            EnemyCombat.FirstAttack(enemy, false);
            return new EnemyAttack(enemy);
        }

        if (EnemyStateMethods.CheckForDodge(enemy))
        {
            return new EnemyDodge(enemy);
        }
        if (EnemyStateMethods.CheckForRoll(enemy))
        {
            return new EnemyRoll(enemy);
        }
        if (EnemyStateMethods.CheckForDefend(enemy))
        {
            return new EnemyDefend(enemy);
        }

        if (EnemyStateMethods.CheckForThrow(enemy))
        {
            return new EnemyThrow(enemy);
        }
        if (EnemyStateMethods.CheckForRun(enemy))
        {
            return new EnemyRun(enemy);
        }
        if (EnemyStateMethods.CheckForIdle(enemy))
        {
            return new EnemyIdle(enemy);
        }

        return this;
    }
    public void StateUpdate(EnemyTypes enemy)
    {
        enemy.IncreaseStamina(Time.deltaTime * 12.5f);
        enemy.enemyMovement.Walk();

    }
    public EnemyWalk(EnemyTypes enemy)
    {
        enemy.attackCount = 0;
        enemy.animator.CrossFade("Walk", 0.2f);
    }
    
}
public class EnemyRun : IEnemyStates
{
    public IEnemyStates CheckForStateChange(EnemyTypes enemy)
    {
        if (EnemyStateMethods.CheckForUpAttack(enemy))
        {
            EnemyCombat.FirstAttack(enemy, true);
            return new EnemyAttack(enemy);
        }
        if (EnemyStateMethods.CheckForAttack(enemy))
        {
            EnemyCombat.FirstAttack(enemy, false);
            return new EnemyAttack(enemy);
        }

        if (EnemyStateMethods.CheckForDodge(enemy))
        {
            return new EnemyDodge(enemy);
        }
        if (EnemyStateMethods.CheckForRoll(enemy))
        {
            return new EnemyRoll(enemy);
        }
        if (EnemyStateMethods.CheckForDefend(enemy))
        {
            return new EnemyDefend(enemy);
        }

        if (EnemyStateMethods.CheckForThrow(enemy))
        {
            return new EnemyThrow(enemy);
        }
        if (EnemyStateMethods.CheckForWalk(enemy))
        {
            return new EnemyWalk(enemy);
        }
        if (EnemyStateMethods.CheckForIdle(enemy))
        {
            return new EnemyIdle(enemy);
        }

        return this;
    }
    public void StateUpdate(EnemyTypes enemy)
    {
        enemy.DecreaseStamina(Time.deltaTime * enemy.enemyMovement.RunConsume);
        enemy.enemyMovement.Run();

    }
    public EnemyRun(EnemyTypes enemy)
    {
        PlayerCombat.attackCount = 0;
        enemy.animator.CrossFade("Run", 0.2f);
        enemy.animator.CrossFade("Empty", 0.2f);
    }
}
public class EnemyAttack : IEnemyStates
{
    public bool attackBuffer;
    public bool upAttackBuffer;
    public IEnemyStates CheckForStateChange(EnemyTypes enemy)
    {
        return this;
    }
    public void StateUpdate(EnemyTypes enemy)
    {
        if (EnemyStateMethods.CheckForAttackBuffer(enemy))
        {
            attackBuffer = true;
        }
        if (EnemyStateMethods.CheckForUpAttackBuffer(enemy))
        {
            upAttackBuffer = true;
        }
    }
    public EnemyAttack(EnemyTypes enemy)
    {
        enemy.DecreaseStamina(enemy.enemyMovement.NeedForAttack);
        enemy.enemyMovement.AttackMoveArrange();
        GameManager.instance.StartCoroutine(ExitAttack(enemy, enemy.currentWeapon.GetComponent<MeleeWeapon>().attackTime));
    }
    IEnumerator ExitAttack(EnemyTypes enemy ,float time)
    {
        yield return new WaitForSeconds(time);
        if (upAttackBuffer && enemy.enemyState is EnemyAttack)
        {
            EnemyCombat.UpAttack(enemy);
            enemy.enemyState = new EnemyAttack(enemy);
        }
        else if (attackBuffer && enemy.enemyState is EnemyAttack)
        {
            EnemyCombat.AttackContinue(enemy);
            enemy.enemyState = new EnemyAttack(enemy);
        }
        else if (enemy.enemyState is EnemyAttack)
        {
            enemy.enemyState = new EnemyWalk(enemy);
        }

    }
}
public class EnemyDodge : IEnemyStates
{
    public IEnemyStates CheckForStateChange(EnemyTypes enemy)
    {
        return this;
    }
    public void StateUpdate(EnemyTypes enemy)
    {

    }
    public EnemyDodge(EnemyTypes enemy)
    {
        enemy.DecreaseStamina(enemy.enemyMovement.NeedForDodge);
        enemy.enemyMovement.DodgeMoveArrange();
        GameManager.instance.StartCoroutine(ExitDodge(enemy, enemy.DodgeTime));
        enemy.animator.CrossFade("Dodge", 0.1f);
        enemy.animator.CrossFade("Empty", 0.1f);
    }
    IEnumerator ExitDodge(EnemyTypes enemy, float time)
    {
        yield return new WaitForSeconds(time);
        if (enemy.enemyState is EnemyDodge)
            enemy.enemyState = new EnemyWalk(enemy);
    }
}

public class EnemyRoll : IEnemyStates
{
    public IEnemyStates CheckForStateChange(EnemyTypes enemy)
    {
        return this;
    }
    public void StateUpdate(EnemyTypes enemy)
    {

    }
    public EnemyRoll(EnemyTypes enemy)
    {
        enemy.DecreaseStamina(enemy.enemyMovement.NeedForRoll);
        enemy.enemyMovement.RollMoveArrange();
        GameManager.instance.StartCoroutine(ExitRoll(enemy, enemy.RollTime));
        enemy.animator.CrossFade("Roll", 0.1f);
        enemy.animator.CrossFade("Empty", 0.1f);
    }
    IEnumerator ExitRoll(EnemyTypes enemy, float time)
    {
        yield return new WaitForSeconds(time);
        if (enemy.enemyState is EnemyRoll)
            enemy.enemyState = new EnemyWalk(enemy);
    }
}
public class EnemyDefend : IEnemyStates
{
    public IEnemyStates CheckForStateChange(EnemyTypes enemy)
    {

        if (EnemyStateMethods.CheckForExitDefend(enemy))
        {
            CloseDefendHitbox(enemy);
            return new EnemyWalk(enemy);
        }
        if (EnemyStateMethods.CheckForDodge(enemy))
        {
            CloseDefendHitbox(enemy);
            return new EnemyDodge(enemy);
        }
        if (EnemyStateMethods.CheckForRoll(enemy))
        {
            CloseDefendHitbox(enemy);
            return new EnemyRoll(enemy);
        }
        if (EnemyStateMethods.CheckForThrow(enemy))
        {
            CloseDefendHitbox(enemy);
            return new EnemyThrow(enemy);
        }

        return this;
    }
    void CloseDefendHitbox(EnemyTypes enemy)
    {
        enemy.DefendHitbox.gameObject.SetActive(false);
    }
    public void StateUpdate(EnemyTypes enemy)
    {
        enemy.enemyMovement.DefendMove();
        enemy.IncreaseStamina(Time.deltaTime * 8f);
        EnemyStateMethods.CheckForDeflect(enemy);
    }
    public EnemyDefend(EnemyTypes enemy)
    {
        enemy.DefendHitbox.gameObject.SetActive(true);
        enemy.currentWeapon.GetComponent<MeleeWeapon>().Defend();
    }
}

public class EnemyThrow : IEnemyStates
{
    public IEnemyStates CheckForStateChange(EnemyTypes enemy)
    {
        if (EnemyStateMethods.CheckForExitThrow(enemy))
        {
            return new EnemyWalk(enemy);
        }
        if (EnemyStateMethods.CheckForDodge(enemy))
        {
            return new EnemyDodge(enemy);
        }
        if (EnemyStateMethods.CheckForRoll(enemy))
        {
            return new EnemyRoll(enemy);
        }
        if (EnemyStateMethods.CheckForDefend(enemy))
        {
            return new EnemyDefend(enemy);
        }

        return this;
    }
    public void StateUpdate(EnemyTypes enemy)
    {
        enemy.enemyMovement.ThrowMove();
        EnemyStateMethods.CheckForThrowOne(enemy);
    }
    public EnemyThrow(EnemyTypes enemy)
    {
        enemy.animator.CrossFade("ThrowState", 0.1f);
    }
}
public class EnemyHurt : IEnemyStates
{
    ICanDamage lastWeapon;
    public IEnemyStates CheckForStateChange(EnemyTypes enemy)
    {
        if (enemy.Stamina == 0)
        {
            enemy.IncreaseStamina(10f);
            return new EnemyPoiseBroken(enemy, lastWeapon);
        }
        return this;
    }
    public void StateUpdate(EnemyTypes enemy)
    {
        enemy.IncreaseStamina(Time.deltaTime * 25f);
    }
    public EnemyHurt(EnemyTypes enemy, ICanDamage iCanDamage)
    {
        lastWeapon = iCanDamage;
        enemy.TakeDamage(iCanDamage);
        if (!(enemy.enemyState is EnemyPoiseBroken))
        {
            enemy.DecreaseStamina(iCanDamage.weight * enemy.poiseMultiplier);
            if (enemy.Stamina == 0)
            {
                //arrange state outside of ctor
            }
            else
            {
                enemy.enemyMovement.HurtMoveArrange();
                GameManager.instance.StartCoroutine(ExitHurt(enemy, enemy.HurtTime));
                enemy.animator.CrossFade("Hurt", 0.1f);
                enemy.animator.CrossFade("Empty", 0.1f);
            }

        }
    }
    IEnumerator ExitHurt(EnemyTypes enemy, float time)
    {
        yield return new WaitForSeconds(time);
        if (enemy.enemyState is EnemyHurt)
            enemy.enemyState = new EnemyWalk(enemy);
    }
}
public class EnemyPoiseBroken : IEnemyStates
{
    EnemyTypes enemyForDestructor;
    public IEnemyStates CheckForStateChange(EnemyTypes enemy)
    {
        return this;
    }
    public void StateUpdate(EnemyTypes enemy)
    {
        enemy.IncreaseStamina(Time.deltaTime * 25f);
    }
    public EnemyPoiseBroken(EnemyTypes enemy, ICanDamage lastWeapon)
    {
        enemyForDestructor = enemy;
        enemy.enemyMovement.PoiseBrokenMoveArrange(lastWeapon.weight * enemy.poiseMultiplier);
        GameManager.instance.StartCoroutine(ExitPoiseBroken(enemy, enemy.PoiseBrokenTime));
        enemy.animator.CrossFade("PoiseBroken", 0.1f);
        enemy.animator.CrossFade("Empty", 0.1f);
    }
    ~EnemyPoiseBroken()
    {
        enemyForDestructor.enemyMovement.isLerpingIdle = false;
    }
    IEnumerator ExitPoiseBroken(EnemyTypes enemy, float time)
    {
        yield return new WaitForSeconds(time);
        if (enemy.enemyState is EnemyPoiseBroken)
            enemy.enemyState = new EnemyWalk(enemy);
    }
}





