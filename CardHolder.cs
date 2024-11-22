using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using ModdingUtils.Utils;
using RSClasses.Cards;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnboundLib.Cards;
using UnboundLib.Utils;
using UnityEngine;

namespace RSClasses {
    public class CardHolder : MonoBehaviour
    {
        public List<GameObject> Cards;
        public List<GameObject> HiddenCards;
        public static Dictionary<string, CardInfo> cards = new Dictionary<string, CardInfo> ();

        internal void RegisterCards()
        {
            foreach (var Card in Cards)
            {
                CustomCard.RegisterUnityCard(Card, RSClasses.ModInitials, Card.GetComponent<CardInfo>().cardName, true, null);
                CustomCardCategories.instance.UpdateAndPullCategoriesFromCard(Card.GetComponent<CardInfo>());
                cards.Add(Card.GetComponent<CardInfo>().cardName, Card.GetComponent<CardInfo>());
            }
            foreach (var Card in HiddenCards)
            {
                CustomCard.RegisterUnityCard(Card, RSClasses.ModInitials, Card.GetComponent<CardInfo>().cardName, false, null);
                CustomCardCategories.instance.UpdateAndPullCategoriesFromCard(Card.GetComponent<CardInfo>());
                ModdingUtils.Utils.Cards.instance.AddHiddenCard(Card.GetComponent<CardInfo>());
                cards.Add(Card.GetComponent<CardInfo>().cardName, Card.GetComponent<CardInfo>());
            }
        }
    }
}