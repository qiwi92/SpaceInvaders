using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameLogic
{
    public class RestartAtWaypoint : MonoBehaviour
    {
        [SerializeField] private Button _restartButton;
        [SerializeField] private Text _lastWaypointLevelText;

        private void Start()
        {
            _lastWaypointLevelText.text = "level " + (GameState.GetLastWaypoint() == 1 ? 1 : GameState.GetLastWaypoint() + 1).ToString("0");

            _restartButton.onClick.AddListener(() =>
            {
                GameState.ResetToLastWaypoint();
                SceneManager.LoadScene(1);
            });
        }
    }
}