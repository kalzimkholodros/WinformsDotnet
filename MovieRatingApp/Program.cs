using System;
using System.Windows.Forms;
using MovieRatingApp.Forms;
using MovieRatingApp.Services;

namespace MovieRatingApp
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static async Task Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var mongoService = await MongoDBService.CreateAsync();
            Application.Run(new LoginForm(mongoService));
        }    
    }
}