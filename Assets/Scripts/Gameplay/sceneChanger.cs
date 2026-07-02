using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneChanger : MonoBehaviour
{
    [SerializeField] string scenename;
    private void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene(scenename);
    }
}
