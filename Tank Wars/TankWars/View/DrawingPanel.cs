using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TankWars;
using System.Diagnostics;

// Author: Mason Seppi and William Nguyen
// University of Utah
namespace View
{
    public class DrawingPanel : Panel
    {
        // Reference to the game controller's world.
        private World theWorld;
        // Game Images
        private Dictionary<int, Image> tankImages;
        private Dictionary<int, Image> turretImages;
        private Dictionary<int, Image> projectileImages;
        private Image backgroundImage;
        private Image wallImage;
        // Collection of animations
        private HashSet<BeamAnimation> beamAnimations;
        private HashSet<BeamAnimation> oldBeamAnimations;
        private Dictionary<int, TankDeathAnimation> tankDeathAnimations;
        private Dictionary<int, TankDeathAnimation> oldTankDeathAnimations;
        // Id of the player
        private int id;
        // Fps counter related
        private int fpsCounter;
        private int currentFPS;
        private int currentTime = 0;
        private Stopwatch stopwatch;
        private bool fpsDisplayed = false;
        /// <summary>
        /// Sets the game's world
        /// </summary>
        /// <param name="world"></param>
        public void SetWorld(World world)
        {
            theWorld = world;
        }
        /// <summary>
        /// Sets the player's id.
        /// </summary>
        /// <param name="_id"></param>
        public void SetID(int _id)
        {
            id = _id;
        }
        /// <summary>
        /// Constructor that initializes the member variables and adds the necessary game images.
        /// </summary>
        /// <param name="world"></param>
        public DrawingPanel(World world)
        {
            DoubleBuffered = true;
            theWorld = world;
            tankImages = new Dictionary<int, Image>();
            turretImages = new Dictionary<int, Image>();
            projectileImages = new Dictionary<int, Image>();
            beamAnimations = new HashSet<BeamAnimation>();
            oldBeamAnimations = new HashSet<BeamAnimation>();
            tankDeathAnimations = new Dictionary<int, TankDeathAnimation>();
            oldTankDeathAnimations = new Dictionary<int, TankDeathAnimation>();
            stopwatch = new Stopwatch();
            stopwatch.Start();

            backgroundImage = Image.FromFile("..\\..\\..\\Resources\\Images\\Background.png");
            wallImage = Image.FromFile("..\\..\\..\\Resources\\Images\\WallSprite.png");
            Image tank1 = Image.FromFile("..\\..\\..\\Resources\\Images\\BlueTank.png");
            tankImages.Add(0, tank1);
            Image tank2 = Image.FromFile("..\\..\\..\\Resources\\Images\\DarkTank.png");
            tankImages.Add(1, tank2);
            Image tank3 = Image.FromFile("..\\..\\..\\Resources\\Images\\LightGreenTank.png");
            tankImages.Add(2, tank3);
            Image tank4 = Image.FromFile("..\\..\\..\\Resources\\Images\\OrangeTank.png");
            tankImages.Add(3, tank4);
            Image tank5 = Image.FromFile("..\\..\\..\\Resources\\Images\\PurpleTank.png");
            tankImages.Add(4, tank5);
            Image tank6 = Image.FromFile("..\\..\\..\\Resources\\Images\\RedTank.png");
            tankImages.Add(5, tank6);
            Image tank7 = Image.FromFile("..\\..\\..\\Resources\\Images\\YellowTank.png");
            tankImages.Add(6, tank7);
            Image tank8 = Image.FromFile("..\\..\\..\\Resources\\Images\\GreenTank.png");
            tankImages.Add(7, tank8);

            Image turret1 = Image.FromFile("..\\..\\..\\Resources\\Images\\BlueTurret.png");
            turretImages.Add(0, turret1);
            Image turret2 = Image.FromFile("..\\..\\..\\Resources\\Images\\DarkTurret.png");
            turretImages.Add(1, turret2);
            Image turret3 = Image.FromFile("..\\..\\..\\Resources\\Images\\LightGreenTurret.png");
            turretImages.Add(2, turret3);
            Image turret4 = Image.FromFile("..\\..\\..\\Resources\\Images\\OrangeTurret.png");
            turretImages.Add(3, turret4);
            Image turret5 = Image.FromFile("..\\..\\..\\Resources\\Images\\PurpleTurret.png");
            turretImages.Add(4, turret5);
            Image turret6 = Image.FromFile("..\\..\\..\\Resources\\Images\\RedTurret.png");
            turretImages.Add(5, turret6);
            Image turret7 = Image.FromFile("..\\..\\..\\Resources\\Images\\YellowTurret.png");
            turretImages.Add(6, turret7);
            Image turret8 = Image.FromFile("..\\..\\..\\Resources\\Images\\GreenTurret.png");
            turretImages.Add(7, turret8);

            Image projectile1 = Image.FromFile("..\\..\\..\\Resources\\Images\\shot-blue.png");
            projectileImages.Add(0, projectile1);
            Image projectile2 = Image.FromFile("..\\..\\..\\Resources\\Images\\shot-grey.png");
            projectileImages.Add(1, projectile2);
            Image projectile3 = Image.FromFile("..\\..\\..\\Resources\\Images\\shot-white.png");
            projectileImages.Add(2, projectile3);
            Image projectile4 = Image.FromFile("..\\..\\..\\Resources\\Images\\shot-brown.png");
            projectileImages.Add(3, projectile4);
            Image projectile5 = Image.FromFile("..\\..\\..\\Resources\\Images\\shot-violet.png");
            projectileImages.Add(4, projectile5);
            Image projectile6 = Image.FromFile("..\\..\\..\\Resources\\Images\\shot-red.png");
            projectileImages.Add(5, projectile6);
            Image projectile7 = Image.FromFile("..\\..\\..\\Resources\\Images\\shot-yellow.png");
            projectileImages.Add(6, projectile7);
            Image projectile8 = Image.FromFile("..\\..\\..\\Resources\\Images\\shot-green.png");
            projectileImages.Add(7, projectile8);
        }

        /// <summary>
        /// Allows the view to add a beam animation to be drawn.
        /// </summary>
        /// <param name="b"></param>
        public void AddBeamAnimation(Beam b)
        {
            BeamAnimation ba = new BeamAnimation(b.GetOrigin(), b.GetDirection());
            this.Invoke(new MethodInvoker(() => beamAnimations.Add(ba)));
        }
        /// <summary>
        /// Allows the view to add a tank death animation to be drawn
        /// </summary>
        /// <param name="t"></param>
        public void AddTankDeathAnimation(Tank t)
        {
            TankDeathAnimation tda = new TankDeathAnimation(t);
            this.Invoke(new MethodInvoker(() => tankDeathAnimations[t.GetID()] = tda));
        }
        /// <summary>
        /// Allows the view to toggle drawing the FPS counter.
        /// </summary>
        /// <param name="state"></param>
        public void DisplayFPSCounter(bool state)
        {
            fpsDisplayed = state;
        }

        // A delegate for DrawObjectWithTransform
        // Methods matching this delegate can draw whatever they want using e  
        public delegate void ObjectDrawer(object o, PaintEventArgs e);


        /// <summary>
        /// This method performs a translation and rotation to drawn an object in the world.
        /// </summary>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        /// <param name="o">The object to draw</param>
        /// <param name="worldX">The X coordinate of the object in world space</param>
        /// <param name="worldY">The Y coordinate of the object in world space</param>
        /// <param name="angle">The orientation of the objec, measured in degrees clockwise from "up"</param>
        /// <param name="drawer">The drawer delegate. After the transformation is applied, the delegate is invoked to draw whatever it wants</param>
        private void DrawObjectWithTransform(PaintEventArgs e, object o, double worldX, double worldY, double angle, ObjectDrawer drawer)
        {
            // "push" the current transform
            System.Drawing.Drawing2D.Matrix oldMatrix = e.Graphics.Transform.Clone();

            e.Graphics.TranslateTransform((int)worldX, (int)worldY);
            e.Graphics.RotateTransform((float)angle);
            drawer(o, e);

            // "pop" the transform
            e.Graphics.Transform = oldMatrix;
        }

        /// <summary>
        /// Draws the tank's body
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void TankDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;

            int width = 60;
            int height = 60;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            // Rectangles are drawn starting from the top-left corner.
            // So if we want the rectangle centered on the player's location, we have to offset it
            // by half its size to the left (-width/2) and up (-height/2)
            Rectangle r = new Rectangle(-(width / 2), -(height / 2), width, height);

            if (t.GetHp() > 0)
                e.Graphics.DrawImage(tankImages[t.GetID() % 8], r);
        }
        /// <summary>
        /// Draws the details about the player.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void PlayerDetailDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;
            int width = 60;
            int height = 60;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Rectangle r = new Rectangle(-(width / 2), -(height / 2), width, height);
            Font font = new Font("Arial", 12);
            if (t.GetHp() > 0)
            {
                using (System.Drawing.SolidBrush greenBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Green))
                using (System.Drawing.SolidBrush yellowBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Yellow))
                using (System.Drawing.SolidBrush redBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red))
                using (System.Drawing.SolidBrush blackBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black))
                {
                    e.Graphics.DrawString(t.GetName() + ":" + t.GetScore(), font, blackBrush, (float)-width / 2, (float)height / 2);
                    if (t.GetHp() == 3)
                    {
                        Rectangle HpBar = new Rectangle(-(width / 2), -height + 20, width, height / 8);
                        e.Graphics.FillRectangle(greenBrush, HpBar);
                    }
                    if (t.GetHp() == 2)
                    {
                        Rectangle HpBar = new Rectangle(-(width / 2), -height + 20, width * 2 / 3, height / 8);
                        e.Graphics.FillRectangle(yellowBrush, HpBar);
                    }
                    if (t.GetHp() == 1)
                    {
                        Rectangle HpBar = new Rectangle(-(width / 2), -height + 20, width / 3, height / 8);
                        e.Graphics.FillRectangle(redBrush, HpBar);
                    }
                }
            }
        }
        /// <summary>
        /// acts as the turret drawer, which draws the turrets on top of the tank body.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void TurretDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;

            int width = 50;
            int height = 50;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Rectangle r = new Rectangle(-(width / 2), -(height / 2), width, height);
            if (t.GetHp() > 0)
                e.Graphics.DrawImage(turretImages[t.GetID() % 8], r);
        }
        /// <summary>
        /// Acts as the projectile drawer, which draw the projectiles in the game world.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void ProjectileDrawer(object o, PaintEventArgs e)
        {
            Projectile p = o as Projectile;

            int width = 30;
            int height = 30;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Rectangle r = new Rectangle(-(width / 2), -(height / 2), width, height);
            e.Graphics.DrawImage(projectileImages[p.GetOwner() % 8], r);
        }
        /// <summary>
        /// Acts as the wallDrawer, which draws the walls in the game world.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void WallDrawer(object o, PaintEventArgs e)
        {
            Wall w = o as Wall;

            int width = 50;
            int height = 50;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            // Rectangles are drawn starting from the top-left corner.
            // So if we want the rectangle centered on the player's location, we have to offset it
            // by half its size to the left (-width/2) and up (-height/2)
            Rectangle r = new Rectangle(-(width / 2), -(height / 2), width, height);
            double wallp1x = w.GetP1().GetX();
            double wallp2x = w.GetP2().GetX();
            double wallp1y = w.GetP1().GetY();
            double wallp2y = w.GetP2().GetY();
            // This draws a wall if it's just a single square.
            if (wallp1x == wallp2x)
            {
                if (wallp1y == wallp2y)
                {
                    e.Graphics.DrawImage(wallImage, r);
                }
            }
            // This draws a horizontal wall from left to right
            if (wallp1x < wallp2x)
            {
                double xDifference = wallp2x - wallp1x;
                for (double i = 0; i <= xDifference; i += width)
                {
                    r = new Rectangle((int)(-(width / 2) + i), -(height / 2), width, height);
                    e.Graphics.DrawImage(wallImage, r);
                }
            }
            // Draws a vertical wall from up down
            if (wallp1y < wallp2y)
            {
                double yDifference = wallp2y - wallp1y;
                for (double i = 0; i <= yDifference; i += height)
                {
                    r = new Rectangle(-(width / 2), (int)(-(height / 2) + i), width, height);
                    e.Graphics.DrawImage(wallImage, r);
                }
            }
            // Draws a horizontal wall from right to left.
            if (wallp1x > wallp2x)
            {
                double xDifference = wallp1x - wallp2x;
                for (double i = 0; i <= xDifference; i += width)
                {
                    r = new Rectangle((int)((width / 2) - i) - 50, (height / 2) - 50, width, height);
                    e.Graphics.DrawImage(wallImage, r);
                }
            }
            // Draws a vertical wall from bottom up
            if (wallp1y > wallp2y)
            {
                double yDifference = wallp1y - wallp2y;
                for (double i = 0; i <= yDifference; i += width)
                {
                    r = new Rectangle(((width / 2)) - 50, (int)((height / 2) - i) - 50, width, height);
                    e.Graphics.DrawImage(wallImage, r);
                }
            }
        }

        /// <summary>
        /// Acts as the drawer, which will draw orange circles that represent power-ups.
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void PowerupDrawer(object o, PaintEventArgs e)
        {
            Powerup p = o as Powerup;

            int width = 16;
            int height = 16;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using (System.Drawing.SolidBrush orangeBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Orange))
            using (System.Drawing.SolidBrush whiteBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White))
            {
                // Circles are drawn starting from the top-left corner.
                // So if we want the circle centered on the powerup's location, we have to offset it
                // by half its size to the left (-width/2) and up (-height/2)
                Rectangle r1 = new Rectangle(-(width / 2), -(height / 2), width, height);
                Rectangle r2 = new Rectangle(-(width / 4), -(height / 4), width/2, height/2);
                e.Graphics.FillEllipse(orangeBrush, r1);
                e.Graphics.FillEllipse(whiteBrush, r2);
            }
        }

        // This method is invoked when the DrawingPanel needs to be re-drawn
        protected override void OnPaint(PaintEventArgs e)
        {
            int viewSize = Size.Width; // view is square, so we can just use width
            // Keeps a reference of the old matrix before it is centered on the player.
            System.Drawing.Drawing2D.Matrix oldMatrix = e.Graphics.Transform.Clone();
            // If the world is null, just draw a black square.
            if (theWorld is null)
            {
                using (System.Drawing.SolidBrush blackBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black))
                {
                    e.Graphics.FillRectangle(blackBrush, new Rectangle(0, 0, viewSize, viewSize));
                }
                return;
            }

            // Center the view on the middle of the world,
            // since the image and world use different coordinate systems
            if (theWorld.Tanks.Count != 0)
            {
                double playerX = theWorld.Tanks[id].GetLocation().GetX();
                double playerY = theWorld.Tanks[id].GetLocation().GetY();
                e.Graphics.TranslateTransform((float)(-playerX + (viewSize / 2)), (float)(-playerY + (viewSize / 2)));
            }
            // Draws background image.
            e.Graphics.DrawImage(backgroundImage, new Rectangle(-(theWorld.size / 2), -(theWorld.size / 2), theWorld.size, theWorld.size));
            // Locks while iterating over the game world's dictionairies so that they cannot be modified.
            lock (theWorld)
            {
                // Draw the walls.
                foreach (Wall wall in theWorld.Walls.Values)
                {
                    DrawObjectWithTransform(e, wall, wall.GetP1().GetX(), wall.GetP1().GetY(), 0, WallDrawer);
                }
                // Draw the Tanks, their turrets, and their details.
                foreach (Tank tank in theWorld.Tanks.Values)
                {
                    DrawObjectWithTransform(e, tank, tank.GetLocation().GetX(), tank.GetLocation().GetY(), tank.GetOrientation().ToAngle(), TankDrawer);
                    DrawObjectWithTransform(e, tank, tank.GetLocation().GetX(), tank.GetLocation().GetY(), tank.GetAiming().ToAngle(), TurretDrawer);
                    DrawObjectWithTransform(e, tank, tank.GetLocation().GetX(), tank.GetLocation().GetY(), 0, PlayerDetailDrawer);
                }
                // Draw the tank death animations.
                foreach (TankDeathAnimation tank in tankDeathAnimations.Values)
                {
                    DrawObjectWithTransform(e, tank, tank.GetLocation().GetX(), tank.GetLocation().GetY(), 0, tank.TankDeathAnimationDrawer);

                    if (tank.GetNumFrames() > tank.GetAnimationFrames())
                    {
                        oldTankDeathAnimations[tank.GetID()] = tank;
                    }
                }
                foreach (TankDeathAnimation tank in oldTankDeathAnimations.Values)
                {
                    tankDeathAnimations.Remove(tank.GetID()); ;
                }
                oldTankDeathAnimations.Clear();
                // Draw the powerups
                foreach (Powerup pow in theWorld.Powerups.Values)
                {
                    DrawObjectWithTransform(e, pow, pow.GetLocation().GetX(), pow.GetLocation().GetY(), 0, PowerupDrawer);
                }
                // Draws the projectiles
                foreach (Projectile proj in theWorld.Projectiles.Values)
                {
                    DrawObjectWithTransform(e, proj, proj.GetLocation().GetX(), proj.GetLocation().GetY(), proj.GetDirection().ToAngle(), ProjectileDrawer);
                }
                // Draw the beam animations.
                foreach (BeamAnimation beam in beamAnimations)
                {
                    DrawObjectWithTransform(e, beam, beam.GetOrigin().GetX(), beam.GetOrigin().GetY(), beam.GetDirection().ToAngle(), beam.BeamDrawer);

                    if (beam.GetNumFrames() > beam.GetAnimationFrames())
                    {
                        oldBeamAnimations.Add(beam);
                    }
                }
                foreach (BeamAnimation beam in oldBeamAnimations)
                {
                    beamAnimations.Remove(beam);
                }
                oldBeamAnimations.Clear();
            }
            // This is to display the FPS counter.
            e.Graphics.Transform = oldMatrix;
            if (fpsDisplayed)
            {
                Font font = new Font("Arial", 24);
                using (System.Drawing.SolidBrush whiteBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White))
                {
                    e.Graphics.DrawString("" + currentFPS, font, whiteBrush, 0, 0);
                }
            }
            long secondsElapsed = stopwatch.ElapsedMilliseconds / 1000;
            if (currentTime != secondsElapsed)
            {
                currentTime = Convert.ToInt32(secondsElapsed);
                currentFPS = fpsCounter;
                fpsCounter = 0;
            }
            fpsCounter++;

            // Do anything that Panel (from which we inherit) needs to do
            base.OnPaint(e);
        }

    }
}

