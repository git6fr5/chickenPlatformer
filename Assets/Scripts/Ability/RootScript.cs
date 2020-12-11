using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootScript : MonoBehaviour
{
    /* --- Debug --- */
    private string DebugTag = "[MnM RootScript]  ";
    private bool DEBUG_collision = false;

    /*--- Object ---*/
    public float lifeTime;

    public AbilityScript abilityObject;
    private AbilityScript abilityScript;

    private bool isStuck = false;

    public LayerMask groundLayer;

    void Start()
    {
        abilityScript = abilityObject.GetComponent<AbilityScript>();
        Destroy(gameObject, lifeTime);
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll; 
        Root();
    }

    void FixedUpdate()
    {
        Root();
    }

    /* --- Collision Functions ---*/

    void OnCollisionEnter2D(Collision2D hitInfo)
    {
        Collider2D hitObject = hitInfo.collider;
        print(DebugTag + "The object " + hitInfo.otherCollider.name + " has collided with " + hitObject.name);
        LayerMask layerMask = LayerMask.GetMask(LayerMask.LayerToName(hitObject.gameObject.layer));

        if (DEBUG_collision) { print(DebugTag + "Projectile has hit an object in the layer " + layerMask.value.ToString() + " but need the layer " + abilityScript.targetLayer.value.ToString()); }

        if (layerMask == groundLayer && !isStuck)
        {
            ContactPoint2D contactPoint = hitInfo.GetContact(hitInfo.contactCount - 1);
            StickInGround(contactPoint);
        }
    }

    void Root()
    {
        Collider2D[] pullColliders = Physics2D.OverlapCircleAll(transform.position, abilityScript.radius, abilityScript.targetLayer);

        foreach (Collider2D pullCollider in pullColliders)
        {
            GameObject pullObject = pullCollider.gameObject;
            pullObject.transform.position = transform.position;

            CharacterScript characterScript = pullObject.GetComponent<CharacterScript>();
            characterScript.Damage(abilityScript.damageValue * Time.fixedDeltaTime / lifeTime, abilityScript.damageType);
        }
    }
    void StickInGround(ContactPoint2D contactPoint)
    {
        isStuck = true;
        transform.right = (Vector3)contactPoint.normal + Vector3.forward * 90;
    }
}
