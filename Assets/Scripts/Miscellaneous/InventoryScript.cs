using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InventoryScript : MonoBehaviour
{
    public Sprite defaultSprite;
    public List<Image> inventoryItems = new List<Image>();

    [HideInInspector] public bool isOpened = false;

    public Image selectedAbilityImage;
    public Text timerBox;
    public GameObject abilityBox;

    private List<Image> abilityImages = new List<Image>();

    private float elapsedTime = 0;

    public void ResetToDefault()
    {
        selectedAbilityImage.sprite = defaultSprite;
        //Close();
    }

    void Update()
    {
        timerBox.text = ((int)(elapsedTime % 60)).ToString();
    }

    void FixedUpdate()
    {
        elapsedTime = elapsedTime + Time.fixedDeltaTime;
    }

    public void Open(GameObject clientObject)
    {
        /*CharacterScript clientScript = clientObject.GetComponent<CharacterScript>();
        for (int i = 0; i < clientScript.abilityObjectList.Count; i++)
        {
            Image abilityImage = Instantiate(selectedAbilityImage, selectedAbilityImage.transform.position, Quaternion.identity, abilityBox.transform);
            abilityImage.transform.position = new Vector3(abilityImage.transform.position.x, abilityImage.transform.position.y + (i+1) * abilityImage.rectTransform.sizeDelta.y, abilityImage.transform.position.z);
            abilityImage.sprite = clientScript.abilityObjectList[i].GetComponent<SpriteRenderer>().sprite;
            abilityImages.Add(abilityImage);
        }*/
        isOpened = true;
    }

    public void Close()
    {
        /*foreach (Image image in abilityImages)
        {
            Destroy(image.gameObject);
        }
        abilityImages = new List<Image>();
        print("Closing Inventory");*/
        isOpened = false;
    }
}
