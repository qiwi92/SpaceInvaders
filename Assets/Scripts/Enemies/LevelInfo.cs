using UnityEngine;

namespace Enemies
{
    [CreateAssetMenu(fileName = "Level", menuName = "LevelInfo", order = 1)]
    public class LevelInfo : ScriptableObject
    {
        public float Speed;
        public float BulletSpeed;
        public float AttackSpeed;

        public int MoneyReward;

        public int BaseHp = 0;
        public int BaseHpMultiplier = 1;

        public LevelInfoRow[] LevelInfoRows;
    }
}