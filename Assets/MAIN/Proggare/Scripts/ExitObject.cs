using UnityEngine;

public class ExitObject : MonoBehaviour
{
    [SerializeField] SceneLoader sceneLoader;

    void Start()
    {
        if (sceneLoader == null) { sceneLoader = FindFirstObjectByType<SceneLoader>(); }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            sceneLoader.LoadNextScene();
        }
    }
}
