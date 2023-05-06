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
            Commands.ChatCommands.Add(new Command("inventoryviewer.view", InventoryView, "inventoryview", "invview", "viewinv", "inview"));
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
                args.Player.SendErrorMessage("Specify a type! type: <inventory,equipment,piggybank,safe,defenderforge,voidvault>");
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
                                    list2 += "\n[i/s" + targetplayer.TPlayer.inventory[i].stack + ":" + targetplayer.TPlayer.inventory[i].netID + "]|";
                                }
                                else
                                {
                                    list2 += "[i/s" + targetplayer.TPlayer.inventory[i].stack + ":" + targetplayer.TPlayer.inventory[i].netID + "]|";
                                }
                            }
                        }
                        Player.SendMessage($"( [c/ffffff:{targetplayer.Name}] ) inventory\n\nHotbar:\n{list1}\ninventory:\n{list2}\ntrash slot:\n[i/s{Player.TPlayer.trashItem.stack}:{Player.TPlayer.trashItem.netID}]", Color.WhiteSmoke);
                        return;
                    }
                case "equipment":
                case "equip":
                    {
                        string list1 = "";
                        string list2 = "";
                        for (int i = 0; i < 10; i++)
                        {
                            int ii = i + 10;
                            if (i < 5)
                            {
                                list2 += "|[i/s" + targetplayer.TPlayer.miscDyes[i].stack + ":" + targetplayer.TPlayer.miscDyes[i].netID + "]|[i/s" + targetplayer.TPlayer.miscEquips[i].stack + ":" + targetplayer.TPlayer.miscEquips[i].netID + "]|\n";
                            }
                            if (i < 3)
                            {
                                list1 += "|[i/s" + targetplayer.TPlayer.dye[i + 3].stack + ":" + targetplayer.TPlayer.dye[i + 3].netID + "]|[i/s" + targetplayer.TPlayer.armor[ii + 3].stack + ":" + targetplayer.TPlayer.armor[ii + 3].netID + "]|[i/s" + targetplayer.TPlayer.armor[i + 3].stack + ":" + targetplayer.TPlayer.armor[i + 3].netID + "]|\t\t|[i/s" + targetplayer.TPlayer.dye[i].stack + ":" + targetplayer.TPlayer.dye[i].netID + "]|[i/s" + targetplayer.TPlayer.armor[ii].stack + ":" + targetplayer.TPlayer.armor[ii].netID + "]|[i/s" + targetplayer.TPlayer.armor[i].stack + ":" + targetplayer.TPlayer.armor[i].netID + "]|\n";
                            }
                            if (i < 6)
                            {
                                //skipping
                            }
                            else
                            {
                                list1 += "|[i/s" + targetplayer.TPlayer.dye[i].stack + ":" + targetplayer.TPlayer.dye[i].netID + "]|[i/s" + targetplayer.TPlayer.armor[ii].stack + ":" + targetplayer.TPlayer.armor[ii].netID + "]|[i/s" + targetplayer.TPlayer.armor[i].stack + ":" + targetplayer.TPlayer.armor[i].netID + "]|\n";
                            }
                        }
                        Player.SendMessage($"( [c/ffffff:{targetplayer.Name}] ) Equipment:\n\narmor & accessory:\n{list1}\nmisc:\n{list2}", Color.Green);
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
                                list1 += "\n[i/s" + targetplayer.TPlayer.bank.item[i].stack + ":" + targetplayer.TPlayer.bank.item[i].netID + "]|";
                            }
                            else
                            {
                                list1 += "[i/s" + targetplayer.TPlayer.bank.item[i].stack + ":" + targetplayer.TPlayer.bank.item[i].netID + "]|";
                            }
                        }
                        Player.SendMessage($"( [c/ffffff:{targetplayer.Name}] ) Piggy Bank\n\n{list1}", Color.Pink);
                        return;
                    }
                case "safe":
                    {
                        string list1 = "|";
                        for (int i = 0; i < 40; i++)
                        {
                            if (i == 10 || i == 20 || i == 30 || i == 40 || i == 50)
                            {
                                list1 += "\n[i/s" + targetplayer.TPlayer.bank2.item[i].stack + ":" + targetplayer.TPlayer.bank2.item[i].netID + "]|";
                            }
                            else
                            {
                                list1 += "[i/s" + targetplayer.TPlayer.bank2.item[i].stack + ":" + targetplayer.TPlayer.bank2.item[i].netID + "]|";
                            }
                        }
                        Player.SendMessage($"( [c/ffffff:{targetplayer.Name}] ) Safe\n\n{list1}", Color.Gray);
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
                                list1 += "\n[i/s" + targetplayer.TPlayer.bank3.item[i].stack + ":" + targetplayer.TPlayer.bank3.item[i].netID + "]|";
                            }
                            else
                            {
                                list1 += "[i/s" + targetplayer.TPlayer.bank3.item[i].stack + ":" + targetplayer.TPlayer.bank3.item[i].netID + "]|";
                            }
                        }
                        Player.SendMessage($"( [c/ffffff:{targetplayer.Name}] ) defender's forge\n\n{list1}", Color.Yellow);
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
                                list1 += "\n[i/s" + targetplayer.TPlayer.bank4.item[i].stack + ":" + targetplayer.TPlayer.bank4.item[i].netID + "]|";
                            }
                            else
                            {
                                list1 += "[i/s" + targetplayer.TPlayer.bank4.item[i].stack + ":" + targetplayer.TPlayer.bank4.item[i].netID + "]|";
                            }
                        }
                        Player.SendMessage($"( [c/ffffff:{targetplayer.Name}] ) Void vault\n\n{list1}", Color.Purple);
                        return;
                    }
                default:
                    {
                        Player.SendErrorMessage("Invalid type! type: <inventory,equipment>");
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