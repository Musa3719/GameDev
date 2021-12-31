using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterStates
{
    ICharacterStates CheckForStateChange();
    void StateUpdate();
}
public static class StateMethods
{
    private static float cutArmorStrenghtRate = 0.2f;
    private static float crushArmorStrenghtRate = 0.1f;
    private static float thrustArmorStrenghtRate = 0.15f;
    private static float explosiveArmorStrenghtRate = 0.05f;

    public static bool Fire1(float staminaNeeded)
    {

        if (Input.GetButtonDown("Fire1") && staminaNeeded <= Player.instance.Stamina)
        {
            PlayerCombat.leftClicked?.Invoke();
            return true;
        }
        return false;
    }

    public static float[] ArrangeDamage(ICanDamage iCanDamage, Humanoid attackedHuman)
    {
        if (attackedHuman.Armor <= 0)
        {
            if(iCanDamage.type == DamageType.Cut)
            {
                var damageToHealth = iCanDamage.damage * 1.75f;
                return new float[] { damageToHealth, 0 };
            }
            else if (iCanDamage.type == DamageType.Crush)
            {
                var damageToHealth = iCanDamage.damage * 1.5f;
                return new float[] { damageToHealth, 0 };
            }
            else if (iCanDamage.type == DamageType.Explosive)
            {
                var damageToHealth = iCanDamage.damage * 1.5f;
                return new float[] { damageToHealth, 0 };
            }
            else if (iCanDamage.type == DamageType.Thrust)
            {
                var damageToHealth = iCanDamage.damage * 1.5f;
                return new float[] { damageToHealth, 0 };
            }
            return new float[] { 0, 0 };
        }
        else
        {
            if (iCanDamage.type == DamageType.Cut)
            {
                float damageToArmor;
                float damageToHealth;
                bool isCrit = Random.Range(0, 100) >= 82 ? true : false;
                if (isCrit)
                {
                    damageToArmor = 0;
                    damageToHealth = iCanDamage.damage * 1.75f;
                }
                else
                {
                    damageToArmor = (1 - ((int)attackedHuman.armorType) * cutArmorStrenghtRate) * iCanDamage.damage;
                    damageToHealth = 0; 
                }

                if (iCanDamage as MeleeWeapon != null)
                {
                    if ((iCanDamage as MeleeWeapon).isUpAttacking)
                    {
                        float AddToHealth = damageToArmor * 7f / 10f;
                        damageToHealth += AddToHealth;
                        damageToArmor -= AddToHealth;
                        if (damageToArmor < 0) damageToArmor = 0;
                    }
                }

                return new float[] { damageToHealth, damageToArmor };
            }
            else if (iCanDamage.type == DamageType.Crush)
            {
                var damageToArmor = (1 - ((int)attackedHuman.armorType) * crushArmorStrenghtRate) * iCanDamage.damage;
                var damageToHealth = damageToArmor / 10f * 1.5f;

                if (iCanDamage as MeleeWeapon != null)
                {
                    if ((iCanDamage as MeleeWeapon).isUpAttacking)
                    {
                        float AddToHealth = damageToArmor * 7f / 10f;
                        damageToHealth += AddToHealth;
                        damageToArmor -= AddToHealth;
                        if (damageToArmor < 0) damageToArmor = 0;
                    }
                }

                return new float[] { damageToHealth, damageToArmor };
            }
            else if (iCanDamage.type == DamageType.Explosive)
            {
                var damageToArmor = (1 - ((int)attackedHuman.armorType) * explosiveArmorStrenghtRate) * iCanDamage.damage;
                var damageToHealth = damageToArmor;

                if (iCanDamage as MeleeWeapon != null)
                {
                    if ((iCanDamage as MeleeWeapon).isUpAttacking)
                    {
                        float AddToHealth = damageToArmor * 7f / 10f;
                        damageToHealth += AddToHealth;
                        damageToArmor -= AddToHealth;
                        if (damageToArmor < 0) damageToArmor = 0;
                    }
                }

                return new float[] { damageToHealth, damageToArmor };
            }
            else if (iCanDamage.type == DamageType.Thrust)
            {
                var damageToArmor = (1 - ((int)attackedHuman.armorType) * thrustArmorStrenghtRate) * iCanDamage.damage;
                var damageToHealth = damageToArmor / 2f;

                if (iCanDamage as MeleeWeapon != null)
                {
                    if ((iCanDamage as MeleeWeapon).isUpAttacking)
                    {
                        float AddToHealth = damageToArmor * 7f / 10f;
                        damageToHealth += AddToHealth;
                        damageToArmor -= AddToHealth;
                        if (damageToArmor < 0) damageToArmor = 0;
                    }
                }

                return new float[] { damageToHealth, damageToArmor };
            }
            return new float[] { 0, 0 };
        }
    }
    
    
    public static bool CanRotationRun()
    {
        if(Player.instance.transform.InverseTransformDirection(PlayerMovement.instance.rb.velocity.normalized).z > 0.03f)
        {
            return true;
        }
        return false;
    }
    public static float FindAnimationLenght(Animator animator, string animName)
    {
        var clips = animator.runtimeAnimatorController.animationClips;
        foreach (var item in clips)
        {
            float multiplier = 1f;
            if (GameManager.instance.AnimationSpeedMultipliers.ContainsKey(animName))
            {
                multiplier = GameManager.instance.AnimationSpeedMultipliers[animName];
            }
            
            if (item.name == animName)
            {
                return item.length / multiplier;
            }
        }
        return 0;
    }
}
public class Idle : ICharacterStates
{
    public ICharacterStates CheckForStateChange()
    {
        if (StateMethods.Fire1(Player.instance.NeedForAttack))
        {
            return new Attack();
        }
        /*if (Input.GetKeyDown(KeyCode.X))
        {
            return new Hurt();
        }*/
        if (Input.GetButtonDown("Dodge") && Player.instance.Stamina > Player.instance.NeedForDodge)
        {
            return new Dodge();
        }
        if (Input.GetButtonDown("Jump") && Player.instance.Stamina > Player.instance.NeedForRoll)
        {
            return new Roll();
        }
        if (Input.GetButtonDown("Fire2"))
        {
            return new Defend();
        }
        if (Input.GetButtonDown("Qua") && !GameManager.FindObjectOfType<QuaDefendTrigger>().isDeflecting)
        {
            return new QuaDefend();
        }
        if (Input.GetButtonDown("Throw"))
        {
            return new Throw();
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if(h!=0 || v != 0)
        {
            if (Input.GetButton("Run") && Player.instance.Stamina > Player.instance.NeedForRun && StateMethods.CanRotationRun())
            {
                return new Run();
            }

            return new Walk();
        }

        return this;
    }
    public void StateUpdate()
    {
        Player.instance.IncreaseStamina(Time.deltaTime * 25f);
        PlayerMovement.instance.Idle();
    }

    public Idle()
    {
        PlayerCombat.attackCount = 0;
        PlayerCombat.leftClicked = PlayerCombat.FirstAttack;
        PlayerMovement.instance.animator.CrossFade("Idle",0.2f);
        PlayerMovement.instance.animator.CrossFade("Empty", 0.2f);
    }
}
public class Walk : ICharacterStates
{
    public ICharacterStates CheckForStateChange()
    {
        if (StateMethods.Fire1(Player.instance.NeedForAttack))
        {
            return new Attack();
        }
        if (Input.GetButtonDown("Dodge") && Player.instance.Stamina > Player.instance.NeedForDodge)
        {
            return new Dodge();
        }
        if (Input.GetButtonDown("Jump") && Player.instance.Stamina > Player.instance.NeedForRoll)
        {
            return new Roll();
        }
        if (Input.GetButtonDown("Fire2"))
        {
            return new Defend();
        }
        if (Input.GetButtonDown("Qua") && !GameManager.FindObjectOfType<QuaDefendTrigger>().isDeflecting)
        {
            return new QuaDefend();
        }
        if (Input.GetButtonDown("Throw"))
        {
            return new Throw();
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (h != 0 || v != 0)
        {
            if (Input.GetButton("Run") && Player.instance.Stamina > Player.instance.NeedForRun && StateMethods.CanRotationRun())
            {
                return new Run();
            }

            return this;
        }

        return new Idle();
    }
    public void StateUpdate()
    {
        Player.instance.IncreaseStamina(Time.deltaTime * 12.5f);
        PlayerMovement.instance.Walk();
        
    }
    public Walk()
    {
        PlayerCombat.attackCount = 0;
        PlayerCombat.leftClicked = PlayerCombat.FirstAttack;
        PlayerMovement.instance.animator.CrossFade("Walk",0.2f);
        CheckForQuaDeflect();
    }
    void CheckForQuaDeflect()
    {
        if (!(Player.instance.playerState is QuaDefend && Input.GetKeyDown(KeyCode.Mouse0)))
        {
            PlayerMovement.instance.animator.CrossFade("Empty", 0.2f);
        }
    }
}
public class Run : ICharacterStates
{
    public ICharacterStates CheckForStateChange()
    {
        if (Player.instance.transform.InverseTransformDirection(PlayerMovement.instance.rb.velocity.normalized).z <= 0f)
        {
            return new Walk();
        }
        if (StateMethods.Fire1(Player.instance.NeedForAttack))
        {
            return new Attack();
        }
        if (Input.GetButtonDown("Dodge") && Player.instance.Stamina > Player.instance.NeedForDodge)
        {
            return new Dodge();
        }
        if (Input.GetButtonDown("Jump") && Player.instance.Stamina > Player.instance.NeedForRoll)
        {
            return new Roll();
        }
        if (Input.GetButtonDown("Fire2"))
        {
            return new Defend();
        }
        if (Input.GetButtonDown("Qua") && !GameManager.FindObjectOfType<QuaDefendTrigger>().isDeflecting)
        {
            return new QuaDefend();
        }
        if (Input.GetButtonDown("Throw"))
        {
            return new Throw();
        }

        if (Player.instance.Stamina <= 0)
        {
            return new Walk();
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (h != 0 || v != 0)
        {
            if (Input.GetButton("Run"))
            {
                return this;
            }

            return new Walk();
        }

        return new Idle();
    }
    public void StateUpdate()
    {
        Player.instance.DecreaseStamina(Time.deltaTime * Player.instance.RunConsume);
        PlayerMovement.instance.Run();
        
    }
    public Run()
    {
        PlayerCombat.attackCount = 0;
        PlayerCombat.leftClicked = PlayerCombat.FirstAttack;
        PlayerMovement.instance.animator.CrossFade("Run",0.2f);
        PlayerMovement.instance.animator.CrossFade("Empty", 0.2f);
    }
}
public class Attack : ICharacterStates
{
    public bool leftClickInputBuffer;
    public bool upAttackInputBuffer;
    public ICharacterStates CheckForStateChange()
    {
        return this;
    }
    public void StateUpdate()
    {
        if(Input.GetButtonDown("Fire1") && Player.instance.NeedForAttack <= Player.instance.Stamina)
        {
            leftClickInputBuffer = true;
        }
        if(Input.GetButtonDown("UpAttack") && Player.instance.NeedForAttack <= Player.instance.Stamina)
        {
            upAttackInputBuffer = true;
        }
    }
    public Attack()
    {
        Player.instance.DecreaseStamina(Player.instance.NeedForAttack);
        PlayerCombat.leftClicked = null;
        PlayerMovement.instance.AttackMoveArrange();
        Player.instance.StartCoroutine(ExitAttack(Player.instance.currentWeapon.GetComponent<MeleeWeapon>().attackTime));
    }
    IEnumerator ExitAttack(float time)
    {
        yield return new WaitForSeconds(time);
        if (upAttackInputBuffer && Player.instance.playerState is Attack)
        {
            PlayerCombat.UpAttack();
            Player.instance.playerState = new Attack();
        }
        else if (leftClickInputBuffer && Player.instance.playerState is Attack)
        {
            PlayerCombat.AttackContinue();
            Player.instance.playerState = new Attack();
        }
        else if(Player.instance.playerState is Attack)
        {
            Player.instance.playerState = new Walk();
        }
        
    }
}
public class Dodge : ICharacterStates
{
    public ICharacterStates CheckForStateChange()
    {
        return this;
    }
    public void StateUpdate()
    {

    }
    public Dodge()
    {
        Player.instance.DecreaseStamina(Player.instance.NeedForDodge);
        PlayerCombat.leftClicked = null;
        PlayerMovement.instance.DodgeMoveArrange();
        Player.instance.StartCoroutine(ExitDodge(Player.instance.DodgeTime));
        Player.instance.animator.CrossFade("Dodge",0.1f);
        PlayerMovement.instance.animator.CrossFade("Empty", 0.1f);
    }
    IEnumerator ExitDodge(float time)
    {
        yield return new WaitForSeconds(time);
        if (Player.instance.playerState is Dodge)
            Player.instance.playerState = new Walk();
    }
}

public class Roll : ICharacterStates
{
    public ICharacterStates CheckForStateChange()
    {
        return this;
    }
    public void StateUpdate()
    {

    }
    public Roll()
    {
        Player.instance.DecreaseStamina(Player.instance.NeedForRoll);
        PlayerCombat.leftClicked = null;
        PlayerMovement.instance.RollMoveArrange();
        Player.instance.StartCoroutine(ExitRoll(Player.instance.RollTime));
        Player.instance.animator.CrossFade("Roll",0.1f);
        PlayerMovement.instance.animator.CrossFade("Empty", 0.1f);
    }
    IEnumerator ExitRoll(float time)
    {
        yield return new WaitForSeconds(time);
        if (Player.instance.playerState is Roll)
            Player.instance.playerState = new Walk();
    }
}
public class Defend : ICharacterStates
{
    public ICharacterStates CheckForStateChange()
    {

        if (!Input.GetButton("Fire2"))
        {
            CloseDefendHitbox();
            return new Walk();
        }
        if (Input.GetButtonDown("Dodge") && Player.instance.Stamina > Player.instance.NeedForDodge)
        {
            CloseDefendHitbox();
            return new Dodge();
        }
        if (Input.GetButtonDown("Jump") && Player.instance.Stamina > Player.instance.NeedForRoll)
        {
            CloseDefendHitbox();
            return new Roll();
        }
        if (Input.GetButtonDown("Qua") && !GameManager.FindObjectOfType<QuaDefendTrigger>().isDeflecting)
        {
            CloseDefendHitbox();
            return new QuaDefend();
        }
        if (Input.GetButtonDown("Throw"))
        {
            CloseDefendHitbox();
            return new Throw();
        }

        return this;
    }
    void CloseDefendHitbox()
    {
        Player.instance.DefendHitbox.gameObject.SetActive(false);
    }
    public void StateUpdate()
    {
        PlayerMovement.instance.DefendMove();
        Player.instance.IncreaseStamina(Time.deltaTime * 8f);
        StateMethods.Fire1(Player.instance.NeedForDeflect);
    }
    public Defend()
    {
        Player.instance.DefendHitbox.gameObject.SetActive(true);
        PlayerCombat.leftClicked = PlayerCombat.Deflect;
        Player.instance.currentWeapon.GetComponent<MeleeWeapon>().Defend();
    }
}

public class QuaDefend : ICharacterStates
{
    public ICharacterStates CheckForStateChange()
    {
        if (StateMethods.Fire1(0))
        {
            return new Walk();
        }
        if (!Input.GetButton("Qua"))
        {
            return new Walk();
        }
        if (Input.GetButtonDown("Dodge") && Player.instance.Stamina > Player.instance.NeedForDodge)
        {
            return new Dodge();
        }
        if (Input.GetButtonDown("Jump") && Player.instance.Stamina > Player.instance.NeedForRoll)
        {
            return new Roll();
        }
        if (Input.GetButtonDown("Fire2"))
        {
            return new Defend();
        }
        if (Input.GetButtonDown("Throw"))
        {
            return new Throw();
        }

        return this;
    }
    public void StateUpdate()
    {
        PlayerMovement.instance.DefendMove();
        Player.instance.IncreaseStamina(Time.deltaTime * 8f);
    }
    public QuaDefend()
    {
        Player.instance.QuaDefendHitbox.gameObject.SetActive(true);
        PlayerCombat.leftClicked = PlayerCombat.QuaDeflect;
        Player.instance.animator.CrossFade("QuaDefend", 0.1f);
    }
    ~QuaDefend()
    {
        if (!GameManager.FindObjectOfType<QuaDefendTrigger>().isDeflecting)
        {
            Player.instance.QuaDefendHitbox.gameObject.SetActive(false);
        }
    }
}

public class Throw : ICharacterStates
{
    public ICharacterStates CheckForStateChange()
    {
        if (!Input.GetButton("Throw"))
        {
            return new Walk();
        }
        if (Input.GetButtonDown("Dodge") && Player.instance.Stamina > Player.instance.NeedForDodge)
        {
            return new Dodge();
        }
        if (Input.GetButtonDown("Jump") && Player.instance.Stamina > Player.instance.NeedForRoll)
        {
            return new Roll();
        }
        if (Input.GetButtonDown("Fire2"))
        {
            return new Defend();
        }
        if (Input.GetButtonDown("Qua") && !GameManager.FindObjectOfType<QuaDefendTrigger>().isDeflecting)
        {
            return new QuaDefend();
        }

        return this;
    }
    public void StateUpdate()
    {
        PlayerMovement.instance.ThrowMove();
        StateMethods.Fire1(0);
    }
    public Throw()
    {
        PlayerCombat.leftClicked = PlayerCombat.Throw;
        Player.instance.animator.CrossFade("ThrowState", 0.1f);
    }
}
public class Hurt : ICharacterStates
{
    ICanDamage lastWeapon;
    public ICharacterStates CheckForStateChange()
    {
        if (Player.instance.Stamina == 0)
        {
            Player.instance.IncreaseStamina(10f);
            return new PoiseBroken(lastWeapon);
        }
        return this;
    }
    public void StateUpdate()
    {
        Player.instance.IncreaseStamina(Time.deltaTime * 25f);
    }
    public Hurt(ICanDamage iCanDamage)
    {
        lastWeapon = iCanDamage;
        Player.instance.TakeDamage(iCanDamage);
        if(!(Player.instance.playerState is PoiseBroken))
        {
            Player.instance.DecreaseStamina(iCanDamage.weight * Player.instance.poiseMultiplier);
            if (Player.instance.Stamina == 0)
            {
                //arrange state outside of ctor
            }
            else
            {
                PlayerCombat.leftClicked = null;
                PlayerMovement.instance.HurtMoveArrange();
                Player.instance.StartCoroutine(ExitHurt(Player.instance.HurtTime));
                Player.instance.animator.CrossFade("Hurt", 0.1f);
                //Debug.Log(PlayerMovement.instance.animator.GetNextAnimatorStateInfo(0).length);
                PlayerMovement.instance.animator.CrossFade("Empty", 0.1f);
            }
            
        }
    }
    IEnumerator ExitHurt(float time)
    {
        yield return new WaitForSeconds(time);
        if (Player.instance.playerState is Hurt)
            Player.instance.playerState = new Walk();
    }
}
public class PoiseBroken : ICharacterStates
{
    public ICharacterStates CheckForStateChange()
    {
        return this;
    }
    public void StateUpdate()
    {
        Player.instance.IncreaseStamina(Time.deltaTime * 25f);
    }
    public PoiseBroken(ICanDamage lastWeapon)
    {
        PlayerCombat.leftClicked = null;
        PlayerMovement.instance.PoiseBrokenMoveArrange(lastWeapon.weight * Player.instance.poiseMultiplier);
        Player.instance.StartCoroutine(ExitPoiseBroken(Player.instance.PoiseBrokenTime));
        Player.instance.animator.CrossFade("PoiseBroken", 0.1f);
        //Debug.Log(PlayerMovement.instance.animator.GetNextAnimatorStateInfo(0).length);
        PlayerMovement.instance.animator.CrossFade("Empty", 0.1f);
    }
    ~PoiseBroken()
    {
        PlayerMovement.instance.isLerpingIdle = false;
    }
    IEnumerator ExitPoiseBroken(float time)
    {
        yield return new WaitForSeconds(time);
        if (Player.instance.playerState is PoiseBroken)
            Player.instance.playerState = new Walk();
    }
}
public class Interacting : ICharacterStates
{
    public ICharacterStates CheckForStateChange()
    {
        return this;
    }
    public void StateUpdate()
    {
        Player.instance.IncreaseStamina(Time.deltaTime * 25f);
        PlayerMovement.instance.Idle();
    }
    public Interacting()
    {
        PlayerCombat.leftClicked = null;
        PlayerMovement.instance.animator.CrossFade("Empty", 0.1f);
    }
    public void ReturnToIdle()
    {
        Player.instance.playerState = new Idle();
    }
}




