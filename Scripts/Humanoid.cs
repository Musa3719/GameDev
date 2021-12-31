using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ArmorType
{
    NotArmored=0,
    Cloth=1,
    LightArmor=2,
    Armored=3
}
public interface IDamagable
{
    public void TakeDamage(ICanDamage iCanDamage);
}
public abstract class Humanoid : MonoBehaviour, IDamagable
{
    public float Health { get; protected set; }
    public float MaxHealth { get; protected set; }
    public float Armor { get; protected set; }
    public float MaxArmor { get; protected set; }

    public float HurtTime { get; protected set; }
    public float RollTime { get; protected set; }
    public float DodgeTime { get; protected set; }
    public float PoiseBrokenTime { get; protected set; }
    public float poiseMultiplier { get; protected set; }
    private float stamina;
    public float Stamina { get { return stamina; } protected set { if (value > 100f) { stamina = 100; } else if (value < 0) { stamina = 0; } else { stamina = value; } } }
    /*private float poise;
    public float Poise
    {
        get
        {
            return poise;
        }
        set
        {
            if (value < 0) poise = 0;
            else if (value > 100) poise = 100;
            else poise = value;
        }
    }*/

    public ArmorType armorType;
    public Animator animator;
    /*public void ArrangePoise()
    {
        if (Poise != 100)
        {
            Poise = Mathf.Clamp(Poise - Time.deltaTime * 6.5f, 0f, 100f);
        }
    }*/
    
    public void MakeRagdoll()
    {
        var rigidbodies = GetComponentsInChildren<Rigidbody>();
        var colliders = GetComponentsInChildren<Collider>();
        animator.enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().isTrigger = true;
        GetComponentInChildren<UnityEngine.Animations.Rigging.Rig>().weight = 0;
        GetComponentInChildren<FootPlacement>().enabled = false;
        foreach (Rigidbody item in rigidbodies)
        {
            item.isKinematic = false;
        }
        foreach (Collider item in colliders)
        {
            item.isTrigger = false;
        }
    }

    // Update is called once per frame
    protected void Update()
    {
        //ArrangePoise();
        /*if (Input.GetKeyDown(KeyCode.C))
        {
            MakeRagdoll();
        }*/
    }
    public virtual void TakeDamage(ICanDamage iCanDamage)//player method
    {
        bool isArmored = Armor > 0 ? true : false;
        float[] damages = StateMethods.ArrangeDamage(iCanDamage, this);
        if (damages[0] > 0)
        {
            //play hurt sound
        }
        if (damages[1] > 0)
        {
            //play armor damaged sound
        }
        Health -= damages[0];
        Armor -= damages[1];
        if (Health <= 0)
        {
            Health = 0;
            if(this is Player)
            {
                //death animation vfx sfx vs
                GameManager.instance.GameOver();
            }
            else if(this is Enemy)
            {
                //enemy dies
            }
            else if(this is Boss)
            {
                //boss dies event
            }
        }
        else if (Armor <= 0 && isArmored)
        {
            Armor = 0;
            //armor break sound, animation vs
        }
    }
    public void IncreaseStamina(float amount)
    {
        Stamina += amount;
    }
    public void DecreaseStamina(float amount)
    {
        Stamina -= amount;
    }
}
