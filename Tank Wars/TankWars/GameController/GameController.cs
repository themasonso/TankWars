using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NetworkUtil;
using TankWars;
using Newtonsoft.Json;
using System.Drawing;
using Controller;

// Author: Mason Seppi and William Nguyen
// University of Utah

namespace Controller
{
    public class GameController
    {
        // Handler and event that informs the view that a connection has been made to a server.
        public delegate void ConnectedHandler();
        public event ConnectedHandler Connected;
        // Handler and event that informs the view when the handshake has been sent so that the client can begin drawing the game.
        public delegate void HandshakeHandler();
        public event HandshakeHandler Handshaked;
        // Handler and event that triggers when something goes wrong.
        public delegate void ErrorHandler(string error);
        public event ErrorHandler Error;
        // Handler and event to inform the view that an update from the server has arrived and been processed.
        public delegate void ServerUpdateHandler();
        public event ServerUpdateHandler UpdateArrived;
        // Handler and event to inform the view that a beam has been fired so that the animation can be drawn.
        public delegate void BeamFiredHandler(Beam b);
        public event BeamFiredHandler BeamFired;
        // Handler and event to inform the view that a tank has died so that we can draw the animation.
        public delegate void TankDiedHandler(Tank t);
        public event TankDiedHandler TankDied;
        // Handler and event to inform the view to display the client's FPS.
        public delegate void DisplayFPSHandler(bool state);
        public event DisplayFPSHandler DisplayFPS;
        // Handler and event to inform the view to quit the application
        public delegate void QuitHandler();
        public event QuitHandler Quit;

        // These are all player command variables
        private string fireType = "none";
        private bool directionUp;
        private bool directionDown;
        private bool directionLeft;
        private bool directionRight;
        private Vector2D aiming;

        // This is the SocketState that represents the server.
        private SocketState theServer;
        // The Game Controller's world that the view references.
        private World theWorld;
        // The player's id we get from the server.
        private int id;
        // Whether the FPS counter is currently displayed.
        private bool fpsDisplayed;

        public GameController()
        {
        }
        /// <summary>
        /// Getter for the player's id.
        /// </summary>
        /// <returns></returns>
        public int GetID()
        {
            return id;
        }
        /// <summary>
        /// Public getter method to get the controller's game world.
        /// </summary>
        /// <returns></returns>
        public World GetWorld()
        {
            return theWorld;
        }
        /// <summary>
        /// Public method that allows the view to connect to a server.
        /// </summary>
        /// <param name="address"></param>
        public void Connect(string address)
        {
            Networking.ConnectToServer(OnConnect, address, 11000);
        }
        /// <summary>
        /// When the client first connects to the server it then informs the view that a connection has been made.
        /// The handshake process begins after data is received.
        /// </summary>
        /// <param name="state"></param>
        private void OnConnect(SocketState state)
        {
            if (state.ErrorOccurred)
            {
                Error("Error connecting to server");
                return;
            }

            theServer = state;
            Connected();

            state.OnNetworkAction = HandShake;
            Networking.GetData(state);
        }
        /// <summary>
        /// Private callback to start the receiving data process from the server.
        /// </summary>
        /// <param name="state"></param>
        private void RecieveData(SocketState state)
        {
            if (state.ErrorOccurred)
            {
                Error("Lost connection to server");
                return;
            }

            ProcessData(state);

            Networking.GetData(state);
        }
        /// <summary>
        /// Private method that handles the game's initial handshake from the server.
        /// Ensures that client receives the correct ID and game world size.
        /// </summary>
        /// <param name="state"></param>
        private void HandShake(SocketState state)
        {
            if (state.ErrorOccurred)
            {
                Error("Handshake failed.");
                return;
            }
            string totalData = state.GetData();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])");
            id = int.Parse(parts[0]);
            int worldSize = int.Parse(parts[1]);
            state.RemoveData(0, parts[0].Length);
            state.RemoveData(0, parts[1].Length);
            theWorld = new World(worldSize);

            state.OnNetworkAction = RecieveData;
            Handshaked();
            Networking.GetData(state);
        }
        /// <summary>
        /// Processes the data received from the server.
        /// Informs the view that an update has been received from the server and processed.
        /// Sends the server the user's inputs.
        /// </summary>
        /// <param name="state"></param>
        private void ProcessData(SocketState state)
        {
            string totalData = state.GetData();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])");
            lock (theWorld)
            {
                foreach (string p in parts)
                {
                    if (p.Length == 0)
                        continue;
                    if (p[p.Length - 1] != '\n')
                        break;
                    if (p.Contains("tank"))
                    {
                        Tank currTank = JsonConvert.DeserializeObject<Tank>(p);
                        if (currTank.IsDead())
                            TankDied(currTank);
                        if (!currTank.GetActive())
                        {
                            TankDied(currTank);
                            theWorld.Tanks.Remove(currTank.GetID());
                        }
                        else
                            theWorld.Tanks[currTank.GetID()] = currTank;
                    }
                    else if (p.Contains("proj"))
                    {
                        Projectile currProjectile = JsonConvert.DeserializeObject<Projectile>(p);
                        if (!currProjectile.GetActive())
                            theWorld.Projectiles.Remove(currProjectile.GetID());
                        else
                            theWorld.Projectiles[currProjectile.GetID()] = currProjectile;
                    }
                    else if (p.Contains("wall"))
                    {
                        Wall currWall = JsonConvert.DeserializeObject<Wall>(p);
                        theWorld.Walls[currWall.GetID()] = currWall;
                    }
                    else if (p.Contains("beam"))
                    {
                        Beam currBeam = JsonConvert.DeserializeObject<Beam>(p);
                        theWorld.Beams[currBeam.GetID()] = currBeam;
                        BeamFired(currBeam);
                    }
                    else if (p.Contains("power"))
                    {
                        Powerup currPowerup = JsonConvert.DeserializeObject<Powerup>(p);
                        if (!currPowerup.GetActive())
                            theWorld.Powerups.Remove(currPowerup.GetID());
                        else
                            theWorld.Powerups[currPowerup.GetID()] = currPowerup;
                    }
                    state.RemoveData(0, p.Length);
                }
            }
            if (UpdateArrived != null)
            {
                UpdateArrived();
            }
            ProcessInputs();
        }
        /// <summary>
        /// Sends the server the client's entered name.
        /// </summary>
        /// <param name="name"></param>
        public void SendName(string name)
        {
            if (name.Length > 15)
            {
                Error("Name is longer than 16 characters");
                return;
            }
            if (theServer != null)
                Networking.Send(theServer.TheSocket, name + "\n");
        }
        /// <summary>
        /// Helper method to process the user's game inputs.
        /// </summary>
        private void ProcessInputs()
        {
            string direction = "none";
            if (directionUp)
                direction = "up";
            if (directionLeft)
                direction = "left";
            if (directionRight)
                direction = "right";
            if (directionDown)
                direction = "down";

            ControlCommand command = new ControlCommand(direction, fireType, aiming);
            Networking.Send(theServer.TheSocket, JsonConvert.SerializeObject(command) + "\n");
        }
        /// <summary>
        /// Handles when the player moves their mouse inside the client.
        /// </summary>
        /// <param name="location"></param>
        public void HandleMouseMovedRequest(Point location)
        {
            Vector2D turretDirection = new Vector2D(location.X, location.Y);
            turretDirection.Normalize();
            aiming = turretDirection;
        }
        /// <summary>
        /// Handles when a player wants to stop shooting.
        /// </summary>
        /// <param name="button"></param>
        public void CancelMouseRequest(string button)
        {
            if (button == "left")
                fireType = "none";
            else if (button == "right")
                fireType = "none";
        }
        /// <summary>
        /// Handles when a player wants to shoot their gun.
        /// </summary>
        /// <param name="button"></param>
        public void HandleMouseRequest(string button)
        {
            if (button == "left")
                fireType = "main";
            else if (button == "right")
                fireType = "alt";
        }
        /// <summary>
        /// Triggers an event to inform the view that the player wants to display their FPS
        /// </summary>
        public void DisplayFPSCounter()
        {
            if (fpsDisplayed)
            {
                DisplayFPS(false);
                fpsDisplayed = false;
            }
            else
            {
                DisplayFPS(true);
                fpsDisplayed = true;
            }
        }
        /// <summary>
        /// Triggers an event to inform the view that the player wants to quit the application
        /// </summary>
        public void HandleQuit()
        {
            Quit();
        }
        /// <summary>
        /// Method to begin moving a player
        /// </summary>
        /// <param name="key"></param>
        public void HandleMoveRequest(Char key)
        {
            if (key == 'W')
                directionUp = true;
            if (key == 'A')
                directionLeft = true;
            if (key == 'S')
                directionDown = true;
            if (key == 'D')
                directionRight = true;
        }
        /// <summary>
        /// Method to cancel a player's movement.
        /// </summary>
        /// <param name="key"></param>
        public void CancelMoveRequest(Char key)
        {
            if (key == 'W')
                directionUp = false;
            if (key == 'A')
                directionLeft = false;
            if (key == 'S')
                directionDown = false;
            if (key == 'D')
                directionRight = false;
        }
    }
}
