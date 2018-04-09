using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class LoadSceneAsync : MonoBehaviour
{
    public LoadSceneMode loadMode = LoadSceneMode.Single;
    public bool allowSceneActivation = true;

    public UnityEvent onLoaded = new UnityEvent();

    private AsyncOperation asyncLoad = null;

    public void Load(string name)
    {
        asyncLoad = SceneManager.LoadSceneAsync(name, loadMode);
        OnLoadStarted(asyncLoad);
    }

    public void Load(int buildIndex)
    {
        asyncLoad = SceneManager.LoadSceneAsync(buildIndex, loadMode);
        OnLoadStarted(asyncLoad);
    }

    public void AllowSceneActivation(bool allow)
    {
        asyncLoad.allowSceneActivation = allow;
    }

    private void OnLoadStarted(AsyncOperation asyncLoad)
    {
        asyncLoad.allowSceneActivation = allowSceneActivation;
        StartCoroutine(WaitForCompletion(asyncLoad));
    }

    private IEnumerator WaitForCompletion(AsyncOperation asyncLoad)
    {
        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                onLoaded.Invoke();
                break;
            }
            yield return null;
        }
    }
}
