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
    /// Class that represents a powerup
    /// Has JSON tags in order to serialize as a JSON object
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Powerup
    {
        /// <summary>
        /// an int representing the powerup's unique ID.
        /// </summary>
        [JsonProperty(PropertyName = "power")]
        private int ID;

        /// <summary>
        /// a Vector2D representing the location of the powerup.
        /// </summary>
        [JsonProperty()]
        private Vector2D loc;

        /// <summary>
        /// a bool indicating if the powerup "died" (was collected by a player) on this frame. 
        /// The server will send the dead powerups only once.
        /// </summary>
        [JsonProperty()]
        private bool died;

        /// <summary>
        /// A Integer representing the frame when the Powerup was spawned
        /// </summary>
        private int frameSpawned;

        /// <summary>
        /// Empty Constructor needed for JSON Deserialization.
        /// </summary>
        public Powerup()
        {

        }

        /// <summary>
        /// 3 parameter Constructor to create a Powerup object
        /// </summary>
        /// <param name="powerID">An Integer representing the ID for the Powerup</param>
        /// <param name="location">A Vector2D representing the location of the Powerup</param>
        /// <param name="currFrame">An Integer representing the frame for when the Powerup was created</param>
        public Powerup(int powerID, Vector2D location, int currFrame)
        {
            ID = powerID;
            loc = location;
            frameSpawned = currFrame;
        }

        /// <summary>
        /// Gets the Powerup's ID
        /// </summary>
        /// <returns>
        /// An Integer representing the Powerup's ID
        /// </returns>
        public int GetID()
        {
            return ID;
        }

        /// <summary>
        /// Gets the Powerup's Location
        /// </summary>
        /// <returns>
        /// A Vector2D representing the Powerup's Location
        /// </returns>
        public Vector2D GetLocation()
        {
            return loc;
        }

        /// <summary>
        /// Gets whether or not the Powerup is active
        /// </summary>
        /// <returns>
        /// A Boolean representing whether or not the Powerup is active
        /// </returns>
        public bool GetActive()
        {
            return !died;
        }

        /// <summary>
        /// Gets the frame on which the Powerup was spawned
        /// </summary>
        /// <returns>
        /// An Integer representing the frame the Powerup was created
        /// </returns>
        public int GetFrameSpawned()
        {
            return frameSpawned;
        }

        /// <summary>
        /// Sets the alive state of the Powerup
        /// </summary>
        /// <param name="state">Boolean representing the state</param>
        public void SetActive(bool state)
        {
            died = !state;
        }
    }
}
