using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BounceScript : MonoBehaviour
{

    /* --- Debug --- */
    private string DebugTag = "[MnM BounceScript]  ";
    private bool DEBUG_collision = false;
    public float lifeTime;

    public GameObject abilityObject;
    private AbilityScript abilityScript;
    private Vector2 contactAngle;
    private Vector3 position;
    private Rigidbody2D body;

    // caster object

    public LayerMask groundLayer;

    void Start()
    {
        abilityScript = abilityObject.GetComponent<AbilityScript>();
        body = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime);
        Fire();
    }

    void LateUpdate()
    {
        FaceDirection();
    }

    /* --- Collision Functions ---*/

    void OnCollisionEnter2D(Collision2D hitInfo)
    {

        Collider2D hitObject = hitInfo.collider;
        if (DEBUG_collision) { print(DebugTag + "The object " + hitInfo.otherCollider.name + " has collided with " + hitObject.name); }
        LayerMask layerMask = LayerMask.GetMask(LayerMask.LayerToName(hitObject.gameObject.layer));

        if (DEBUG_collision) { print(DebugTag + "Projectile has hit an object in the layer " + layerMask.value.ToString() + " but need the layer " + abilityScript.targetLayer.value.ToString()); }

        if (layerMask == abilityScript.targetLayer)
        {
            CharacterScript characterScript = hitObject.gameObject.GetComponent<CharacterScript>();
            characterScript.Damage(abilityScript.damageValue, abilityScript.damageType);

            Destroy(gameObject);
        }
        if (layerMask == groundLayer)
        {
            ContactPoint2D contactPoint = hitInfo.GetContact(hitInfo.contactCount - 1);
        }
    }

    void Fire()
    {
        Vector2 targetDirection = -((Vector2)abilityScript.casterObject.transform.position - abilityScript.target);
        targetDirection.Normalize();
        body.AddForce(abilityScript.force * targetDirection);
        transform.right = targetDirection;
    }

    void FaceDirection()
    {
        float projectileAngle = 0;
        if (body.velocity.y != 0)
        {
            projectileAngle = Mathf.Atan(body.velocity.y / body.velocity.x) / Mathf.PI * 180;
        }
        transform.eulerAngles = Vector3.forward * projectileAngle;
    }
}
