using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterScript : MonoBehaviour
{

    /* --- Debug --- */
    private string DebugTag = "[MnM CharacterScript]  ";
    private bool DEBUG_status = false;
    private bool DEBUG_motion = false;
    private bool DEBUG_ability = false;

    public bool Client = false;

    /* --- Status --- */

    public float characterHealthBase = 3f;
    protected float characterModifierHealth = 0;
    public float magicalShieldBase = 2f;
    public float mechanicalShieldBase = 2f;

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
    private bool aggro;
    private GameObject enemyObject;
    private Vector3 enemyDirection;

    /* --- Motion --- */

    public float runSpeed = 40f;
    float horizontalMove = 0f;
    float left = -1f;
    float right = 1f;
    bool jump = false;
    bool crouch = false;

    /* --- Ability --- */

    bool cast = false;
    public GameObject abilityObjectBase;
    private GameObject abilityObject;
    private string abilityName;
    private string abilityType;
    private bool abilityIsBuff;

    public LayerMask enemyLayer;
    public LayerMask friendlyLayer;
    private LayerMask targetLayer;

    public CharacterController2D controller2D;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        DamageTypes();
        ShieldBases();
        Health();
        Ability();
    }

    // Update is called once per frame
    void Update()
    {
        StatusFlag();
        if (Client)
        {
            MotionControlFlag();
            AbilityControlFlag();
        }
        else
        {
            AIAggroFlag();
            AIMotionControlFlag();
            AIAbilityControlFlag();
        }
    }

    void FixedUpdate()
    {
        if (Client)
        {
            MotionControl();
            AbilityControl();
        }
        else
        {
            AIMotionControl();
            AIAbilityControl();
        }
    }

    /* --- Status Functions --- */

    private void StatusFlag()
    {
        if (tookDamage == true)
        {
            animator.SetTrigger("hurt");
            Health();
            tookDamage = false;
        }
        if (onDeath == true)
        {
            animator.SetTrigger("dead");
            Death();
        }
    }

    private void StatusBar()
    {
        if (DEBUG_status) { print(DebugTag + "Running StatusBar"); }

        int numHearts = (int)Mathf.Ceil(characterHealth);
        if (DEBUG_status) { print(DebugTag + "Running StatusBar for Hearts with " + numHearts.ToString()); }
        heartObjects = DisplayOnStatusBar(numHearts, 2, spriteDataBase[0], heartObjects);
        int magicalShields = (int)Mathf.Ceil(GetShield("magical"));
        magicalShieldObjects = DisplayOnStatusBar(magicalShields, 1, spriteDataBase[1], magicalShieldObjects);
        int mechanicalShields = (int)Mathf.Ceil(GetShield("mechanical"));
        mechanicalShieldObjects = DisplayOnStatusBar(mechanicalShields, 0, spriteDataBase[2], mechanicalShieldObjects);
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
        Destroy(gameObject, 0.4f);
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
            horizontalMove = 0f;
        }
    }

    /* --- Motion Functions --- */

    private void MotionControlFlag()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        if (Input.GetButtonDown("Jump"))
        {
            if (DEBUG_motion) { print(DebugTag + "Pressed Jump"); }
            jump = true;
            //animator.SetBool("isJumping", true);
        }

        if (Input.GetButtonDown("Crouch")) { if (DEBUG_motion) { print(DebugTag + "Pressed Crouch"); } crouch = true; }
        else if (Input.GetButtonUp("Crouch")) { if (DEBUG_motion) { print(DebugTag + "Released Crouch"); } crouch = false; }

        animator.SetFloat("speed", Mathf.Abs(horizontalMove));
    }

    private void MotionControl()
    {
        if (DEBUG_motion) { print(DebugTag + "Running MotionControl"); }
        controller2D.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
        jump = false;
    }

    private void AIMotionControlFlag()
    {
        if (aggro) 
        {
            if (enemyDirection.x > 0) { horizontalMove = right * runSpeed; }
            else { horizontalMove = left * runSpeed; }
        }
        else
        {
            horizontalMove = 0f;
        }


        animator.SetFloat("speed", Mathf.Abs(horizontalMove));
    }

    private void AIMotionControl()
    {
        controller2D.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
        jump = false;
    }

    public void OnLanding()
    {
        //animator.SetBool("isJumping", false);
    }

    /* --- Ability Functions --- */

    private void Ability()
    {
        abilityObject = Instantiate(abilityObjectBase, gameObject.transform.position, Quaternion.identity, gameObject.transform);
        abilityName = abilityObjectBase.name;
        abilityType = abilityObjectBase.GetComponent<AbilityScript>().abilityType;
        abilityIsBuff = abilityObjectBase.GetComponent<AbilityScript>().isBuff;
        if (abilityIsBuff)
        {
            targetLayer = friendlyLayer;
        }
        else
        {
            targetLayer = enemyLayer;
        }
        if (DEBUG_ability) { print("Name of active ability is " + abilityName + " which is a " + abilityType + " ability, and it targets the layer " + ((int)targetLayer).ToString()); }
    }

    private void AbilityControlFlag()
    {
        if (Input.GetKeyDown("z")) { print(DebugTag + "Pressed Cast"); cast = true; }
    }

    private void AbilityControl()
    {
        controller2D.Cast(abilityObject, cast, Camera.main.ScreenToWorldPoint(Input.mousePosition), targetLayer);
        cast = false;
    }

    private void AIAbilityControlFlag()
    {
        if (aggro)
        {
            cast = true;
        }
        else
        {
            cast = false;
        }
        //animator.SetTrigger("attack");
    }

    private void AIAbilityControl()
    {
        controller2D.Cast(abilityObject, cast, enemyDirection, targetLayer);
        cast = false;
    }

}
