using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace EkeGame2
{
    public enum SpawnableEffect_Type
    {
        TINY_STARS,
    }
    public enum Animation_State
    {
        idle = 0, running = 1, jumping = 2, falling = 3, landing = 4, hurt = 5, jumpSquat = 6, falldamage = 7, byWall = 8, death = 9, wallcling = 10, throwing = 11, airroll=12
    }
    public enum GameObject_State
    {
        onGround, Flying, Air, Jumping, Death, AirRoll
    }
    public enum EnemyType
    {
        Purple, Red, Green, Blue, Orange
    }
    public enum ProjectileOwner
    {
        PLAYER, ENEMY
    }
    public enum Projectile_State
    {
        CHARGING, IN_MOTION
    }
    public enum Projectile_Trajectory
    {
        HIGH, MEDIUM, LOW
    }
    public enum Hitbox_Sides
    {
        TOP, RIGHT, BOTTOM, LEFT
    }
    public enum Platform_Type
    {
        MOVING_PLATFORM_UPnDOWN,MOVING_PLATFORM_LEFTnRIGHT, MOVING_PLATFORM_CIRCLE
    }
    public enum Platform_State
    {
        MOVING, DECELERATING, ACCELERATING, PAUSE
    }
}
