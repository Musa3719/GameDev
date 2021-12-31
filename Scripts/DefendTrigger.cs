using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefendTrigger : MonoBehaviour
{
    public List<GameObject> TouchingObjects;
    private void Awake()
    {
        TouchingObjects = new List<GameObject>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<MeleeWeapon>(out MeleeWeapon meleeWeapon))
        {
            if (!TouchingObjects.Contains(other.gameObject))
                TouchingObjects.Add(other.gameObject);

            Player.instance.DecreaseStamina(5);
            if (Player.instance.Stamina == 0 || meleeWeapon.isUpAttacking)
            {
                //vfx sfx vs
                //defend broke animation
                Player.instance.playerState = new PoiseBroken(meleeWeapon);
            }
            else
            {
                //vfx sfx vs
                //defender deflect animation with different positions and rotations
                meleeWeapon.gameObject.GetComponent<Collider>().enabled = false;//take no damage
            }
        }

    }
    void OnTriggerExit(Collider other)
    {
        if (TouchingObjects.Contains(other.gameObject))
            TouchingObjects.Remove(other.gameObject);
    }

}
