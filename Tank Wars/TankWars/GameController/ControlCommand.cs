using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TankWars;

// Author: Mason Seppi and William Nguyen
// University of Utah

namespace Controller
{
    /// <summary>
    /// Class that represents a Control Command.
    /// Has JSON tags in order to serialize as a JSON object
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ControlCommand
    {
        /// <summary>
        /// a string representing whether the player wants to move or not, and the desired direction. Possible values are: "none", "up", "left", "down", "right".
        /// </summary>
        [JsonProperty()]
        string moving;
        /// <summary>
        /// a string representing whether the player wants to fire or not, and the desired type. 
        /// Possible values are: "none", "main", (for a normal projectile) and "alt" (for a beam attack).
        /// </summary>
        [JsonProperty()]
        string fire;
        /// <summary>
        /// a Vector2D representing where the player wants to aim their turret. This vector must be normalized.
        /// </summary>
        [JsonProperty()]
        Vector2D tdir;

        /// <summary>
        /// Constructor which assigns whether the player wants to move, whether they want to fire, and what direction their turret is pointing.
        /// </summary>
        public ControlCommand(string direction, string projectileType, Vector2D turretDirection)
        {
            moving = direction;
            fire = projectileType;
            tdir = turretDirection;
        }

        public string GetMovementDirection()
        {
            return moving;
        }

        public string GetFireType()
        {
            return fire;
        }

        public Vector2D GetTurretDirection()
        {
            return tdir;
        }
    }
}
