using NetworkUtil;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using TankWars;

// Author: Mason Seppi and William Nguyen
// University of Utah

namespace Controller
{
    /// <summary>
    /// Class that represents a Game Server for the Tank Wars game
    /// </summary>
    public class GameServer
    {
        // A map of clients that are connected, each with an ID
        private Dictionary<long, SocketState> newClients;
        private Dictionary<long, SocketState> connectedClients;
        private HashSet<long> disconnectedClients;
        // A map of clients and their inputs requests
        private Dictionary<long, ControlCommand> userInputs;

        // World object that contains the objects for the game world
        private World theWorld;
        // Stopwatch object for counting the frames the server sends
        private Stopwatch watch;
        private Stopwatch fpsWatch;
        // Random object to help generate random spawn locations
        private Random rand;

        // Counter for world object IDs
        private int nextWallID;
        private int nextProjID;
        private int nextBeamID;
        private int nextPowerID;

        // Current delay of powerups
        private int powerupDelay;

        // Settings read in from XML settings file
        private int universeSize;
        private int msPerFrame;
        private int framesPerShot;
        private int respawnRate;
        private int numOfFrames;
        private bool bulletHell;
        private int maxHp;
        private int projectileSpeed;
        private double tankSpeed;
        private int tankSize;
        private int wallSize;
        private int maxPowerups;
        private int maxPowerupDelay;
        private int projectileDamagePoints;
        private int beamDamagePoints;
        private int projectileKillPoints;
        private int beamKillPoints;
        private int numOfAllowedBounces;

        /// Server FPS variables
        private int fpsCounter;
        private int currentTime = 0;
        private int currentFPS;

        /// <summary>
        /// Main method to create a GameServer object
        /// start the server and prevent from closing
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            GameServer server = new GameServer();
            server.StartServer();

            // Sleep to prevent the program from closing,
            // since all the real work is done in separate threads.
            // StartServer is non-blocking.
            Console.Read();
        }

        /// <summary>
        /// Empty constructor to initialize the server's state
        /// </summary>
        public GameServer()
        {
            // Create new objects for member variables
            newClients = new Dictionary<long, SocketState>();
            connectedClients = new Dictionary<long, SocketState>();
            disconnectedClients = new HashSet<long>();
            userInputs = new Dictionary<long, ControlCommand>();
            watch = new Stopwatch();
            fpsWatch= new Stopwatch();
            rand = new Random();

            // Sets defaults values from the World Constants
            maxHp = Constants.MaxHP;
            projectileSpeed = Constants.ProjectileSpeed;
            tankSpeed = Constants.TankSpeed;
            tankSize = Constants.TankSize;
            wallSize = Constants.WallSize;
            maxPowerups = Constants.MaxPowerups;
            maxPowerupDelay = Constants.PowerupDelay;
            projectileDamagePoints = Constants.ProjectileDamagePoints;
            beamDamagePoints = Constants.BeamDamagePoints;
            projectileKillPoints = Constants.ProjectileKillPoints;
            beamKillPoints = Constants.BeamKillPoints;
            numOfAllowedBounces = Constants.numOfAllowedBounces;

            // Read Server setings XML file
            LoadXMLSettings();
        }

        /// <summary>
        /// Method to read an XML file that looks for certain tags 
        /// to load in settings and walls for the game
        /// </summary>
        private void LoadXMLSettings()
        {
            try
            {
                // Create a XMLReaderSettings so that it ignores white space and comments
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreComments = true;
                settings.IgnoreWhitespace = true;

                // Opens XML settings file from Resources folder
                using (XmlReader reader = XmlReader.Create("../../../Resources/settings.xml", settings))
                {
                    // X,Y Values for points of the wall
                    double p1x = 0;
                    double p2x = 0;
                    double p1y = 0;
                    double p2y = 0;

                    // Reads in lines from XML file until it reaches the end
                    while (reader.Read())
                    {
                        // Checks for starting element tags
                        if (reader.IsStartElement())
                        {
                            // Switch based on the tags
                            switch (reader.Name)
                            {
                                case "GameSettings":
                                    break;
                                case "UniverseSize":
                                    reader.Read();
                                    universeSize = int.Parse(reader.Value);
                                    theWorld = new World(universeSize);
                                    break;
                                case "MSPerFrame":
                                    reader.Read();
                                    msPerFrame = int.Parse(reader.Value);
                                    break;
                                case "FramesPerShot":
                                    reader.Read();
                                    framesPerShot = int.Parse(reader.Value);
                                    break;
                                case "RespawnRate":
                                    reader.Read();
                                    respawnRate = int.Parse(reader.Value);
                                    break;
                                case "MaxHP":
                                    reader.Read();
                                    maxHp = int.Parse(reader.Value);
                                    break;
                                case "ProjectileSpeed":
                                    reader.Read();
                                    projectileSpeed = int.Parse(reader.Value);
                                    break;
                                case "TankSpeed":
                                    reader.Read();
                                    tankSpeed = int.Parse(reader.Value);
                                    break;
                                case "TankSize":
                                    reader.Read();
                                    tankSize = int.Parse(reader.Value);
                                    break;
                                case "WallSize":
                                    reader.Read();
                                    wallSize = int.Parse(reader.Value);
                                    break;
                                case "MaxPowerups":
                                    reader.Read();
                                    maxPowerups = int.Parse(reader.Value);
                                    break;
                                case "MaxPowerupDelay":
                                    reader.Read();
                                    maxPowerupDelay = int.Parse(reader.Value);
                                    break;
                                case "ProjectileDamagePoints":
                                    reader.Read();
                                    projectileDamagePoints = int.Parse(reader.Value);
                                    break;
                                case "BeamDamagePoints":
                                    reader.Read();
                                    beamDamagePoints = int.Parse(reader.Value);
                                    break;
                                case "ProjectileKillPoints":
                                    reader.Read();
                                    projectileKillPoints = int.Parse(reader.Value);
                                    break;
                                case "BeamKillPoints":
                                    reader.Read();
                                    beamKillPoints = int.Parse(reader.Value);
                                    break;
                                case "BulletHell":
                                    reader.Read();
                                    if (reader.Value.ToLower() == "on")
                                        bulletHell = true;
                                    else if (reader.Value.ToLower() == "off")
                                        bulletHell = false;
                                    break;
                                case "NumberOfBounces":
                                    reader.Read();
                                    numOfAllowedBounces = int.Parse(reader.Value);
                                    break;
                                case "Wall":
                                    while (reader.NodeType != XmlNodeType.EndElement)
                                    {
                                        if (reader.NodeType == XmlNodeType.Element)
                                        {
                                            // Check for the first point
                                            if (reader.Name == "p1")
                                            {
                                                reader.Read();
                                                reader.Read();
                                                p1x = Double.Parse(reader.Value);
                                                reader.Read();
                                                reader.Read();
                                                reader.Read();
                                                p1y = Double.Parse(reader.Value);
                                            }
                                            // Check for the second point
                                            if (reader.Name == "p2")
                                            {
                                                reader.Read();
                                                reader.Read();
                                                p2x = Double.Parse(reader.Value);
                                                reader.Read();
                                                reader.Read();
                                                reader.Read();
                                                p2y = Double.Parse(reader.Value);
                                            }
                                        }
                                        reader.Read();
                                        // Check if we have reached the end of the wall
                                        if (reader.NodeType == XmlNodeType.EndElement)
                                        {
                                            reader.Read();
                                            reader.Read();
                                        }
                                    }
                                    // Create Vector2D objects representing the points of the wall
                                    Vector2D p1 = new Vector2D(p1x, p1y);
                                    Vector2D p2 = new Vector2D(p2x, p2y);
                                    // Create a Wall object from the points and nextWallID counter
                                    Wall newWall = new Wall(nextWallID++, p1, p2);
                                    // Add the wall to the world
                                    theWorld.Walls[newWall.GetID()] = newWall;
                                    break;
                                default:
                                    throw new Exception("Couldn't read settings file.");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Start accepting Tcp sockets connections from clients
        /// </summary>
        public void StartServer()
        {
            // This begins an "event loop"
            Networking.StartServer(NewClientConnected, 11000);

            Console.WriteLine("Server is running");
            fpsWatch.Start();

            // Infinite loop that runs the game logic
            while (true)
            {
                // Number of seconds that have passed
                long seconds = fpsWatch.ElapsedMilliseconds / 1000;
                // Checks if one second has passed
                if (currentTime != seconds)
                {
                    // Reset the time
                    currentTime = Convert.ToInt32(seconds);
                    // Get the number of frames within the second
                    currentFPS = fpsCounter;
                    // Reset the counter
                    fpsCounter = 0;
                }
                Console.WriteLine("FPS:" + currentFPS);

                watch.Start();
                // Empty while loop to delay the server to keep information sent at a constant rate
                while (watch.ElapsedMilliseconds < msPerFrame) {  }
                watch.Reset();

                // Updates the game world
                UpdateGameWorld();

                // Foreach loop where the server sends the updated game world to each client
                // Foreach loop for each world object with the exception of walls and sends them to the client
                lock (connectedClients)
                {
                    foreach (SocketState client in connectedClients.Values)
                    {
                        // Lock here because tanks can be added at any point
                        lock (theWorld)
                        {
                            // loop through tanks send them
                            foreach (Tank tank in theWorld.Tanks.Values)
                            {
                                // tries to send clients updated game world if not possible then adds them to HashSet of disconnected clients
                                if (!Networking.Send(client.TheSocket, JsonConvert.SerializeObject(tank) + "\n"))
                                    AddDisconnectedClient(client.ID);
                            }
                            // loop through projectiles send them
                            foreach (Projectile proj in theWorld.Projectiles.Values)
                            {
                                // tries to send clients updated game world if not possible then adds them to HashSet of disconnected clients
                                if (!Networking.Send(client.TheSocket, JsonConvert.SerializeObject(proj) + "\n"))
                                    AddDisconnectedClient(client.ID);
                            }
                            // loop through beams send them
                            foreach (Beam beam in theWorld.Beams.Values)
                            {
                                // tries to send clients updated game world if not possible then adds them to HashSet of disconnected clients
                                if (!Networking.Send(client.TheSocket, JsonConvert.SerializeObject(beam) + "\n"))
                                    AddDisconnectedClient(client.ID);
                            }
                            // loop through powerups send them
                            foreach (Powerup power in theWorld.Powerups.Values)
                            {
                                // tries to send clients updated game world if not possible then adds them to HashSet of disconnected clients
                                if (!Networking.Send(client.TheSocket, JsonConvert.SerializeObject(power) + "\n"))
                                    AddDisconnectedClient(client.ID);
                            }
                        }
                    }
                }

                // Lock because clients can disconnect at any time
                // Resulting in clients being added to the hashset
                lock (disconnectedClients)
                {
                    // remove disconnected clients from clients dictionary
                    foreach (long id in disconnectedClients)
                    {
                        // remove clients
                        RemoveClient(id);

                        // deactivate the disconnected client's tank if they are dead or alive
                        if (theWorld.Tanks.ContainsKey((int)id))
                            theWorld.Tanks[(int)id].SetActive(false);
                        else if (theWorld.DeadTanks.ContainsKey((int)id))
                            theWorld.DeadTanks[(int)id].SetActive(false);
                    }
                }

                // Lock because clients can connect at any time
                lock (connectedClients)
                {
                    // Sending connected clients disconnected tanks
                    foreach (SocketState client in connectedClients.Values)
                    {
                        // Lock because clients can join at any time
                        // Results in tanks to be added to the world
                        lock (theWorld)
                        {
                            // Check for deactivated tanks
                            foreach (Tank tank in theWorld.Tanks.Values)
                            {
                                if (!tank.GetActive())
                                {
                                    // try to send disconnected to connected clients
                                    if (!Networking.Send(client.TheSocket, JsonConvert.SerializeObject(tank) + '\n'))
                                        AddDisconnectedClient(client.ID);
                                }
                            }
                        }
                    }
                }

                // Lock because clients can disconnect at any time
                // Resulting in disconnected clients being added
                lock (disconnectedClients)
                {
                    // Remove deactivated tanks if they are alive or dead
                    foreach (long id in disconnectedClients)
                    {
                        // Lock because clients can connect at any time
                        // resulting in a tank being added
                        lock (theWorld)
                        {
                            if (theWorld.Tanks.ContainsKey((int)id))
                                theWorld.Tanks.Remove((int)id);
                            else if (theWorld.DeadTanks.ContainsKey((int)id))
                                theWorld.DeadTanks.Remove((int)id);
                        }
                    }
                }
                // Increment the frame counter
                numOfFrames++;
                // Increment the fps counter
                fpsCounter++;
            }
        }

        /// <summary>
        /// Method to be invoked by the networking library
        /// when a new client connects
        /// </summary>
        /// <param name="state">The SocketState representing the new client</param>
        private void NewClientConnected(SocketState state)
        {
            if (state.ErrorOccurred)
                return;

            // Locks because clients can connect at any time
            lock (newClients)
            {
                // Add the client to the newClient's dictionary
                newClients[state.ID] = state;
            }
            Console.WriteLine("Accepted New Connection");

            // Sets the state's OnNetworkAction to Handshake process
            state.OnNetworkAction = Handshake;

            // Get data from the network to start Handshake process
            Networking.GetData(state);
        }

        /// <summary>
        /// Method to be invoked by the networking library
        /// when a network action occurs to try to start the handshake process
        /// Checks if the server received a name from the client appended with a '\n' (new line)
        /// If it did then the server will send the client their id and the universe size
        /// Before sending them the walls
        /// </summary>
        /// <param name="state">The SocketState that represents the client</param>
        private void Handshake(SocketState state)
        {
            // Remove the client if they aren't still connected
            if (state.ErrorOccurred)
            {
                RemoveClient(state.ID);
                return;
            }

            // Get data that client sent
            string totalData = state.GetData();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])");

            // First message sent should be a string representing their name
            string name = parts[0].Replace("\n", "");
            // Player's id is the SocketState's id
            long id = state.ID;

            // Remove data from the SocketState's growable buffer
            state.RemoveData(0, parts[0].Length);

            // Lock here because clients can connect at any time
            lock (theWorld)
            {
                // Create a new tank
                Tank tank = new Tank();
                // Generate a random spawn location
                Vector2D location = GenerateSpawnLocation(tank);
                // Assign the tank
                tank = new Tank((int)id, name, location, maxHp);

                // Set the state's OnNetworkAction to ProcessCommands
                // Any information the client sends after this point should only be ControlCommands
                state.OnNetworkAction = ProcessCommands;

                // Send the id and world size to the Client
                // If send failed then disconnect client
                if (!Networking.Send(state.TheSocket, id + "\n" + theWorld.size + "\n"))
                    AddDisconnectedClient(state.ID);

                // Send the walls to client
                foreach (Wall wall in theWorld.Walls.Values)
                {
                    // Try to send wall to client
                    // If send failed then disconnect client
                    if (!Networking.Send(state.TheSocket, JsonConvert.SerializeObject(wall) + "\n"))
                        AddDisconnectedClient(state.ID);
                }

                // Add the tank to the world
                theWorld.Tanks[(int)id] = tank;
            }

            // Adds the client to connectedClients dictionary because they complete the handshake
            AddClient(state.ID, state);

            Console.WriteLine("Client:" + state.ID + " Connected");
            // Continue the event loop that receives messages from this client
            Networking.GetData(state);
        }

        /// <summary>
        /// Method to be invoked by the Networking library
        /// when a network action occurs to try to parse what the client sent
        /// If the client sent something that was not a ControlCommand
        /// Then the is added to the list of clients to be disconnected
        /// </summary>
        /// <param name="sender">The SocketState that represents the client</param>
        private void ProcessCommands(SocketState state)
        {
            // Remove the client if they aren't still connected
            // Add the disconnected to the HashSet of disconnected clients
            if (state.ErrorOccurred)
            {
                RemoveClient(state.ID);
                AddDisconnectedClient(state.ID);
                return;
            }

            // Get data that client sent
            string totalData = state.GetData();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])");

            // Loop until we have processed all messages.
            // We may have received more than one.
            foreach (string p in parts)
            {
                // Ignore empty strings added by the regex splitter
                if (p.Length == 0)
                    continue;
                // The regex splitter will include the last string even if it doesn't end with a '\n',
                // So we need to ignore it if this happens. 
                if (p[p.Length - 1] != '\n')
                    break;

                // Lock here because userInputs can be added and updated at any time
                lock (userInputs)
                {
                    // Deserialize message from client into a control command
                    ControlCommand input = JsonConvert.DeserializeObject<ControlCommand>(p);

                    // Handle malformed client data sent to server
                    // If anything malformed is received
                    // Then disconnect the client by adding it the hashset of disconnected clients
                    // Lock because clients can disconnect at any time
                    lock (disconnectedClients)
                    {
                        switch (input.GetMovementDirection())
                        {
                            case "none":
                                userInputs[state.ID] = input;
                                break;
                            case "left":
                                userInputs[state.ID] = input;
                                break;
                            case "right":
                                userInputs[state.ID] = input;
                                break;
                            case "up":
                                userInputs[state.ID] = input;
                                break;
                            case "down":
                                userInputs[state.ID] = input;
                                break;
                            default:
                                AddDisconnectedClient(state.ID);
                                return;
                        }
                        switch (input.GetFireType())
                        {
                            case "none":
                                userInputs[state.ID] = input;
                                break;
                            case "main":
                                userInputs[state.ID] = input;
                                break;
                            case "alt":
                                userInputs[state.ID] = input;
                                break;
                            default:
                                AddDisconnectedClient(state.ID);
                                return;
                        }
                        if (input.GetTurretDirection() is null)
                        {
                            AddDisconnectedClient(state.ID);
                            return;
                        }
                    }


                    // Lock here because clients can disconnect at any time
                    // So it is possible for this tank to not exist if there
                    // Was not a lock to prevent it from being removed
                    lock (theWorld)
                    {
                        // Check if the player tried to shoot an alternate projectile
                        if (input.GetFireType() == "alt")
                            theWorld.Tanks[(int)state.ID].SetAlternateFireCommandReceived(true);
                    }
                }

                // Remove it from the SocketState's growable buffer
                state.RemoveData(0, p.Length);
            }
            Networking.GetData(state);
        }

        /// <summary>
        /// Method for updating the game world
        /// 
        /// Check for dead tanks in the dictionary of alive tanks if there are any then move them to the dictionary of dead tanks
        /// Check to see if dead tanks are ready to be respawned
        /// Remove any dead projectiles, beams, or powerups from the world before adding anything new
        /// Loop through all of the inputs the server has received
        /// Check to see if a tank has fired recently if not then fire a projectile for that tank
        /// Try to move all tanks 
        /// Check for Tank-Wall collisions
        /// Then orient them correctly
        /// Check to see if a tank tried to fire a beam
        /// If they did check for Beam-Tank collisions
        /// Check for Projectile-Wall, Out of Bounds, and Projectile-Tank collisions
        /// If Projectile collided with tanks
        /// Update tank's health accordingly
        /// If tank's health is 0 then set the tank to be dead
        /// If the tank's health is 0 then update the score of the player who shot the projectile
        /// If any tank's are dead add them to the dead tanks dictionary
        /// Spawn new powerups
        /// </summary>
        private void UpdateGameWorld()
        {
            // Lock here because tanks can be added to the world at any time
            lock (theWorld)
            {
                // Check for dead tanks
                foreach (Tank tank in theWorld.Tanks.Values)
                {
                    // If the tank is dead add them to dictionary dead tanks
                    if (tank.IsDead())
                        theWorld.DeadTanks[tank.GetID()] = tank;
                    // Otherwise they are alive so remove living tanks from the dead tanks
                    else
                        theWorld.DeadTanks.Remove(tank.GetID());
                }
                // Check if dead tanks are ready to respawn
                foreach (Tank tank in theWorld.DeadTanks.Values)
                {
                    // Remove dead tanks from the world
                    theWorld.Tanks.Remove(tank.GetID());
                    // Respawn connected tanks
                    if (numOfFrames - tank.GetFrameDied() >= respawnRate)
                    {
                        // Set tank to not be dead and health points back to tank max health points
                        tank.SetIsDead(false);
                        tank.SetHp(maxHp);
                        // Generate a random spawn location
                        Vector2D spawnLocation = GenerateSpawnLocation(tank);
                        tank.SetLocation(spawnLocation);
                        // Add them back into the world
                        theWorld.Tanks[tank.GetID()] = tank;
                    }
                }

                // Removes dead projectiles
                foreach (Projectile proj in theWorld.DeadProjectiles.Values)
                    theWorld.Projectiles.Remove(proj.GetID());

                // Remove dead beams
                foreach (Beam beam in theWorld.DeadBeams.Values)
                    theWorld.Beams.Remove(beam.GetID());

                // Remove dead powerups
                foreach (Powerup power in theWorld.DeadPowerups.Values)
                    theWorld.Powerups.Remove(power.GetID());
            }

            // Lock here because new inputs from clients can be received at any time
            lock (userInputs)
            {
                // Loop through all player inputs
                foreach (long id in userInputs.Keys)
                {
                    // Gets the ControlCommand out of the userInputs dictionary
                    userInputs.TryGetValue(id, out ControlCommand command);

                    // Lock here because tanks can be added at any point
                    lock (theWorld)
                    {
                        // Skips dead player's commands
                        if (!theWorld.Tanks.ContainsKey((int)id))
                            continue;
                        // Process the command for firing
                        ProcessFire(id, command);
                    }

                    // Variables that represent the tank's velocity and orientation
                    // Process the command for moving
                    ProcessMovement(id, command, out Vector2D velocity, out Vector2D orientation);

                    // Lock here because tanks can be added at any point
                    lock (theWorld)
                    {
                        // Check to see if the command sent was from an alive tank
                        // Ignores dead tanks commands
                        if (theWorld.Tanks.ContainsKey((int)id))
                        {
                            // Save the current position of the tank
                            Vector2D oldLocation = theWorld.Tanks[(int)id].GetLocation();
                            Vector2D newLocation = oldLocation + velocity;

                            // Tank-Powerup Detection
                            foreach (Powerup p in theWorld.Powerups.Values)
                            {
                                // Check if tank drove over powerup
                                // If true then deactivate powerup then add one to alternate ammo counter
                                if (DetectTankPowerupCollision(p, theWorld.Tanks[(int)id]))
                                {
                                    DeactivatePowerup(p);
                                    AddAlternateAmmo(theWorld.Tanks[(int)id], 1);
                                }
                            }

                            // Out of Bounds Detection
                            if (DetectOutOfBounds(newLocation))
                                newLocation = Warp(oldLocation, newLocation);

                            // Tank-Wall Detection
                            foreach (Wall w in theWorld.Walls.Values)
                            {
                                // Check if tank's new location would have collided with a wall
                                // If true then do move it by settings new location to its current location
                                if (DetectWallCollision(w, newLocation, theWorld.Tanks[(int)id]))
                                    newLocation = oldLocation;
                            }

                            // Update the theWorld with the tank's new location, orientation, and aiming
                            theWorld.Tanks[(int)id].SetLocation(newLocation);
                            theWorld.Tanks[(int)id].SetAiming(command.GetTurretDirection());
                            theWorld.Tanks[(int)id].SetOrientation(orientation);

                            // Spawn a beam if a beam command was received
                            if (theWorld.Tanks[(int)id].GetAlternateFireReceived())
                            {
                                // If alternate ammo is greater than 0
                                if (theWorld.Tanks[(int)id].GetAltAmmo() > 0)
                                {
                                    RemoveAlternateAmmo(theWorld.Tanks[(int)id], 1);
                                    // Fire beam by placing it in the world
                                    Beam b = new Beam(nextBeamID++, (int)id, theWorld.Tanks[(int)id].GetLocation(), theWorld.Tanks[(int)id].GetAiming());
                                    theWorld.Beams[b.GetID()] = b;
                                }
                            }
                            // Set the tank's BeamCommandReceived to false
                            // As their command was processed
                            theWorld.Tanks[(int)id].SetAlternateFireCommandReceived(false);
                        }
                    }
                }

                //Loops through the projectiles and updates their positions.
                lock (theWorld)
                {
                    // Update active projectiles
                    foreach (Projectile p in theWorld.Projectiles.Values)
                    {
                        // Calculate the projectiles velocity and new location
                        Vector2D velocity = p.GetDirection() * projectileSpeed;
                        Vector2D oldLocation = p.GetLocation();
                        Vector2D newLocation = p.GetLocation() + velocity;

                        // Check for bulletHell game mode is on and if the projectile has bounced less number of allowed bounces
                        if (bulletHell && p.GetNumBounces() < numOfAllowedBounces)
                            BulletHellBounceCalculation(p, newLocation);
                        // Check if bulletHell is off or projectile has bounced maximum number of times
                        else if (!bulletHell || p.GetNumBounces() >= numOfAllowedBounces)
                        {
                            // Projectile-Wall Collision
                            foreach (Wall w in theWorld.Walls.Values)
                            {
                                if (DetectWallCollision(w, newLocation, p))
                                    DeactivateProjectile(p);
                            }
                        }

                        // Checks if bulletHell is on
                        // If it is warp the projectile
                        if (bulletHell)
                        {
                            // Out of Bounds Detection
                            if (DetectOutOfBounds(newLocation))
                            {
                                // Warp the projectile
                                newLocation = Warp(newLocation, oldLocation);
                                // If the projectile hasn't warped yet then set it to be true
                                if (!p.GetWarped())
                                    p.SetWarped(true);
                                // Otherwise deactive the projectile
                                else
                                    DeactivateProjectile(p);
                            }
                        }
                        // Default deactivate out of bounds projectiles
                        else
                        {
                            if (DetectOutOfBounds(newLocation))
                                DeactivateProjectile(p);
                        }

                        // Update projectile's location
                        p.SetLocation(newLocation);

                        // Projectile-Tank Collision Detection
                        foreach (Tank t in theWorld.Tanks.Values)
                        {
                            // Prevents tanks from hurting themselves
                            if (p.GetOwner() == t.GetID())
                                continue;

                            // Checks if projectile hit a tank
                            if (DetectProjectileTankCollision(p, t))
                                ProjectileHitTank(p, t);
                        }
                    }

                    // Beam-Tank collision
                    foreach (Beam beam in theWorld.Beams.Values)
                    {
                        foreach (Tank tank in theWorld.Tanks.Values)
                        {
                            // Check if the beam collided with any tanks
                            if (Intersects(beam.GetOrigin(), beam.GetDirection(), tank.GetLocation(), tankSize))
                                BeamHitTank(beam, tank);
                        }
                        // Add beam to dictionary of dead beams for removal
                        theWorld.DeadBeams[beam.GetID()] = beam;
                    }

                    // Determine where powerups spawn.
                    // Check if there is space for a new powerup
                    if (theWorld.Powerups.Count < maxPowerups)
                    {
                        // Creates a null Powerup so that it can be changed if the correct amount of frames have passed to spawn one
                        Powerup newPowerup = null;
                        // Generate a new powerup delay
                        powerupDelay = rand.Next(1, maxPowerupDelay);

                        // Check if no powerups exist currently
                        if (theWorld.Powerups.Count == 0)
                        {
                            // Check if correct amount of frames have passed
                            if (numOfFrames % powerupDelay == 0)
                            {
                                // Pick random location
                                Vector2D location = GenerateSpawnLocation(newPowerup);

                                // Create new powerup
                                newPowerup = new Powerup(nextPowerID++, location, numOfFrames);
                            }
                        }

                        // Check if there are any existing powerups
                        foreach (Powerup power in theWorld.Powerups.Values)
                        {
                            // Checking if the correct amount of frames have passed before trying to spawn another powerup
                            if (numOfFrames - power.GetFrameSpawned() >= powerupDelay)
                            {
                                // Pick new random location
                                Vector2D location = GenerateSpawnLocation(newPowerup);

                                // Create new powerup
                                newPowerup = new Powerup(nextPowerID++, location, numOfFrames);
                            }
                        }

                        // Checks if the new powerup is not null
                        // If it is not null then we spawn it
                        if (newPowerup != null)
                            theWorld.Powerups[newPowerup.GetID()] = newPowerup;
                    }
                }
            }
        }

        /// <summary>
        /// Process the ControlCommand for the type of turret fire requested
        /// </summary>
        /// <param name="id">Long representing client's ID</param>
        /// <param name="command">ControlCommand that the client sent</param>
        private void ProcessFire(long id, ControlCommand command)
        {
            // Check to see if the player tried to fire a regular projectile
            if (command.GetFireType() == "main")
            {
                // Create a null projectile so that it can be change if the tank can actually fire
                Projectile newProjectile = null;

                // Checking if the client requesting the fire command has fired too recently to fire again
                if (numOfFrames - theWorld.Tanks[(int)id].GetFrameFired() >= framesPerShot)
                    theWorld.Tanks[(int)id].SetProjectileFired(false);

                // If the tank has not fired too recently, we create a projectile and assign it to the new projectile object
                if (!theWorld.Tanks[(int)id].GetProjectileFired())
                    newProjectile = new Projectile(nextProjID++, (int)id, theWorld.Tanks[(int)id].GetLocation(), theWorld.Tanks[(int)id].GetAiming(), numOfFrames);

                // If a tank has fired a projectile has fired recently then newProjectile will be null and the request will ignored
                // Otherwise the server fire the projectile on the current frame and say that the tank has fired.
                if (newProjectile != null)
                {
                    theWorld.Projectiles[newProjectile.GetID()] = newProjectile;
                    theWorld.Tanks[(int)id].SetFrameFired(numOfFrames);
                    theWorld.Tanks[(int)id].SetProjectileFired(true);
                }
            }
        }

        /// <summary>
        /// Process the ControlCommand for the type of movement requested
        /// </summary>
        /// <param name="id">Long representing client's ID</param>
        /// <param name="command">ControlCommand that the client sent</param>
        /// <param name="velocity">Vector2D representing velocity of the tank</param>
        /// <param name="orientation">Vector2D representing turret orientation of the tank</param>
        private void ProcessMovement(long id, ControlCommand command, out Vector2D velocity, out Vector2D orientation)
        {
            // Check which direction the tank wanted to move in
            // Orient them and compute their velocity accordingly
            // If the didn't move then velocity is 0 and orietation stays the same
            switch (command.GetMovementDirection())
            {
                case ("up"):
                    orientation = new Vector2D(0, -1);
                    velocity = orientation * tankSpeed;
                    break;
                case ("left"):
                    orientation = new Vector2D(-1, 0);
                    velocity = orientation * tankSpeed;
                    break;
                case ("right"):
                    orientation = new Vector2D(1, 0);
                    velocity = orientation * tankSpeed;
                    break;
                case ("down"):
                    orientation = new Vector2D(0, 1);
                    velocity = orientation * tankSpeed;
                    break;
                default:
                    if (theWorld.Tanks.ContainsKey((int)id))
                        orientation = theWorld.Tanks[(int)id].GetOrientation();
                    else
                        orientation = new Vector2D(0, -1);
                    velocity = new Vector2D(0, 0);
                    break;
            }
        }

        /// <summary>
        /// Method that reflects the location across the axis if the new location goes out of bounds
        /// </summary>
        /// <param name="oldLocation"></param>
        /// <param name="newLocation"></param>
        /// <returns></returns>
        private Vector2D Warp(Vector2D oldLocation, Vector2D newLocation)
        {
            // If the location is horizontally changing, reflect horizontally.
            if (oldLocation.GetX() > universeSize / 2 || oldLocation.GetX() < -(universeSize / 2))
                newLocation = new Vector2D(oldLocation.GetX() * -1, oldLocation.GetY());
            // If the location is vertically changing, reflect vertically.
            else if (oldLocation.GetY() > universeSize / 2 || oldLocation.GetY() < -(universeSize / 2))
                newLocation = new Vector2D(oldLocation.GetX(), oldLocation.GetY() * -1);
            return newLocation;
        }

        /// <summary>
        /// Method to be called when bulletHell is on it does the calculations for projectiles to bounce of walls
        /// </summary>
        /// <param name="p"></param>
        /// <param name="newLocation"></param>
        private void BulletHellBounceCalculation(Projectile p, Vector2D newLocation)
        {
            bool alreadyBounced = false;
            //projectile-wall collision for bullet hell mode.
            foreach (Wall w in theWorld.Walls.Values)
            {
                //detects whether a projectile collides with a wall.
                if (DetectWallCollision(w, newLocation, p))
                {
                    Vector2D currDirection = theWorld.Projectiles[p.GetID()].GetDirection();
                    //determine whether the wall is a vertical.
                    if (w.GetP1().GetY() != w.GetP2().GetY() && !alreadyBounced)
                    {
                        // determine where the wall is being hit, so it can be bounced correctly.
                        if (p.GetLocation().GetX() > w.GetP1().GetX() - (wallSize / 2) && p.GetLocation().GetX() < w.GetP1().GetX() + (wallSize / 2) && p.GetLocation().GetY() < w.GetP1().GetY())
                        {
                            Vector2D wallN = new Vector2D(0, 1);
                            Vector2D wallNDoubled = new Vector2D(0, 2);
                            Vector2D newDirection = currDirection - wallNDoubled * currDirection.GetY();
                            p.SetDirection(newDirection);
                        }
                        else if (p.GetLocation().GetX() > w.GetP2().GetX() - (wallSize / 2) && p.GetLocation().GetX() < w.GetP2().GetX() + (wallSize / 2) && p.GetLocation().GetY() > w.GetP2().GetY())
                        {
                            Vector2D wallN = new Vector2D(0, 1);
                            Vector2D wallNDoubled = new Vector2D(0, 2);
                            Vector2D newDirection = currDirection - wallNDoubled * currDirection.GetY();
                            p.SetDirection(newDirection);
                        }
                        else
                        {
                            Vector2D wallN = new Vector2D(1, 0);
                            Vector2D wallNDoubled = new Vector2D(2, 0);
                            Vector2D newDirection = currDirection - wallNDoubled * currDirection.GetX();
                            p.SetDirection(newDirection);
                        }
                        alreadyBounced = true;
                    }
                    //determine whether the wall is horizontal.
                    if (w.GetP1().GetX() != w.GetP2().GetX() && !alreadyBounced)
                    {
                        //determine where the wall is being hit so that it can be bounced correctly.
                        if (p.GetLocation().GetY() > w.GetP1().GetY() - (wallSize / 2) && p.GetLocation().GetY() < w.GetP1().GetY() + (wallSize / 2) && p.GetLocation().GetX() < w.GetP1().GetX())
                        {
                            Vector2D wallN = new Vector2D(1, 0);
                            Vector2D wallNDoubled = new Vector2D(2, 0);
                            Vector2D newDirection = currDirection - wallNDoubled * currDirection.GetX();
                            p.SetDirection(newDirection);
                        }
                        else if (p.GetLocation().GetY() > w.GetP2().GetY() - (wallSize / 2) && p.GetLocation().GetY() < w.GetP2().GetY() + (wallSize / 2) && p.GetLocation().GetX() > w.GetP2().GetX())
                        {
                            Vector2D wallN = new Vector2D(1, 0);
                            Vector2D wallNDoubled = new Vector2D(2, 0);
                            Vector2D newDirection = currDirection - wallNDoubled * currDirection.GetX();
                            p.SetDirection(newDirection);
                        }
                        else
                        {
                            Vector2D wallN = new Vector2D(0, 1);
                            Vector2D wallNDoubled = new Vector2D(0, 2);
                            Vector2D newDirection = currDirection - wallNDoubled * currDirection.GetY();
                            p.SetDirection(newDirection);
                        }
                        alreadyBounced = true;
                    }
                    p.TickBounces();
                }
            }
        }

        #region Collision Detection
        /// <summary>
        /// Method to detect if a projectile collided with a tank
        /// If a projectile collides with a tank
        /// </summary>
        /// <param name="proj"></param>
        /// <param name="tank"></param>
        /// <returns>
        /// True if there was a collision
        /// False if there was not
        /// </returns>
        private bool DetectProjectileTankCollision(Projectile proj, Tank tank)
        {
            // Calculates distance vector between tank and proj
            Vector2D distanceVector = tank.GetLocation() - proj.GetLocation();

            // If distance is less than or equal to the size of the tank then projectile hit
            if (distanceVector.Length() <= tankSize)
                return true;

            return false;
        }

        /// <summary>
        /// Method to detect if a tank and powerup collided
        /// Meaning that the tank drove over the powerup and picked it up
        /// Adds one alternate ammo to the tank's ammo counter
        /// Then deactivates the powerup
        /// </summary>
        /// <param name="power"></param>
        /// <param name="tank"></param>
        /// <returns>
        /// True if there was a collision
        /// False if there was not
        /// </returns>
        private bool DetectTankPowerupCollision(Powerup power, Tank tank)
        {
            // calculates distance vector between powerup and location
            Vector2D distanceVector = tank.GetLocation() - power.GetLocation();

            // if distance is less than or equal to the size of the tank then they collided
            if (distanceVector.Length() <= tankSize)
                return true;

            return false;
        }

        /// <summary>
        /// Method to generate a Vector2D that represents a location inside the universe
        /// that does not collide with any walls
        /// </summary>
        /// <param name="o"></param>
        /// <returns>
        /// Vector2D represents the location
        /// </returns>
        private Vector2D GenerateSpawnLocation(object o)
        {
            int locationX = rand.Next(-universeSize / 2, universeSize / 2);
            int locationY = rand.Next(-universeSize / 2, universeSize / 2);
            Vector2D location = new Vector2D(locationX, locationY);
            // loops through all the walls once, checking if spawn location is in a wall.
            foreach (Wall w1 in theWorld.Walls.Values)
            {
                //loops through all the walls again and sees if spawn location is in a wall.
                foreach (Wall w2 in theWorld.Walls.Values)
                {
                    // If its in the second wall then generate a new location
                    if (DetectWallCollision(w2, location, o))
                    {
                        locationX = rand.Next(-universeSize / 2, universeSize / 2);
                        locationY = rand.Next(-universeSize / 2, universeSize / 2);
                        location = new Vector2D(locationX, locationY);
                    }
                }
                // If its in the first wall then generate a new location
                if (DetectWallCollision(w1, location, o))
                {
                    locationX = rand.Next(-universeSize / 2, universeSize / 2);
                    locationY = rand.Next(-universeSize / 2, universeSize / 2);
                    location = new Vector2D(locationX, locationY);
                }
            }

            return location;
        }

        /// <summary>
        /// Method to detect if a location and object type collides with a wall
        /// </summary>
        /// <param name="w"></param>
        /// <param name="location"></param>
        /// <param name="o"></param>
        /// <returns>
        /// True if the object type and location collides with a wall
        /// False if they do not
        /// </returns>
        private bool DetectWallCollision(Wall w, Vector2D location, object o)
        {
            // These are the points of the wall's rectangle.
            double topLeftWallX = 0;
            double topLeftWallY = 0;
            double bottomRightWallX = 0;
            double bottomRightWallY = 0;
            // x and y values of the points for the wall.
            double wallp1x = w.GetP1().GetX();
            double wallp2x = w.GetP2().GetX();
            double wallp1y = w.GetP1().GetY();
            double wallp2y = w.GetP2().GetY();
            // x and y values of the points for the collision rectangle.
            double topLeftCollisionX = 0;
            double topLeftCollisionY = 0;
            double bottomRightCollisionX = 0;
            double bottomRightCollisionY = 0;

            if (wallp1x < wallp2x || wallp1y < wallp2y)
            {
                // top left corner of wall
                topLeftWallX = wallp1x - (wallSize / 2);
                topLeftWallY = wallp1y - (wallSize / 2);
                // bottom right corner of wall
                bottomRightWallX = wallp2x + (wallSize / 2);
                bottomRightWallY = wallp2y + (wallSize / 2);
            }
            else if (wallp2x < wallp1x || wallp2y < wallp1y)
            {
                // top left corner of wall
                topLeftWallX = wallp2x - (wallSize / 2);
                topLeftWallY = wallp2y - (wallSize / 2);
                // bottom right corner of wall
                bottomRightWallX = wallp1x + (wallSize / 2);
                bottomRightWallY = wallp1y + (wallSize / 2);
            }

            // collision rectangle
            if (o is Tank || o is Powerup)
            {
                topLeftCollisionX = topLeftWallX - (tankSize / 2);
                topLeftCollisionY = topLeftWallY - (tankSize / 2);

                bottomRightCollisionX = bottomRightWallX + (tankSize / 2);
                bottomRightCollisionY = bottomRightWallY + (tankSize / 2);
            }
            else if (o is Projectile)
            {
                topLeftCollisionX = topLeftWallX;
                topLeftCollisionY = topLeftWallY;

                bottomRightCollisionX = bottomRightWallX;
                bottomRightCollisionY = bottomRightWallY;
            }

            // x,y points of location
            double pointX = location.GetX();
            double pointY = location.GetY();

            // rectangle collision detection
            // check range of x values first
            if (pointX > topLeftCollisionX && pointX < bottomRightCollisionX)
            {
                // check range of y values if x is in the range of x values
                if (pointY > topLeftCollisionY && pointY < bottomRightCollisionY)
                    return true;
            }
            // check range of y values second
            else if (pointY > topLeftCollisionY && pointY < bottomRightCollisionY)
            {
                // check range of x if y is in the range of y values
                if (pointX > topLeftCollisionX && pointX < bottomRightCollisionX)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Method that checks if a location is out of the universe boundaries
        /// </summary>
        /// <param name="location"></param>
        /// <returns>
        /// True if the location is outside the universe
        /// False if location is inside the universe
        /// </returns>
        private bool DetectOutOfBounds(Vector2D location)
        {
            //if the x value is less than or greater than the bounds of the universe, return true.
            if (location.GetX() < -(universeSize / 2) || location.GetX() > (universeSize / 2))
                return true;
            //if the y value is less than or greater than the bounds of the universe, return true.
            if (location.GetY() < -(universeSize / 2) || location.GetY() > (universeSize / 2))
                return true;

            return false;
        }

        /// <summary>
        /// Provided Code
        /// Determines if a ray interescts a circle
        /// </summary>
        /// <param name="rayOrig">The origin of the ray</param>
        /// <param name="rayDir">The direction of the ray</param>
        /// <param name="center">The center of the circle</param>
        /// <param name="r">The radius of the circle</param>
        /// <returns></returns>
        public static bool Intersects(Vector2D rayOrig, Vector2D rayDir, Vector2D center, double r)
        {
            // ray-circle intersection test
            // P: hit point
            // ray: P = O + tV
            // circle: (P-C)dot(P-C)-r^2 = 0
            // substituting to solve for t gives a quadratic equation:
            // a = VdotV
            // b = 2(O-C)dotV
            // c = (O-C)dot(O-C)-r^2
            // if the discriminant is negative, miss (no solution for P)
            // otherwise, if both roots are positive, hit

            double a = rayDir.Dot(rayDir);
            double b = ((rayOrig - center) * 2.0).Dot(rayDir);
            double c = (rayOrig - center).Dot(rayOrig - center) - r * r;

            // discriminant
            double disc = b * b - 4.0 * a * c;

            if (disc < 0.0)
                return false;

            // find the signs of the roots
            // technically we should also divide by 2a
            // but all we care about is the sign, not the magnitude
            double root1 = -b + Math.Sqrt(disc);
            double root2 = -b - Math.Sqrt(disc);

            return (root1 > 0.0 && root2 > 0.0);
        }
        #endregion

        #region Game Helper Methods
        /// Helper method for when a Beam hits a Tank
        /// Subtract the number of damage points a beam does from the tank's health points
        /// Check if the tank died by seeing if its health points are 0
        /// Kill the tank by setting it to be dead and the frame it was killed on
        /// If it died then add the number of points for a projectile kill
        /// <param name="beam"></param>
        /// <param name="tank"></param>
        private void BeamHitTank(Beam beam, Tank tank)
        {
            // Decrease this tank's HP
            // tanks cannot have less 0 HP
            if (tank.GetHp() - beamDamagePoints < 0)
                tank.SetHp(0);
            else
                tank.SetHp(tank.GetHp() - beamDamagePoints);

            // Increase score if beam destroys the tank
            if (tank.GetHp() == 0)
            {
                KillTank(tank);
                AddPlayerScore(beam.GetOwner(), beamKillPoints);
            }
        }

        /// <summary>
        /// Helper method for when a Projectile hits a Tank
        /// Subtract the number of damage points a projectile from the tank's health points
        /// Check if the tank died by seeing if its health points are 0
        /// Kill the tank by setting it to be dead and the frame it was killed on
        /// If it died then add the number of points for a projectile kill
        /// </summary>
        /// <param name="proj"></param>
        /// <param name="tank"></param>
        private void ProjectileHitTank(Projectile proj, Tank tank)
        {
            // Projectile hit so deactivate it and add to our dead projectiles
            DeactivateProjectile(proj);

            // Decrease this tank's HP
            // tanks cannot have less 0 HP
            if (tank.GetHp() - projectileDamagePoints < 0)
                tank.SetHp(0);
            else
                tank.SetHp(tank.GetHp() - projectileDamagePoints);

            // Increase score if the projectile destroys tank
            if (tank.GetHp() == 0)
            {
                KillTank(tank);
                AddPlayerScore(proj.GetOwner(), projectileKillPoints);
            }
        }

        /// <summary>
        /// Helper method that deactivates a projectile
        /// And adds it the dictionary of dead projectiles
        /// </summary>
        /// <param name="proj"></param>
        private void DeactivateProjectile(Projectile proj)
        {
            proj.SetActive(false);
            theWorld.DeadProjectiles[proj.GetID()] = proj;
        }

        /// <summary>
        /// Helper method that adds a given amount alternate ammo to a tank's ammo count
        /// </summary>
        /// <param name="tank"></param>
        private static void AddAlternateAmmo(Tank tank, int amount)
        {
            tank.SetAltAmmo(tank.GetAltAmmo() + amount);
        }

        /// <summary>
        /// Helper method that removes a given amount alternate ammo to a tank's alternate ammo count
        /// Tank's alternate ammo count cannot be less 0
        /// </summary>
        /// <param name="tank"></param>
        /// <param name="amount"></param>
        private void RemoveAlternateAmmo(Tank tank, int amount)
        {
            if (tank.GetAltAmmo() - amount < 0)
                tank.SetAltAmmo(0);
            else
                tank.SetAltAmmo(tank.GetAltAmmo() - amount);
        }

        /// <summary>
        /// Helper method that deactivates a powerup
        /// And adds it the dictionary of dead powerups
        /// </summary>
        /// <param name="proj"></param>
        private void DeactivatePowerup(Powerup power)
        {
            power.SetActive(false);
            theWorld.DeadPowerups[power.GetID()] = power;
        }

        /// <summary>
        /// Increments the player's score by a given amount
        /// </summary>
        /// <param name="shooterID"></param>
        private void AddPlayerScore(int shooterID, int amount)
        {
            // Tanks can be killed at time resulting in tanks being removed or added
            lock (theWorld)
            {
                // Check if the tank that shot is alive
                if (theWorld.Tanks.ContainsKey(shooterID))
                    theWorld.Tanks[shooterID].SetScore(theWorld.Tanks[shooterID].GetScore() + amount);

                // Checks if the tank that shot was dead
                else if (theWorld.DeadTanks.ContainsKey(shooterID))
                    theWorld.DeadTanks[shooterID].SetScore(theWorld.DeadTanks[shooterID].GetScore() + amount);
            }
        }

        /// <summary>
        /// Helper method that sets a tank to be dead
        /// </summary>
        /// <param name="tank">Tank that is to be killed</param>
        private void KillTank(Tank tank)
        {
            tank.SetHp(0);
            tank.SetIsDead(true);
            tank.SetFrameDied(numOfFrames);
        }
        #endregion

        #region Clients Modification
        /// <summary>
        /// Adds a client to the clients dictionary
        /// In a thread safe manner
        /// </summary>
        /// <param name="id">The ID of the client</param>
        /// <param name="state">The SocketState of the client</param>
        private void AddClient(long id, SocketState state)
        {
            // Save the client state
            // Need to lock here because clients can disconnect at any time
            lock (connectedClients)
            {
                connectedClients[id] = state;
            }
        }

        /// <summary>
        /// Removes a client from the clients dictionary
        /// In a thread safe manner
        /// </summary>
        /// <param name="id">The ID of the client</param>
        private void RemoveClient(long id)
        {
            // Lock here because clients can connect at any time
            lock (connectedClients)
            {
                // try to remove client from clients dictionary
                if (connectedClients.Remove(id))
                    // if successful print out that the client disconnected
                    Console.WriteLine("Client " + id + " disconnected");
            }
        }

        /// <summary>
        /// Adds a disconnected client to hashset of disconnect clients
        /// In a thread safe manner
        /// </summary>
        /// <param name="id">The ID of the client</param>
        private void AddDisconnectedClient(long id)
        {
            lock (disconnectedClients)
            {
                disconnectedClients.Add(id);
            }
        }
        #endregion
    }
}