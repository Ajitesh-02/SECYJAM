using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameReset : MonoBehaviour
{
    public static GameReset Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void ResetGameAfter(float delay)
    {
        StartCoroutine(ResetCoroutine(delay));
    }

    private IEnumerator ResetCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}