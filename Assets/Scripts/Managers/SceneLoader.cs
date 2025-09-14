using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] string sceneName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void GoScene(string name)
    {
        SceneManager.LoadSceneAsync(name);
    }
    public void OnClick()
    {
        GoScene(sceneName);
    }

}
