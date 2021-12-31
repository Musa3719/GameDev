using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : EnemyTypes
{



    private void Awake()
    {
        animator = GetComponent<Animator>();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
