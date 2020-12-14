using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformScript : MonoBehaviour
{

    /* --- Debug --- */
    private string DebugTag = "[MnM RespawnAnchorScript]  ";
    private bool DEBUG_collision = false;

    public LayerMask playerLayer;
    public bool isFalling;
    public float waitTime = 0f;

    public bool isMoving;
    public Vector2 direction;
    public float length;
    public float speed;
    private float elapsedLength = 0;
    private Vector2 initPos;

    /* --- External Scripts ---*/

    public InteractableAnimation animation2D;

    void Start()
    {
        direction.Normalize();
        initPos = transform.position;
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            //print(elapsedLength);
            if (Mathf.Abs(elapsedLength) > length)
            {
                direction = -direction;
            }
            transform.position = transform.position + (Vector3)direction * speed;
            elapsedLength = Mathf.Abs(Vector2.Distance(initPos, transform.position));
        }
    }

    /* --- Collision Functions ---*/

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        LayerMask layerMask = LayerMask.GetMask(LayerMask.LayerToName(hitInfo.gameObject.layer));

        if (DEBUG_collision) { print(DebugTag + gameObject.name + " has hit an object in the layer " + layerMask.value.ToString() + " but need the layer " + playerLayer.value.ToString()); }

        if (layerMask == playerLayer)
        {
            if (isFalling)
            {
                StartCoroutine("Fall", waitTime);
            }
            if (isMoving)
            {
                Connect(hitInfo);
            }
        }
    }

    void OnTriggerExit2D(Collider2D hitInfo)
    {
        LayerMask layerMask = LayerMask.GetMask(LayerMask.LayerToName(hitInfo.gameObject.layer));

        if (DEBUG_collision) { print(DebugTag + gameObject.name + " has hit an object in the layer " + layerMask.value.ToString() + " but need the layer " + playerLayer.value.ToString()); }

        if (layerMask == playerLayer)
        {
            if (isMoving)
            {
                Disconnect(hitInfo);
            }
        }
    }

    public IEnumerator Fall(float elapsedTime)
    {
        yield return new WaitForSeconds(elapsedTime);

        if (GetComponent<Rigidbody2D>())
        {
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            GetComponent<Rigidbody2D>().gravityScale = 1f;
        }

        yield return null;
    }

    public void Connect(Collider2D hitInfo)
    {
        hitInfo.gameObject.transform.parent.SetParent(gameObject.transform);
    }

    public void Disconnect(Collider2D hitInfo)
    {
        hitInfo.gameObject.transform.parent.SetParent(null);
    }
}
