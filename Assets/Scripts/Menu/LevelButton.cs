using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    public void LoadScene()
    {
        SceneManager.LoadScene(1);
    }
}
