using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MainMenuManager : MonoBehaviour
    {
        public GameObject mainMenu;
        public GameObject tutorial;
        public void StartGame()
        {
            SceneManager.LoadScene("GameScene");
        }

        public void Tutorial()
        {
            mainMenu.SetActive(false);
            tutorial.SetActive(true);
        }

        public void MainMenu()
        {
            mainMenu.SetActive(true);
            tutorial.SetActive(false);
        }
    }
}
