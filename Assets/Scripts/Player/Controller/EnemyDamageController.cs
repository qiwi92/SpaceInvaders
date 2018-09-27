using UnityEngine;

namespace Player.Controller
{
    public class EnemyDamageController : MonoBehaviour
    {
        [SerializeField] private Sprite _littleDamageSprite;
        [SerializeField] private Sprite _mediumDamageSprite;
        [SerializeField] private Sprite _heavyDamageSprite;
        [SerializeField] private SpriteMask _spriteMask;

        [SerializeField] private ParticleSystem[] _particleSystems;
        [SerializeField] private GameObject _explotionParent;

        [SerializeField] private ParticleSystem _explotionParticleSystem;
        private ParticleSystem.EmitParams _emitParams = new ParticleSystem.EmitParams {applyShapeToPosition = true};
        [SerializeField] private Transform[] _anchors;
        private readonly bool[] _complete = new[] {false, false, false};

        public void Start()
        {
            _spriteMask.enabled = false;
            EnableParticles(false);
        }
  
        public void SetDamage(float damagePercentage)
        {
            if (damagePercentage < .75f && damagePercentage > .51f)
            {
                _spriteMask.enabled = true;

                _spriteMask.sprite = _littleDamageSprite;

                if (!_complete[0])
                {
                    _complete[0] = true;
                    _emitParams.position = _anchors[0].position;
                    _explotionParticleSystem.Emit(_emitParams, 10);
                }
                
            }
            if (damagePercentage < .50f && damagePercentage > .26f)
            {
                EnableParticles(true);
                _spriteMask.sprite = _mediumDamageSprite;

                if (!_complete[1])
                {
                    _complete[1] = true;
                    _emitParams.position = _anchors[1].position;
                    _explotionParticleSystem.Emit(_emitParams, 10);
                }
            }
            if (damagePercentage < .25f )
            {
                _spriteMask.sprite = _heavyDamageSprite;

                if (!_complete[2])
                {
                    _complete[2] = true;
                    _emitParams.position = _anchors[2].position;
                    _explotionParticleSystem.Emit(_emitParams, 10);
                }
            }

            
        }

        public void Disable()
        {
            EnableParticles(false);
        }

        private void EnableParticles(bool enable)
        {
            foreach (var particleSys in _particleSystems)
            {
                if (enable)
                {
                    _explotionParent.gameObject.SetActive(true);
                    particleSys.Play();
                }
                else
                {
                    _explotionParent.gameObject.SetActive(false);
                    particleSys.Stop();
                }
                
            }
        }

    }
}