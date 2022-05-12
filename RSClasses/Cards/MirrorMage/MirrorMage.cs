using ClassesManagerReborn.Util;
using RSClasses.MonoBehaviours;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnboundLib.Cards;
using UnboundLib.GameModes;
using UnityEngine;

namespace RSClasses.Cards.MirrorMage
{
    class MirrorMage : CustomCard
    {
        static private IEnumerator TimeReflect(Player player)
        {
            UnityEngine.Debug.Log(player);
            if (!player.data.currentCards.Contains(Card)) yield break;
            player.transform.position = Vector3.Scale(new Vector3(-1, 1, 1), player.transform.position);
            yield break;
        }
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
            gun.reloadTimeAdd = 0.25f;

            cardInfo.allowMultiple = false;
            gameObject.GetOrAddComponent<ClassNameMono>();
            if (RSClasses.Debug) { UnityEngine.Debug.Log($"[{RSClasses.ModInitials}][Card] {GetTitle()} has been setup."); }
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Edits values on player when card is selected
            List<ObjectsToSpawn> objectsToSpawn = gun.objectsToSpawn.ToList();
            ObjectsToSpawn mirror = new ObjectsToSpawn { };
            mirror.AddToProjectile = new GameObject("MirrorSpawner", typeof(MirrorSpawner));
            objectsToSpawn.Add(mirror);

            gun.objectsToSpawn = objectsToSpawn.ToArray();

            player.gameObject.GetOrAddComponent<spawnBulletMono>();
            GameModeManager.AddHook($"TimeLoop-{player.playerID}-Load", gm => TimeReflect(player));
            if (RSClasses.Debug) { UnityEngine.Debug.Log($"[{RSClasses.ModInitials}][Card] {GetTitle()} has been added to player {player.playerID}."); }
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Run when the card is removed from the player
            GameModeManager.RemoveHook($"TimeLoop-{player.playerID}-Load", gm => TimeReflect(player));
            if (RSClasses.Debug) { UnityEngine.Debug.Log($"[{RSClasses.ModInitials}][Card] {GetTitle()} has been removed from player {player.playerID}."); }
        }

        internal static CardInfo Card = null;
        protected override string GetTitle()
        {
            return "Mirror Mage";
        }
        protected override string GetDescription()
        {
            return "Gain the ability to influence the mirror";
        }
        protected override GameObject GetCardArt()
        {
            return RSClasses.ArtAssets.LoadAsset<GameObject>("C_MirrorMage");
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Common;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Reload time",
                    amount = "+0.25s",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.PoisonGreen;
        }
        public override string GetModName()
        {
            return RSClasses.ModInitials;
        }
    }
}
