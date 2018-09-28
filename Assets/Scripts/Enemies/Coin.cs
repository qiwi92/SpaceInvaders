using UniRx;
using UnityEngine;

namespace Enemies
{
    public class Coin : MonoBehaviour
    {
        public int Value;
        public bool IsDead;

        [SerializeField] private float _movementSpeed;
        public ReactiveProperty<int> Amount { get; set; }

        private void Update()
        {
            if (transform.position.y > -10)
            {
                transform.position += _movementSpeed * Time.smoothDeltaTime * Vector3.down;
            }
            else
            {
                IsDead = true;
            }

            if (IsDead)
            {
                Amount.Value -= 1;
                Destroy(gameObject);
            }
        }
    }
}