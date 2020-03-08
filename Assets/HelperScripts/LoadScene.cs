using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadScene : MonoBehaviour
{
    [SerializeField] private StringVariable loadSceneEffectName = null;
    [SerializeField] private StringVariable newSceneToLoad = null;
    public void LoadSceneCallback(StringVariable name)
    {
        SceneManager.LoadScene(name, LoadSceneMode.Single);
    }

    public void LoadSceneWithFadeCallback(StringVariable name)
    {
        if (name != null)
        {
            newSceneToLoad.SetValue(name);
            Scene scene = SceneManager.GetSceneByName(loadSceneEffectName);
            if (scene.name != loadSceneEffectName.value)
            {
                SceneManager.LoadScene(loadSceneEffectName, LoadSceneMode.Additive);
            }
        }
        else
        {
            Debug.LogWarning("Please set a valid name string variable");
        }
    }
}
