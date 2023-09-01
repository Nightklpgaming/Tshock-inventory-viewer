using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShockAPI;
using Terraria;
using TerrariaApi.Server;
using Microsoft.Xna.Framework;

namespace inventoryviewers
{
    [ApiVersion(2, 1)]
    public class inventoryviewers : TerrariaPlugin
    {
        public override string Author => "Nightklp";

        public override string Description => "just a simple plugin that checks a inventory of a player...";

        public override string Name => "Inventory viewer";

        public override Version Version => new Version(1, 0, 0, 1);

        public inventoryviewers(Main game) : base(game)
        {

        }

        public override void Initialize()
        {
            Commands.ChatCommands.Add(new Command("inventoryviewer.view", InventoryView, "inventoryview", "invview", "viewinv", "inview")
            {
                HelpText = "View inventory contents of a players"
            });
        }

        public void InventoryView(CommandArgs args)
        {
            TSPlayer Player = args.Player;
            if (args.Parameters.Count != 1 && args.Parameters.Count != 2)
            {
                Player.SendErrorMessage("Invalid syntax. Proper syntax: /inventorycheck <player> <type>");
                Player.SendMessage("You can view player contents using this command\n" + 
                    $"Example: /inview [c/abff96:{Player.Name}] [c/96ffdc:inv] (View inventory contents)", Color.WhiteSmoke);
                return;
            }
            if (args.Parameters.Count == 0)
            {
                args.Player.SendErrorMessage("Specify a Player!");
                return;
            }
            if (args.Parameters.Count == 1)
            {
                args.Player.SendErrorMessage("Specify a type!");
                Player.SendMessage("[c/0000f4:List of type]\n" +
                    "[c/f4f400:inventory,inv]\ninfo: shows the player's backpack\n\n" +
                    "[c/f4f400:equipment,equip]\ninfo: shows the player's accessories/armor/loadouts/misc and etc...\n\n" +
                    "[c/f4f400:piggybank,piggy,pig]\ninfo: shows the player's piggy bank\n\n" +
                    "[c/f4f400:safe]\ninfo: shows the player's safe\n\n" +
                    "[c/f4f400:defenderforge,forge]\ninfo: shows the player's defender's forge\n\n" +
                    "[c/f4f400:voidvault,void,vault]\ninfo: shows the player's void vault\n\n" +
                    "[c/f4f400:all]\ninfo: shows All contents of a players... [c/f40000:warning: this can flood your chat message]", Color.WhiteSmoke);
                return;
            }
            //argument /cmd <arg0> <arg1>
            string arg0 = args.Parameters[0];
            string arg1 = args.Parameters[1];
            //finding player
            var foundPlr = TSPlayer.FindByNameOrID(arg0);
            if (foundPlr.Count == 0)
            {
                args.Player.SendErrorMessage("Invalid player!");
                return;
            }
            var targetplayer = foundPlr[0];

            string targetplayerlogin = "[c/5c5c5c:status: ][c/f40000:This player hasn't been logged in!]";
            if (targetplayer.IsLoggedIn)
            {
                targetplayerlogin = "[c/5c5c5c:status: ][c/05f400:this player is logged in.]";
            }
            switch (arg1)
            {
                case "inventory":
                case "inv":
                    {
                        string list1 = "|";
                        string list2 = "|";
                        for (int i = 0; i < NetItem.InventorySlots; i++)
                        {
                            if (i < 10)
                            {
                                list1 += "[i/s" + targetplayer.TPlayer.inventory[i].stack + ":" + targetplayer.TPlayer.inventory[i].netID + "]|";
                            }
                            else
                            if (i < NetItem.InventorySlots)
                            {
                                if (i == 20 || i == 30 || i == 40 || i == 50)
                                {
                                    list2 += "\n|[i/s" + targetplayer.TPlayer.inventory[i].stack + ":" + targetplayer.TPlayer.inventory[i].netID + "]|";
                                }
                                else
                                {
                                    list2 += "[i/s" + targetplayer.TPlayer.inventory[i].stack + ":" + targetplayer.TPlayer.inventory[i].netID + "]|";
                                }
                            }
                        }
                        Player.SendMessage($"( [c/ffffff:{targetplayer.Name}] ) inventory\n\nHotbar:\n{list1}\ninventory:\n{list2}\ntrash slot:\n[i/s{Player.TPlayer.trashItem.stack}:{Player.TPlayer.trashItem.netID}]\n\n{targetplayerlogin}", Color.WhiteSmoke);
                        return;
                    }
                case "equipment":
                case "equip":
                    {
                        string loadoutused = "( in use )loadout " + (targetplayer.TPlayer.CurrentLoadoutIndex + 1) + ":\n";
                        string loadout1 = "loadout 1:\n";
                        string loadout2 = "loadout 2:\n";
                        string loadout3 = "loadout 3:\n";
                        string misclist = "";
                        string e = $"{targetplayer.TPlayer.Loadouts[0].Armor[1]}";
                        for (int i = 0; i < 10; i++)
                        {
                            int ii = i + 10;
                            if (i < 5)
                            {
                                misclist += "|[i/s" + targetplayer.TPlayer.miscDyes[i].stack + ":" + targetplayer.TPlayer.miscDyes[i].netID + "]|[i/s" + targetplayer.TPlayer.miscEquips[i].stack + ":" + targetplayer.TPlayer.miscEquips[i].netID + "]|\n";
                            }
                            if (i < 3)
                            {
                                loadoutused += "|[i/s" + targetplayer.TPlayer.dye[i + 3].stack + ":" + targetplayer.TPlayer.dye[i + 3].netID + "]|[i/s" + targetplayer.TPlayer.armor[ii + 3].stack + ":" + targetplayer.TPlayer.armor[ii + 3].netID + "]|[i/s" + targetplayer.TPlayer.armor[i + 3].stack + ":" + targetplayer.TPlayer.armor[i + 3].netID + "]|====|[i/s" + targetplayer.TPlayer.dye[i].stack + ":" + targetplayer.TPlayer.dye[i].netID + "]|[i/s" + targetplayer.TPlayer.armor[ii].stack + ":" + targetplayer.TPlayer.armor[ii].netID + "]|[i/s" + targetplayer.TPlayer.armor[i].stack + ":" + targetplayer.TPlayer.armor[i].netID + "]|\n";
                                loadout1 += "|[i/s" + targetplayer.TPlayer.Loadouts[0].Dye[i + 3].stack + ":" + targetplayer.TPlayer.Loadouts[0].Dye[i + 3].netID + "]|[i/s" + targetplayer.TPlayer.Loadouts[0].Armor[ii + 3].stack + ":" + targetplayer.TPlayer.Loadouts[0].Armor[ii + 3].netID + "]|[i/s" + targetplayer.TPlayer.Loadouts[0].Armor[i + 3].stack + ":" + targetplayer.TPlayer.Loadouts[0].Armor[i + 3].netID + "]|====|[i/s" + targetplayer.TPlayer.Loadouts[0].Dye[i].stack + ":" + targetplayer.TPlayer.Loadouts[0].Dye[i].netID + "]|[i/s" + targetplayer.TPlayer.Loadouts[0].Armor[ii].stack + ":" + targetplayer.TPlayer.Loadouts[0].Armor[ii].netID + "]|[i/s" + targetplayer.TPlayer.Loadouts[0].Armor[i].stack + ":" + targetplayer.TPlayer.Loadouts[0].Armor[i].netID + "]|\n";
                                loadout2 += "|[i/s" + targetplayer.TPlayer.Loadouts[1].Dye[i + 3].stack + ":" + targetplayer.TPlayer.Loadouts[1].Dye[i + 3].netID + "]|[i/s" + targetplayer.TPlayer.Loadouts[1].Armor[ii + 3].stack + ":" + targetplayer.TPlayer.Loadouts[1].Armor[ii + 3].netID + "]|[i/s" + targetplayer.TPlayer.Loadouts[1].Armor[i + 3].stack + ":" + targetplayer.TPlayer.Loadouts[1].Armor[i + 3].netID + "]|====|[i/s" + targetplayer.TPlayer.Loadouts[1].Dye[i].stack + ":" + targetplayer.TPlayer.Loadouts[1].Dye[i].netID + "]|[i/s" + targetplayer.TPlayer.Loadouts[1].Armor[ii].stack + ":" + targetplayer.TPlayer.Loadouts[1].Armor[ii].netID + "]|[i/s" + targetplayer.TPlayer.Loadouts[1].Armor[i].stack + ":" + targetplayer.TPlayer.Loadouts[1].Armor[i].netID + "]|\n";
                                loadout3 += "|[i/s" + targetplayer.TPlayer.Loadouts[2].Dye[i + 3].stack + ":" + targetplayer.TPlayer.Loadouts[2].Dye[i + 3].netID + "]|[i/s" + targetplayer.TPlayer.Loadouts[2].Armor[ii + 3].stack + ":" + targetplayer.TPlayer.Loadouts[2].Armor[ii + 3].netID + "]|[i/s" + targetplayer.TPlayer.Loadouts[2].Armor[i + 3].stack + ":" + targetplayer.TPlayer.Loadouts[2].Armor[i + 3].netID + "]|====|[i/s" + targetplayer.TPlayer.Loadouts[2].Dye[i].stack + ":" + targetplayer.TPlayer.Loadouts[2].Dye[i].netID + "]|[i/s" + targetplayer.TPlayer.Loadouts[2].Armor[ii].stack + ":" + targetplayer.TPlayer.Loadouts[2].Armor[ii].netID + "]|[i/s" + targetplayer.TPlayer.Loadouts[2].Armor[i].stack + ":" + targetplayer.TPlayer.Loadouts[2].Armor[i].netID + "]|\n";
                            }
                            if (i < 6)
                            {
                                //skipping
                            }
                            else
                            {
                                loadoutused += "|[i/s" + targetplayer.TPlayer.dye[i].stack + ":" + targetplayer.TPlayer.dye[i].netID + "]|[i/s" + targetplayer.TPlayer.armor[ii].stack + ":" + targetplayer.TPlayer.armor[ii].netID + "]|[i/s" + targetplayer.TPlayer.armor[i].stack + ":" + targetplayer.TPlayer.armor[i].netID + "]|\n";
                                loadout1 += "|[i/s" + targetplayer.TPlayer.Loadouts[0].Dye[i].stack + ":" + targetplayer.TPlayer.Loadouts[0].Dye[i].netID + "]|[i/s" + targetplayer.TPlayer.Loadouts[0].Armor[ii].stack + ":" + targetplayer.TPlayer.Loadouts[0].Armor[ii].netID + "]|[i/s" + targetplayer.TPlayer.Loadouts[0].Armor[i].stack + ":" + targetplayer.TPlayer.Loadouts[0].Armor[i].netID + "]|\n";
                                loadout2 += "|[i/s" + targetplayer.TPlayer.Loadouts[1].Dye[i].stack + ":" + targetplayer.TPlayer.Loadouts[1].Dye[i].netID + "]|[i/s" + targetplayer.TPlayer.Loadouts[1].Armor[ii].stack + ":" + targetplayer.TPlayer.Loadouts[1].Armor[ii].netID + "]|[i/s" + targetplayer.TPlayer.Loadouts[1].Armor[i].stack + ":" + targetplayer.TPlayer.Loadouts[1].Armor[i].netID + "]|\n";
                                loadout3 += "|[i/s" + targetplayer.TPlayer.Loadouts[2].Dye[i].stack + ":" + targetplayer.TPlayer.Loadouts[2].Dye[i].netID + "]|[i/s" + targetplayer.TPlayer.Loadouts[2].Armor[ii].stack + ":" + targetplayer.TPlayer.Loadouts[2].Armor[ii].netID + "]|[i/s" + targetplayer.TPlayer.Loadouts[2].Armor[i].stack + ":" + targetplayer.TPlayer.Loadouts[2].Armor[i].netID + "]|\n";
                            }
                        }
                        switch (targetplayer.TPlayer.CurrentLoadoutIndex)
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
                        Player.SendMessage($"( [c/ffffff:{targetplayer.Name}] ) Equipment:\n\n{loadout1}\n\n{loadout2}\n\n{loadout3}\nmisc:\n{misclist}\n\n{targetplayerlogin}", Color.Green);
                        return;
                    }
                case "piggybank":
                case "piggy":
                case "pig":
                    {
                        string list1 = "|";
                        for (int i = 0; i < 40; i++)
                        {
                            if (i == 10 || i == 20 || i == 30 || i == 40 || i == 50)
                            {
                                list1 += "\n|[i/s" + targetplayer.TPlayer.bank.item[i].stack + ":" + targetplayer.TPlayer.bank.item[i].netID + "]|";
                            }
                            else
                            {
                                list1 += "[i/s" + targetplayer.TPlayer.bank.item[i].stack + ":" + targetplayer.TPlayer.bank.item[i].netID + "]|";
                            }
                        }
                        Player.SendMessage($"( [c/ffffff:{targetplayer.Name}] ) Piggy Bank\n\n{list1}\n\n{targetplayerlogin}", Color.Pink);
                        return;
                    }
                case "safe":
                    {
                        string list1 = "|";
                        for (int i = 0; i < 40; i++)
                        {
                            if (i == 10 || i == 20 || i == 30 || i == 40 || i == 50)
                            {
                                list1 += "\n|[i/s" + targetplayer.TPlayer.bank2.item[i].stack + ":" + targetplayer.TPlayer.bank2.item[i].netID + "]|";
                            }
                            else
                            {
                                list1 += "[i/s" + targetplayer.TPlayer.bank2.item[i].stack + ":" + targetplayer.TPlayer.bank2.item[i].netID + "]|";
                            }
                        }
                        Player.SendMessage($"( [c/ffffff:{targetplayer.Name}] ) Safe\n\n{list1}\n\n{targetplayerlogin}", Color.Gray);
                        return;
                    }
                case "defenderforge":
                case "forge":
                    {
                        string list1 = "|";
                        for (int i = 0; i < 40; i++)
                        {
                            if (i == 10 || i == 20 || i == 30 || i == 40 || i == 50)
                            {
                                list1 += "\n|[i/s" + targetplayer.TPlayer.bank3.item[i].stack + ":" + targetplayer.TPlayer.bank3.item[i].netID + "]|";
                            }
                            else
                            {
                                list1 += "[i/s" + targetplayer.TPlayer.bank3.item[i].stack + ":" + targetplayer.TPlayer.bank3.item[i].netID + "]|";
                            }
                        }
                        Player.SendMessage($"( [c/ffffff:{targetplayer.Name}] ) defender's forge\n\n{list1}\n\n{targetplayerlogin}", Color.Yellow);
                        return;
                    }
                case "voidvault":
                case "void":
                case "vault":
                    {
                        string list1 = "|";
                        for (int i = 0; i < 40; i++)
                        {
                            if (i == 10 || i == 20 || i == 30 || i == 40 || i == 50)
                            {
                                list1 += "\n|[i/s" + targetplayer.TPlayer.bank4.item[i].stack + ":" + targetplayer.TPlayer.bank4.item[i].netID + "]|";
                            }
                            else
                            {
                                list1 += "[i/s" + targetplayer.TPlayer.bank4.item[i].stack + ":" + targetplayer.TPlayer.bank4.item[i].netID + "]|";
                            }
                        }
                        Player.SendMessage($"( [c/ffffff:{targetplayer.Name}] ) Void vault\n\n{list1}\n\n{targetplayerlogin}", Color.Purple);
                        return;
                    }
                case "all":
                    {
                        //inventory [main]
                        string inventory1 = "|";
                        string inventory2 = "|";
                        for (int i = 0; i < NetItem.InventorySlots; i++)
                        {
                            if (i < 10)
                            {
                                inventory1 += "[i/s" + targetplayer.TPlayer.inventory[i].stack + ":" + targetplayer.TPlayer.inventory[i].netID + "]|";
                            }
                            else
                            if (i < NetItem.InventorySlots)
                            {
                                if (i == 20 || i == 30 || i == 40 || i == 50)
                                {
                                    inventory2 += "\n|[i/s" + targetplayer.TPlayer.inventory[i].stack + ":" + targetplayer.TPlayer.inventory[i].netID + "]|";
                                }
                                else
                                {
                                    inventory2 += "[i/s" + targetplayer.TPlayer.inventory[i].stack + ":" + targetplayer.TPlayer.inventory[i].netID + "]|";
                                }
                            }
                        }

                        //equipment
                        string loadoutused = "( in use )loadout " + (targetplayer.TPlayer.CurrentLoadoutIndex + 1) + ":\n";
                        string loadout1 = "loadout 1:\n";
                        string loadout2 = "loadout 2:\n";
                        string loadout3 = "loadout 3:\n";
                        string misclist = "";
                        string e = $"{targetplayer.TPlayer.Loadouts[0].Armor[1]}";
                        for (int i = 0; i < 10; i++)
                        {
                            int ii = i + 10;
                            if (i < 5)
                            {
                                misclist += "|[i/s" + targetplayer.TPlayer.miscDyes[i].stack + ":" + targetplayer.TPlayer.miscDyes[i].netID + "]|[i/s" + targetplayer.TPlayer.miscEquips[i].stack + ":" + targetplayer.TPlayer.miscEquips[i].netID + "]|\n";
                            }
                            if (i < 3)
                            {
                                loadoutused += "|[i/s" + targetplayer.TPlayer.dye[i + 3].stack + ":" + targetplayer.TPlayer.dye[i + 3].netID + "]|[i/s" + targetplayer.TPlayer.armor[ii + 3].stack + ":" + targetplayer.TPlayer.armor[ii + 3].netID + "]|[i/s" + targetplayer.TPlayer.armor[i + 3].stack + ":" + targetplayer.TPlayer.armor[i + 3].netID + "]|====|[i/s" + targetplayer.TPlayer.dye[i].stack + ":" + targetplayer.TPlayer.dye[i].netID + "]|[i/s" + targetplayer.TPlayer.armor[ii].stack + ":" + targetplayer.TPlayer.armor[ii].netID + "]|[i/s" + targetplayer.TPlayer.armor[i].stack + ":" + targetplayer.TPlayer.armor[i].netID + "]|\n";
                                loadout1 += "|[i/s" + targetplayer.TPlayer.Loadouts[0].Dye[i + 3].stack + ":" + targetplayer.TPlayer.Loadouts[0].Dye[i + 3].netID + "]|[i/s" + targetplayer.TPlayer.Loadouts[0].Armor[ii + 3].stack + ":" + targetplayer.TPlayer.Loadouts[0].Armor[ii + 3].netID + "]|[i/s" + targetplayer.TPlayer.Loadouts[0].Armor[i + 3].stack + ":" + targetplayer.TPlayer.Loadouts[0].Armor[i + 3].netID + "]|====|[i/s" + targetplayer.TPlayer.Loadouts[0].Dye[i].stack + ":" + targetplayer.TPlayer.Loadouts[0].Dye[i].netID + "]|[i/s" + targetplayer.TPlayer.Loadouts[0].Armor[ii].stack + ":" + targetplayer.TPlayer.Loadouts[0].Armor[ii].netID + "]|[i/s" + targetplayer.TPlayer.Loadouts[0].Armor[i].stack + ":" + targetplayer.TPlayer.Loadouts[0].Armor[i].netID + "]|\n";
                                loadout2 += "|[i/s" + targetplayer.TPlayer.Loadouts[1].Dye[i + 3].stack + ":" + targetplayer.TPlayer.Loadouts[1].Dye[i + 3].netID + "]|[i/s" + targetplayer.TPlayer.Loadouts[1].Armor[ii + 3].stack + ":" + targetplayer.TPlayer.Loadouts[1].Armor[ii + 3].netID + "]|[i/s" + targetplayer.TPlayer.Loadouts[1].Armor[i + 3].stack + ":" + targetplayer.TPlayer.Loadouts[1].Armor[i + 3].netID + "]|====|[i/s" + targetplayer.TPlayer.Loadouts[1].Dye[i].stack + ":" + targetplayer.TPlayer.Loadouts[1].Dye[i].netID + "]|[i/s" + targetplayer.TPlayer.Loadouts[1].Armor[ii].stack + ":" + targetplayer.TPlayer.Loadouts[1].Armor[ii].netID + "]|[i/s" + targetplayer.TPlayer.Loadouts[1].Armor[i].stack + ":" + targetplayer.TPlayer.Loadouts[1].Armor[i].netID + "]|\n";
                                loadout3 += "|[i/s" + targetplayer.TPlayer.Loadouts[2].Dye[i + 3].stack + ":" + targetplayer.TPlayer.Loadouts[2].Dye[i + 3].netID + "]|[i/s" + targetplayer.TPlayer.Loadouts[2].Armor[ii + 3].stack + ":" + targetplayer.TPlayer.Loadouts[2].Armor[ii + 3].netID + "]|[i/s" + targetplayer.TPlayer.Loadouts[2].Armor[i + 3].stack + ":" + targetplayer.TPlayer.Loadouts[2].Armor[i + 3].netID + "]|====|[i/s" + targetplayer.TPlayer.Loadouts[2].Dye[i].stack + ":" + targetplayer.TPlayer.Loadouts[2].Dye[i].netID + "]|[i/s" + targetplayer.TPlayer.Loadouts[2].Armor[ii].stack + ":" + targetplayer.TPlayer.Loadouts[2].Armor[ii].netID + "]|[i/s" + targetplayer.TPlayer.Loadouts[2].Armor[i].stack + ":" + targetplayer.TPlayer.Loadouts[2].Armor[i].netID + "]|\n";
                            }
                            if (i < 6)
                            {
                                //skipping
                            }
                            else
                            {
                                loadoutused += "|[i/s" + targetplayer.TPlayer.dye[i].stack + ":" + targetplayer.TPlayer.dye[i].netID + "]|[i/s" + targetplayer.TPlayer.armor[ii].stack + ":" + targetplayer.TPlayer.armor[ii].netID + "]|[i/s" + targetplayer.TPlayer.armor[i].stack + ":" + targetplayer.TPlayer.armor[i].netID + "]|\n";
                                loadout1 += "|[i/s" + targetplayer.TPlayer.Loadouts[0].Dye[i].stack + ":" + targetplayer.TPlayer.Loadouts[0].Dye[i].netID + "]|[i/s" + targetplayer.TPlayer.Loadouts[0].Armor[ii].stack + ":" + targetplayer.TPlayer.Loadouts[0].Armor[ii].netID + "]|[i/s" + targetplayer.TPlayer.Loadouts[0].Armor[i].stack + ":" + targetplayer.TPlayer.Loadouts[0].Armor[i].netID + "]|\n";
                                loadout2 += "|[i/s" + targetplayer.TPlayer.Loadouts[1].Dye[i].stack + ":" + targetplayer.TPlayer.Loadouts[1].Dye[i].netID + "]|[i/s" + targetplayer.TPlayer.Loadouts[1].Armor[ii].stack + ":" + targetplayer.TPlayer.Loadouts[1].Armor[ii].netID + "]|[i/s" + targetplayer.TPlayer.Loadouts[1].Armor[i].stack + ":" + targetplayer.TPlayer.Loadouts[1].Armor[i].netID + "]|\n";
                                loadout3 += "|[i/s" + targetplayer.TPlayer.Loadouts[2].Dye[i].stack + ":" + targetplayer.TPlayer.Loadouts[2].Dye[i].netID + "]|[i/s" + targetplayer.TPlayer.Loadouts[2].Armor[ii].stack + ":" + targetplayer.TPlayer.Loadouts[2].Armor[ii].netID + "]|[i/s" + targetplayer.TPlayer.Loadouts[2].Armor[i].stack + ":" + targetplayer.TPlayer.Loadouts[2].Armor[i].netID + "]|\n";
                            }
                        }
                        switch (targetplayer.TPlayer.CurrentLoadoutIndex)
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

                        //piggy
                        string piggybank = "|";
                        string safe = "|";
                        string forge = "|";
                        string voidvault = "|";
                        for (int i = 0; i < 40; i++)
                        {
                            if (i == 10 || i == 20 || i == 30 || i == 40 || i == 50)
                            {
                                piggybank += "\n|[i/s" + targetplayer.TPlayer.bank.item[i].stack + ":" + targetplayer.TPlayer.bank.item[i].netID + "]|";
                                safe += "\n|[i/s" + targetplayer.TPlayer.bank2.item[i].stack + ":" + targetplayer.TPlayer.bank2.item[i].netID + "]|";
                                forge += "\n|[i/s" + targetplayer.TPlayer.bank3.item[i].stack + ":" + targetplayer.TPlayer.bank3.item[i].netID + "]|";
                                voidvault += "\n|[i/s" + targetplayer.TPlayer.bank4.item[i].stack + ":" + targetplayer.TPlayer.bank4.item[i].netID + "]|";
                            }
                            else
                            {
                                piggybank += "[i/s" + targetplayer.TPlayer.bank.item[i].stack + ":" + targetplayer.TPlayer.bank.item[i].netID + "]|";
                                safe += "[i/s" + targetplayer.TPlayer.bank2.item[i].stack + ":" + targetplayer.TPlayer.bank2.item[i].netID + "]|";
                                forge += "[i/s" + targetplayer.TPlayer.bank3.item[i].stack + ":" + targetplayer.TPlayer.bank3.item[i].netID + "]|";
                                voidvault += "[i/s" + targetplayer.TPlayer.bank4.item[i].stack + ":" + targetplayer.TPlayer.bank4.item[i].netID + "]|";
                            }
                        }
                        Player.SendMessage($"( [c/ffffff:{targetplayer.Name}] ) Inventory:\n{inventory1}\n{inventory2}\ntrashslot: [i/s{Player.TPlayer.trashItem.stack}:{Player.TPlayer.trashItem.netID}]\n\n" + 
                            $"Equipment:\n{loadout1}\n{loadout2}\n{loadout3}\nmisc:\n{misclist}\n\n" + 
                            $"piggy bank:\n{piggybank}\n\n" + 
                            $"safe:\n{safe}\n\n" + 
                            $"defender\'s forge:\n{forge}\n\n" + 
                            $"void vault:\n{voidvault}\n\n" + 
                            $"{targetplayerlogin}", Color.Gray);
                        return;
                    }
                default:
                    {
                        Player.SendErrorMessage("Invalid type!");
                        Player.SendMessage("[c/0000f4:List of type]" +
                            "[c/f4f400:inventory,inv]\ninfo: shows the player's backpack\n\n" +
                            "[c/f4f400:equipment,equip]\ninfo: shows the player's accessories/armor/misc and etc...\n\n" +
                            "[c/f4f400:piggybank,piggy,pig]\ninfo: shows the player's piggy bank\n\n" +
                            "[c/f4f400:safe]\ninfo: shows the player's safe\n\n" +
                            "[c/f4f400:defenderforge,forge]\ninfo: shows the player's defender's forge\n\n" +
                            "[c/f4f400:voidvault,void,vault]\ninfo: shows the player's void vault\n\n" +
                            "[c/f4f400:all]\ninfo: shows All contents of a players... [c/f40000:warning: this can flood your chat message]", Color.WhiteSmoke);
                        return;
                    }

            }
            return;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Deregister hooks here
            }
            base.Dispose(disposing);
        }
    }
}