using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbableScript : MonoBehaviour
{

    /* --- Debug --- */
    private string DebugTag = "[MnM RespawnAnchorScript]  ";
    private bool DEBUG_collision = false;

    public LayerMask playerLayer;

    private float bufferTime = 0.2f;

    public Rigidbody2D body;

    /* --- External Scripts ---*/

    public InteractableAnimation animation2D;

    /* --- Collision Functions ---*/

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        LayerMask layerMask = LayerMask.GetMask(LayerMask.LayerToName(hitInfo.gameObject.layer));

        if (DEBUG_collision) { print(DebugTag + gameObject.name + " has hit an object in the layer " + layerMask.value.ToString() + " but need the layer " + playerLayer.value.ToString()); }

        if (layerMask == playerLayer)
        {
            CharacterScript characterScript = hitInfo.gameObject.GetComponent<CharacterScript>();
            body = hitInfo.gameObject.GetComponent<Rigidbody2D>();
            body.constraints = RigidbodyConstraints2D.FreezePositionX;
            StartCoroutine("AttachBuffer", bufferTime);
            body.gravityScale = body.gravityScale/10;
            body.velocity = Vector2.zero;

            characterScript.climbing = true;
        }
    }

    void OnTriggerExit2D(Collider2D hitInfo)
    {
        LayerMask layerMask = LayerMask.GetMask(LayerMask.LayerToName(hitInfo.gameObject.layer));

        if (DEBUG_collision) { print(DebugTag + gameObject.name + " has hit an object in the layer " + layerMask.value.ToString() + " but need the layer " + playerLayer.value.ToString()); }

        if (layerMask == playerLayer)
        {
            CharacterScript characterScript = hitInfo.gameObject.GetComponent<CharacterScript>();
            Rigidbody2D body = hitInfo.gameObject.GetComponent<Rigidbody2D>();
            body.gravityScale = body.gravityScale * 10;

            characterScript.climbing = false;
        }
    }

    public IEnumerator AttachBuffer(float buffer)
    {
        yield return new WaitForSeconds(buffer);
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
        body = null;
        yield return null;
    }
}
