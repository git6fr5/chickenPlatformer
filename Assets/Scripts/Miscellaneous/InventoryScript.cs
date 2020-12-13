using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InventoryScript : MonoBehaviour
{
    public Sprite defaultSprite;
    public Image selectedAbilityImage;
    public List<Image> inventoryItems = new List<Image>();

    public Text textBox;
    public int[] time = new int[3];

    private float elapsedTime = 0;

    public void ResetToDefault()
    {
        selectedAbilityImage.sprite = defaultSprite;
    }

    void Update()
    {
        textBox.text = ((int)(elapsedTime % 60)).ToString();
    }

    void FixedUpdate()
    {
        elapsedTime = elapsedTime + Time.fixedDeltaTime;
        //textBox.text = time[0].ToString() + ":" + time[1].ToString() + ":" + time[2].ToString();
    }
}
