using UnityEngine;
using UnityEngine.SceneManagement;

public class ConstructionLevelsManager : MonoBehaviour
{
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}