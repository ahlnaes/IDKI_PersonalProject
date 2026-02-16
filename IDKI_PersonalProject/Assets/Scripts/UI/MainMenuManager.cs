using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private float uiSpawnDistance = 2f;
        public GameObject mainMenu;
        public GameObject tutorial;

        private Camera vrCamera;

        private void Start()
        {
            vrCamera = Camera.main;
            PlaceInFrontOfPlayer(mainMenu);
            mainMenu.SetActive(true);
            tutorial.SetActive(false);
        }

        private void PlaceInFrontOfPlayer(GameObject panel)
        {
            if (vrCamera == null || panel == null) return;
            var camTransform = vrCamera.transform;
            var forward = camTransform.forward;
            forward.y = 0f;
            forward.Normalize();
            panel.transform.position = camTransform.position + forward * uiSpawnDistance;
            panel.transform.rotation = Quaternion.LookRotation(forward);
        }

        public void StartGame()
        {
            SceneManager.LoadScene("GameScene");
        }

        public void Tutorial()
        {
            mainMenu.SetActive(false);
            PlaceInFrontOfPlayer(tutorial);
            tutorial.SetActive(true);
        }

        public void MainMenu()
        {
            mainMenu.SetActive(true);
            tutorial.SetActive(false);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
