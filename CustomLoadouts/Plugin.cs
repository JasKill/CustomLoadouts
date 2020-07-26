using CustomLoadouts.EventHandlers;
using CustomLoadouts.Properties;
using Exiled.API.Enums;
using Exiled.API.Features;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text;
using Server = Exiled.Events.Handlers.Server;
using Player = Exiled.Events.Handlers.Player;
using YamlDotNet.Serialization;

namespace CustomLoadouts
{
    public class Plugin : Plugin<Config>
    {
        public override string Name { get; } = "CustomLoadouts";
        public override string Author { get; } = "KarlEssinger, porting Killers0992, changed JasKill";
        public override Version Version { get; } = new Version(2, 0, 1);
        public override Version RequiredExiledVersion { get; } = new Version(2, 0, 7);
        public override string Prefix { get; } = "CustomLoadouts";
        public override PluginPriority Priority { get; } = PluginPriority.Default;

        public ServerHandlers ServerHandlers;
        public PlayerHandlers PlayerHandlers;
        public static Plugin Singleton;

        public static string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string PluginDirectory = Path.Combine(appData, "EXILED", "Plugins", "CustomLoadouts");
        public bool debug;
        public bool verbose;

        public override void OnEnabled()
        {
            Singleton = this;

            if (!Directory.Exists(PluginDirectory))
            {
                Directory.CreateDirectory(PluginDirectory);
            }
            if (!File.Exists(Path.Combine(PluginDirectory, "config.yml")))
            {
                File.WriteAllText(Path.Combine(PluginDirectory, "config.yml"), Encoding.UTF8.GetString(Resources.config));
            }
            Log.Info("Plugin CustomLoadouts started.");

            RegisterEvents();
            Reload();
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            UnregisterEvents();
            base.OnDisabled();
        }

        public override void OnReloaded() => Log.Info($"Plugin {Name} has been reloaded!");

        public void RegisterEvents()
        {
            ServerHandlers = new ServerHandlers(this);
            PlayerHandlers = new PlayerHandlers(this);

            Server.SendingRemoteAdminCommand += ServerHandlers.OnRACommand;
            Player.ChangingRole += PlayerHandlers.OnChangingRole;
        }

        public void UnregisterEvents()
        {
            Server.SendingRemoteAdminCommand -= ServerHandlers.OnRACommand;
            Player.ChangingRole -= PlayerHandlers.OnChangingRole;

            PlayerHandlers = null;
            ServerHandlers = null;
        }

        public void Reload()
        {
            string path = Config.Cl_global ? Path.Combine(PluginDirectory, "config.yml") : Path.Combine(PluginDirectory, ServerConsole.Port.ToString(), "config.yml");
            Log.Info("Loading config " + path + "...");

            if (!Config.Cl_global)
            {
                if (!Directory.Exists(Path.Combine(PluginDirectory, ServerConsole.Port.ToString())))
                {
                    Directory.CreateDirectory(Path.Combine(PluginDirectory, ServerConsole.Port.ToString()));
                }
            }

            if (!File.Exists(path))
            {
                File.WriteAllText(path, Encoding.UTF8.GetString(Resources.config));
            }

            // Reads file contents into FileStream
            FileStream stream = File.OpenRead(path);

            // Converts the FileStream into a YAML Dictionary object
            IDeserializer deserializer = new DeserializerBuilder().Build();
            object yamlObject = deserializer.Deserialize(new StreamReader(stream));

            // Converts the YAML Dictionary into JSON String
            ISerializer serializer = new SerializerBuilder().JsonCompatible().Build();
            string jsonString = serializer.Serialize(yamlObject);

            JObject json = JObject.Parse(jsonString);

            // Sets config variables
            debug = json.SelectToken("debug").Value<bool>();
            verbose = json.SelectToken("verbose").Value<bool>();

            PlayerHandlers.loadouts = json.SelectToken("customloadouts").Value<JObject>();
            Log.Info("Config loaded.");
        }
    }
}
