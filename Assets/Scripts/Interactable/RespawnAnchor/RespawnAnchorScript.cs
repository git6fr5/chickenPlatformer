using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnAnchorScript : MonoBehaviour
{

    /* --- Debug --- */
    private string DebugTag = "[MnM RespawnAnchorScript]  ";
    private bool DEBUG_collision = false;

    public LayerMask playerLayer;

    /* --- External Scripts ---*/

    public InteractableAnimation animation2D;

    /* --- Collision Functions ---*/

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        LayerMask layerMask = LayerMask.GetMask(LayerMask.LayerToName(hitInfo.gameObject.layer));

        if (DEBUG_collision) { print(DebugTag + "Projectile has hit an object in the layer " + layerMask.value.ToString() + " but need the layer " + playerLayer.value.ToString()); }

        if (layerMask == playerLayer)
        {
            ResetPreviousRespawnAnchor(hitInfo);
            SetRespawnAnchor(hitInfo);
        }
    }

    public void SetRespawnAnchor(Collider2D hitInfo)
    {
        CharacterScript characterScript = hitInfo.gameObject.GetComponent<CharacterScript>();
        characterScript.respawnObject = gameObject;
        animation2D.active = true; animation2D.activating = true;
    }

    public void ResetPreviousRespawnAnchor(Collider2D hitInfo)
    {
        CharacterScript characterScript = hitInfo.gameObject.GetComponent<CharacterScript>();
        if (characterScript.respawnObject) // this should always be true, but just in case
        {
            characterScript.respawnObject.GetComponent<RespawnAnchorScript>().animation2D.active = false;

        }
    }
}
