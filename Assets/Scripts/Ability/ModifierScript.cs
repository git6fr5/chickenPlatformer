using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierScript : MonoBehaviour
{

    string DebugTag = "[MnM ModifierScript] ";
    //bool DEBUG_bounce = false;
    bool DEBUG_root = true;
    public float lifeTime;
    private float livedTime = 0f;

    public GameObject abilityObject;
    private AbilityScript abilityScript;

    public bool bounce = false;
    public bool root = false;

    void Start()
    {
        abilityScript = abilityObject.GetComponent<AbilityScript>();

        if (bounce)
        {
            BouncePoint();
        }
        if (root)
        {
            RootPoint();
        }
    }

    void FixedUpdate()
    {
        livedTime = livedTime + Time.fixedDeltaTime;
        if (livedTime > lifeTime && lifeTime != -1)
        {
            abilityScript.Apply(abilityScript.gameObject, false, abilityScript.targetLayer);
        }
    }

    void OnTriggerStay2D(Collider2D hitInfo)
    {

        if (bounce)
        {
            Bounce(hitInfo);
        }

        if (root)
        {
            Root(hitInfo);
        }

    }

    void OnTriggerExit2D(Collider2D hitInfo)
    {

        if (bounce)
        {
        }

        if (root)
        {
            RootExit(hitInfo);
        }
    }

    void Bounce(Collider2D hitInfo)
    {
        Rigidbody2D casterBody = abilityScript.casterObject.GetComponent<Rigidbody2D>();
        casterBody.AddForce(new Vector2(0f, abilityScript.force));

        LayerMask layerMask = LayerMask.GetMask(LayerMask.LayerToName(hitInfo.gameObject.layer));

        if (layerMask == abilityScript.targetLayer)
        {
            CharacterScript characterScript = hitInfo.gameObject.GetComponent<CharacterScript>();
            characterScript.Damage(abilityScript.damageValue, abilityScript.damageType);
        }
    }

    void BouncePoint()
    {
        Transform groundCheckTransform = abilityScript.casterObject.transform.Find("GroundCheck");
        if (!groundCheckTransform)
        {
            groundCheckTransform = abilityScript.casterObject.transform;
        }
        transform.SetParent(groundCheckTransform);
        transform.localPosition = Vector3.zero;
    }

    void Root(Collider2D hitInfo)
    {
        if (DEBUG_root) { print(DebugTag + "Root"); }

        LayerMask layerMask = LayerMask.GetMask(LayerMask.LayerToName(hitInfo.gameObject.layer));

        if (layerMask == abilityScript.targetLayer)
        {
            Rigidbody2D characterBody = hitInfo.gameObject.GetComponent<Rigidbody2D>();
            characterBody.constraints = RigidbodyConstraints2D.FreezeAll;

            CharacterScript characterScript = hitInfo.gameObject.GetComponent<CharacterScript>();
            characterScript.Damage(abilityScript.damageValue * Time.fixedDeltaTime / lifeTime, abilityScript.damageType);
        }
        
    }

    void RootExit(Collider2D hitInfo)
    {
        if (DEBUG_root) { print(DebugTag + "UnRoot"); }

        LayerMask layerMask = LayerMask.GetMask(LayerMask.LayerToName(hitInfo.gameObject.layer));

        if (layerMask == abilityScript.targetLayer)
        {
            Rigidbody2D characterBody = hitInfo.gameObject.GetComponent<Rigidbody2D>();
            characterBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    void RootPoint()
    {
        Transform groundCheckTransform = abilityScript.casterObject.transform.Find("GroundCheck");
        transform.SetParent(null);
        if (!groundCheckTransform)
        {
            groundCheckTransform = abilityScript.casterObject.transform;
        }
        transform.position = new Vector3 (groundCheckTransform.position.x, groundCheckTransform.position.y, -1);
        transform.eulerAngles = Vector3.forward;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
    }

}