using Microsoft.EntityFrameworkCore;
using System.Windows;
using WMS_Suite.DataAccess;

namespace WMS_Suite
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            using (var context = new AppDbContext())
            {
                context.Database.Migrate(); // Creates the DB and tables if they don't exist
            }
        }
    }
}
