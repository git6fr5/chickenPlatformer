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

    public void ResetToDefault()
    {
        selectedAbilityImage.sprite = defaultSprite;
    }

    void FixedUpdate()
    {
        textBox.text = time[0].ToString() + ":" + time[1].ToString() + ":" + time[2].ToString();
    }
}
