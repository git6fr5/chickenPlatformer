using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityScript : MonoBehaviour
{
    /* --- Debug --- */
    private string DebugTag = "[MnM AbilityScript]  ";
    //private bool DEBUG_cast = false;
    private bool DEBUG_collision = false;

    /* --- Parameters ---*/
    public LayerMask collectorLayer;

    public string abilityType;
    public bool isBuff = false;
    public bool isAOE = false;

    public bool isPassive = false;
    private bool passiveApplied = false;
    private GameObject passiveObject;

    public float cooldown;
    private float currentCooldown = 0f;

    public GameObject objectBase;
    public float force;
    public float radius;

    /* --- Status ---*/
    public float damageValue;
    public string damageType;

    [HideInInspector] public GameObject casterObject;
    [HideInInspector] public Vector2 target;
    [HideInInspector] public LayerMask targetLayer;

    void FixedUpdate()
    {
        CheckCooldown();
    }

    void OnCollisionEnter2D(Collision2D hitInfo)
    {
        CollectAbility(hitInfo);

    }

    void CheckCooldown()
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

    /* --- Ability Box Functions --- */

    void CollectAbility(Collision2D hitInfo)
    {
        Collider2D hitCollider = hitInfo.collider;
        if (DEBUG_collision) { print(DebugTag + "The object " + hitInfo.otherCollider.name + " has collided with " + hitCollider.name); }
        LayerMask layerMask = LayerMask.GetMask(LayerMask.LayerToName(hitCollider.gameObject.layer));

        if (layerMask == collectorLayer)
        {
            CharacterScript hitCharacterScript = hitCollider.gameObject.GetComponent<CharacterScript>();
            bool alreadyAccquired = false;
            foreach (GameObject abilityObject in hitCharacterScript.abilityObjectList)
            {
                if (abilityObject.name == name)
                {
                    alreadyAccquired = true;
                    break;
                }
            }

            if (!alreadyAccquired)
            {
                hitCharacterScript.abilityObjectList.Add(gameObject);
                transform.parent = hitCollider.gameObject.transform;
                GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<Rigidbody2D>().simulated = false;
                GetComponent<BoxCollider2D>().enabled = false;
                transform.localPosition = new Vector3(0, 0, 0);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    // abilities are cast by pressing a button
    public void Cast(GameObject _casterObject, bool cast, Vector2 _target, LayerMask _targetLayer)
    {
        if (cast)
        {
            casterObject = _casterObject;
            target = _target;
            targetLayer = _targetLayer;

            if (currentCooldown == 0f)
            {
                Vector2 targetDirection = -((Vector2)casterObject.transform.position - target);
                targetDirection.Normalize();
                GameObject projectileObject = Instantiate(objectBase, casterObject.transform.position + (Vector3)targetDirection, Quaternion.identity);
                projectileObject.SetActive(true);
                currentCooldown = cooldown;
            }
        }
    }

    // abilities that apply an effect when selected
    public void Apply(GameObject _casterObject, bool apply, LayerMask _targetLayer)
    {
        casterObject = _casterObject;
        targetLayer = _targetLayer;

        if (!passiveApplied)
        {
            passiveObject = Instantiate(objectBase, Vector3.zero, Quaternion.identity);
            passiveObject.SetActive(apply);
            passiveApplied = true;
        }
        else if (passiveApplied && !apply)
        {
            Destroy(passiveObject);
            passiveApplied = false;
        }
    }

}