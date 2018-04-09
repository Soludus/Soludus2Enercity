using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public LoadSceneMode loadMode = LoadSceneMode.Single;

    public void Load(string name)
    {
        SceneManager.LoadScene(name, loadMode);
    }

    public void Load(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex, loadMode);
    }

    public void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
