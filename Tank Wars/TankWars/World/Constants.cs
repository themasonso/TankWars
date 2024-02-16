using System;
using System.Collections.Generic;
using System.Text;

// Author: Mason Seppi and William Nguyen
// University of Utah

namespace TankWars
{
    public class Constants
    {
        // Maximum Health Points of tank
        public const int MaxHP = 3;

        // Projectile Speed 25 units per frame
        public const int ProjectileSpeed = 25;

        // Tank Speed 3.0 units per frame
        public const double TankSpeed = 3.0;

        // Tank Size 60 x 60
        public const int TankSize = 60;

        // Wall Size 50 x 50
        public const int WallSize = 50;

        // Number of Powerups
        public const int MaxPowerups = 2;

        // Frames
        public const int PowerupDelay = 1650;

        // Amount of damage a projectile does
        public const int ProjectileDamagePoints = 1;

        // Amount of damage a beam does
        public const int BeamDamagePoints = 3;

        // Points gained for kiiling with projectile
        public const int ProjectileKillPoints = 1;

        // Points gained for killing with beam
        public const int BeamKillPoints = 2;

        // Number of allowed bounces in Bullet Hell mode
        public const int numOfAllowedBounces = 3;
    }
}
