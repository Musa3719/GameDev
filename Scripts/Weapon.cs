using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum DamageType
{
    Cut,//kesme
    Thrust,//knife, dagger, arrow
    Explosive,
    Crush,//ezme
}
public abstract class ICanDamage : MonoBehaviour
{
    public float damage;
    public DamageType type;
    public float weight;
}
public abstract class Weapon : ICanDamage
{
    
    public GameObject Owner;
    public float attackTime;
}
public abstract class MeleeWeapon : Weapon
{
    public bool isUpAttacking;
    
    public abstract void Attack(string animName);
    
    public abstract void Defend();
    public abstract void Deflect();
}

public class Sword : MeleeWeapon
{
    public Sword(GameObject owner, float damage)
    {
        this.damage = damage;
        type = DamageType.Cut;
        Owner = owner;
        weight = 5;
        attackTime = StateMethods.FindAnimationLenght(Owner.GetComponent<Humanoid>().animator, "SwordAttack");
    }
    
    public override void Attack(string animName)
    {
        if (animName.StartsWith("Up"))
        {
            isUpAttacking = true;
        }
        GetComponent<Collider>().enabled = true;
        Invoke("CloseCollider", attackTime);
        Owner.GetComponent<Humanoid>().animator.CrossFade("Sword "+animName, 0.1f);
        //play Sword VFX, SFX vs vs
    }
    void CloseCollider()
    {
        isUpAttacking = false;
        GetComponent<Collider>().enabled = false;
    }
    public override void Defend()
    {
        
        //Play Sword Defend animation
    }
    public override void Deflect()
    {
        //play Sword Deflect animation, VFX, SFX vs vs
        //Player.instance.animator.CrossFade("Deflect", 0.1f);
    }
}
public class Warhammer : MeleeWeapon
{
    public Warhammer(GameObject owner, float damage)
    {
        this.damage = damage;
        type = DamageType.Crush;
        Owner = owner;
        weight = 10;
        attackTime = StateMethods.FindAnimationLenght(Owner.GetComponent<Humanoid>().animator, "WarhammerAttack");
    }
    public override void Attack(string animName)//additional damage direct to enemy health(not armour)
    {
        GetComponent<Collider>().enabled = true;
        Invoke("CloseCollider", attackTime);
        Owner.GetComponent<Humanoid>().animator.CrossFade("Warhammer "+animName, 0.1f);
        //play warhammer VFX, SFX vs vs
    }
    void CloseCollider()
    {
        GetComponent<Collider>().enabled = false;
    }
    public override void Defend()
    {
        
        //Play warhammer Defend animation
    }
    public override void Deflect()
    {
        //play warhammer Deflect animation, VFX, SFX vs vs (biraz geri gider)
        //Player.instance.animator.CrossFade("Deflect", 0.1f);
    }
}

public abstract class Projectile : ICanDamage
{
    public Vector3 speed;
    protected Rigidbody rb;
    
    protected void Awake()//call parent awake
    {
        rb.velocity = speed;
    }
    protected void Update()//call parent update
    {
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y - Time.deltaTime * 0.4f, rb.velocity.z);
    }

    public virtual void ToBackDirection()
    {
        rb.velocity = new Vector3(-rb.velocity.x, rb.velocity.y, -rb.velocity.z);
    } 
    public virtual void ToRandomDirection()
    {
        rb.velocity = new Vector3(Random.Range(-1f, 1f), rb.velocity.normalized.y, Random.Range(0f, 2f)).normalized * rb.velocity.magnitude;
        rb.useGravity = true;
    }
    
}