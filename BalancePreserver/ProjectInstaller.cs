using System.ComponentModel;
using System.Configuration.Install;

namespace BalancePreserver
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }
    }
}
