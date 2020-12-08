using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InventoryScript : MonoBehaviour
{
    public Sprite defaultSprite;
    public Image selectedAbilityImage;
    public List<Image> inventoryItems = new List<Image>();

    public void ResetToDefault()
    {
        selectedAbilityImage.sprite = defaultSprite;
    }
}
