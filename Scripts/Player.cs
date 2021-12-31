using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Humanoid
{
    public static Player instance;
    public int attackPatternCount;
    #region StaminaNeeds
    public float NeedForRun { get; private set; }
    public float RunConsume { get; private set; }
    public float NeedForAttack { get; private set; }
    public float NeedForDeflect { get; private set; }
    public float NeedForDodge { get; private set; }
    public float NeedForRoll { get; private set; }
    
    #endregion

    public GameObject[] playerWeapons;
    public GameObject currentWeapon;
    public ICharacterStates playerState;

    public Collider QuaDefendHitbox;
    public Collider DefendHitbox;

    public GameObject selectedThrowable;

    public float LastTimeDeflectedCounter;

    private void Awake()
    {
        instance = this;
        LastTimeDeflectedCounter = 100f;
        selectedThrowable = GameManager.instance.ThrowablePrefabs[0];
        animator = GetComponentInChildren<Animator>();
        playerState = new Idle();
        GetComponent<PlayerCombat>().ChangeWeapon(1);
        armorType = ArmorType.Armored;

        NeedForRun = 8f;
        RunConsume = 10f;
        NeedForAttack = 6f;
        NeedForDeflect = 12f;
        NeedForDodge = 6f;
        NeedForRoll = 20f;

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

        //Fetch Player save data
    }
    public void ChangeThrowable()
    {
        //if(isThereAny) then selected=new
    }
    
    public void TakeArmour(float armor)
    {
        Armor += armor;
        if (Armor > MaxArmor)
            Armor = MaxArmor;
    }
    public void TakeHealth(float health)
    {
        Health += health;
        if (Health > MaxHealth)
            Health = MaxHealth;
    }
    // Update is called once per frame
    void Update()
    {
        if (GameManager.isPaused) return;

        base.Update();
        LastTimeDeflectedCounter += Time.deltaTime;
        playerState = playerState.CheckForStateChange();
        playerState.StateUpdate();
    }
    
    

    
}
