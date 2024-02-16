This is a client for the Tank Wars game.

4/23/2021
Features:
+ Functioning Tank Wars Game server
+ Allows for multiple Tank Wars game clients to connect asynchronously.
+ Contains a special game mode titled "Bullet Hell" that can be toggled on and off via the settings file.
+ While Bullet Hell is toggled Projectiles will bounce off of walls 3 times. This can be changed via the XML settings file.
+ Projectiles will also warp to the other side of the map if they go out of bounds while Bullet Hell is on.
+ Look inside the XMl settings file to see what settings can be customized.

Implementation:
+ The server follows MVC architecture:
We separate the Game Server (controller) logic from the world the same as the client. Now, there is additional functionality in the model.
Our view is a console that prints the FPS at which the server is sending to the clients. It also prints whenever a client connects and disconnects.
Our game world now has an additional 4 dictionairies to keep track of dead objects.

+ Server uses custom networking to communicate with the clients.
By following the Network protocol that uses JSON to send and recieve data over the network
Any information sent by the either the client or server are terminated by '\n'
The inital handshake must be completed before the client can connect to the game. The client must send their name terminated by a new line
before the server will send them their player ID and the universe size. Then, the server will send walls as well as the rest of the objects.
If the process hasn't started the server won't send the client anything until the process is completed.
If the handshake is not completed and/or the client sends us bad/malformed data then the server will disconnect them from the game.

+ Now uses a lot of getter and setter methods to follow MVC architecture and to allow our controller to update the game world.

+ Protects critical sections via locks to prevent race conditions, such as the world being modified by one thread while another thread is accessing it.

+ Use an XML reader to read through a settings file and load in the settings and the walls.
If no settings are found, they will default to theWorld's Constants values.
If no walls are found, the world will have no walls.

+ Uses the provided beam code for beam-tank collision.

+ Generates random spawn location by generating a random point in the world and checking if it collides with any walls.


Problems:

+ There might be some unaccounted for race conditions due to threads accessing things while they're being modified
that we did not find in our testing.

+ Epilepsy Problem: if a tank repeatedly warps at the edge of the world the client will display rapid flashing between the 2 warp locations.

Design Decisions:

+Went with the Bullet Hell game mode because bouncing projectiles off of walls allows for more creative and fun ways to destroy other tanks.

+Projectile Warping adds chaos to go with the theme of Bullet Hell.

+ We made all the settings customizable in the XML file to allow the game to be played a variety of ways and if one of the settings isn't found, we just use a default
because the game cannot run without each of them having an associated value.

+ We use our own networking library because we passed all the grading tests and didn't find anything wrong with it.

+ The game logic code uses a lot of helper methods to make reading the code easier and allows us to use already completed functionalities elsewhere.

+ Our UpdateGameWorld function loops through every object and might loop through another dictionary object because this was recommended by Professor Kopta

+ We have an empty while loop because we need the server to send game updates at a specific interval. This is so that the server doesn't get ahead of clients
and clients send more information than the server can handle.

+ We disconnect players when they send malformed data because we couldn't figure out another way. We disconnect them to stop them from sending us something bad and destroying the whole server.
They could still reconnect if they wanted, as we have no other safety features to prevent this.

Authors:
Mason Seppi
William Nguyen

4/9/2021
Features:
+ Custom Tank death explosion and Beam attack animations
+ Custom Powerup look
+ Dynamic Health Bars that change when Tank's health points get lower
+ Client FPS Counter (Toggled by the 'F' Key)
+ Can quit the Client Applicaition by pressing the 'Q' key

Implementation:
+Follows MVC (Model View Controller) architecture:
By separating the drawing of the game concerns and other GUI things like player name input and server address input to the form (View).
It also passes user inputs to the controller (Controller) so that they can be process according and trigger the correct events.
The controller processes information received from the server and updates the the game world (Model).
The game world has 5 dictionaries of objects being: Tank, Wall, Projectile, Beam, Powerup.

+Uses custom Networking Library that we also wrote to communicate from client to server:
By following the Network protocol that uses JSON to send and recieve data over the network
Any information sent by the either the client or server are terminated by '\n'

+ Uses a lot of getter methods because of MVC architecture

+ Locking of critical sections in drawing and updating the world:
Preventing modification of the dictionaries while it is being iterated over.

+ Uses MethodInvokers to prevent Cross Threaded Operations

Problems:
+ Semi fluid tank movement.
+ Client's performance at drawing with lots of players is slow due to drawing the entire world on every frame.
+ Cannot easily reconnect or connect to another server after clicking the connect button.

Design Decisions:
+ Animations are relatively simple and not very complex due to time constraints and unfamiliarity with drawing them
+ Tank movement is not 100% fluid because of time constraints and not knowing how properly implement it
+ Added Client FPS Counter used for debugging reasons when testing Client's performance compared to the Server's
+ Added 'Q' key quit functionality so that quitting out of the game was easier to do
+ Uses provided Game Assets for ease of implementation
+ Client is not exactly one to one in feature set of the provided Client due to time constraints and lack of knowledge on certain features
+ Client draws game world centered on the player because the player cannot move the game view around

Authors:
+ Mason Seppi
+ William Nguyen