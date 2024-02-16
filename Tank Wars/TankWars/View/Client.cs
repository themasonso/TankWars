using Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
    /// Form class that creates the windows Form
    /// </summary>
    public partial class Client : Form
    {
        // Controller that handles user input and data transfer between server and client
        private GameController theController;
        // Reference to the controller's game world so that it can be passed to the drawingPanel for drawing
        private World theWorld;
        // Custom Panel that draws the game images
        private DrawingPanel drawingPanel;
        // Button for trying to connect to a server
        private Button startButton;
        // The player's name
        private Label nameLabel;
        private TextBox nameText;
        // The server's address
        private Label serverLabel;
        private TextBox serverText;
        // Help button explaing controls of game and authors of Client
        private Button helpButton;

        // Constants that define the Client's window size and menu size
        private const int viewSize = 800;
        private const int menuSize = 40;

        public Client(GameController ctrl)
        {
            InitializeComponent();
            // Initialize View variables
            theController = ctrl;
            ClientSize = new Size(viewSize, viewSize + menuSize);
            theWorld = theController.GetWorld();
            // Registers a handler to the controller's UpdateArrived event
            theController.UpdateArrived += OnFrame;

            #region form control
            // Defines a label for the server input
            serverLabel = new Label();
            serverLabel.Text = "server:";
            serverLabel.Location = new Point(5, 10);
            serverLabel.Size = new Size(40, 15);
            this.Controls.Add(serverLabel);

            // Defines a textbox for the server input
            serverText = new TextBox();
            serverText.Text = "localhost";
            serverText.Location = new Point(50, 5);
            serverText.Size = new Size(70, 15);
            this.Controls.Add(serverText);

            // Defines a label for the player's name input
            nameLabel = new Label();
            nameLabel.Text = "name:";
            nameLabel.Location = new Point(135, 10);
            nameLabel.Size = new Size(40, 15);
            this.Controls.Add(nameLabel);

            // Defines a textbox for the player's name
            nameText = new TextBox();
            nameText.Text = "player";
            nameText.Location = new Point(180, 5);
            nameText.Size = new Size(70, 15);
            this.Controls.Add(nameText);

            // Defines a button to allow the user to try to connect to the server entered
            startButton = new Button();
            startButton.Text = "connect";
            startButton.Location = new Point(255, 5);
            startButton.Size = new Size(70, 20);
            startButton.Click += startConnection;
            this.Controls.Add(startButton);

            // Defines a button to allow the user to see help information about the client like game cotnrols and authors
            helpButton = new Button();
            helpButton.Text = "Help";
            helpButton.Size = new Size(70, 20);
            helpButton.Location = new Point(viewSize - helpButton.Size.Width, 5);
            helpButton.Click += DisplayHelp;
            this.Controls.Add(helpButton);
            #endregion

            // Registers a handler to the controller's Connected event when the client first connects to the server
            theController.Connected += HandleConnected;
            // Registers a handler to the controller's Error event when an error occurs in the controller to be display in the view
            theController.Error += HandleError;
            // Registers a handler to the controller's Handshaked event when the client was able to recieve a player id and world size
            theController.Handshaked += HandleHandshaked;
            // Registers a handler to the controller's Quit event when the client wants to quit the application
            theController.Quit += HandleQuit;

            // Defines a drawing panel so that game images can be drawn
            drawingPanel = new DrawingPanel(theWorld);
            drawingPanel.Location = new Point(0, menuSize);
            drawingPanel.Size = new Size(viewSize, viewSize);
            this.Controls.Add(drawingPanel);

            // Registers a handler to the controller's TankDied event so that the drawing panel can start drawing the death animation
            theController.TankDied += drawingPanel.AddTankDeathAnimation;
            // Registers a handler to the controller's BeamFired event so that the drawing panel can start drawing the beam animation
            theController.BeamFired += drawingPanel.AddBeamAnimation;
            // Registers a handler to the controller's DisplayFPS event so that the drawing panel can start drawing the Client's FPS
            theController.DisplayFPS += drawingPanel.DisplayFPSCounter;

            // Registers a handler to the drawing panel's MouseDown event so that the controller knows that a mouse button was down
            drawingPanel.MouseDown += HandleMouseDown;
            // Registers a handler to the drawing panel's MouseUp event so that the controller knows that a mouse button was up
            drawingPanel.MouseUp += HandleMouseUp;
            // Registers a handler to the drawing panel's MouseMove event so that the controller knows that the mouse was moved 
            drawingPanel.MouseMove += HandleMouseMoved;

            // Registers a handler to the form's KeyDown event so that the controller knows that a key was down
            this.KeyDown += HandleKeyDown;
            // Register a handler to the form's KeyUp even so that the controller knows that a key was up
            this.KeyUp += HandleKeyUp;
        }

        /// <summary>
        /// This method is invoked when an update has arrived and been processed by the server
        /// It invalidates the form so that it is drawn as fast as possible
        /// </summary>
        private void OnFrame()
        {
            try
            {
                Invoke(new MethodInvoker(() => this.Invalidate(true)));
            }
            catch
            {
            }
        }

        /// <summary>
        /// Method that handles when a mouse is moved and informs the controller that it was moved
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMouseMoved(object sender, MouseEventArgs e)
        {
            int halfView = viewSize / 2;
            int mouseX = e.X - halfView;
            int mouseY = e.Y - halfView;
            theController.HandleMouseMovedRequest(new Point(mouseX, mouseY));
        }

        /// <summary>
        /// Method that handles when a mouse button is up and informs the controller that a mouse button is up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                theController.CancelMouseRequest("left");
            if (e.Button == MouseButtons.Right)
                theController.CancelMouseRequest("right");
        }

        /// <summary>
        /// Method that handles when a mouse button is down and informs the controller that a mouse button is down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                theController.HandleMouseRequest("left");
            if (e.Button == MouseButtons.Right)
                theController.HandleMouseRequest("right");
        }

        /// <summary>
        /// Method that handles when a key is up and informs the controller that a key is up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
                theController.CancelMoveRequest('W');
            if (e.KeyCode == Keys.A)
                theController.CancelMoveRequest('A');
            if (e.KeyCode == Keys.S)
                theController.CancelMoveRequest('S');
            if (e.KeyCode == Keys.D)
                theController.CancelMoveRequest('D');
        }

        /// <summary>
        /// Method that handles when a key is down and informs the controller that a key is down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
                theController.HandleMoveRequest('W');
            if (e.KeyCode == Keys.A)
                theController.HandleMoveRequest('A');
            if (e.KeyCode == Keys.S)
                theController.HandleMoveRequest('S');
            if (e.KeyCode == Keys.D)
                theController.HandleMoveRequest('D');
            if (e.KeyCode == Keys.F)
                theController.DisplayFPSCounter();
            if (e.KeyCode == Keys.Q)
                theController.HandleQuit();

            e.SuppressKeyPress = true;
            e.Handled = true;
        }

        /// <summary>
        /// Method that handles when an Error event is fired from the controller
        /// It displays the error inside a MessageBox
        /// </summary>
        /// <param name="error"></param>
        private void HandleError(string error)
        {
            MessageBox.Show(error);
        }

        /// <summary>
        /// Method that handles when the Connected event is fired from the controller
        /// It sends the Player's name to the server from the controller
        /// </summary>
        private void HandleConnected()
        {
            theController.SendName(nameText.Text);
        }

        /// <summary>
        /// Method that handles when the Handshaked event is fired from the controller
        /// It sets the view's world to be a reference from the game controller's world
        /// Sets the drawing panel's world to be a reference from the same world
        /// And sets the drawing panel's player ID to be the one from the controller 
        /// </summary>
        private void HandleHandshaked()
        {
            theWorld = theController.GetWorld();
            drawingPanel.SetWorld(theWorld);
            drawingPanel.SetID(theController.GetID());
        }

        /// <summary>
        /// Method that handles when the Quit event is fired from the controller
        /// It exits the application
        /// </summary>
        private void HandleQuit()
        {
            Application.Exit();
        }

        /// <summary>
        /// Method that is registered to the startButton
        /// It checks the user's name and server address to make sure they aren't empty
        /// Disables editing of the textboxes and disables the button
        /// Sends the controller the player's name and server address
        /// So that it can start the connection process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startConnection(object sender, EventArgs e)
        {
            if (serverText.Text == "")
            {
                MessageBox.Show("Enter a server.");
                return;
            }
            if (nameText.Text == "")
            {
                MessageBox.Show("Enter a name.");
                return;
            }
            startButton.Enabled = false;
            serverText.Enabled = false;
            nameText.Enabled = false;
            KeyPreview = true;

            theController.Connect(serverText.Text);
        }

        /// <summary>
        /// Method that is registered to the helpButton
        /// It displays a string that details the controls of the game
        /// and the authors at the bottom
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisplayHelp(object sender, EventArgs e)
        {
            MessageBox.Show("Controls:\n" +
                "W: Move Up\n" +
                "A: Move Left\n" +
                "S: Move Down\n" +
                "D: Move Right\n" +
                "Mouse: Aim\n" +
                "Left Click: Fire Projectile\n" +
                "Right Click: Fire Beam\n" +
                "F: Show FPS\n" +
                "Q: Quit\n" +
                "\nAuthors:\n" +
                "Mason Seppi and William Nguyen"
                );
        }
    }
}
