using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TankWars;


// Author: Mason Seppi and William Nguyen
// University of Utah
namespace View
{
    /// <summary>
    /// Class that represents a Tank Death's Animation
    /// </summary>
    public class TankDeathAnimation
    {
        // Vector2D that represents the tank's location
        private Vector2D location;
        // Int that represents the tank's id
        private int id;
        // Int that represents the current animation's frame;
        private int numFrames;
        // Constant int that is the total of frames the animation takes
        private const int animationFrames = 60;
        // Constant int that is the speed at which the animation runs
        private const int animationSpeed = 3;

        /// <summary>
        /// Constructor that creates a TankDeathAnimation from a tank
        /// </summary>
        /// <param name="t"></param>
        public TankDeathAnimation(Tank t)
        {
            location = t.GetLocation();
            id = t.GetID();
        }

        /// <summary>
        /// Public getter for the location of the animation
        /// </summary>
        /// <returns></returns>
        public Vector2D GetLocation()
        {
            return location;
        }

        /// <summary>
        /// Public getter for ID of the animation
        /// </summary>
        /// <returns></returns>
        public int GetID()
        {
            return id;
        }

        /// <summary>
        /// Public getter for the current animation frame
        /// </summary>
        /// <returns></returns>
        public int GetNumFrames()
        {
            return numFrames;
        }

        /// <summary>
        /// Public getter for the total number of animation frames
        /// </summary>
        /// <returns></returns>
        public int GetAnimationFrames()
        {
            return animationFrames;
        }

        /// <summary>
        /// Public method that is the drawer for the Tank Deaths Animation
        /// Draws circles moving from the center of the tank to the edge of the tank
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        public void TankDeathAnimationDrawer(object o, PaintEventArgs e)
        {
            int width = 10;
            int height = 10;

            using (System.Drawing.SolidBrush greenBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Green))
            {
                Rectangle r1 = new Rectangle(-(width / 2) + numFrames, -(height / 2) + numFrames, width, height);
                e.Graphics.FillEllipse(greenBrush, r1);
                Rectangle r2 = new Rectangle(-(width / 2) + numFrames, -(height / 2) - numFrames, width, height);
                e.Graphics.FillEllipse(greenBrush, r2);
                Rectangle r3 = new Rectangle(-(width / 2) - numFrames, -(height / 2) + numFrames, width, height);
                e.Graphics.FillEllipse(greenBrush, r3);
                Rectangle r4 = new Rectangle(-(width / 2) - numFrames, -(height / 2) - numFrames, width, height);
                e.Graphics.FillEllipse(greenBrush, r4);
            }
            numFrames += animationSpeed;
        }
    }
}
