using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShockAPI;
using Terraria;
using TerrariaApi.Server;
using Microsoft.Xna.Framework;

namespace inventorychecker
{
    [ApiVersion(2, 1)]
    public class inventoryviewers : TerrariaPlugin
    {
        public override string Author => "Nightklp";

        public override string Description => "just some simple code so you dont to do it yourself";

        public override string Name => "Inventory viewer";

        public override Version Version => new Version(1, 0, 0, 0);

        public inventoryviewers(Main game) : base(game)
        {

        }

        public override void Initialize()
        {
            Commands.ChatCommands.Add(new Command("inventoryviewer.view", InventoryView, "inventoryview", "invview", "viewinv"));
        }

        public void InventoryView(CommandArgs args)
        {
            TSPlayer Player = args.Player;
            if (args.Parameters.Count != 1 && args.Parameters.Count != 2)
            {
                Player.SendErrorMessage("Invalid syntax. Proper syntax: /inventorycheck <player> <type>");
                return;
            }
            if (args.Parameters.Count == 0)
            {
                args.Player.SendErrorMessage("Specify a Player!");
                return;
            }
            if (args.Parameters.Count == 1)
            {
                args.Player.SendErrorMessage("Specify a type! type: <inventory,equipment>");
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
                case "help":
                    {
                        Player.SendInfoMessage("view inventory\n\n/inventoryview <Player> <type>\n\ntype: <inventory,equipment>\n\nexample:\n/inventoryview john inventory");
                        return;
                    }
                case "inventory":
                case "inv":
                    {
                        string list1 = "|";
                        string list2 = "|";
                        for (int i = 0; i < NetItem.InventorySlots; i++)
                        {
                            if (i < 10)
                            {
                                list1 = list1 + "[i/s" + targetplayer.TPlayer.inventory[i].stack + ":" + targetplayer.TPlayer.inventory[i].netID + "]|";
                            }
                            else
                            if (i < NetItem.InventorySlots)
                            {
                                if (i == 20 || i == 30 || i == 40 || i == 50)
                                {
                                    list2 = list2 + "\n[i/s" + targetplayer.TPlayer.inventory[i].stack + ":" + targetplayer.TPlayer.inventory[i].netID + "]|";
                                }
                                else
                                {
                                    list2 = list2 + "[i/s" + targetplayer.TPlayer.inventory[i].stack + ":" + targetplayer.TPlayer.inventory[i].netID + "]|";
                                }
                            }
                        }
                        Player.SendMessage($"[ {targetplayer.Name} ] inventory\n\nHotbar:\n{list1}\ninventory:\n{list2}\ntrashed:\n[i/s{Player.TPlayer.trashItem.stack}:{Player.TPlayer.trashItem.netID}]", Color.Gray);
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
                            if (i < 3)
                            {
                                list1 = list1 + "|[i/s" + targetplayer.TPlayer.dye[i + 3].stack + ":" + targetplayer.TPlayer.dye[i + 3].netID + "]|[i/s" + targetplayer.TPlayer.armor[ii + 3].stack + ":" + targetplayer.TPlayer.armor[ii + 3].netID + "]|[i/s" + targetplayer.TPlayer.armor[i + 3].stack + ":" + targetplayer.TPlayer.armor[i + 3].netID + "]|\t\t|[i/s" + targetplayer.TPlayer.dye[i].stack + ":" + targetplayer.TPlayer.dye[i].netID + "]|[i/s" + targetplayer.TPlayer.armor[ii].stack + ":" + targetplayer.TPlayer.armor[ii].netID + "]|[i/s" + targetplayer.TPlayer.armor[i].stack + ":" + targetplayer.TPlayer.armor[i].netID + "]|\n";
                            }
                            else
                            {
                                list1 = list1 + "|[i/s" + targetplayer.TPlayer.dye[i].stack + ":" + targetplayer.TPlayer.dye[i].netID + "]|[i/s" + targetplayer.TPlayer.armor[ii].stack + ":" + targetplayer.TPlayer.armor[ii].netID + "]|[i/s" + targetplayer.TPlayer.armor[i].stack + ":" + targetplayer.TPlayer.armor[i].netID + "]|\n";
                            }
                            if (i < 5)
                            {
                                list2 = list2 + "|[i/s" + targetplayer.TPlayer.miscDyes[i].stack + ":" + targetplayer.TPlayer.miscDyes[i].netID + "]|[i/s" + targetplayer.TPlayer.miscEquips[i].stack + ":" + targetplayer.TPlayer.miscEquips[i].netID + "]|\n";
                            }
                        }
                        Player.SendMessage($"[ {targetplayer.Name} ] Equipment:\n\narmor & accessory:\n{list1}\nmisc:\n{list2}", Color.Gray);
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