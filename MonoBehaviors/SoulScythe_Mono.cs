using HarmonyLib;
using RSClasses.MonoBehaviours;
using UnboundLib;
using UnityEngine;

namespace RSClasses.MonoBehaviors
{
    internal class SoulScythe_Mono : MonoBehaviour
    {
        Player player;
        Scythe_Mono scythes;

        private void Start()
        {
            player = GetComponentInParent<Player>(); // Get player
            scythes = player.GetComponent<Scythe_Mono>();
        }

        public void OnKill()
        {
            GameObject scythe = new GameObject("Scythe", typeof(Scythe));
            scythe.transform.SetParent(player.transform);
            scythes.scythes.Add(scythe.GetComponent<Scythe>());
        }
    }

    [HarmonyPatch(typeof(HealthHandler), "RPCA_Die")]
    public class SoulScytheDeathPatch
    {
        static void Prefix(HealthHandler __instance)
        {
            CharacterData data = (CharacterData)__instance.GetFieldValue("data");
            Player killer = data.lastSourceOfDamage;
            if (killer != null)
            {
                var soulScythe = killer.gameObject.GetComponentInChildren<SoulScythe_Mono>();
                if (soulScythe != null)
                {
                    soulScythe.OnKill();
                }
            }
        }
    }
}
