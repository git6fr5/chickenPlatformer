using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProjectileScript : MonoBehaviour
{

    /* --- Debug --- */
    private string DebugTag = "[MnM ProjectileScript]  ";
    private bool DEBUG_collision = false;
    public float lifeTime;

    public GameObject abilityObject;
    private AbilityScript abilityScript;

    void Start()
    {
        abilityScript = abilityObject.GetComponent<AbilityScript>();
        Destroy(gameObject, lifeTime);
        Fire();
    }

    /* --- Collision Functions ---*/

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        LayerMask layerMask = LayerMask.GetMask(LayerMask.LayerToName(hitInfo.gameObject.layer));

        if (DEBUG_collision) { print(DebugTag + "Projectile has hit an object in the layer " + layerMask.value.ToString() + " but need the layer " + abilityScript.targetLayer.value.ToString() ); }

        if (layerMask == abilityScript.targetLayer)
        {
            CharacterScript characterScript = hitInfo.gameObject.GetComponent<CharacterScript>();
            characterScript.Damage(abilityScript.damageValue, abilityScript.damageType);

            Destroy(gameObject);
        }
    }

    void Fire()
    {
        Vector2 targetDirection = -((Vector2)abilityScript.casterObject.transform.position - abilityScript.target);
        targetDirection.Normalize();
        Rigidbody2D body = GetComponent<Rigidbody2D>();
        body.AddForce(abilityScript.force * targetDirection);
    }

}
