using Exiled.Events.EventArgs;
using Exiled.Permissions.Extensions;

namespace CustomLoadouts.EventHandlers
{
    public class ServerHandlers
    {
        private readonly Plugin plugin;
        public ServerHandlers(Plugin plugin) => this.plugin = plugin;

        public void OnRACommand(SendingRemoteAdminCommandEventArgs ev)
        {
            if (ev.Name.Equals("cl_reload"))
            {
                ev.IsAllowed = false;
                if (ev.Sender.CheckPermission("customloadouts.reload"))
                {
                    plugin.Reload();
                    ev.Sender.RemoteAdminMessage("CustomLoadouts#CustomLoadouts has been reloaded.");
                }
                else
                {
                    ev.Sender.RemoteAdminMessage("CustomLoadouts#You don't have permission to use that command.");
                }
            }
        }
    }
}
