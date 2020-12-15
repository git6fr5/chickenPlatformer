using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainScript : MonoBehaviour
{

    /* --- Debug --- */
    private string DebugTag = "[MnM TerrainScript]  ";
    private bool DEBUG_collision = false;

    public LayerMask playerLayer;

    public bool isMuddy;
    public bool isWater;

    private float weightForce;

    void Start()
    {

    }

    /* --- Collision Functions ---*/

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        LayerMask layerMask = LayerMask.GetMask(LayerMask.LayerToName(hitInfo.gameObject.layer));

        if (DEBUG_collision) { print(DebugTag + gameObject.name + " has hit an object in the layer " + layerMask.value.ToString() + " but need the layer " + playerLayer.value.ToString()); }

        if (layerMask == playerLayer)
        {
            if (isMuddy)
            {
                CharacterScript characterScript = hitInfo.gameObject.GetComponent<CharacterScript>();
                characterScript.runSpeed = characterScript.runSpeed / 3;
                characterScript.controller2D.m_JumpForce = characterScript.controller2D.m_JumpForce / 3;
            }
        }
    }

    void OnTriggerStay2D(Collider2D hitInfo)
    {
        /*LayerMask layerMask = LayerMask.GetMask(LayerMask.LayerToName(hitInfo.gameObject.layer));

        if (DEBUG_collision) { print(DebugTag + gameObject.name + " has hit an object in the layer " + layerMask.value.ToString() + " but need the layer " + playerLayer.value.ToString()); }

        if (layerMask == playerLayer)
        {
            if (isMuddy)
            {
                CharacterScript characterScript = hitInfo.gameObject.GetComponent<CharacterScript>();
                hitInfo.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, weightForce));
            }
        }*/
    }

    void OnTriggerExit2D(Collider2D hitInfo)
    {
        LayerMask layerMask = LayerMask.GetMask(LayerMask.LayerToName(hitInfo.gameObject.layer));

        if (DEBUG_collision) { print(DebugTag + gameObject.name + " has hit an object in the layer " + layerMask.value.ToString() + " but need the layer " + playerLayer.value.ToString()); }

        if (layerMask == playerLayer)
        {
            if (isMuddy)
            {
                CharacterScript characterScript = hitInfo.gameObject.GetComponent<CharacterScript>();
                characterScript.runSpeed = characterScript.runSpeed * 3;
                characterScript.controller2D.m_JumpForce = characterScript.controller2D.m_JumpForce * 3;

            }
        }
    }
}
