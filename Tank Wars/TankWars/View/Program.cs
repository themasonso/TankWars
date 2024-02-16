using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Controller;

// Author: Mason Seppi and William Nguyen
// University of Utah

namespace View
{
    /// <summary>
    /// Starts the Tank Wars' Client
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// Creates a controller that is then passed to the client
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            GameController controller = new GameController();
            Client form = new Client(controller);
            Application.Run(form);
        }
    }
}
