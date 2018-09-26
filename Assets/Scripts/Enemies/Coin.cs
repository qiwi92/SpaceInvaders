using UnityEngine;

namespace Enemies
{
    public class Coin : MonoBehaviour
    {
        public int Value;
        public bool IsDead;

        [SerializeField] private float _movementSpeed;

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
                Destroy(gameObject);
            }
        }
    }
}