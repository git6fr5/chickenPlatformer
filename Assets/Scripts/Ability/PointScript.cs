using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointScript : MonoBehaviour
{
    /* --- Debug --- */
    private string DebugTag = "[MnM PointScript]  ";
    private bool DEBUG_collision = false;

    /*--- Object ---*/
    public float lifeTime;

    public AbilityScript abilityObject;
    private AbilityScript abilityScript;

    void Start()
    {
        abilityScript = abilityObject.GetComponent<AbilityScript>();
        Destroy(gameObject, lifeTime);
        Pull();
    }

    void FixedUpdate()
    {
        Pull();
    }

    /* --- Collision Functions ---*/

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        LayerMask layerMask = LayerMask.GetMask(LayerMask.LayerToName(hitInfo.gameObject.layer));

        if (DEBUG_collision) { print(DebugTag + "Projectile has hit an object in the layer " + layerMask.value.ToString() + " but need the layer " + abilityScript.targetLayer.value.ToString()); }

        if (layerMask == abilityScript.targetLayer)
        {
            print(" HIT A SOMETHING SOEMTHING" + abilityScript.damageValue.ToString() + ", " + abilityScript.damageType);
            CharacterScript characterScript = hitInfo.gameObject.GetComponent<CharacterScript>();
            characterScript.Damage(abilityScript.damageValue, abilityScript.damageType);

            Destroy(gameObject);
        }
    }

    void Pull()
    {
        Collider2D[] pullColliders = Physics2D.OverlapCircleAll(transform.position, abilityScript.radius, abilityScript.targetLayer);

        foreach (Collider2D pullCollider in pullColliders)
        {
            GameObject pullObject = pullCollider.gameObject;
            Rigidbody2D pullBody = pullObject.GetComponent<Rigidbody2D>();
            Vector2 pullDirection = (transform.position - pullObject.transform.position);
            pullDirection.Normalize();
            pullBody.AddForce( pullDirection * Time.fixedDeltaTime * abilityScript.force);
        }
    }
}
