﻿using ClassesManagerReborn;
using System.Collections;

// Handles class setup for Mirror Mage

namespace RSClasses.Cards.MirrorMage
{
    class MirrorMageClass : ClassHandler
    {
        public override IEnumerator Init()
        {
            CardInfo classCard = CardHolder.cards["Mirror Mage"];
            ClassesRegistry.Register(CardHolder.cards["Mirror Mage"],           CardType.Entry);
            ClassesRegistry.Register(CardHolder.cards["Polished Mirror"],       CardType.Card,      CardHolder.cards["Mirror Mage"]);
            ClassesRegistry.Register(CardHolder.cards["Prism"],                 CardType.Gate,      CardHolder.cards["Mirror Mage"]);
            ClassesRegistry.Register(CardHolder.cards["Reflection Replacement"],CardType.Gate,      CardHolder.cards["Mirror Mage"]);
            ClassesRegistry.Register(CardHolder.cards["Mirror Mind"],           CardType.Gate,      CardHolder.cards["Reflection Replacement"]);
            ClassesRegistry.Register(CardHolder.cards["Fracture"],              CardType.Gate,      CardHolder.cards["Reflection Replacement"]);
            ClassesRegistry.Register(CardHolder.cards["Voidseer"],              CardType.SubClass,  CardHolder.cards["Fracture"]);
            ClassesRegistry.Register(CardHolder.cards["Shatter"],               CardType.Card,      CardHolder.cards["Voidseer"]);
            ClassesRegistry.Register(CardHolder.cards["Weakened Mirror"],       CardType.Card,      CardHolder.cards["Voidseer"]);
            ClassesRegistry.Register(CardHolder.cards["Forced Reflection"],     CardType.Card,      CardHolder.cards["Voidseer"]);
            ClassesRegistry.Register(CardHolder.cards["Forced Refraction"],     CardType.Card, new CardInfo[] { CardHolder.cards["Voidseer"], CardHolder.cards["Prism"] });
            ClassesRegistry.Register(CardHolder.cards["Kaleido Witch"],         CardType.SubClass, new CardInfo[] { CardHolder.cards["Prism"], CardHolder.cards["Mirror Mind"] });
            ClassesRegistry.Register(CardHolder.cards["Emerald Glitter"],       CardType.Card,      CardHolder.cards["Kaleido Witch"]);
            ClassesRegistry.Register(CardHolder.cards["Ruby Dust"],             CardType.Card,      CardHolder.cards["Kaleido Witch"]);
            ClassesRegistry.Register(CardHolder.cards["Sapphire Shards"],       CardType.Card,      CardHolder.cards["Kaleido Witch"]);
            ClassesRegistry.Register(CardHolder.cards["Kaleido Party"],         CardType.Card,      CardHolder.cards["Kaleido Witch"]);
            yield return null;
        }
    }
}
