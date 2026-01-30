using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void Load(string name)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(name);
    }

}