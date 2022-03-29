using RSCards.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using UnboundLib.Cards;
using UnboundLib.Extensions;
using UnboundLib.Networking;
using UnboundLib.Utils;
using UnityEngine;

namespace RSCards.Cards
{
    class HarmingField : CustomCard
    {
        private static float rangePerCard = 3f;
        private class HarmingFieldSpawner : MonoBehaviour
        {
            void Start()
            {
                if (!(this.gameObject.GetComponent<SpawnedAttack>().spawner != null))
                {
                    return;
                }

                //this.gameObject.transform.localScale = new Vector3(1f, 1f, 1f) * (this.gameObject.GetComponent<SpawnedAttack>().spawner.GetComponent<Block>().GetAdditionalData().harmingFieldRange / HarmingField.rangePerCard);

                //this.gameObject.AddComponent<RemoveAfterSeconds>().seconds = 5f;
                //this.gameObject.transform.GetChild(1).GetComponent<LineEffect>().SetFieldValue("inited", false);
                //typeof(LineEffect).InvokeMember("Init",
                //    BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic,
                //    null, this.gameObject.transform.GetChild(1).GetComponent<LineEffect>(), new object[] { });
                //this.gameObject.transform.GetChild(1).GetComponent<LineEffect>().radius = (HarmingField.rangePerCard - 1.4f);
                //this.gameObject.transform.GetChild(1).GetComponent<LineEffect>().SetFieldValue("startWidth", 0.5f);
                //this.gameObject.transform.GetChild(1).GetComponent<LineEffect>().Play();
            }
        }
        private static GameObject HarmingFieldVisual_ = null;
        private static GameObject HarmingFieldVisual
        {
            get
            {
                if (HarmingFieldVisual_ != null) { return HarmingFieldVisual_; }
                else
                {
                    List<CardInfo> activecards = ((ObservableCollection<CardInfo>)typeof(CardManager).GetField("activeCards", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null)).ToList();
                    List<CardInfo> inactivecards = (List<CardInfo>)typeof(CardManager).GetField("inactiveCards", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
                    List<CardInfo> allcards = activecards.Concat(inactivecards).ToList();
                    GameObject E_HealingCircle = allcards.Where(card => card.cardName.ToLower() == "healing field").First().GetComponent<CharacterStatModifiers>().AddObjectToPlayer.GetComponent<SpawnObjects>().objectToSpawn[0];
                    //HarmingFieldVisual_ = UnityEngine.GameObject.Instantiate(E_HealingCircle, new Vector3(0, 100000f, 0f), Quaternion.identity);
                    //HarmingFieldVisual_.name = "E_HarmingField";
                    DontDestroyOnLoad(HarmingFieldVisual_);
                    HarmingFieldVisual_ = E_HealingCircle;
                    //foreach (ParticleSystem parts in HarmingFieldVisual_.GetComponentsInChildren<ParticleSystem>())
                    //{
                    //    parts.startColor = Color.red;
                    //}
                    //HarmingFieldVisual_.transform.GetChild(1).GetComponent<LineEffect>().colorOverTime.colorKeys = new GradientColorKey[] { new GradientColorKey(Color.green, 0f) };
                    //UnityEngine.GameObject.Destroy(HarmingFieldVisual_.transform.GetChild(2).gameObject);
                    //HarmingFieldVisual_.transform.GetChild(1).GetComponent<LineEffect>().offsetMultiplier = 0f;
                    //HarmingFieldVisual_.transform.GetChild(1).GetComponent<LineEffect>().playOnAwake = true;
                    //UnityEngine.GameObject.Destroy(HarmingFieldVisual_.GetComponent<FollowPlayer>());
                    //HarmingFieldVisual_.GetComponent<DelayEvent>().time = 3f;
                    //UnityEngine.GameObject.Destroy(HarmingFieldVisual_.GetComponent<SoundImplementation.SoundUnityEventPlayer>());
                    //UnityEngine.GameObject.Destroy(HarmingFieldVisual_.GetComponent<Explosion>());
                    //UnityEngine.GameObject.Destroy(HarmingFieldVisual_.GetComponent<Explosion_Overpower>());
                    //UnityEngine.GameObject.Destroy(discombobVisual_.GetComponent<SpawnedAttack>());
                    //HarmingFieldVisual_.GetComponent<RemoveAfterSeconds>().seconds = 4;
                    //HarmingFieldVisual_.AddComponent<HarmingFieldSpawner>();
                    return HarmingFieldVisual_;
                }
            }
            set { }
        }

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
            if (RSCards.Debug) { UnityEngine.Debug.Log($"[{RSCards.ModInitials}][Card] {GetTitle()} has been setup."); }
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Edits values on player when card is selected
            if (block.GetAdditionalData().harmingFieldRange == 0f)
            {
                List<CardInfo> activecards = ((ObservableCollection<CardInfo>)typeof(CardManager).GetField("activeCards", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null)).ToList();
                List<CardInfo> inactivecards = (List<CardInfo>)typeof(CardManager).GetField("inactiveCards", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
                List<CardInfo> allcards = activecards.Concat(inactivecards).ToList();
                GameObject E_HealingCircle = allcards.Where(card => card.cardName.ToLower() == "healing field").First().GetComponent<CharacterStatModifiers>().AddObjectToPlayer.GetComponent<SpawnObjects>().objectToSpawn[0];
                block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Combine(block.BlockAction, new Action<BlockTrigger.BlockTriggerType>(this.GetDoBlockAction(player, block)));
                HarmingFieldVisual_ = UnityEngine.GameObject.Instantiate(E_HealingCircle, new Vector3(0, 100000f, 0f), Quaternion.identity);
                DontDestroyOnLoad(HarmingFieldVisual_);
                block.objectsToSpawn.Add(HarmingFieldVisual_);
            }
            if (RSCards.Debug) { UnityEngine.Debug.Log($"[{RSCards.ModInitials}][Card] {GetTitle()} has been added to player {player.playerID}."); }
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Run when the card is removed from the player
            if (RSCards.Debug) { UnityEngine.Debug.Log($"[{RSCards.ModInitials}][Card] {GetTitle()} has been removed from player {player.playerID}."); }
        }

        public Action<BlockTrigger.BlockTriggerType> GetDoBlockAction(Player player, Block block)
        {
            return delegate (BlockTrigger.BlockTriggerType trigger)
            {
                if (trigger != BlockTrigger.BlockTriggerType.None)
                {
                    Vector2 pos = block.transform.position;
                    Player[] players = PlayerManager.instance.players.ToArray();

                    for (int i = 0; i < players.Length; i++)
                    {
                        // don't apply the effect to the player who activated it...
                        if (players[i].playerID == player.playerID) { continue; }

                        // apply to players within range, that are within line-of-sight
                        if (Vector2.Distance(pos, players[i].transform.position) < block.GetAdditionalData().harmingFieldRange && PlayerManager.instance.CanSeePlayer(player.transform.position, players[i]).canSee)
                        {
                            //NetworkingManager.RPC(typeof(HarmingField), "OnHarmingFieldActivate", new object[] { players[i].playerID, block.GetAdditionalData().discombobulateDuration });
                        }
                    }


                }
            };
        }

        protected override string GetTitle()
        {
            return "Harming Field";
        }
        protected override string GetDescription()
        {
            return "Blocking Creates a Harming Field";
        }
        protected override GameObject GetCardArt()
        {
            return null;
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
                    positive = true,
                    stat = "Effect",
                    amount = "No",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.ColdBlue;
        }
        public override string GetModName()
        {
            return RSCards.ModInitials;
        }
    }
}
