using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuaDefendTrigger : MonoBehaviour
{
    public bool isDeflecting;
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<Projectile>(out Projectile projectile))
        {
            if (isDeflecting)
            {
                projectile.ToBackDirection();
            }
            else
            {
                projectile.ToRandomDirection();
            }
        }
    }
    public void OpenIsDeflecting()
    {
        isDeflecting = true;
        float time = StateMethods.FindAnimationLenght(Player.instance.animator, "QuaDeflect");
        Invoke("ToFalse", time);
    }
    public void ToFalse()
    {
        isDeflecting = false;
        Player.instance.QuaDefendHitbox.gameObject.SetActive(false);
    }
}
