using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Styx.Tools
{
    public static class World
    {
        public static Player Me => Entities.GetInstance().me;

        public static List<Player> Staff
        {
            get
            {
                return Entities.GetInstance().getActivePlayers().Where(p => p.AccessLevel >= 50 && p.isMe == false)
                    .ToList();
            }
        }

        public static List<Player> Players
        {
            get
            {
                return Entities.GetInstance().getActivePlayers().Where(p => p.statsCurrent[EntityStats.Stat.Health] > 0)
                    .ToList();
            }
        }

        public static List<NPC> Monsters
        {
            get { return Entities.GetInstance().getActiveNPCs().Where(m => Me.CanAttack(m) && m.IsAlive()).ToList(); }
        }

        public static Player GetClosestPlayer()
        {
            var players = Players.Where(p => !p.isMe).ToList();
            var distances = players.Select(p => Me.wrapper.transform.position - p.wrapper.transform.position)
                .Select(dis => dis.magnitude).ToList();
            return distances.Count <= 0 ? null : players[distances.IndexOf(distances.Min())];
        }

        public static Player GetPlayer(string name)
        {
            return Players.FirstOrDefault(p => p.name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public static List<NPC> MonstersInRadius(float radius, Vector3 playerPos)
        {
            return Entities.GetInstance().getActiveNPCs().Where(m =>
                Me.CanAttack(m) && m.IsAlive() && m.HealthPercent > 0f &&
                (playerPos - m.wrapper.transform.position).magnitude <= radius).ToList();
        }

        public static bool IsAlive(this NPC npc)
        {
            return npc.statsCurrent[EntityStats.Stat.Health] > 0 && npc.serverState != Entity.State.Dead &&
                   npc.visualState != Entity.State.Dead;
        }

        public static NPC GetClosestMonster(string name)
        {
            List<NPC> monsters;

            if (name == "*") monsters = Monsters;
            else
                monsters = Monsters.Where(m =>
                    m.name.Equals(name, StringComparison.OrdinalIgnoreCase) && Me.CanAttack(m) && m.IsAlive() &&
                    m.HealthPercent > 0f).ToList();

            var distances = monsters.Select(mon => Me.wrapper.transform.position - mon.wrapper.transform.position)
                .Select(dis => dis.magnitude).ToList();
            return distances.Count <= 0 ? null : monsters[distances.IndexOf(distances.Min())];
        }

        public static NPC GetClosestMonsterNew(string name, float radius, Vector3 playerPos)
        {
            List<NPC> monsters;

            if (name == "*")
            {
                if (radius == 0 || playerPos == null)
                    monsters = Monsters.Where(m => Me.CanAttack(m) && m.IsAlive() && m.HealthPercent > 0f).ToList();
                else
                    monsters = Monsters.Where(m =>
                        Me.CanAttack(m) && m.IsAlive() && m.HealthPercent > 0f &&
                        (playerPos - m.wrapper.transform.position).magnitude <= radius).ToList();
            }
            else
            {
                if (radius == 0 || playerPos == null)
                    monsters = Monsters.Where(m =>
                        m.name.Equals(name, StringComparison.OrdinalIgnoreCase) && Me.CanAttack(m) && m.IsAlive() &&
                        m.HealthPercent > 0f).ToList();
                else
                    monsters = Monsters.Where(m =>
                            m.name.Equals(name, StringComparison.OrdinalIgnoreCase) && Me.CanAttack(m) && m.IsAlive() &&
                            m.HealthPercent > 0f && (playerPos - m.wrapper.transform.position).magnitude <= radius)
                        .ToList();
            }

            var distances = monsters.Select(mon => Me.wrapper.transform.position - mon.wrapper.transform.position)
                .Select(dis => dis.magnitude).ToList();
            return distances.Count <= 0 ? null : monsters[distances.IndexOf(distances.Min())];
        }

        public static NPC GetMonster(int id)
        {
            return Monsters.FirstOrDefault(m => m.ID.Equals(id));
        }

        public static NPC GetClosestNpc(string name)
        {
            List<NPC> npcs;

            if (name == "*") npcs = Entities.GetInstance().getActiveNPCs().ToList();
            else
                npcs = Entities.GetInstance().getActiveNPCs()
                    .Where(n => n.name.Equals(name, StringComparison.OrdinalIgnoreCase)).ToList();

            var distances = npcs.Select(npc => Me.wrapper.transform.position - npc.wrapper.transform.position)
                .Select(dis => dis.magnitude).ToList();
            return distances.Count <= 0 ? null : npcs[distances.IndexOf(distances.Min())];
        }

        public static InventoryItem GetInventoryItem(string name)
        {
            return Session.MyPlayerData.items.FirstOrDefault(i =>
                i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public static QuestObjective GetQuestObjective(string name)
        {
            return Session.MyPlayerData.CurQuests.Select(Quests.Get).Where(q => q != null).SelectMany(q => q.Objectives)
                .FirstOrDefault(qo => qo.Desc.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public static void TeleportToCursorPosition()
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit))
                Entities.GetInstance().me.wrapper.transform.position = hit.point;
        }

        public static bool PlayerHasValidTarget(NPC target)
        {
            if (Entities.GetInstance().me.CanAttack(target) && target.IsAlive() && target.HealthPercent > 0f)
                return true;
            return false;
        }
    }
}