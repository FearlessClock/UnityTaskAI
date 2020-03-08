using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AddScene : MonoBehaviour
{
    public void AddSceneCallback(StringVariable name)
    {
        if(name != null)
        {
            Scene scene = SceneManager.GetSceneByName(name);
            if (scene.name != name.value)
            {
                SceneManager.LoadScene(name, LoadSceneMode.Additive);
            }
        }
        else
        {
            Debug.LogWarning("Please set a valid name string variable");
        }
    }
}
