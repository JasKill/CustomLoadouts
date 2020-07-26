using Exiled.API.Interfaces;
using System.ComponentModel;

namespace CustomLoadouts
{
    public class Config : IConfig
    {
        [Description("Wether or not debug messages should be shown.")]
        public bool IsEnabled { get; set; } = false;
        [Description("Whether to use the global config folder or not, defaults to true..")]
        public bool Cl_global { get; set; } = true;
    }
}
