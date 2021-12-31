using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : EnemyTypes
{

    public Enemy(float health, float armor, GameObject weaponPrefab, ArmorType armorType)
    {
        MaxHealth = health;
        Health = MaxHealth;
        MaxArmor = armor;
        Armor = MaxArmor;
        this.armorType = armorType;
        weapon = weaponPrefab;
        animator = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
