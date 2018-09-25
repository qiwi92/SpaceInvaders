using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameLogic
{
    public class EndScreenManager : MonoBehaviour
    {
        private void Update()
        {
            if (Input.anyKey)
            {
                SceneManager.LoadScene(1);
            }
        }
    }
}