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
    /// Class that represents a tank
    /// Has JSON tags in order to serialize as a JSON object
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Tank
    {
        /// <summary>
        /// An integer representing the tank's unique ID.  
        /// </summary>
        [JsonProperty(PropertyName = "tank")]
        private int ID;

        /// <summary>
        /// A string representing the player's name.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        private string name;

        /// <summary>
        /// A Vector2D representing the tank's location.
        /// </summary>
        [JsonProperty(PropertyName = "loc")]
        private Vector2D location;

        /// <summary>
        /// A Vector2D representing the tank's orientation. 
        /// This will always be an axis-aligned vector (purely horizontal or vertical).
        /// </summary>
        [JsonProperty(PropertyName = "bdir")]
        private Vector2D orientation;

        /// <summary>
        /// A Vector2D representing the direction of the tank's turret (where it's aiming). 
        /// </summary>
        [JsonProperty(PropertyName = "tdir")]
        private Vector2D aiming = new Vector2D(0, -1);

        /// <summary>
        /// An integer representing the player's score.
        /// </summary>
        [JsonProperty(PropertyName = "score")]
        private int score = 0;

        /// <summary>
        /// An integer representing the hit points of the tank. 
        /// This value ranges from 0 - 3. 
        /// If it is 0, then this tank is temporarily destroyed, and waiting to respawn.
        /// </summary>
        [JsonProperty(PropertyName = "hp")]
        private int hitPoints;

        /// <summary>
        /// A boolean indicating if the tank died on that frame. 
        /// This will only be true on the exact frame in which the tank died.
        /// </summary>
        [JsonProperty(PropertyName = "died")]
        private bool died = false;

        /// <summary>
        /// A boolean indicating if the player controlling that tank disconnected on that frame. 
        /// The server will send the tank with this flag set to true only once, then it will discontinue sending that tank for the rest of the game. 
        /// </summary>
        [JsonProperty(PropertyName = "dc")]
        private bool disconnected = false;

        /// <summary>
        /// A boolean indicating if the player joined on this frame.
        /// This will only be true for one frame. 
        /// This field may not be needed, but may be useful for certain additional View related features.
        /// </summary>
        [JsonProperty(PropertyName = "join")]
        private bool joined = false;

        /// <summary>
        /// A Boolean representing the state if the Tank has fired a projectile
        /// </summary>
        private bool projectileFired = false;

        /// <summary>
        /// An Integer representing the frame on which the Tank fired a projectile
        /// </summary>
        private int frameFired;

        /// <summary>
        /// An Integer representing the frame on which the Tank died
        /// </summary>
        private int frameDied;

        /// <summary>
        /// A Boolean representing the state if the Tank receieved a ControlCommand requesting to fire an alternate shot
        /// </summary>
        private bool alternateFireCommandReceived = false;

        /// <summary>
        /// An Integer representing the alternate shot ammo counter for the tank
        /// </summary>
        private int altFireAmmo = 0;

        /// <summary>
        /// Empty Constructor needed for JSON Deserialization.
        /// </summary>
        public Tank()
        {

        }

        /// <summary>
        /// 4 parameter Constructor to create a Tank object
        /// </summary>
        /// <param name="id">An Integer representing the ID for the Tank</param>
        /// <param name="playerName">An String representing the Tank's name</param>
        /// <param name="loc">A Vector2D representing the location of the Tank</param>
        /// <param name="hpAmount">An Integer representing the amount of health points the Tank has</param>
        public Tank(int id, string playerName, Vector2D loc, int hpAmount)
        {
            ID = id;
            name = playerName;
            location = loc;
            hitPoints = hpAmount;
            orientation = new Vector2D(0,-1);
        }

        /// <summary>
        /// Gets the Tank's ID
        /// </summary>
        /// <returns>
        /// An Integer representing the Tank's ID
        /// </returns>
        public int GetID()
        {
            return ID;
        }

        /// <summary>
        /// Gets whether or not the Tank has fired a projectile
        /// </summary>
        /// <returns>
        /// A Boolean representing the state if the Tank fired a projectile
        /// </returns>
        public bool GetProjectileFired()
        {
            return projectileFired;
        }

        /// <summary>
        /// Gets the frame on which the Tank fired a projectile
        /// </summary>
        /// <returns>
        /// An Integer representing the frame on which the Tank fired
        /// </returns>
        public int GetFrameFired()
        {
            return frameFired;
        }

        /// <summary>
        /// Gets the frame on which the Tank died
        /// </summary>
        /// <returns>
        /// An Integer representing the frame on which the Tank died
        /// </returns>
        public int GetFrameDied()
        {
            return frameDied;
        }

        /// <summary>
        /// Gets whether or not a ControlCommand requesting an alternate fire was received
        /// </summary>
        /// <returns>
        /// A Boolean representing the state of the Tank's alternate fire was received
        /// </returns>
        public bool GetAlternateFireReceived()
        {
            return alternateFireCommandReceived;
        }

        /// <summary>
        /// Gets the Tank's location
        /// </summary>
        /// <returns>
        /// A Vector2D representing the Tank's location
        /// </returns>
        public Vector2D GetLocation()
        {
            return location;
        }


        /// <summary>
        /// Gets the Tank's Orientation
        /// </summary>
        /// <returns>
        /// A Vector2D representing the Tank's orientation
        /// </returns>
        public Vector2D GetOrientation()
        {
            return orientation;
        }

        /// <summary>
        /// Gets the direction for where the is Tank's aiming
        /// </summary>
        /// <returns>
        /// A Vector2D representing the Tank's turret direction
        /// </returns>
        public Vector2D GetAiming()
        {
            return aiming;
        }

        /// <summary>
        /// Gets whether or not a Tank is active
        /// </summary>
        /// <returns>
        /// A Boolean representing a Tank's connected state to the server
        /// </returns>
        public bool GetActive()
        {
            return !disconnected;
        }

        /// <summary>
        /// Gets whether the tank is currently dead
        /// </summary>
        /// <returns>
        /// A Boolean representing a Tank's living state
        /// </returns>
        public bool IsDead()
        {
            return died;
        }

        /// <summary>
        /// Gets the Tank's name
        /// </summary>
        /// <returns>
        /// A String representing the Tank's name
        /// </returns>
        public string GetName()
        {
            return name;
        }

        /// <summary>
        /// Gets the Tank's current score
        /// </summary>
        /// <returns>
        /// An Integer representing the amount of points the Tank has for destroying other tanks
        /// </returns>
        public int GetScore()
        {
            return score;
        }

        /// <summary>
        /// Gets the Tank's current HP
        /// </summary>
        /// <returns>
        /// An Integer representing the amount of health points a Tank has
        /// </returns>
        public int GetHp()
        {
            return hitPoints;
        }

        /// <summary>
        /// Gets the Tank's current alternate ammo count
        /// </summary>
        /// <returns>
        /// An Integer representing the Tank's alternate ammo
        /// </returns>
        public int GetAltAmmo()
        {
            return altFireAmmo;
        }

        /// <summary>
        /// Sets the location of the Tank
        /// </summary>
        /// <param name="loc">Vector2D representing the location</param>
        public void SetLocation(Vector2D loc)
        {
            location = loc;
        }

        /// <summary>
        /// Sets the Tank's body orientation
        /// </summary>
        /// <param name="tankOrientation">Vector2D representing the body orientation</param>
        public void SetOrientation(Vector2D tankOrientation)
        {
            orientation = tankOrientation;
        }

        /// <summary>
        /// Sets the state for the Tank firing a projectile
        /// </summary>
        /// <param name="firedRecently">Boolean representing the state</param>
        public void SetProjectileFired(bool firedRecently)
        {
            projectileFired = firedRecently;
        }

        /// <summary>
        /// Sets the frame for which the Tank fired on
        /// </summary>
        /// <param name="frame">Integer representing the frame</param>
        public void SetFrameFired(int frame)
        {
            frameFired = frame;
        }

        /// <summary>
        /// Sets the Tank's turret direction
        /// </summary>
        /// <param name="direction">Vector2D representing the Tank's turret direction. Must be Normalized.</param>
        public void SetAiming(Vector2D direction)
        {
            aiming = direction;
        }

        /// <summary>
        /// Sets the Tank's connected state to the server
        /// </summary>
        /// <param name="state">Boolean representing connected state to server</param>
        public void SetActive(bool state)
        {
            if (!state)
            {
                disconnected = true;
                hitPoints = 0;
                died = true;
            }
        }

        /// <summary>
        /// Sets the Tank's amount of health points
        /// </summary>
        /// <param name="amount">Integer representing Tank's health points</param>
        public void SetHp(int amount)
        {
            hitPoints = amount;
        }

        /// <summary>
        /// Sets the Tank's game score
        /// </summary>
        /// <param name="amount">Integer representing Tank's game score</param>
        public void SetScore(int amount)
        {
            score = amount;
        }

        /// <summary>
        /// Sets the Tank's living state
        /// </summary>
        /// <param name="state">Boolean representing the Tank's living state</param>
        public void SetIsDead(bool state)
        {
            died = state;
        }

        /// <summary>
        /// Sets the frame on which the Tank died
        /// </summary>
        /// <param name="frame">Integer representing the frame for which the Tank died on</param>
        public void SetFrameDied(int frame)
        {
            frameDied = frame;
        }

        /// <summary>
        /// Sets the Tank's alternate ammo count
        /// </summary>
        /// <param name="amount">Integer representing the alternate ammo count for the Tank</param>
        public void SetAltAmmo(int amount)
        {
            altFireAmmo = amount;
        }

        /// <summary>
        /// Sets the Tank's state for when an alternate fire command was received
        /// </summary>
        /// <param name="firedState">Boolean representing if the an alternate fire command was received</param>
        public void SetAlternateFireCommandReceived(bool firedState)
        {
            alternateFireCommandReceived = firedState;
        }
    }
}
