using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierScript : MonoBehaviour
{

    public ModifierState modifierState;
    public enum ModifierState { rooted, stunned, slowed }

    public GameObject abilityObject;

    void Start()
    {
        if (modifierState == ModifierState.rooted)
        {
            print(modifierState);
        }
    }

}
