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
    /// Class that represents a wall
    /// Has JSON tags in order to serialize as a JSON object
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Wall
    {
        /// <summary>
        /// An integer representing the wall's unique ID.
        /// </summary>
        [JsonProperty(PropertyName = "wall")]
        private int ID;

        /// <summary>
        /// A Vector2D representing one endpoint of the wall.
        /// </summary>
        [JsonProperty()]
        private Vector2D p1;

        /// <summary>
        /// A Vector2D representing the other endpoint of the wall.
        /// </summary>
        [JsonProperty()]
        private Vector2D p2;

        /// <summary>
        /// Empty Constructor needed for JSON Deserialization.
        /// </summary>
        public Wall()
        {

        }

        /// <summary>
        /// 3 parameter Constructor to create a Wall object
        /// </summary>
        /// <param name="id">An Integer representing the ID of the wall</param>
        /// <param name="point1">A Vector2D representing the first point of the wall</param>
        /// <param name="point2">A Vector2D representing the second point of the wall</param>
        public Wall(int id, Vector2D point1, Vector2D point2)
        {
            ID = id;
            p1 = point1;
            p2 = point2;
        }

        /// <summary>
        /// Gets the Wall's ID
        /// </summary>
        /// <returns>
        /// An Integer representing the Wall's ID
        /// </returns>
        public int GetID()
        {
            return ID;
        }
        /// <summary>
        /// Gets the Wall's first point
        /// </summary>
        /// <returns>
        /// Vector2D representing the Wall's first point
        /// </returns>
        public Vector2D GetP1()
        {
            return p1;
        }

        /// <summary>
        /// Gets the Wall's second point
        /// </summary>
        /// <returns>
        /// Vector2D representing the Wall's second point
        /// </returns>
        public Vector2D GetP2()
        {
            return p2;
        }
    }
}
