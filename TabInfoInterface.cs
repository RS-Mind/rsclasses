using System;
using HarmonyLib;
using TabInfo.Utils;
using UnboundLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using RSClasses.Extensions;

namespace RSClasses
{
    public class TabinfoInterface
    {
        public static void Setup()
        {
            var astronomerCategory = TabInfoManager.RegisterCategory("Astronomer Stats", 2);
            TabInfoManager.RegisterStat(astronomerCategory, "\r\rScythe Count", (p) => p.data.GetAdditionalData().scytheCount > 0, (p) => $"{p.data.GetAdditionalData().scytheCount}");
            TabInfoManager.RegisterStat(astronomerCategory, "\r\r\rScythe Damage", (p) => p.data.GetAdditionalData().scytheCount > 0, (p) => $"{p.data.GetAdditionalData().scytheDamage}");
            TabInfoManager.RegisterStat(astronomerCategory, "\r\rScythe Speed", (p) => p.data.GetAdditionalData().scytheCount > 0, (p) => $"{p.data.GetAdditionalData().scytheSpeed / 250f}");
            TabInfoManager.RegisterStat(astronomerCategory, "Barrier Count", (p) => p.data.GetAdditionalData().barrierCount > 0, (p) => $"{p.data.GetAdditionalData().barrierCount}");
            TabInfoManager.RegisterStat(astronomerCategory, "Barrier Speed", (p) => p.data.GetAdditionalData().barrierCount > 0, (p) => $"{p.data.GetAdditionalData().barrierSpeed / 100f}");
            TabInfoManager.RegisterStat(astronomerCategory, "Orbital Size", (p) => p.data.GetAdditionalData().barrierCount > 0 || p.data.GetAdditionalData().scytheCount > 0, (p) => $"{p.data.GetAdditionalData().orbitalRadius}");
            TabInfoManager.RegisterStat(astronomerCategory, "\rComet Count", (p) => p.data.GetAdditionalData().cometCount > 0, (p) => $"{p.data.GetAdditionalData().cometCount}");
            TabInfoManager.RegisterStat(astronomerCategory, "\rComet Speed", (p) => p.data.GetAdditionalData().cometCount > 0, (p) => $"{p.data.GetAdditionalData().cometSpeed / 2f}");
            var mirrorCategory = TabInfoManager.RegisterCategory("Mirror Mage Stats", 2);
            TabInfoManager.RegisterStat(mirrorCategory, "Reflection Replacement Cooldown", (p) => p.data.currentCards.Contains(CardHolder.cards["Reflection Replacement"]), (p) => $"{p.data.GetAdditionalData().reflectionCooldown}");
            TabInfoManager.RegisterStat(mirrorCategory, "Fracture Duration", (p) => p.data.currentCards.Contains(CardHolder.cards["Fracture"]), (p) => $"{p.data.GetAdditionalData().fractureDuration}");
            TabInfoManager.RegisterStat(mirrorCategory, "Fracture Size", (p) => p.data.currentCards.Contains(CardHolder.cards["Fracture"]), (p) => $"{p.data.GetAdditionalData().fractureSize / 0.0225f}");
        }
    }
}