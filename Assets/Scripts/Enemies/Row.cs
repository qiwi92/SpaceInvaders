using System;
using Player.Controller;

namespace Enemies
{
    [Serializable]
    public class LevelInfoRow
    {
        public EnemyInfo[] EnemyType;
    }

    [Serializable]
    public class EnemyInfo
    {
        public EnemyType EnemyType;
        public ColorType ColorType;
    }
}