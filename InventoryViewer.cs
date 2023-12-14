using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShockAPI;
using Terraria;
using TerrariaApi.Server;
using Microsoft.Xna.Framework;
using Mono.CompilerServices.SymbolWriter;
using Google.Protobuf.WellKnownTypes;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.X509;
using System.Diagnostics;

namespace InventoryViewer
{
    [ApiVersion(2, 1)]
    public class InventoryViewer : TerrariaPlugin
    {
        public override string Author => "nightklp";
        public override string Description => "just a simple plugin that view an inventory of a specific player...";
        public override string Name => "Inventory viewer";
        public override Version Version => new Version(1, 0, 0, 3);


        public static Dictionary<string, string> tracktarget = new Dictionary<string, string>();

        public static string StatusTitle = "[c/f4d100:[][c/f4d100:inventoryViewer][c/f4d100:]]";

        public InventoryViewer(Main game) : base(game)
        {
            //amogus
        }

        public override void Initialize()
        {
            ServerApi.Hooks.ServerLeave.Register(this, OnServerLeave);

            ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);


            Commands.ChatCommands.Add(new Command("inventoryviewer.view", InventoryView, "inventoryview", "invview", "viewinv", "inview")
            {
                HelpText = "View inventory contents of a players\ndo [ /inventoryview help ] for more info"
            });
        }

        private void OnServerLeave(LeaveEventArgs args)
        {
            foreach (var value in tracktarget)
            {
                var gettracker = TSPlayer.FindByNameOrID(value.Value);
                TSPlayer tracker = gettracker[0];

                if (Main.player[args.Who].name == tracker.Name)
                {
                    tracktarget.Remove(value.Key);
                }
                else if (Main.player[args.Who].name == value.Key)
                {
                    tracktarget.Remove(value.Key);
                    tracker.SendMessage($"{StatusTitle} player named {Main.player[args.Who].name} left...", Color.Orange);
                }
            }
        }

        private void OnUpdate(EventArgs args)
        {
            foreach (var value in tracktarget)
            {
                var gettracker = TSPlayer.FindByNameOrID(value.Value);
                TSPlayer tracker = gettracker[0];

                var gettarget = TSPlayer.FindByNameOrID(value.Key);
                TSPlayer target = gettarget[0];

                getpreviousInv(target, tracker);
                
            }
        }

        public void InventoryView(CommandArgs args)
        {
            TSPlayer Player = args.Player;
            if (args.Parameters.Count != 1 && args.Parameters.Count != 2)
            {
                Player.SendErrorMessage("Invalid syntax. Proper syntax: /inventoryview <player> <type>\ndo [ /inventoryview help ] for more info");
                return;
            }

            if (args.Parameters.Count == 0)
            {
                args.Player.SendErrorMessage("Specify a Player!\ndo [ /inventoryview help ] for more info");
                return;
            }

            string helptext = "You can view player contents using this command\n" +
                    $"Example: /inventoryview [c/abff96:{Player.Name}] [c/96ffdc:inv] (View inventory contents)\n" +
                    "[c/0000f4:List of type]\n" +
                    "[c/85f400:main]\n" +
                    "[c/f4f400:inventory,inv]\ninfo: shows the player's backpack\n\n" +
                    "[c/f4f400:equipment,equip]\ninfo: shows the player's accessories/armor/loadouts/misc and etc...\n\n" +
                    "[c/85f400:secondary:]\n" +
                    "[c/f4f400:piggybank,piggy,pig]\ninfo: shows the player's piggy bank\n\n" +
                    "[c/f4f400:safe]\ninfo: shows the player's safe\n\n" +
                    "[c/f4f400:defenderforge,forge]\ninfo: shows the player's defender's forge\n\n" +
                    "[c/f4f400:voidvault,void,vault]\ninfo: shows the player's void vault\n\n" +
                    "[c/85f400:others]\n" +
                    "[c/f4f400:all]\ninfo: shows All contents of a players... \n[c/f40000:warning: this can flood your chat message]\n\n" +
                    "[c/f4f400:track]\ninfo: get logged when a player inventory changes... \nturnoff: to turn it off repeat the command again\n[c/f40000:warning: this can flood your chat message]";

            if (args.Parameters[0] == "help")
            {
                Player.SendMessage(helptext, Color.WhiteSmoke);
            }

            if (args.Parameters.Count == 1)
            {
                args.Player.SendErrorMessage("Specify a type!\ndo [ /inventoryview help ] for more info");
                return;
            }

            var foundPlr = TSPlayer.FindByNameOrID(args.Parameters[0]);
            if (foundPlr.Count == 0)
            {
                args.Player.SendErrorMessage("Invalid player!");
                return;
            }

            var targetplayer = foundPlr[0];
            string targetplayername = targetplayer.Name;

            //makes a variable to check if this player is loggen in or not ( usefull to avoid false ban )
            string targetplayerlogin = "[c/5c5c5c:status: ][c/f40000:This player hasn't been logged in!]";
            if (targetplayer.IsLoggedIn)
            {
                targetplayerlogin = "[c/5c5c5c:status: ][c/05f400:this player is logged in.]";
            }

            #region Types
            switch (args.Parameters[1])
            {
                case "inventory":
                case "inv":
                    {
                        InventoryString get = GetInventory(Player, targetplayer, "inventory");
                        Player.SendMessage($"( [c/ffffff:{targetplayer.Name}] ) inventory:\n{get.Inventory}\n\n{targetplayerlogin}", Color.WhiteSmoke);
                        return;
                    }
                case "equipment":
                case "equip":
                    {
                        InventoryString get = GetInventory(Player, targetplayer, "equipment");
                        Player.SendMessage($"( [c/ffffff:{targetplayer.Name}] ) Equipment:\n{get.Equipment}\n\n{targetplayerlogin}", Color.Green);
                        return;
                    }
                case "piggybank":
                case "piggy":
                case "pig":
                    {
                        InventoryString get = GetInventory(Player, targetplayer, "piggybank");
                        Player.SendMessage($"( [c/ffffff:{targetplayername}] ) Piggy Bank:\n{get.PiggyBank}\n\n{targetplayerlogin}", Color.Pink);
                        return;
                    }
                case "safe":
                    {
                        InventoryString get = GetInventory(Player, targetplayer, "safe");
                        Player.SendMessage($"( [c/ffffff:{targetplayername}] ) Safe:\n{get.Safe}\n\n{targetplayerlogin}", Color.Gray);
                        return;
                    }
                case "defenderforge":
                case "forge":
                    {
                        InventoryString get = GetInventory(Player, targetplayer, "defenderforge");
                        Player.SendMessage($"( [c/ffffff:{targetplayername}] ) defender's forge:\n{get.DefenderForge}\n\n{targetplayerlogin}", Color.Yellow);
                        return;
                    }
                case "voidvault":
                case "void":
                case "vault":
                    {
                        InventoryString get = GetInventory(Player, targetplayer, "voidvault");
                        Player.SendMessage($"( [c/ffffff:{targetplayername}] ) Void vault\n{get.VoidVault}\n\n{targetplayerlogin}", Color.Purple);
                        return;
                    }
                case "all":
                    {
                        InventoryString get = GetInventory(Player, targetplayer, "all");
                        Player.SendMessage($"( [c/ffffff:{targetplayername}] ) Inventory:\n{get.Inventory}\n\n" +
                            $"Equipment:\n{get.Equipment}\n" +
                            $"piggy bank:\n{get.PiggyBank}\n" +
                            $"safe:\n{get.Safe}\n" +
                            $"defender's forge:\n{get.DefenderForge}\n" +
                            $"void vault:\n{get.VoidVault}\n" +
                            $"{targetplayerlogin}", Color.Gray);
                        return;
                    }
                case "track":
                    {
                        if (!Player.RealPlayer)
                        {
                            Player.SendErrorMessage("you can only use this In-game!");
                            return;
                        }

                        bool check = false;
                        foreach (var value in tracktarget)
                        {
                            if (targetplayername == value.Key && Player.Name == value.Value)
                            {
                                tracktarget.Remove(targetplayername);
                                check = true;
                                Player.SendMessage($"{StatusTitle} you can no longer track [c/ffffff:{targetplayername}]", Color.Orange);
                            }
                        }
                        if (!check)
                        {
                            tracktarget.Add(targetplayername, Player.Name);
                            TSPlayer insert = new TSPlayer(targetplayer.Index);
                            
                            //set
                            targetplayer.SetData("previnv1", targetplayer.TPlayer.inventory.Clone());
                            targetplayer.SetData("prevbank1", targetplayer.TPlayer.bank.item.Clone());
                            targetplayer.SetData("prevbank2", targetplayer.TPlayer.bank2.item.Clone());
                            targetplayer.SetData("prevbank3", targetplayer.TPlayer.bank3.item.Clone());
                            targetplayer.SetData("prevbank4", targetplayer.TPlayer.bank4.item.Clone());

                            Player.SendMessage($"{StatusTitle} Tracking [c/ffffff:{targetplayername}] inventory ( repeat this command to turn it off )", Color.Orange);
                        }
                        return;
                    }
                default:
                    {
                        Player.SendErrorMessage("Invalid type!");
                        return;
                    }

            }
            #endregion
            return;
        }

        #region Functions
        /// <summary>
        /// functions of /inventoryview 
        /// insert a type to get a value [inventory, equipment, piggybank, safe, voidvault, defenderforge]
        /// </summary>
        /// <param name="executer"></param>
        /// <param name="target"></param>
        /// <param name="type"></param>
        private InventoryString GetInventory(TSPlayer executer, TSPlayer target, string type)
        {
            InventoryString get = new InventoryString(null, null, null, null, null, null);

            if (executer.RealPlayer)
            {
                #region inventory
                if (type == "inventory" || type == "all")
                {
                    int d2 = 10;
                    for (int i = 0; i < target.TPlayer.inventory.Length; i++)
                    {

                        string sp = $"/s{target.TPlayer.inventory[i].stack}";
                        if (target.TPlayer.inventory[i].prefix != 0) sp = $"/p{target.TPlayer.inventory[i].prefix}";

                        if (i == d2)
                        {
                            get.Inventory += $"\n|[i{sp}:{target.TPlayer.inventory[i].netID}]|";
                            d2 += 10;
                        }
                        else
                        {
                            get.Inventory += $"[i{sp}:{target.TPlayer.inventory[i].netID}]|";
                        }
                    }
                    get.Inventory += $"\n[i/s{target.TPlayer.trashItem.stack}:{target.TPlayer.trashItem.netID}]";
                }
                #endregion


                #region Equipment
                if (type == "equipment" || type == "all")
                {
                    string loadoutused = $"( in use )loadout {target.TPlayer.CurrentLoadoutIndex + 1}:\n";
                    string loadout1 = "";
                    string loadout2 = "";
                    string loadout3 = "";
                    string misclist = "";
                    for (int i = 0; i < 10; i++)
                    {
                        int ii = i + 10;
                        if (i < 5)
                        {
                            misclist += $"|[i/s{target.TPlayer.miscDyes[i].stack}:{target.TPlayer.miscDyes[i].netID}]|[i/s{target.TPlayer.miscEquips[i].stack}:{target.TPlayer.miscEquips[i].netID}]|\n";
                        }
                        if (i < 3)
                        {
                            loadoutused += $"|[i/s{target.TPlayer.dye[i + 3].stack}:{target.TPlayer.dye[i + 3].netID}]|[i/s{target.TPlayer.armor[ii + 3].stack}:{target.TPlayer.armor[ii + 3].netID}]|[i/s{target.TPlayer.armor[i + 3].stack}:{target.TPlayer.armor[i + 3].netID}]|====|[i/s{target.TPlayer.dye[i].stack}:{target.TPlayer.dye[i].netID}]|[i/s{target.TPlayer.armor[ii].stack}:{target.TPlayer.armor[ii].netID}]|[i/s{target.TPlayer.armor[i].stack}:{target.TPlayer.armor[i].netID}]|\n";
                        }
                        if (i >= 6 && i <= 8)
                        {
                            loadoutused += $"|[i/s{target.TPlayer.dye[i].stack}:{target.TPlayer.dye[i].netID}]|[i/s{target.TPlayer.armor[ii].stack}:{target.TPlayer.armor[ii].netID}]|[i/s{target.TPlayer.armor[i].stack}:{target.TPlayer.armor[i].netID}]|\n";
                        }
                    }
                    for (int il = 0; il < 3; il++)
                    {
                        string loadoutget = $"loadout {il}:";
                        for (int i = 0; i < 10; i++)
                        {
                            int ii = i + 10;
                            if (i < 3)
                            {
                                loadoutget += $"|[i/s{target.TPlayer.Loadouts[il].Dye[i + 3].stack}:{target.TPlayer.Loadouts[il].Dye[i + 3].netID}]|[i/s{target.TPlayer.Loadouts[il].Armor[ii + 3].stack}:{target.TPlayer.Loadouts[il].Armor[ii + 3].netID}]|[i/s{target.TPlayer.Loadouts[il].Armor[i + 3].stack}:{target.TPlayer.Loadouts[il].Armor[i + 3].netID}]|====|[i/s{target.TPlayer.Loadouts[il].Dye[i].stack}:{target.TPlayer.Loadouts[il].Dye[i].netID}]|[i/s{target.TPlayer.Loadouts[il].Armor[ii].stack}:{target.TPlayer.Loadouts[il].Armor[ii].netID}]|[i/s{target.TPlayer.Loadouts[il].Armor[i].stack}:{target.TPlayer.Loadouts[il].Armor[i].netID}]|\n";
                            }
                            if (i >= 6 && i <= 8)
                            {
                                loadoutget += $"|[i/s{target.TPlayer.Loadouts[il].Dye[i].stack}:{target.TPlayer.Loadouts[il].Dye[i].netID}]|[i/s{target.TPlayer.Loadouts[il].Armor[ii].stack}:{target.TPlayer.Loadouts[il].Armor[ii].netID}]|[i/s{target.TPlayer.Loadouts[il].Armor[i].stack}:{target.TPlayer.Loadouts[il].Armor[i].netID}]|\n";
                            }
                        }
                        switch (il)
                        {
                            case 0:
                                loadout1 = loadoutget;
                                break;
                            case 1:
                                loadout2 = loadoutget;
                                break;
                            case 2:
                                loadout3 = loadoutget;
                                break;
                        }
                    }
                    switch (target.TPlayer.CurrentLoadoutIndex)
                    {
                        case 0:
                            loadout1 = loadoutused;
                            break;
                        case 1:
                            loadout2 = loadoutused;
                            break;
                        case 2:
                            loadout3 = loadoutused;
                            break;
                    }
                    get.Equipment = $"{loadout1}\n{loadout2}\n{loadout3}\nmisc:\n{misclist}";
                }
                #endregion


                #region  PiggyBank
                if (type == "piggybank" || type == "all")
                {
                    int d2 = 10;
                    for (int i = 0; i < target.TPlayer.bank.item.Length; i++)
                    {

                        string sp = $"/s{target.TPlayer.bank.item[i].stack}";
                        if (target.TPlayer.bank.item[i].prefix != 0) sp = $"/p{target.TPlayer.bank.item[i].prefix}";

                        if (i == d2)
                        {
                            get.PiggyBank += $"\n|[i/s{sp}:{target.TPlayer.bank.item[i].netID}]|";
                            d2 += 10;
                        }
                        else
                        {
                            get.PiggyBank += $"[i/s{sp}:{target.TPlayer.bank.item[i].netID}]|";
                        }
                    }
                }
                #endregion


                #region Safe
                if (type == "safe" || type == "all")
                {
                    int d2 = 10;
                    for (int i = 0; i < target.TPlayer.bank2.item.Length; i++)
                    {

                        string sp = $"/s{target.TPlayer.bank2.item[i].stack}";
                        if (target.TPlayer.bank2.item[i].prefix != 0) sp = $"/p{target.TPlayer.bank2.item[i].prefix}";

                        if (i == d2)
                        {
                            get.Safe += $"\n|[i/s{sp}:{target.TPlayer.bank2.item[i].netID}]|";
                            d2 += 10;
                        }
                        else
                        {
                            get.Safe += $"[i/s{sp}:{target.TPlayer.bank2.item[i].netID}]|";
                        }
                    }
                }
                #endregion


                #region Defender Forge
                if (type == "defenderforge" || type == "all")
                {
                    int d2 = 10;
                    for (int i = 0; i < target.TPlayer.bank3.item.Length; i++)
                    {

                        string sp = $"/s{target.TPlayer.bank3.item[i].stack}";
                        if (target.TPlayer.bank3.item[i].prefix != 0) sp = $"/p{target.TPlayer.bank3.item[i].prefix}";

                        if (i == d2)
                        {
                            get.DefenderForge += $"\n|[i/s{sp}:{target.TPlayer.bank3.item[i].netID}]|";
                            d2 += 10;
                        }
                        else
                        {
                            get.DefenderForge += $"[i/s{sp}:{target.TPlayer.bank3.item[i].netID}]|";
                        }
                    }
                }
                #endregion


                #region Void Vault
                if (type == "voidvault" || type == "all")
                {
                    int d2 = 10;
                    for (int i = 0; i < target.TPlayer.bank4.item.Length; i++)
                    {

                        string sp = $"/s{target.TPlayer.bank4.item[i].stack}";
                        if (target.TPlayer.bank4.item[i].prefix != 0) sp = $"/p{target.TPlayer.bank4.item[i].prefix}";

                        if (i == d2)
                        {
                            get.VoidVault += $"\n|[i/s{sp}:{target.TPlayer.bank4.item[i].netID}]|";
                            d2 += 10;
                        }
                        else
                        {
                            get.VoidVault += $"[i/s{sp}:{target.TPlayer.bank4.item[i].netID}]|";
                        }
                    }
                }
                #endregion
            }
            else
            {


                #region prefix list
                string[] prefixlist = { "" ,"Large", "Massive", "Dangerous", "Savage", "Sharp", "Pointy", "Tiny",
                    "Terrible", "Small", "Dull", "Unhappy", "Bulky", "Shameful", "Heavy", "Light", "Sighted", "Sighted",
                    "Sighted", "Intimidating", "Deadly", "Staunch", "Awful", "Lethargic", "Awkward", "Powerful", "Mystic",
                   "Adept", "Masterful", "Inept", "Ignorant", "Deranged", "Intense", "Taboo", "Celestial", "Furious", "Keen",
                    "Superior", "Forceful", "Broken", "Damaged", "Shoddy", "Quick", "Deadly", "Agile", "Nimble", "Murderous",
                    "Slow", "Sluggish", "Lazy", "Annoying", "Nasty", "Manic", "Hurtful", "Strong", "Unpleasant", "Weak",
                    "Ruthless", "Frenzying", "Godly", "Demonic", "Zealous", "Hard", "Guarding", "Armored", "Warding",
                    "Arcane", "Precise", "Lucky", "Jagged", "Spiked", "Angry", "Menacing", "Brisk", "Fleeting", "Hasty",
                    "Quick", "Wild", "Rash", "Intrepid", "Violent", "Legendary", "Unreal", "Mythical", "Legendary", "Piercing" };
                #endregion


                #region inventory
                if (type == "inventory" || type == "all")
                {
                    int d2 = 10;
                    for (int i = 0; i < target.TPlayer.inventory.Length; i++)
                    {

                        string p = "";
                        if (target.TPlayer.inventory[i].prefix != 0) p = $"{prefixlist[target.TPlayer.inventory[i].prefix]}";

                        string s = "";
                        if (target.TPlayer.inventory[i].stack > 1) s = $"({target.TPlayer.inventory[i].stack}) ";

                        if (i == d2)
                        {
                            get.Inventory += $"\n|[{p} {target.TPlayer.inventory[i].Name} {s}]|";
                            d2 += 10;
                        }
                        else
                        {
                            get.Inventory += $"[{p} {target.TPlayer.inventory[i].Name} {s}]|";
                        }
                    }
                    get.Inventory += $"\n[ {target.TPlayer.trashItem.stack} ({target.TPlayer.trashItem.stack}) ]";
                }
                #endregion


                #region Equipment
                if (type == "equipment" || type == "all")
                {
                    string loadoutused = $"( in use )loadout {target.TPlayer.CurrentLoadoutIndex + 1}:\n";
                    string loadout1 = "";
                    string loadout2 = "";
                    string loadout3 = "";
                    string misclist = "";
                    for (int i = 0; i < 10; i++)
                    {
                        int ii = i + 10;

                        if (i < 5)
                        {
                            misclist += $"|[{target.TPlayer.miscDyes[i].Name}]|[{target.TPlayer.miscEquips[i].Name}]|\n";
                        }
                        if (i < 3)
                        {
                            loadoutused += $"|[{target.TPlayer.dye[i + 3].Name}]|[{prefixlist[target.TPlayer.armor[ii + 3].prefix]} {target.TPlayer.armor[ii + 3].Name}]|[{prefixlist[target.TPlayer.armor[i + 3].prefix]} {target.TPlayer.armor[i + 3].Name} ]|====|[ {target.TPlayer.dye[i].Name} ]|[ {target.TPlayer.armor[ii].Name} ]|[ {target.TPlayer.armor[i].Name} ]|\n";
                        }
                        if (i >= 6 && i <= 8)
                        {
                            loadoutused += $"|[ {target.TPlayer.dye[i].Name} ]|[ {prefixlist[target.TPlayer.armor[ii].prefix]} {target.TPlayer.armor[ii].Name} ]|[ {prefixlist[target.TPlayer.armor[i].prefix]} {target.TPlayer.armor[i].Name} ]|\n";
                        }
                    }
                    for (int il = 0; il < 3; il++)
                    {
                        string loadoutget = $"loadout {il}:";
                        for (int i = 0; i < 10; i++)
                        {
                            int ii = i + 10;
                            if (i < 3)
                            {
                                loadoutget += $"|[ {target.TPlayer.Loadouts[il].Dye[i + 3].Name} ]|[ {prefixlist[target.TPlayer.Loadouts[il].Armor[ii + 3].prefix]} {target.TPlayer.Loadouts[il].Armor[ii + 3].Name} ]|[ {prefixlist[target.TPlayer.Loadouts[il].Armor[i + 3].prefix]} {target.TPlayer.Loadouts[il].Armor[i + 3].Name}]|====|[ {target.TPlayer.Loadouts[il].Dye[i].Name}]|[ {target.TPlayer.Loadouts[il].Armor[ii].Name} ]|[ {target.TPlayer.Loadouts[il].Armor[i].Name} ]|\n";
                            }
                            if (i >= 6 && i <= 8)
                            {
                                loadoutget += $"|[ {target.TPlayer.Loadouts[il].Dye[i].Name} ]|[ {prefixlist[target.TPlayer.Loadouts[il].Armor[ii].prefix]} {target.TPlayer.Loadouts[il].Armor[ii].Name} ]|[ {prefixlist[target.TPlayer.Loadouts[il].Armor[i].prefix]} {target.TPlayer.Loadouts[il].Armor[i].Name}]|\n";
                            }
                        }
                        switch (il)
                        {
                            case 0:
                                loadout1 = loadoutget;
                                break;
                            case 1:
                                loadout2 = loadoutget;
                                break;
                            case 2:
                                loadout3 = loadoutget;
                                break;
                        }
                    }
                    switch (target.TPlayer.CurrentLoadoutIndex)
                    {
                        case 0:
                            loadout1 = loadoutused;
                            break;
                        case 1:
                            loadout2 = loadoutused;
                            break;
                        case 2:
                            loadout3 = loadoutused;
                            break;
                    }
                    get.Equipment = $"{loadout1}\n{loadout2}\n{loadout3}\nmisc:\n{misclist}";
                }
                #endregion


                #region  PiggyBank
                if (type == "piggybank" || type == "all")
                {
                    int d2 = 10;
                    for (int i = 0; i < target.TPlayer.bank.item.Length; i++)
                    {

                        string p = "";
                        if (target.TPlayer.bank.item[i].prefix != 0) p = $"{prefixlist[target.TPlayer.bank.item[i].prefix]}";

                        string s = $"({target.TPlayer.bank.item[i].stack}) ";
                        if (target.TPlayer.bank.item[i].stack == 1) s = "";

                        if (i == d2)
                        {
                            get.Inventory += $"\n|[{p} {target.TPlayer.bank.item[i].Name} {s}]|";
                            d2 += 10;
                        }
                        else
                        {
                            get.Inventory += $"[{p}{target.TPlayer.bank.item[i].Name} {s}]|";
                        }
                    }
                }
                #endregion


                #region Safe
                if (type == "safe" || type == "all")
                {
                    int d2 = 10;
                    for (int i = 0; i < target.TPlayer.bank2.item.Length; i++)
                    {

                        string p = "";
                        if (target.TPlayer.bank2.item[i].prefix != 0) p = $"{prefixlist[target.TPlayer.bank2.item[i].prefix]}";

                        string s = $"({target.TPlayer.bank2.item[i].stack}) ";
                        if (target.TPlayer.bank2.item[i].stack == 1) s = "";

                        if (i == d2)
                        {
                            get.Inventory += $"\n|[{p} {target.TPlayer.bank2.item[i].Name} {s}]|";
                            d2 += 10;
                        }
                        else
                        {
                            get.Inventory += $"[{p}{target.TPlayer.bank2.item[i].Name} {s}]|";
                        }
                    }
                }
                #endregion


                #region Defender Forge
                if (type == "defenderforge" || type == "all")
                {
                    int d2 = 10;
                    for (int i = 0; i < target.TPlayer.bank3.item.Length; i++)
                    {

                        string p = "";
                        if (target.TPlayer.bank3.item[i].prefix != 0) p = $"{prefixlist[target.TPlayer.bank3.item[i].prefix]}";

                        string s = $"({target.TPlayer.bank3.item[i].stack}) ";
                        if (target.TPlayer.bank3.item[i].stack == 1) s = "";

                        if (i == d2)
                        {
                            get.Inventory += $"\n|[{p} {target.TPlayer.bank3.item[i].Name} {s}]|";
                            d2 += 10;
                        }
                        else
                        {
                            get.Inventory += $"[{p}{target.TPlayer.bank3.item[i].Name} {s}]|";
                        }
                    }
                }
                #endregion


                #region Void Vault
                if (type == "voidvault" || type == "all")
                {
                    int d2 = 10;
                    for (int i = 0; i < target.TPlayer.bank4.item.Length; i++)
                    {

                        string p = "";
                        if (target.TPlayer.bank4.item[i].prefix != 0) p = $"{prefixlist[target.TPlayer.bank4.item[i].prefix]}";

                        string s = $"({target.TPlayer.bank4.item[i].stack}) ";
                        if (target.TPlayer.bank4.item[i].stack == 1) s = "";

                        if (i == d2)
                        {
                            get.Inventory += $"\n|[{p} {target.TPlayer.bank4.item[i].Name} {s}]|";
                            d2 += 10;
                        }
                        else
                        {
                            get.Inventory += $"[{p}{target.TPlayer.bank4.item[i].Name} {s}]|";
                        }
                    }
                }
                #endregion
            }

            return get;
        }

        private void getpreviousInv(TSPlayer target, TSPlayer tracker)
        {
            #region Inventory

            for (int i = 0; i < target.TPlayer.inventory.Length; i++)
            {
                Item now = target.TPlayer.inventory[i];
                Item prev = target.GetData<Item[]>("previnv1")[i];

                string nowps = $"/s{now.stack}";
                string prevps = $"/s{prev.stack}";

                if (now.prefix != 0) nowps = $"/p{now.prefix}";

                if (prev.prefix != 0) prevps = $"/p{prev.prefix}";

                if (now.netID != prev.netID || now.stack != prev.stack || now.prefix != prev.prefix)
                {
                    if (i == 58)
                    {
                        tracker.SendMessage($"{StatusTitle} {target.Name} [c/ffffff:holds the item:] [i{nowps}:{now.netID}]", Color.Yellow);
                        target.SetData("previnv1", target.TPlayer.inventory.Clone());
                    } else
                    {
                        tracker.SendMessage($"{StatusTitle} {target.Name} [c/ffffff:inventory Slot No.{i}] changed [i{prevps}:{prev.netID}] => [i{nowps}:{now.netID}]", Color.Yellow);
                        target.SetData("previnv1", target.TPlayer.inventory.Clone());
                    }
                }
            }

            #endregion

            //not sure if equipment tracking is usfull...
            
            #region PiggyBank
            for (int i = 0; i < target.TPlayer.bank.item.Length; i++)
            {
                Item now = target.TPlayer.bank.item[i];
                Item prev = target.GetData<Item[]>("prevbank1")[i];

                string nowps = $"/s{now.stack}";
                string prevps = $"/s{prev.stack}";

                if (now.prefix != 0) nowps = $"/p{now.prefix}";

                if (prev.prefix != 0) prevps = $"/p{prev.prefix}";

                if (now.netID != prev.netID || now.stack != prev.stack || now.prefix != prev.prefix)
                {
                    tracker.SendMessage($"{StatusTitle} {target.Name} [c/fd67e9:pig Slot No.{i}] changed [i{prevps}:{prev.netID}] => [i{nowps}:{now.netID}]", Color.Yellow);
                    target.SetData("prevbank1", target.TPlayer.bank.item.Clone());
                }
            }
            #endregion

            #region Safe
            for (int i = 0; i < target.TPlayer.bank2.item.Length; i++)
            {
                Item now = target.TPlayer.bank2.item[i];
                Item prev = target.GetData<Item[]>("prevbank2")[i];

                string nowps = $"/s{now.stack}";
                string prevps = $"/s{prev.stack}";

                if (now.prefix != 0) nowps = $"/p{now.prefix}";

                if (prev.prefix != 0) prevps = $"/p{prev.prefix}";

                if (now.netID != prev.netID || now.stack != prev.stack || now.prefix != prev.prefix)
                {
                    tracker.SendMessage($"{StatusTitle} {target.Name} [c/5e5e5e:safe Slot No.{i}] changed [i{prevps}:{prev.netID}] => [i{nowps}:{now.netID}]", Color.Yellow);
                    target.SetData("prevbank2", target.TPlayer.bank2.item.Clone());
                }
            }
            #endregion

            #region DefenderForge
            for (int i = 0; i < target.TPlayer.bank3.item.Length; i++)
            {
                Item now = target.TPlayer.bank3.item[i];
                Item prev = target.GetData<Item[]>("prevbank3")[i];

                string nowps = $"/s{now.stack}";
                string prevps = $"/s{prev.stack}";

                if (now.prefix != 0) nowps = $"/p{now.prefix}";

                if (prev.prefix != 0) prevps = $"/p{prev.prefix}";

                if (now.netID != prev.netID || now.stack != prev.stack || now.prefix != prev.prefix)
                {
                    tracker.SendMessage($"{StatusTitle} {target.Name} [c/fffb54:defenderforge Slot No.{i}] changed [i{prevps}:{prev.netID}] => [i{nowps}:{now.netID}]", Color.Yellow);
                    target.SetData("prevbank3", target.TPlayer.bank3.item.Clone());
                }
            }
            #endregion

            #region Void Vault
            for (int i = 0; i < target.TPlayer.bank4.item.Length; i++)
            {
                Item now = target.TPlayer.bank4.item[i];
                Item prev = target.GetData<Item[]>("prevbank4")[i];

                string nowps = $"/s{now.stack}";
                string prevps = $"/s{prev.stack}";

                if (now.prefix != 0) nowps = $"/p{now.prefix}";

                if (prev.prefix != 0) prevps = $"/p{prev.prefix}";

                if (now.netID != prev.netID || now.stack != prev.stack || now.prefix != prev.prefix)
                {
                    tracker.SendMessage($"{StatusTitle} {target.Name} [c/8d00cb:voidvault Slot No.{i}] changed [i{prevps}:{prev.netID}] => [i{nowps}:{now.netID}]", Color.Yellow);
                    target.SetData("prevbank4", target.TPlayer.bank4.item.Clone());
                }
            }
            #endregion
            
        }
        #endregion
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.ServerLeave.Deregister(this, OnServerLeave);

                ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);
            }
            base.Dispose(disposing);
        }
    }

    #region OBJECTS
    class InventoryString
    {
        public string Inventory;
        public string Equipment;
        public string PiggyBank;
        public string Safe;
        public string DefenderForge;
        public string VoidVault;
        public InventoryString(string inventory, string equipment, string piggybank, string safe, string defenderforge, string voidvault)
        {
            Inventory = inventory;
            Equipment = equipment;
            PiggyBank = piggybank;
            Safe = safe;
            DefenderForge = defenderforge;
            VoidVault = voidvault;
        }
    }
    #endregion
}
