using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkeGame2
{
    public enum Animation_State
    {
        idle=0, running=1, jumping=2, falling=3, landing=4, hurt=5, jumpSquat=6, falldamage=7, byWall=8, death=9, wallcling=10, throwing=11
    }
    public enum GameObject_State
    {
        onGround, Flying, Air, Jumping, Death
    }
    public enum EnemyType
    {
        Purple,Red,Green,Blue,Orange
    }
    public enum ProjectileOwner
    {
        PLAYER, ENEMY
    }
}
