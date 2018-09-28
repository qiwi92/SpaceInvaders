using UnityEngine;

namespace Player.Controller
{
    public class ParalaxBackground : MonoBehaviour
    {
        [SerializeField] private float _amount;
        [SerializeField] private Transform _playerTransform;
        private void Update()
        {
            transform.position = new Vector3(_playerTransform.position.x * _amount, transform.position.y);
        }
    }
}