using ClassesManagerReborn.Util;
using InControl;
using Photon.Realtime;
using RSClasses.MonoBehaviors;
using RSClasses.MonoBehaviours;
using System.Collections;
using UnboundLib;
using UnboundLib.Cards;
using UnboundLib.Utils.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RSClasses.Cards.MirrorMage
{
    class Shatter : CustomCard
    {
        GameObject shatter;
        ClassNameMono className;

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
            cardInfo.allowMultiple = true;
            className = gameObject.GetOrAddComponent<ClassNameMono>();
            className.className = MirrorMageClass.nameVoidseer;
            if (RSClasses.Debug) { UnityEngine.Debug.Log($"[{RSClasses.ModInitials}][Card] {GetTitle()} has been setup."); }
        }

        private void Update()
        {
            if (shatter == null)
            {
                shatter = Instantiate(RSClasses.ArtAssets.LoadAsset<GameObject>("Shatter_Loop"));
                shatter.transform.SetPositionAndRotation(transform.position, transform.rotation);
                shatter.transform.localScale = new Vector3(0.0225f, 0.0225f, 1);
                shatter.GetComponent<Canvas>().sortingLayerName = "MostFront";
                var script = shatter.GetOrAddComponent<ShatterLoop>();
                script.parent = className;
                var anim = shatter.GetComponent<Animator>();
                anim.SetBool(0, true);
            }
            Vector3 pos = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
            shatter.transform.SetPositionAndRotation(pos, transform.rotation);
            if (GetComponentInChildren<Canvas>())
            {
                Vector3 localPos = transform.position + (transform.up * (GetComponentInChildren<Canvas>().gameObject.transform.lossyScale.x * 300f));
                shatter.transform.SetPositionAndRotation(localPos, transform.rotation);
                shatter.transform.localScale = new Vector3(GetComponentInChildren<Canvas>().gameObject.transform.lossyScale.x * 4f, GetComponentInChildren<Canvas>().gameObject.transform.lossyScale.y * 4f, 100f);
            }
            if (RSClasses.instance.pickPhase) { shatter.layer = 31; }
            else { shatter.layer = 10; }
            shatter.SetActive(className.isActiveAndEnabled);
        }
        private void OnDestroy()
        {
            Destroy(shatter);
        }

        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Edits values on player when card is selected
            var fracture = player.gameObject.GetOrAddComponent<ShatterTrigger>().mono;
            fracture.radius *= 1.5f;
            fracture.duration += 1f;
            if (RSClasses.Debug) { UnityEngine.Debug.Log($"[{RSClasses.ModInitials}][Card] {GetTitle()} has been added to player {player.playerID}."); }
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Run when the card is removed from the player
            var fracture = player.gameObject.GetOrAddComponent<ShatterTrigger>().mono;
            fracture.radius /= 1.5f;
            fracture.duration -= 1f;
            if (RSClasses.Debug) { UnityEngine.Debug.Log($"[{RSClasses.ModInitials}][Card] {GetTitle()} has been removed from player {player.playerID}."); }
        }

        internal static CardInfo Card = null;
        protected override string GetTitle()
        {
            return "Shatter";
        }
        protected override string GetDescription()
        {
            return "";
        }
        protected override GameObject GetCardArt()
        {
            return RSClasses.ArtAssets.LoadAsset<GameObject>("C_Shatter");
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Uncommon;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Fracture duration",
                    amount = "+1s",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Fracture size",
                    amount = "+50%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.DefensiveBlue;
        }
        public override string GetModName()
        {
            return RSClasses.ModInitials;
        }

        internal static IEnumerator PickStart()
        {
            RSClasses.instance.pickPhase = true;
            yield break;
        }

        internal static IEnumerator PickEnd()
        {
            RSClasses.instance.pickPhase = false;
            yield break;
        }
    }
}
