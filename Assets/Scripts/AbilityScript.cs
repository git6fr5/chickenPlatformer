using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityScript : MonoBehaviour
{
    /* --- Debug --- */
    private string DebugTag = "[MnM AbilityScript]  ";
    private bool DEBUG_cast = false;

    /* --- Parameters ---*/
    public string abilityType;
    public bool isBuff = false;

    public float cooldown;
    private float currentCooldown = 0f;

    public GameObject objectBase;
    public float force;
    public float radius;

    /* --- Status ---*/
    public float damageValue;
    public string damageType;

    public GameObject casterObject;
    public Vector2 target;
    public LayerMask targetLayer;

    public void Cast(GameObject _casterObject, Vector2 _target, LayerMask _targetLayer)
    {
        casterObject = _casterObject;
        target = _target;
        targetLayer = _targetLayer;

        if (currentCooldown == 0f)
        {
            if (DEBUG_cast) { print(DebugTag + "Cast Ability successfully"); }
            if (abilityType == "projectile")
            {
                CastProjectile(casterObject, target, targetLayer);
            }
            if (abilityType == "point")
            {
                CastPoint(casterObject, target, targetLayer);
            }

            currentCooldown = cooldown;
        }
    }

    void FixedUpdate()
    {
        if (currentCooldown > 0f)
        {
            currentCooldown = currentCooldown - Time.fixedDeltaTime;
        }
        else if (currentCooldown <= 0f)
        {
            currentCooldown = 0f;
        }
    }

    public GameObject CastProjectile(GameObject casterObject, Vector2 target, LayerMask targetLayer)
    {
        Vector2 targetDirection = -((Vector2)casterObject.transform.position - target);
        targetDirection.Normalize();
        GameObject projectileObject = Instantiate(objectBase, casterObject.transform.position + (Vector3)targetDirection, Quaternion.identity);
        projectileObject.SetActive(true);
        return projectileObject;
    }

    public GameObject CastPoint(GameObject casterObject, Vector2 target, LayerMask targetLayer)
    {
        GameObject pointObject = Instantiate(objectBase, target, Quaternion.identity);
        pointObject.SetActive(true);
        return pointObject;
    }
}
