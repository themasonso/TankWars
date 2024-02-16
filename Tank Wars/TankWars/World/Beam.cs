using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TankWars;

// Author: Mason Seppi and William Nguyen
// University of Utah

namespace TankWars
{
    /// <summary>
    /// Class that represents a Beam
    /// Has JSON tags in order to serialize as a JSON object
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Beam
    {
        /// <summary>
        /// an int representing the beam's unique ID.
        /// </summary>
        [JsonProperty(PropertyName ="beam")]
        int ID;

        /// <summary>
        /// a Vector2D representing the origin of the beam.
        /// </summary>
        [JsonProperty()]
        Vector2D org;

        /// <summary>
        /// a Vector2D representing the direction of the beam.
        /// </summary>
        [JsonProperty()]
        Vector2D dir;

        /// <summary>
        /// an int representing the ID of the tank that fired the beam. You can use this to draw the beams with a different color or image for each player.
        /// </summary>
        [JsonProperty()]
        int owner;

        /// <summary>
        /// Empty Constructor needed for JSON Deserialization.
        /// </summary>
        public Beam()
        {

        }

        /// <summary>
        /// 4 parameter Constructor to create a Beam object that takes in
        /// </summary>
        /// <param name="beamID">An Integer representing the ID for the Beam</param>
        /// <param name="beamOwner">An Integer representing the ID for the Beam's owner</param>
        /// <param name="origin">A Vector2D representing the origin of the Beam</param>
        /// <param name="direction">A Vector2D reprsenting the direction of the Beam</param>
        public Beam(int beamID, int beamOwner, Vector2D origin, Vector2D direction)
        {
            ID = beamID;
            owner = beamOwner;
            org = origin;
            dir = direction;
        }

        /// <summary>
        /// Gets the Beam's ID
        /// </summary>
        /// <returns>
        /// An Integer representing the Beam's ID
        /// </returns>
        public int GetID()
        {
            return ID;
        }

        /// <summary>
        /// Gets the Beam's origin
        /// </summary>
        /// <returns>
        /// A Vector2D representing the Beam's origin
        /// </returns>
        public Vector2D GetOrigin()
        {
            return org;
        }

        /// <summary>
        /// Gets the Beam's direction
        /// </summary>
        /// <returns>
        /// A Vector2D representing the Beam's direction
        /// </returns>
        public Vector2D GetDirection()
        {
            return dir;
        }

        /// <summary>
        /// Get the Beam's owner
        /// </summary>
        /// <returns>
        /// An Integer representing the ID of the Beam's owner
        /// </returns>
        public int GetOwner()
        {
            return owner;
        }
    }
}
