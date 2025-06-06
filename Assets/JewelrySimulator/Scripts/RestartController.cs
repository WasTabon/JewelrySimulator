using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartController : MonoBehaviour
{
    public void HandleLoad()
    {
        SceneManager.LoadScene("Loading");
    }
}
