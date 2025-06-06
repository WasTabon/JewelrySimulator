using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingController : MonoBehaviour
{
    private void Start()
    {
        Invoke("HandleLoad", 1f);
    }

    private void HandleLoad()
    {
        SceneManager.LoadScene("Main");
    }
}
