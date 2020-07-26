using Exiled.Events.EventArgs;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Exiled.Permissions.Extensions;
using System.Linq;
using System;
using Exiled.API.Features;
using MEC;

namespace CustomLoadouts.EventHandlers
{
    public class PlayerHandlers
    {
        private readonly Plugin plugin;
        public PlayerHandlers(Plugin plugin) => this.plugin = plugin;

        internal HashSet<string> spawning = new HashSet<string>();
        internal JObject loadouts;
        internal Random rnd = new Random();

        public void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (!spawning.Contains(ev.Player.UserId))
            {
                spawning.Add(ev.Player.UserId);
                try
                {
                    JProperty[] permissionNodes = loadouts.Properties().ToArray();
                    foreach (JProperty permissionNode in permissionNodes)
                    {
                        if (ev.Player.CheckPermission("customloadouts." + permissionNode.Name))
                        {
                            try
                            {
                                JProperty[] roles = permissionNode.Value.Value<JObject>().Properties().ToArray();
                                foreach (JProperty role in roles)
                                {
                                    if (ev.NewRole.ToString().ToUpper() == role.Name.ToUpper() || role.Name.ToUpper() == "ALL")
                                    {
                                        try
                                        {
                                            foreach (JObject itemGroupNode in role.Value.Children())
                                            {
                                                // Converts the JObject to key/value pair
                                                JProperty itemGroup = itemGroupNode.Properties().First();

                                                // Attempts to parse the percentage chance from the config
                                                if (float.TryParse(itemGroup.Name, out float chance))
                                                {
                                                    // Rolls a D100
                                                    float d100 = rnd.Next(1, 10000) / 100;

                                                    // Success if dice roll is lower than the percentage chance
                                                    if (chance >= d100)
                                                    {
                                                        if (plugin.debug)
                                                        {
                                                            Log.Info(itemGroupNode.Path + ": Succeded random chance. " + chance + " >= " + d100);
                                                        }

                                                        // Gives all items in the item bundle to the player
                                                        foreach (string itemName in itemGroup.Value as JArray)
                                                        {
                                                            switch (itemName.ToUpper())
                                                            {
                                                                case "REMOVEAMMO":
                                                                    // Deletes the existing ammo if set in the config
                                                                    try
                                                                    {
                                                                        Timing.CallDelayed(1.5f, () =>
                                                                        {
                                                                            ev.Player.SetAmmo(Exiled.API.Enums.AmmoType.Nato556, 0);
                                                                            ev.Player.SetAmmo(Exiled.API.Enums.AmmoType.Nato762, 0);
                                                                            ev.Player.SetAmmo(Exiled.API.Enums.AmmoType.Nato9, 0);
                                                                        });
                                                                        if (plugin.verbose)
                                                                        {
                                                                            Log.Info("Cleared ammo of " + ev.NewRole + " " + ev.Player.Nickname + "(" + ev.Player.UserId + ").");
                                                                        }
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        Log.Error("Error occured while resetting ammo of " + ev.Player + ".");
                                                                        if (plugin.debug)
                                                                        {
                                                                            Log.Error(e.ToString());
                                                                        }
                                                                    }
                                                                    break;

                                                                case "REMOVEITEMS":
                                                                    // Deletes the existing items if set in the config
                                                                    try
                                                                    {
                                                                        ev.Items.Clear();

                                                                        if (plugin.debug)
                                                                        {
                                                                            Log.Info("Cleared inventory of " + ev.NewRole + " " + ev.Player.Nickname + "(" + ev.Player.UserId + ").");
                                                                        }
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        Log.Error("Error occured while resetting inventory of " + ev.Player + ".");
                                                                        if (plugin.debug)
                                                                        {
                                                                            Log.Error(e.ToString());
                                                                        }
                                                                    }
                                                                    break;

                                                                case "AMMO556":
                                                                    // Gives a mag of 5.56mm ammo
                                                                    try
                                                                    {
                                                                        Timing.CallDelayed(1.5f, () =>
                                                                        {
                                                                            uint amount = ev.Player.GetAmmo(Exiled.API.Enums.AmmoType.Nato556) + 25;
                                                                            ev.Player.SetAmmo(Exiled.API.Enums.AmmoType.Nato556, amount);
                                                                        });
                                                                        if (plugin.debug)
                                                                        {
                                                                            Log.Info(ev.NewRole + " " + ev.Player.Nickname + "(" + ev.Player.UserId + ") was given a mag of 5.56mm ammo (25 shots).");
                                                                        }
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        Log.Error("Error occured while giving a mag of 5.56mm ammo to " + ev.Player + ".");
                                                                        if (plugin.debug)
                                                                        {
                                                                            Log.Error(e.ToString());
                                                                        }
                                                                    }
                                                                    break;

                                                                case "AMMO762":
                                                                    // Gives a mag of 7.62mm ammo
                                                                    try
                                                                    {
                                                                        Timing.CallDelayed(1.5f, () =>
                                                                        {
                                                                            uint amount = ev.Player.GetAmmo(Exiled.API.Enums.AmmoType.Nato762) + 35;
                                                                            ev.Player.SetAmmo(Exiled.API.Enums.AmmoType.Nato762, amount);
                                                                        });
                                                                        if (plugin.debug)
                                                                        {
                                                                            Log.Info(ev.NewRole + " " + ev.Player.Nickname + "(" + ev.Player.UserId + ") was given a mag of 7.62mm ammo (35 shots).");
                                                                        }
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        Log.Error("Error occured while giving a mag of 7.62mm ammo to " + ev.Player + ".");
                                                                        if (plugin.debug)
                                                                        {
                                                                            Log.Error(e.ToString());
                                                                        }
                                                                    }
                                                                    break;

                                                                case "AMMO9MM":
                                                                    // Gives a clip of 9mm ammo
                                                                    try
                                                                    {
                                                                        Timing.CallDelayed(1.5f, () =>
                                                                        {
                                                                            uint amount = ev.Player.GetAmmo(Exiled.API.Enums.AmmoType.Nato9) + 15;
                                                                            ev.Player.SetAmmo(Exiled.API.Enums.AmmoType.Nato9, amount);
                                                                        });
                                                                        if (plugin.debug)
                                                                        {
                                                                            Log.Info(ev.NewRole + " " + ev.Player.Nickname + "(" + ev.Player.UserId + ") was given a clip of 9mm ammo (15 shots).");
                                                                        }
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        Log.Error("Error occured while giving a clip of 9mm ammo to " + ev.Player + ".");
                                                                        if (plugin.debug)
                                                                        {
                                                                            Log.Error(e.ToString());
                                                                        }
                                                                    }
                                                                    break;

                                                                default:
                                                                    // Parses the string to the enumerable itemtype
                                                                    try
                                                                    {
                                                                        ItemType item = (ItemType)Enum.Parse(typeof(ItemType), itemName);

                                                                        if (ev.Items.Count <= 7)
                                                                        {
                                                                            ev.Items.Add(item);
                                                                        }
                                                                        else
                                                                        {
                                                                            Timing.CallDelayed(0.5f, () => Exiled.API.Extensions.Item.Spawn(item, ItemDur(item), ev.Player.Position));
                                                                        }

                                                                        if (plugin.debug)
                                                                        {
                                                                            Log.Info(ev.NewRole + " " + ev.Player.Nickname + "(" + ev.Player.UserId + ") was given item " + itemName + ".");
                                                                        }
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        Log.Error("Error occured while giving item \"" + itemName + "\" to " + ev.Player + ".");
                                                                        if (plugin.debug)
                                                                        {
                                                                            Log.Error(e.ToString());
                                                                        }
                                                                    }
                                                                    break;
                                                            }
                                                        }
                                                    }
                                                    else if (plugin.debug)
                                                    {
                                                        Log.Info(itemGroupNode.Path + ": Failed random chance. " + chance + " < " + d100);
                                                    }
                                                }
                                                else
                                                {
                                                    Log.Error("Invalid chance: " + itemGroup.Name);
                                                }
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            Log.Error("Error giving items: " + e.ToString());
                                        }
                                    }
                                    else
                                    {
                                        if (plugin.debug)
                                            Log.Info(ev.NewRole + " != " + role.Name + " Player Name " + ev.Player.Nickname);
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Error("Error checking role: " + e.ToString());
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error("Error checking permission: " + e.ToString());
                }
                spawning.Remove(ev.Player.UserId);
            }
        }

        public int ItemDur(ItemType weapon)
        {
            switch (weapon)
            {
                case ItemType.GunCOM15:
                    return 12;
                case ItemType.GunE11SR:
                    return 40;
                case ItemType.GunProject90:
                    return 50;
                case ItemType.GunMP7:
                    return 35;
                case ItemType.GunLogicer:
                    return 75;
                case ItemType.GunUSP:
                    return 18;
                default:
                    return 50;
            }
        }
    }
}
