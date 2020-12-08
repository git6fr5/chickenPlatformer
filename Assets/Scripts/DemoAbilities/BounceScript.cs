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

    // caster object

    public LayerMask groundLayer;

    void Start()
    {
        abilityScript = abilityObject.GetComponent<AbilityScript>();
        BouncePoint();
    }

    /* --- Collision Functions ---*/

    void OnTriggerStay2D(Collider2D hitInfo)
    {
        LayerMask layerMask = LayerMask.GetMask(LayerMask.LayerToName(hitInfo.gameObject.layer));

        if (DEBUG_collision) { print(DebugTag + "Projectile has hit an object in the layer " + layerMask.value.ToString() + " but need the layer " + abilityScript.targetLayer.value.ToString()); }

        if (layerMask == abilityScript.targetLayer)
        {
            CharacterScript characterScript = hitInfo.gameObject.GetComponent<CharacterScript>();
            characterScript.Damage(abilityScript.damageValue, abilityScript.damageType);
            Bounce();
        }

        if (layerMask == groundLayer)
        {
            Bounce();
        }
    }

    void Bounce()
    {
        Rigidbody2D casterBody = abilityScript.casterObject.GetComponent<Rigidbody2D>();
        casterBody.AddForce(new Vector2(0f, abilityScript.force));
    }

    void BouncePoint()
    {
        print(abilityScript.casterObject);
        Transform groundCheckTransform = abilityScript.casterObject.transform.Find("GroundCheck");
        transform.SetParent(groundCheckTransform);
        transform.localPosition = Vector3.zero;
    }
}
