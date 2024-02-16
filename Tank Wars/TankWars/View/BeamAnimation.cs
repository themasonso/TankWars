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
    /// Class that represents a Beam animation
    /// </summary>
    public class BeamAnimation
    {
        // Vector2D that represents the origin of the beam
        private Vector2D origin;
        // Vector2D that represents the orientation of the beam
        private Vector2D orientation;
        // Int that represents the current animation's frame
        private int numFrames;
        // Constant int that is the total of frames the animation takes
        private const int animationFrames = 60;
        // Constant int that is the speed at which the animation runs
        private const int animationSpeed = 3;
        // Random object used to generate random integers
        private Random random;
        private int randX;
        private int randY;

        /// <summary>
        /// Constructor that creates a Beam animation from a position and direction Vector2D
        /// </summary>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        public BeamAnimation(Vector2D position, Vector2D direction)
        {
            origin = new Vector2D(position);
            orientation = new Vector2D(direction);
            random = new Random();
        }

        /// <summary>
        /// Public getter for the origin of the beam
        /// </summary>
        /// <returns></returns>
        public Vector2D GetOrigin()
        {
            return origin;
        }

        /// <summary>
        /// Public getter for the direction of the beam
        /// </summary>
        /// <returns></returns>
        public Vector2D GetDirection()
        {
            return orientation;
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
        /// Draws a large white line that shrinks with circles that fly out from the center of the beam
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        public void BeamDrawer(object o, PaintEventArgs e)
        {
            Beam b = o as Beam;
            int beamLength = 2000;
            int width = 10;
            int height = 10;
            using (Pen pen = new Pen(Color.White, animationFrames - numFrames))
            using (System.Drawing.SolidBrush lightBlueBrush = new System.Drawing.SolidBrush(System.Drawing.Color.LightBlue))
            {
                e.Graphics.DrawLine(pen, new Point(0, 0), new Point(0, -beamLength));
                randX = random.Next(0, 5);
                randY = random.Next(0, 15);
                for (int i = 1; i < beamLength; i *= 3)
                {
                    e.Graphics.FillEllipse(lightBlueBrush, new Rectangle((-(width / 2) + randX - numFrames), (-30 - i - randY), width, height));
                    e.Graphics.FillEllipse(lightBlueBrush, new Rectangle((-(width / 2) - randX + numFrames), (-30 - i - randY), width, height));
                }
            }
            numFrames += animationSpeed;
        }
    }
}
