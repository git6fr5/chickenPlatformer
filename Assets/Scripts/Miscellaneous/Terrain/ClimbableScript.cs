using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbableScript : MonoBehaviour
{

    /* --- Debug --- */
    //private string DebugTag = "[MnM RespawnAnchorScript]  ";
    //private bool DEBUG_collision = false;

    public LayerMask playerLayer;

    private float bufferTime = 0.4f;

    private Rigidbody2D body;

    /* --- External Scripts ---*/

    public InteractableAnimation animation2D;

    /* --- Collision Functions ---*/

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        LayerMask layerMask = LayerMask.GetMask(LayerMask.LayerToName(hitInfo.gameObject.layer));
        if (layerMask == playerLayer)
        {
            CharacterScript characterScript = hitInfo.gameObject.GetComponent<CharacterScript>();
            if (characterScript.verticalMove != 0)
            {
                body = hitInfo.gameObject.GetComponent<Rigidbody2D>();
                body.constraints = RigidbodyConstraints2D.FreezePositionX; // RigidbodyConstraints.FreezeRotation;
                body.velocity = Vector2.zero;
                StartCoroutine("AttachBuffer", bufferTime);
            }
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