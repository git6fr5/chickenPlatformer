using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnAnchorScript : MonoBehaviour
{

    /* --- Debug --- */
    private string DebugTag = "[MnM RespawnAnchorScript]  ";
    private bool DEBUG_collision = false;

    public LayerMask playerLayer;

    /* --- Collision Functions ---*/

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        LayerMask layerMask = LayerMask.GetMask(LayerMask.LayerToName(hitInfo.gameObject.layer));

        if (DEBUG_collision) { print(DebugTag + "Projectile has hit an object in the layer " + layerMask.value.ToString() + " but need the layer " + playerLayer.value.ToString()); }

        if (layerMask == playerLayer)
        {
            CharacterScript characterScript = hitInfo.gameObject.GetComponent<CharacterScript>();
            characterScript.respawnObject = gameObject;
        }
    }
}
