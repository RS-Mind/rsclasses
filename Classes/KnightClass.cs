using ClassesManagerReborn;
using System.Collections;

// Handles class setup for Knight

namespace RSClasses
{
    class KnightClass : ClassHandler
    {
        public override IEnumerator Init()
        {
            CardInfo classCard = CardHolder.cards["Knight"];
            ClassesRegistry.Register(CardHolder.cards["Knight"], CardType.Entry);
            ClassesRegistry.Register(CardHolder.cards["Longsword"], CardType.Card, CardHolder.cards["Knight"]);
            ClassesRegistry.Register(CardHolder.cards["Lunge"], CardType.Gate, CardHolder.cards["Knight"]);
            ClassesRegistry.Register(CardHolder.cards["Parry"], CardType.Gate, CardHolder.cards["Knight"]);
            ClassesRegistry.Register(CardHolder.cards["Crusader"], CardType.SubClass, CardHolder.cards["Parry"]);
            ClassesRegistry.Register(CardHolder.cards["Heavily Armored"], CardType.Card, CardHolder.cards["Crusader"]);
            ClassesRegistry.Register(CardHolder.cards["Divine Smite"], CardType.Gate, CardHolder.cards["Crusader"]);
            ClassesRegistry.Register(CardHolder.cards["Shield Bash"], CardType.Card, CardHolder.cards["Crusader"]);
            ClassesRegistry.Register(CardHolder.cards["Absolute Kill"], CardType.Card, CardHolder.cards["Divine Smite"]);
            ClassesRegistry.Register(CardHolder.cards["Spectre"], CardType.SubClass, CardHolder.cards["Lunge"]);
            ClassesRegistry.Register(CardHolder.cards["Ghostly Form"], CardType.Card, CardHolder.cards["Spectre"]);
            ClassesRegistry.Register(CardHolder.cards["Refine Blade"], CardType.Card, CardHolder.cards["Spectre"]);
            ClassesRegistry.Register(CardHolder.cards["Spectral Saber"], CardType.Gate, CardHolder.cards["Spectre"]);
            ClassesRegistry.Register(CardHolder.cards["Phantom Lunge"], CardType.Card, CardHolder.cards["Spectral Saber"]);
            yield return null;
        }
    }
}
