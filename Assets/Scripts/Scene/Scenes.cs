using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes : MonoBehaviour
{
    public void Transition()
    {
        SceneManager.LoadScene(1);
    }
}
