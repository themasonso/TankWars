using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using TankWars;

// Author: Mason Seppi and William Nguyen
// University of Utah

namespace TankWars
{
    /// <summary>
    /// Class that represents a projectile
    /// Has JSON tags in order to serialize as a JSON object
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Projectile
    {
        /// <summary>
        /// An integer representing the projectile's unique ID.
        /// </summary>
        [JsonProperty(PropertyName = "proj")]
        private int ID;

        /// <summary>
        /// A Vector2D representing the projectile's location.
        /// </summary>
        [JsonProperty()]
        private Vector2D loc;

        /// <summary>
        /// A Vector2D representing the projectile's orientation.
        /// </summary>
        [JsonProperty()]
        private Vector2D dir;

        /// <summary>
        /// A bool representing if the projectile died on this frame 
        /// (hit something or left the bounds of the world). 
        /// The server will send the dead projectiles only once.
        /// </summary>
        [JsonProperty()]
        private bool died;

        /// <summary>
        /// An int representing the ID of the tank that created the projectile.
        /// You can use this to draw the projectiles with a different color or image for each player.
        /// </summary>
        [JsonProperty()]
        private int owner;

        /// <summary>
        /// An Integer representing the frame that the Projectile was fired on
        /// </summary>
        private int frameFired;

        /// <summary>
        /// An Integer representing the number of times the Projectile has bounced
        /// </summary>
        private int numBounced;

        /// <summary>
        /// An Boolean representing whether or not the projectile has warped
        /// </summary>
        private bool warped;

        /// <summary>
        /// Empty Constructor needed for JSON Deserialization.
        /// </summary>
        public Projectile()
        {

        }

        /// <summary>
        /// 5 parameter Constructor to create a Projectile object
        /// </summary>
        /// <param name="projID">An Integer representing the ID for the Projectile</param>
        /// <param name="ownerID">An Integer representing the ID for the Projectile's owner</param>
        /// <param name="location">A Vector2D representing the location of the Projectile</param>
        /// <param name="direction">A Vector2D reprsenting the direction of the Projectile</param>
        /// <param name="currFrame">An Integer represneting the frame for when the Projectile was created</param>
        public Projectile(int projID, int ownerID, Vector2D location, Vector2D direction, int currFrame)
        {
            ID = projID;
            loc = location;
            dir = direction;
            owner = ownerID;
            frameFired = currFrame;
            died = false;
            warped = false;
        }

        /// <summary>
        /// Gets the Projectile's ID
        /// </summary>
        /// <returns>
        /// An Integer representing the Projectile's ID
        /// </returns>
        public int GetID()
        {
            return ID;
        }

        /// <summary>
        /// Gets the frame on which the Projectile was fired
        /// </summary>
        /// <returns>
        /// An Integer representing the frame the Projectile was created
        /// </returns>
        public int GetFrameFired()
        {
            return frameFired;
        }

        /// <summary>
        /// Gets the owner of the Projectile
        /// </summary>
        /// <returns>
        /// An Integer representing the ID of the Projectile's owner
        /// </returns>
        public int GetOwner()
        {
            return owner;
        }

        /// <summary>
        /// Gets the Projectile's location
        /// </summary>
        /// <returns>
        /// A Vector2D representing the location of the Projectile
        /// </returns>
        public Vector2D GetLocation()
        {
            return loc;
        }

        /// <summary>
        /// Gets the Projectile's direction
        /// </summary>
        /// <returns>
        /// A Vector2D representing the direction of the Projectile
        /// </returns>
        public Vector2D GetDirection()
        {
            return dir;
        }

        /// <summary>
        /// Gets whether or not the Projectile is active
        /// </summary>
        /// <returns>
        /// A Boolean representing the alive state of the Projectile
        /// </returns>
        public bool GetActive()
        {
            return !died;
        }

        /// <summary>
        /// Gets the number of times the bullet has bounced
        /// </summary>
        /// <returns>
        /// A Integer representing the number of bounces for the Projectile
        /// </returns>
        public int GetNumBounces()
        {
            return numBounced;
        }

        /// <summary>
        /// Gets the whether or not the Projectile has warped
        /// </summary>
        /// <returns>
        /// Boolean representing the warped state
        /// </returns>
        public bool GetWarped()
        {
            return warped;
        }

        /// <summary>
        /// Sets the alive state of the Projectile
        /// </summary>
        /// <param name="state">Boolean representing the state</param>
        public void SetActive(bool state)
        {
            died = !state;
        }

        /// <summary>
        /// Sets the location of the Projectile
        /// </summary>
        /// <param name="newLocation">Vector2D representing the location</param>
        public void SetLocation(Vector2D newLocation)
        {
            loc = newLocation;
        }

        /// <summary>
        /// Sets the direction of the Projectile
        /// </summary>
        /// <param name="newDirection">Vector2D representing the direction</param>
        public void SetDirection(Vector2D newDirection)
        {
            dir = newDirection;
        }

        /// <summary>
        /// Ticks up the number of bounces
        /// </summary>
        public void TickBounces()
        {
            numBounced++;
        }

        /// <summary>
        /// Sets the warped state
        /// </summary>
        /// <param name="state">Boolean representing the state</param>
        public void SetWarped(bool state)
        {
            warped = state;
        }
    }
}
