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
    public bool linear = true; // these 2 could be enumerated
    public bool rotate = false;
    [Range(0, 1)] public float turnRate = 0f;

    public GameObject abilityObject;
    private AbilityScript abilityScript;
    private Vector2 contactAngle;
    private Vector3 position;
    private Rigidbody2D body;

    public LayerMask groundLayer;
    public bool destroyOnGround = false; // these 2 could be enumerated
    public bool stuckOnGround = false;
    private bool isStuck = false;

    private float projectileAngle = 0f;
    private float orientationAngle = 0f;

    public GameObject passiveAbilityObjectBase;
    private GameObject passiveAbilityObject;
    private AbilityScript passiveAbilityScript;

    void Start()
    {
        abilityScript = abilityObject.GetComponent<AbilityScript>();
        if (passiveAbilityObjectBase)
        {
            print("has a passive ability");
            //passiveAbilityObject = Instantiate(passiveAbilityObjectBase, transform.position, Quaternion.identity, transform);
            passiveAbilityScript = passiveAbilityObjectBase.GetComponent<AbilityScript>();
        }

        body = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime);

        SetDirection();
        Fire();
    }

    void LateUpdate()
    {
        if (linear)
        {
            FaceDirection();
        }
        else if (rotate)
        {
            RotateDirection();
        }
    }

    /* --- Collision Functions ---*/

    void OnCollisionEnter2D(Collision2D hitInfo)
    {

        Collider2D hitObject = hitInfo.collider;
        //print(DebugTag + "The object " + hitInfo.otherCollider.name + " has collided with " + hitObject.name);
        LayerMask layerMask = LayerMask.GetMask(LayerMask.LayerToName(hitObject.gameObject.layer));

        if (DEBUG_collision) { print(DebugTag + "Projectile has hit an object in the layer " + layerMask.value.ToString() + " but need the layer " + abilityScript.targetLayer.value.ToString()); }

        if (abilityScript && layerMask == abilityScript.targetLayer && !isStuck)
        {
            CharacterScript characterScript = hitObject.gameObject.GetComponent<CharacterScript>();
            characterScript.Damage(abilityScript.damageValue, abilityScript.damageType);

            Destroy(gameObject);
        }
        if (layerMask == groundLayer)
        {
            if (passiveAbilityObjectBase)
            {
                if (passiveAbilityScript.isPassive)
                {
                    //print("attempting to cast root from seed");
                    passiveAbilityScript.Apply(gameObject, true, abilityScript.targetLayer);
                }
            }
            if (destroyOnGround)
            {
                Destroy(gameObject);
            }
            else if (stuckOnGround)
            {
                ContactPoint2D contactPoint = hitInfo.GetContact(hitInfo.contactCount - 1);
                StickInGround(contactPoint);
                rotate = false;
            }
        }
    }

    void Fire()
    {
        Vector2 targetDirection = abilityScript.targetDirection;
        targetDirection.Normalize();
        body.AddForce(abilityScript.force * targetDirection);
        transform.right = targetDirection;
    }

    void FaceDirection()
    {
        Vector3 direction;
        Vector3 orientation;
        if (body.velocity.y != 0)
        {
            projectileAngle = Mathf.Atan(body.velocity.y / body.velocity.x) / Mathf.PI * 180;
        }
        if (body.velocity.x < 0)
        {
            projectileAngle = 180f - projectileAngle;
        }
        
        direction = Vector3.forward * projectileAngle;
        orientation = Vector3.right * orientationAngle; // + Vector3.up * orientationAngle;

        transform.eulerAngles = direction + orientation;
        //print(orientationAngle);

    }

    void RotateDirection()
    {
        projectileAngle = projectileAngle + 360 * Time.fixedDeltaTime;
        transform.eulerAngles = Vector3.forward * projectileAngle;
    }

    void SetDirection()
    {
        if (abilityScript.targetDirection.x < 0)
        {
            orientationAngle = 180;
        }
        else
        {
            orientationAngle = 0;
        }
    }

    void StickInGround(ContactPoint2D contactPoint)
    {
        isStuck = true;
        body.constraints = RigidbodyConstraints2D.FreezeAll;
    }
}