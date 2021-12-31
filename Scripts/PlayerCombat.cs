using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public static PlayerCombat instance;
    public static Action leftClicked;

    public static int attackCount;

    private void Awake()
    {
        instance = this;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeWeapon(int number)
    {
        Player.instance.currentWeapon = Player.instance.playerWeapons[number - 1];
        //change positions and rotations
    }
    
    public static void FirstAttack()
    {
        attackCount = 1;
        if (Input.GetButtonDown("UpAttack"))
        {
            attackCount = 0;
            Player.instance.currentWeapon.GetComponent<MeleeWeapon>().Attack("UpAttack");
        }
        if (Player.instance.LastTimeDeflectedCounter <= 1f)//if deflected last
        {
            Player.instance.currentWeapon.GetComponent<MeleeWeapon>().Attack("DeflectAttack");
        }
        else if (Player.instance.playerState is Walk || Player.instance.playerState is Idle)
        {
            Player.instance.currentWeapon.GetComponent<MeleeWeapon>().Attack("WalkAttack");
        }
        else if(Player.instance.playerState is Run)
        {
            Player.instance.currentWeapon.GetComponent<MeleeWeapon>().Attack("RunAttack");
        }
        else if(Player.instance.playerState is Defend)
        {
            Player.instance.currentWeapon.GetComponent<MeleeWeapon>().Attack("DefendAttack");
        }
    }
    public static void UpAttack()
    {
        attackCount = 0;
        Player.instance.currentWeapon.GetComponent<MeleeWeapon>().Attack("UpAttack");
    }
    public static void AttackContinue()
    {
        attackCount += 1;
        Player.instance.currentWeapon.GetComponent<MeleeWeapon>().Attack("Attack" + attackCount % Player.instance.attackPatternCount);
    }
    public static void Throw()
    {
        Player.instance.animator.CrossFade("Throw",0.1f);
        Instantiate(Player.instance.selectedThrowable, Player.instance.transform.position, Quaternion.identity);
    }
    public static void QuaDeflect()
    {
        Player.instance.animator.CrossFade("QuaDeflect",0.1f);
        FindObjectOfType<QuaDefendTrigger>().OpenIsDeflecting();
    }
    public static void Deflect()
    {
        var touchingObjects = Player.instance.DefendHitbox.GetComponent<DefendTrigger>().TouchingObjects;
        foreach (var item in touchingObjects)
        {
            if (!item.CompareTag("MeleeWeapon") ||(item.GetComponent<MeleeWeapon>().isUpAttacking))
            {
                touchingObjects.Remove(item);
            }
        }
        if (touchingObjects.Count > 0)
        {
            Player.instance.LastTimeDeflectedCounter = 0f;
            Player.instance.currentWeapon.GetComponent<MeleeWeapon>().Deflect();
            foreach (var item in touchingObjects)
            {
                item.GetComponent<MeleeWeapon>().Owner.GetComponent<Humanoid>().DecreaseStamina(Player.instance.currentWeapon.GetComponent<MeleeWeapon>().weight * Player.instance.poiseMultiplier);
                if (item.GetComponent<MeleeWeapon>().Owner.GetComponent<Humanoid>().Stamina == 0)
                {
                    item.GetComponent<MeleeWeapon>().Owner.GetComponent<EnemyTypes>().enemyState =
                        new EnemyPoiseBroken(item.GetComponent<MeleeWeapon>().Owner.GetComponent<EnemyTypes>(), Player.instance.currentWeapon.GetComponent<ICanDamage>());
                }
            }
        }
    }
    
    
}
