using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameLogic
{
    public class VictoryScreenManager : MonoBehaviour
    {
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private Button _shopButton;

        private void Start()
        {
            _nextLevelButton.onClick.AddListener(() => { SceneManager.LoadScene(1); });
        }
    }
}