using System.Linq;
using Player.Controller;
using UnityEngine;

namespace Enemies
{
    [CreateAssetMenu(fileName = "EnemySprites", menuName = "EnemySprites", order = 1)]
    public class EnemySprites : ScriptableObject
    {
        [SerializeField] private EnemySprite[] _enemySprites;

        public Sprite GetSprite(ColorType color, EnemyType enemyType)
        {
            return _enemySprites.FirstOrDefault(sprite => sprite.Color == color && sprite.EnemyType == enemyType)?.Sprite;
        }
    }
}