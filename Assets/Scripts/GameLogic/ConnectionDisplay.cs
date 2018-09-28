using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace GameLogic
{
    public class ConnectionDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject _connecting;
        [SerializeField] private GameObject _connectionFailed;
        [SerializeField] private GameObject _loadingAnimation;

        private void Start()
        {
            _loadingAnimation.SetActive(true);
            _connecting.SetActive(false);
            _connectionFailed.SetActive(false);
        }

        private void OnEnable()
        {
            StopCoroutine(Blyat());
            _loadingAnimation.SetActive(true);
            _connecting.SetActive(false);
            _connectionFailed.SetActive(false);

            StartCoroutine(Blyat());
            
        }

        private IEnumerator Blyat()
        {
            yield return new WaitForSeconds(0.5f);
            _connecting.SetActive(true);

            yield return new WaitForSeconds(4f);
            _connectionFailed.SetActive(true);
            _loadingAnimation.SetActive(false);

        }
    }
}