using System;
using UnityEngine;

namespace Enemies
{
    public static class DirectionExtensions
    {
        public static Vector2 ToVector2(this EnemyBlockDirection direction)
        {
            switch (direction)
            {
                case EnemyBlockDirection.Down:
                    return Vector2.down * 0.5f;
                case EnemyBlockDirection.Left:
                    return Vector2.left * 0.8f;
                case EnemyBlockDirection.Right:
                    return Vector2.right * 0.8f;
                default:
                    throw new ArgumentException();
            }
        }
    }
}