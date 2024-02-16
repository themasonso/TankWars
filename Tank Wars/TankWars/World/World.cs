using System;
using System.Collections.Generic;

// Author: Mason Seppi and William Nguyen
// University of Utah

namespace TankWars
{
    public class World
    {
        // Dictionaries containing objects that represent the Game World
        public Dictionary<int, Tank> Tanks
        { get; private set; }
        public Dictionary<int, Projectile> Projectiles
        { get; private set; }
        public Dictionary<int, Wall> Walls
        { get; private set; }
        public Dictionary<int, Beam> Beams
        { get; private set; }
        public Dictionary<int, Powerup> Powerups
        { get; private set; }

        // The game world's size
        public int size
        { get; private set; }

        // Dictionaries containing dead objects that represent the Game Worlld
        public Dictionary<int, Tank> DeadTanks
        { get; private set; }
        public Dictionary<int, Projectile> DeadProjectiles
        { get; private set; }
        public Dictionary<int, Beam> DeadBeams
        { get; private set; }
        public Dictionary<int, Powerup> DeadPowerups
        { get; private set; }


        /// <summary>
        /// 1 paramerter Constructor that initializes Game Server's member variables.
        /// </summary>
        /// <param name="_size"></param>
        public World(int _size)
        {
            // Alive objects
            Tanks = new Dictionary<int, Tank>();
            Projectiles = new Dictionary<int, Projectile>();
            Walls = new Dictionary<int, Wall>();
            Beams = new Dictionary<int, Beam>();
            Powerups = new Dictionary<int, Powerup>();

            // World size
            size = _size;

            // Dead objects
            DeadTanks = new Dictionary<int, Tank>();
            DeadProjectiles = new Dictionary<int, Projectile>();
            DeadBeams = new Dictionary<int, Beam>();
            DeadPowerups = new Dictionary<int, Powerup>();
        }

    }
}
