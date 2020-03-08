using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnloadSceneCallback : MonoBehaviour
{
    public void UnloadScene(StringVariable sceneName)
    {
        Scene toUnload = SceneManager.GetSceneByName(sceneName);
        if(sceneName == null || sceneName.value == null || toUnload.name == null)
        {
            Debug.Log("Could not unload a scene on object " + name );
        }else if (toUnload != null && toUnload.name.Equals(sceneName.value))
        {
            SceneManager.UnloadSceneAsync(toUnload);
        }
    }

    public void UnloadScene(string scenename)
    {
        StringVariable var = ScriptableObject.CreateInstance<StringVariable>();
        var.value = scenename;
        UnloadScene(var);
    }
}
