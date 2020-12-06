using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyBoxScript : MonoBehaviour
{

    /* --- Debug --- */
    private string DebugTag = "[MnM SkyBoxScript]  ";
    private bool DEBUG_time = false;
    private bool DEBUG_sky = true;

    /* --- Time --- */
    public float time = 0f; // in seconds
    private float tickRate = 20f; // how many seconds in game correspond to 1 IRL seconds
    public float dayLength = 86400f; // in seconds, adjusted for tickRate in start

    private int seconds;
    private int minute;
    private int hour;
    private bool day;

    /*--- Sky ---*/
    private SpriteRenderer skySpriteRenderer;
    private float height;
    private float width;
    private Vector3 position;
    private float offSet;

    private float cameraHeight = 5;

    public GameObject trail;

    // Start is called before the first frame update
    void Start()
    {
        seconds = 0;
        minute = 0;
        hour = 0;
        dayLength = dayLength / tickRate;

        skySpriteRenderer = GetComponent<SpriteRenderer>();
        height = skySpriteRenderer.bounds.size.y;
        width = skySpriteRenderer.bounds.size.x;

        offSet = (height / 2) - cameraHeight;

        position = transform.localPosition;

        //trail.transform.localPosition = new Vector3(position.x, position.y - height, position.z);

        if (DEBUG_sky) { print(DebugTag + "Sky, height: " + height + "width: " + width);  }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        hour = (int)Mathf.Floor(time / 3600);
        minute = (int)Mathf.Floor(time / 60) - (hour*3600);
        seconds = (int)Mathf.Floor(time) - (minute * 60) - (hour * 3600);
        if (DEBUG_time) { print(DebugTag + "In Game Time is " + hour.ToString() + "h:" + minute.ToString() + "m:" + seconds.ToString() + "s"); }

        position.y = offSet + (time / dayLength) * height;
        transform.localPosition = position;
    }

    void FixedUpdate()
    {
        time = time + (Time.fixedDeltaTime * tickRate);
        time = (time % dayLength);

        // at time = 0, position. y = offset
        // at time = dayLength, position.y = offset - height
        // therefore y(time) = offset - (z(time) ) height
        // z(dayLength) = 1 => time/dayLength
    }
}
