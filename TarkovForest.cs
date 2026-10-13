using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace TarkovForestMod
{
    [BepInPlugin("com.spartak.tarkovforest", "Tarkov Forest", "1.0.0")]
    public class TarkovForestPlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            var harmony = new Harmony("com.spartak.tarkovforest");
            harmony.PatchAll();
            Logger.LogInfo("Tarkov Forest Mod успешно загружен!");
        }
    }

    // ХУК 1: Бесконечные патроны для МЦ 20-01
    [HarmonyPatch(typeof(TheForest.Items.Inventory.PlayerInventory), "LoseItem")]
    class InfiniteAmmoPatch
    {
        static bool Prefix(string itemName, int amount)
        {
            if (itemName.Contains("Ammo") || itemName.Contains("Shell"))
            {
                return false; 
            }
            return true;
        }
    }

    // ХУК 2: Здоровье аборигенов из Таркова (440 общее, 35 в голову)
    [HarmonyPatch(typeof(EnemyHealth), "Start")]
    class EnemyHealthPatch
    {
        static void Postfix(EnemyHealth __instance)
        {
            if (__instance != null)
            {
                __instance.Health = 440;
                __instance.MaxHealth = 440;
            }
        }
    }

    [HarmonyPatch(typeof(EnemyHealth), "HitHeadshot")]
    class EnemyHeadshotPatch
    {
        static bool Prefix(EnemyHealth __instance, int damage)
        {
            if (__instance != null)
            {
                __instance.Hit(500); 
                return false;
            }
            return true;
        }
    }

    // ХУК 3: Спавн дробовика МЦ 20-01 на пассажирском кресле в самолете
    [HarmonyPatch(typeof(TheForest.Utils.LocalPlayer), "Start")]
    class SpawnWeaponPatch
    {
        private static bool spawned = false;

        static void Postfix()
        {
            if (!spawned)
            {
                GameObject plane = GameObject.Find("PlaneWreckage") ?? GameObject.FindGameObjectWithTag("Respawn");
                
                if (plane != null)
                {
                    GameObject mc2001 = GameObject.CreatePrimitive(PrimitiveType.Cube); 
                    mc2001.name = "MC_20_01_Scavenger";
                    
                    mc2001.transform.position = plane.transform.position + new Vector3(1.5f, 0.5f, -2.0f);
                    mc2001.transform.localScale = new Vector3(0.2f, 0.2f, 1.2f); 
                    
                    var pickup = mc2001.AddComponent<BoxCollider>();
                    pickup.isTrigger = true;
                    
                    spawned = true;
                }
            }
        }
    }
}
