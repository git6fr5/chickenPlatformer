using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierScript : MonoBehaviour
{
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
    }

    void OnTriggerStay2D(Collider2D hitInfo)
    {

        if (bounce)
        {
            Bounce();
        }

        if (root)
        {
            Root();
        }

        LayerMask layerMask = LayerMask.GetMask(LayerMask.LayerToName(hitInfo.gameObject.layer));

        if (layerMask == abilityScript.targetLayer)
        {
            CharacterScript characterScript = hitInfo.gameObject.GetComponent<CharacterScript>();
            characterScript.Damage(abilityScript.damageValue, abilityScript.damageType);
        }

    }

    void Bounce()
    {
        Rigidbody2D casterBody = abilityScript.casterObject.GetComponent<Rigidbody2D>();
        casterBody.AddForce(new Vector2(0f, abilityScript.force));
    }

    void BouncePoint()
    {
        Transform groundCheckTransform = abilityScript.casterObject.transform.Find("GroundCheck");
        transform.SetParent(groundCheckTransform);
        transform.localPosition = Vector3.zero;
    }

    void Root()
    {
        Rigidbody2D casterBody = abilityScript.casterObject.GetComponent<Rigidbody2D>();
        casterBody.constraints = RigidbodyConstraints2D.FreezeAll;
    }
}