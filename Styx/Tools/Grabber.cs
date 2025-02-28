using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Styx.Tools
{
    public class Grabber
    {
        public string GrabInventory()
        {
            var sb = new StringBuilder();

            foreach (var item in Session.MyPlayerData.items.Where(i => i != null))
            {
                sb.AppendLine($"Name: {item.Name}");
                sb.AppendLine($"Quantity: {item.Qty}");
                sb.AppendLine($"Char Item ID: {item.CharItemID}");
                sb.AppendLine($"ID: {item.ID}");
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public string GrabMachines()
        {
            var sb = new StringBuilder();
            var machines = (from p in BaseMachine.Map.Values where p is BaseMachine select p)
                .OrderBy(m => (World.Me.wrapper.transform.position - m.transform.position).magnitude).ToList();
            if (machines.Count >= 1)
                foreach (var m in machines)
                {
                    sb.AppendLine($"Name: {m.name}");
                    sb.AppendLine($"ID: {m.ID}");
                    sb.AppendLine(
                        $"Distance: {(World.Me.wrapper.transform.position - m.transform.position).magnitude}");
                    sb.AppendLine();
                }

            return sb.ToString();
        }

        public string GrabShops()
        {
            List<Shop> shops;
            if ((shops = PrivateMembers.AvailableShops) == null)
                return string.Empty;

            var sb = new StringBuilder();

            foreach (var s in shops.Where(shop => shop != null))
            {
                sb.AppendLine($"{s.Name} (ID: {s.ID})");
                foreach (var si in s.Items)
                {
                    sb.AppendLine($"Name: {si.Name}");
                    sb.AppendLine($"ID: {si.ID}");
                    sb.AppendLine($"Cost: {si.Cost} {si.CurrencyString}");
                    sb.AppendLine($"Guardian only: {si.IsGuardian}");
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        public string GrabMergeShops()
        {
            List<MergeShop> mergeShops;
            if ((mergeShops = PrivateMembers.AvailableMergeShops) == null)
                return string.Empty;

            var sb = new StringBuilder();

            foreach (var s in mergeShops.Where(mergeShop => mergeShop != null))
            {
                sb.AppendLine($"{s.Name} (ID: {s.MergeShopID})");
                var curmerges = (from p in s.Merges.Values orderby p.SortOrder select p).ToList();
                foreach (var si in curmerges)
                {
                    sb.AppendLine($"Name: {si.Name}");
                    sb.AppendLine($"ID: {si.ID}");
                    sb.AppendLine($"Guardian only: {si.IsGuardian}");
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        public string GrabQuests()
        {
            List<Quest> quests;

            if ((quests = PrivateMembers.AvailableQuests) == null)
                return string.Empty;

            var sb = new StringBuilder();

            foreach (var q in quests)
            {
                sb.AppendLine("----------------------------------------------------------------");
                sb.AppendLine($"Name: {q.Name}");
                sb.AppendLine($"ID: {q.ID}");
                sb.AppendLine($"Map ID: {q.MapID}");
                sb.AppendLine($"Repeatable: {q.IsRepeatable}");
                sb.AppendLine();
                sb.AppendLine("--- Objectives ---");
                foreach (var qo in q.Objectives)
                {
                    sb.AppendLine($"Name: {qo.Desc}");
                    sb.AppendLine($"Quantity: {qo.Qty}");
                    sb.AppendLine($"ID: {qo.ID}");
                    sb.AppendLine($"Map: {qo.MapName} (ID: {qo.MapID})");
                    sb.AppendLine();
                }

                sb.AppendLine("--- Rewards ---");
                foreach (var qr in q.Rewards)
                {
                    sb.AppendLine($"Name: {qr.Name}");
                    sb.AppendLine($"Quantity: {qr.Qty}");
                    sb.AppendLine($"ID: {qr.ID}");
                    sb.AppendLine();
                }

                sb.AppendLine("----------------------------------------------------------------");
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public string GrabMonsters()
        {
            List<NPC> monsters;
            if ((monsters = World.Monsters) == null)
                return string.Empty;

            var sb = new StringBuilder();

            foreach (var mon in monsters)
            {
                sb.AppendLine($"Name: {mon.name}");
                sb.AppendLine($"ID: {mon.ID}");
                sb.AppendLine($"NPC ID: {mon.NPCID}");
                sb.AppendLine($"Spawn ID: {mon.SpawnID}");
                sb.AppendLine($"Level: {mon.Level}");
                sb.AppendLine($"Health: {mon.statsCurrent[EntityStats.Stat.Health]}");
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}