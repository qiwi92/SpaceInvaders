using System;
using Player.Controller;
using UnityEngine;

namespace Enemies
{
    [Serializable]
    public class EnemySprite
    {
        public ColorType Color;
        public EnemyType EnemyType;
        public Sprite Sprite;
    }
}