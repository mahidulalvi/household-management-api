using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(HouseholdManagementAPI.Startup))]

namespace HouseholdManagementAPI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
