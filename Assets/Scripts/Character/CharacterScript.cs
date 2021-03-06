﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterScript : MonoBehaviour
{

    /* --- Debug --- */
    private string DebugTag = "[MnM CharacterScript]  ";
    private bool DEBUG_status = false;
    private bool DEBUG_motion = false;
    private bool DEBUG_ability = false;
    //private bool DEBUG_inventory = false;

    public bool Client = false;

    /* --- Status --- */

    public float characterHealthBase = 3f;
    protected float characterModifierHealth = 0;
    public float magicalShieldBase = 0f;
    public float mechanicalShieldBase = 0f;

    private Dictionary<string, float> damageTypes = new Dictionary<string, float>();
    private Dictionary<string, float> shieldBases = new Dictionary<string, float>();

    protected float characterHealth;
    protected float totalDamage;

    public Sprite[] spriteDataBase;
    public GameObject spriteObject;

    private GameObject[] heartObjects = new GameObject[0];
    private GameObject[] magicalShieldObjects = new GameObject[0];
    private GameObject[] mechanicalShieldObjects = new GameObject[0];

    bool tookDamage = false;
    bool onDeath = false;

    public float aggroRadius;
    public float attackRange;
    private bool aggro;
    private GameObject enemyObject;
    private Vector3 enemyDirection;

    public GameObject respawnObject;

    /* --- Motion --- */

    public float runSpeed = 40f;
    float horizontalMove = 0f;

    public float climbSpeed = 20f;
    [HideInInspector] public float verticalMove = 0f;

    float left = -1f;
    float right = 1f;
    bool jump = false;
    bool crouch = false;
    [HideInInspector] public bool inAir = true;
    [HideInInspector] public bool climbing = false;

    public GameObject fallCheck;

    /* --- Ability --- */

    bool cast = false;

    private bool hasAbility = false;
    public List<GameObject> abilityObjectList = new List<GameObject>();

    private GameObject selectedAbilityObject;
    private AbilityScript selectedAbilityScript;
    public int selectedAbilityIndex = 0;
    private bool selectingAbility = false;

    private string abilityName;
    private string abilityType;
    private bool abilityIsBuff;
    private bool abilityIsPassive;

    public LayerMask enemyLayer;
    public LayerMask friendlyLayer;
    private LayerMask targetLayer;

    /*--- Inventory ---*/

    public int coins = 0;
    public GameObject coinObject;

    public GameObject inventoryObject;
    private InventoryScript inventoryScript;
    bool openInventory = false;
    //float inventoryTime = 0.2f;

    /* --- External Scripts ---*/

    public CharacterController2D controller2D;
    public CharacterAnimation animation2D;

    // Start is called before the first frame update
    void Start()
    {
        DamageTypes();
        ShieldBases();
        Health();
        SelectAbility(selectedAbilityIndex);
        if (Client)
        {
            Inventory();
        }
    }

    // Update is called once per frame
    void Update()
    {
        StatusFlag();
        OutOfWorldFlag();

        if (Client)
        {
            MotionControlFlag();
            AbilityControlFlag();
            InventoryControlFlag();
        }
        else
        {
            AIAggroFlag();
            AIMotionControlFlag();
            AIAbilityControlFlag();
        }
        if (openInventory)
        {
            InventorySelectFlag();
        }
    }

    void FixedUpdate()
    {
        //print("calling fixed update");
        MotionControl();
        AbilityControl();
        if (Client)
        {
           InventoryControl();
        }
    }

    /* --- Status Functions --- */

    private void StatusFlag()
    {
        if (tookDamage == true)
        {
            Health();
            tookDamage = false;
            animation2D.hurt = true;
        }
        if (onDeath == true)
        {
            Death();
            animation2D.death = true;
        }
    }

    private void StatusControl()
    {

    }

    private void StatusBar()
    {
        if (DEBUG_status) { print(DebugTag + "Running StatusBar"); }

        int numHearts = (int)Mathf.Ceil(characterHealth);
        if (DEBUG_status) { print(DebugTag + "Running StatusBar for Hearts with " + numHearts.ToString()); }
        heartObjects = DisplayOnStatusBar(numHearts, 0, spriteDataBase[0], heartObjects);
        int magicalShields = (int)Mathf.Ceil(GetShield("magical"));
        magicalShieldObjects = DisplayOnStatusBar(magicalShields, 1, spriteDataBase[1], magicalShieldObjects);
        int mechanicalShields = (int)Mathf.Ceil(GetShield("mechanical"));
        mechanicalShieldObjects = DisplayOnStatusBar(mechanicalShields, 2, spriteDataBase[2], mechanicalShieldObjects);
    }

    private GameObject[] DisplayOnStatusBar(int size, int row_index, Sprite sprite, GameObject[] spriteObjects)
    {
        if (DEBUG_status) { print(DebugTag + "Running DisplayOnStatusBar"); }
        for (int i = 0; i < spriteObjects.Length; i++)
        {
            Destroy(spriteObjects[i]);
        }
        spriteObjects = new GameObject[size];

        float yOffSet = +1.5f + transform.position.y;
        float xOffSet = 0;
        if ((size % 2) == 0) {
            xOffSet = -(size - 1) / 2;
        }
        else
        {
            xOffSet = (-(size - 1) / 2) + 0.5f;
        }

        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        float row = (float)row_index;

        for (int i = 0; i < size; i++)
        {
            float col = spriteRenderer.bounds.min.x + (float)i;
            spriteObjects[i] = Instantiate(spriteObject, new Vector3(col + xOffSet, row + yOffSet, Vector3.back.z), Quaternion.identity, gameObject.transform);
            spriteObjects[i].GetComponent<SpriteRenderer>().sprite = sprite;
        }
        return spriteObjects;
    }

    private float Health()
    {
        if (DEBUG_status) { print(DebugTag + "Running Health"); }
        characterHealth = characterHealthBase + characterModifierHealth - totalDamage;
        if (characterHealth <= 0) { characterHealth = 0; onDeath = true; }

        StatusBar();
        return characterHealth;
    }

    private void ShieldBases()
    {
        if (DEBUG_status) { print(DebugTag + "Running DamageTypes"); }

        List<string> types = new List<string>();
        shieldBases.Add("magical", magicalShieldBase);
        shieldBases.Add("mechanical", mechanicalShieldBase);
    }

    private float GetShield(string keyString)
    {
        float shieldHealth = shieldBases[keyString] - damageTypes[keyString];
        if (shieldBases[keyString] - damageTypes[keyString] > 0)
        {
            return shieldHealth;
        }
        return 0f;
    }

    private void DamageTypes()
    {
        if (DEBUG_status) { print(DebugTag + "Running DamageTypes"); }

        List<string> types = new List<string>();
        damageTypes.Add("magical", 0);
        damageTypes.Add("mechanical", 0);
    }

    public float Damage(float damageValue, string damageType)
    {
        if (DEBUG_status) { print(DebugTag + "Running Damage"); }
        damageTypes[damageType] = damageTypes[damageType] + damageValue;

        totalDamage = 0;
        foreach (KeyValuePair<string, float> damage in damageTypes)
        {
            if (damage.Value - shieldBases[damage.Key] > 0)
            {
                totalDamage = totalDamage + damage.Value - shieldBases[damage.Key];
            }
        }
        if (DEBUG_status) { print(DebugTag + "Total Damage of " + totalDamage.ToString()); }

        tookDamage = true;
        return totalDamage;
    }

    public void Death()
    {
        foreach (GameObject _abilityObject in abilityObjectList)
        {
            GameObject droppedAbilityObject = Instantiate(_abilityObject, transform.position, Quaternion.identity);
            droppedAbilityObject.GetComponent<SpriteRenderer>().enabled = true;
            droppedAbilityObject.GetComponent<Rigidbody2D>().simulated = true;
            droppedAbilityObject.GetComponent<BoxCollider2D>().enabled = true;
            droppedAbilityObject.transform.parent = null;
            Destroy(_abilityObject);
        }
        if (Client)
        {
            Client = false;
            StartCoroutine(Respawn(1.0f));
        }
        else
        {
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            DropCoins();
            Destroy(gameObject, 1.0f);
        }
        onDeath = false;
    }

    private IEnumerator Respawn(float elapsedTime)
    {
        yield return new WaitForSeconds(elapsedTime);

        damageTypes = new Dictionary<string, float>();
        shieldBases = new Dictionary<string, float>();
        abilityObjectList = new List<GameObject>();
        inventoryScript.ResetToDefault();
        totalDamage = 0;

        DamageTypes();
        ShieldBases();
        Health();
        SelectAbility(0);

        transform.position = new Vector3(respawnObject.transform.position.x, respawnObject.transform.position.y, 0);

        Client = true;

        yield return null;
 
    }

    private void DropCoins()
    {
        for (int i = 0; i<coins; i++)
        {
            Instantiate(coinObject, transform.position, Quaternion.identity).SetActive(true);
        }
    }

    private void AIAggroFlag()
    {
        Collider2D[] enemyColliders = Physics2D.OverlapCircleAll(transform.position, aggroRadius, enemyLayer);

        if (enemyColliders.Length > 0)
        {
            aggro = true;
            Collider2D enemyCollider = enemyColliders[0];
            enemyObject = enemyCollider.gameObject;
            enemyDirection = (enemyObject.transform.position - transform.position);
        }
        else
        {
            aggro = false;
        }
    }

    /* --- Motion Functions --- */

    private void MotionControlFlag()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        verticalMove = Input.GetAxisRaw("Vertical") * climbSpeed;

        if (Input.GetButtonDown("Jump"))
        {
            if (DEBUG_motion) { print(DebugTag + "Pressed Jump"); }
            jump = true;
            inAir = true;
        }

        if (Input.GetButtonDown("Crouch")) 
        { 
            if (DEBUG_motion) { print(DebugTag + "Pressed Crouch"); } 
            crouch = true;
        }
        else if (Input.GetButtonUp("Crouch")) 
        { 
            if (DEBUG_motion) { print(DebugTag + "Released Crouch"); } 
            crouch = false;
        }
    }

    private void MotionControl()
    {
        if (DEBUG_motion) { print(DebugTag + "Running MotionControl"); }
        climbing = controller2D.Move(horizontalMove * Time.fixedDeltaTime, verticalMove * Time.fixedDeltaTime, crouch, jump);
        jump = false;

        animation2D.crouch = crouch;
        animation2D.x_speed = Mathf.Abs(horizontalMove);
        animation2D.inAir = inAir;
        animation2D.climbing = climbing;
    }

    private void AIMotionControlFlag()
    {
        if (aggro) 
        {
            if (Vector3.Distance(transform.position, enemyObject.transform.position) < attackRange)
            {
                horizontalMove = 0f;
            }
            else if (enemyDirection.x > 0) { horizontalMove = right * runSpeed; }
            else { horizontalMove = left * runSpeed; }
        }
        else
        {
            horizontalMove = 0f;
        }
    }

    public void OnLanding()
    {
        if (DEBUG_motion) { print(DebugTag + gameObject.name + " landed"); }
        inAir = false;
    }

    /* --- Ability Functions --- */

    public void SelectAbility(int index)
    {
        if (DEBUG_ability) { print("Attempting to select a new ability for " + gameObject.name);  }
        if (index < abilityObjectList.Count)
        {
            selectedAbilityObject = abilityObjectList[index];
            if (DEBUG_ability) { print("Selected a new ability for " + selectedAbilityObject.name);  }

            //abilityObject = Instantiate(abilityObject, gameObject.transform.position, Quaternion.identity, gameObject.transform);
            abilityName = selectedAbilityObject.name;
            selectedAbilityScript = selectedAbilityObject.GetComponent<AbilityScript>();
            abilityType = selectedAbilityScript.abilityType;
            abilityIsBuff = selectedAbilityScript.isBuff;
            abilityIsPassive = selectedAbilityScript.isPassive;

            if (abilityIsBuff)
            {
                targetLayer = friendlyLayer;
            }
            else
            {
                targetLayer = enemyLayer;
            }
            if (Client)
            {
                inventoryScript.selectedAbilityImage.sprite = selectedAbilityObject.GetComponent<SpriteRenderer>().sprite;
            }
            if (DEBUG_ability) { print("Name of active ability is " + abilityName + " which is a " + abilityType + " ability, and it targets the layer " + ((int)targetLayer).ToString()); }
            hasAbility = true;
        }
        else
        {
            selectedAbilityObject = null;
            hasAbility = false;
        }
    }

    private void AbilityControlFlag()
    {
        for (int i = 0; i < 10; i++)
        {
            if (Input.GetKeyDown(i.ToString()))
            {
                selectingAbility = true;
                selectedAbilityIndex = i - 1;
            }
        }
        if (Input.GetKeyDown("z") || Input.GetMouseButtonDown(0)) 
        { 
            print(DebugTag + "Pressed Cast");
            enemyDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            cast = true; 
        }
        if ((Input.GetKeyDown("x") || Input.GetMouseButtonDown(1)) && (abilityObjectList.Count > 0) )
        {
            selectingAbility = true;
            selectedAbilityIndex = (selectedAbilityIndex + 1) % abilityObjectList.Count;
        }
    }

    private void AbilityControl()
    {
        if (hasAbility && !(abilityIsPassive))
        {
            selectedAbilityScript.Cast(gameObject, cast, enemyDirection, targetLayer);
        }
        else if (hasAbility && (abilityIsPassive))
        {
            selectedAbilityScript.Apply(gameObject, true, targetLayer);
        }
        if (selectingAbility)
        {
            // when changing abilities, make sure to remove any passive effects
            if (abilityIsPassive)
            {
                selectedAbilityScript.Apply(selectedAbilityObject, false, targetLayer);
            }
            SelectAbility(selectedAbilityIndex);
        }
        selectingAbility = false;
        cast = false;
    }

    private void AIAbilityControlFlag()
    {
        if (aggro && (Vector3.Distance(transform.position, enemyObject.transform.position) <= attackRange))
        {
            cast = true;
        }
        else
        {
            cast = false;
        }
        //animation2D.animator.SetTrigger("attack");
    }

    /* --- Inventory Functions --- */

    private void Inventory()
    {
        inventoryObject.SetActive(true);
        inventoryScript = inventoryObject.GetComponent<InventoryScript>();
    }

    private void InventoryControlFlag()
    {
        if (Input.GetKeyDown("e")) { print(DebugTag + "Pressed Pause"); openInventory = !openInventory; }
    }

    private void InventorySelectFlag()
    {

    }

    private void InventoryControl()
    {
        /*if (openInventory && (!inventoryScript.isOpened))
        {
            if (DEBUG_inventory) { print(DebugTag + "opening inventory"); }
            inventoryScript.Open(gameObject);
        }
        else if (!openInventory && (inventoryScript.isOpened))
        {
            inventoryScript.Close();
        }*/
    }

    private void OutOfWorldFlag()
    {
        if (Mathf.Abs(transform.position.y) > 100)
        {
            if (Client)
            {
                transform.position = new Vector3(respawnObject.transform.position.x, respawnObject.transform.position.y, 0);
            }
            else
            {
                onDeath = true;
            }
        }
    }
}
