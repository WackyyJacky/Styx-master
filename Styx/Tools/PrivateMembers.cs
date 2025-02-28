using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Styx.Tools
{
    // TODO: Remove class, move members to suitable classes
    public static class PrivateMembers
    {
        public static Chat ChatInstance =>
            (Chat) typeof(Chat).GetField("Instance", BindingFlags.Public | BindingFlags.Static)?.GetValue(null);

        public static List<Shop> AvailableShops
        {
            get
            {
                return ((Dictionary<int, Shop>) typeof(Shops)
                        .GetField("map", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null))
                    .Select(kvp => kvp.Value).ToList();
            }
            // public static void Init(Dictionary<int, Shop> map);
        }

        public static List<MergeShop> AvailableMergeShops
        {
            get
            {
                return ((Dictionary<int, MergeShop>) typeof(MergeShops).GetField("map").GetValue(null))
                    .Select(kvp => kvp.Value).ToList();
            }
            // public static Dictionary<int, MergeShop> map = new Dictionary<int, MergeShop>();
        }

        public static List<Quest> AvailableQuests
        {
            get
            {
                return ((Dictionary<int, Quest>) typeof(Quests)
                        .GetField("map", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null))
                    .Select(kvp => kvp.Value).ToList();
            }
        }

        public static MethodInfo OnLoginClick =>
            typeof(LoginState).GetMethod("onLoginClick", BindingFlags.NonPublic | BindingFlags.Instance);

        public static MethodInfo OnPlayClick =>
            typeof(CharSelect).GetMethod("OnPlayClick", BindingFlags.NonPublic | BindingFlags.Instance);

        public static ServerInfo SelectedServer
        {
            get
            {
                CharSelect cs;
                if ((cs = Object.FindObjectOfType<CharSelect>()) == null) return null;
                return (ServerInfo) typeof(CharSelect)
                    .GetField("selectedServer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(cs);
            }
            set
            {
                CharSelect cs;
                if ((cs = Object.FindObjectOfType<CharSelect>()) != null)
                    typeof(CharSelect).GetField("selectedServer", BindingFlags.NonPublic | BindingFlags.Instance)
                        .SetValue(cs, value);
            }
        }

        public static bool IsAssetTransforming => (bool) Entities.GetInstance().me.GetType()
            .GetField("isAssetTransforming", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(Entities.GetInstance().me);

        public static Loader LoaderInstance => (Loader) typeof(Loader)
            .GetField("mInstance", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);

        public static UIOptions OptionsInstance => (UIOptions) typeof(UIOptions)
            .GetField("mInstance", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);

        public static UIShop ShopsIntsance => (UIShop) typeof(UIShop)
            .GetField("shopitems", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);

        public static float RespawnTime
        {
            get
            {
                var instance = Object.FindObjectOfType<UIRespawnTimer>();
                if (instance != null)
                    return (float) instance.GetType()
                        .GetField("remainingTime", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(instance);
                return 1.0f;
            }
        }

        public static float ItemCooldownTime
        {
            get
            {
                var instance = Object.FindObjectOfType<UIInventoryUseItem>();
                if (instance != null)
                    return (float) instance.GetType()
                        .GetField("remainingCoolDown", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(instance);
                return 1.0f;
            }
        }

        public static List<ComLoot> LootBags =>
            ((Dictionary<int, ComLoot>) typeof(LootChest).GetField("Bags", BindingFlags.NonPublic | BindingFlags.Static)
                .GetValue(null)).Select(l => l.Value).ToList();

        internal static void OnEntityListUpdated()
        {
            var e = new Entities();
            var mi = e.GetType().GetMethod("OnEntityListUpdated");
            mi.Invoke(e, new object[] { });
        }
    }
}