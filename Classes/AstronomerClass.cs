using ClassesManagerReborn;
using System.Collections;

// Handles class setup for Astronomer

namespace RSClasses.Cards.Astronomer
{
    class AstronomerClass : ClassHandler
    {
        public override IEnumerator Init()
        {
            CardInfo classCard = CardHolder.cards["Astronomer"];
            ClassesRegistry.Register(CardHolder.cards["Astronomer"],         CardType.Entry);
            ClassesRegistry.Register(CardHolder.cards["Domain Extension"],  CardType.Card, CardHolder.cards["Astronomer"]);
            ClassesRegistry.Register(CardHolder.cards["Faster Barriers"],   CardType.Card, CardHolder.cards["Astronomer"]);
            ClassesRegistry.Register(CardHolder.cards["Gravity Well"],      CardType.Card, CardHolder.cards["Astronomer"]);
            ClassesRegistry.Register(CardHolder.cards["Sharper Scythes"],   CardType.Card, CardHolder.cards["Astronomer"]);
            ClassesRegistry.Register(CardHolder.cards["Bigger Barriers"],   CardType.Gate, CardHolder.cards["Astronomer"]);
            ClassesRegistry.Register(CardHolder.cards["Twin Scythes"],      CardType.Gate, CardHolder.cards["Astronomer"]);
            ClassesRegistry.Register(CardHolder.cards["Stargazer"],         CardType.SubClass,  new CardInfo[] { CardHolder.cards["Bigger Barriers"], CardHolder.cards["Twin Scythes"] });
            ClassesRegistry.Register(CardHolder.cards["Coupled Comets"],    CardType.Card, CardHolder.cards["Stargazer"]);
            ClassesRegistry.Register(CardHolder.cards["Icemelt"],           CardType.Card, CardHolder.cards["Stargazer"]);
            ClassesRegistry.Register(CardHolder.cards["Stardust"],          CardType.Card, CardHolder.cards["Stargazer"]);
            ClassesRegistry.Register(CardHolder.cards["Stellar Impact"],    CardType.Card, CardHolder.cards["Stargazer"]);
            ClassesRegistry.Register(CardHolder.cards["Guardian"],          CardType.SubClass,  new CardInfo[] { CardHolder.cards["Bigger Barriers"], CardHolder.cards["Twin Scythes"] });
            ClassesRegistry.Register(CardHolder.cards["Perfect Guard"],     CardType.Card, CardHolder.cards["Guardian"]);
            ClassesRegistry.Register(CardHolder.cards["Shield Spikes"],     CardType.Card, CardHolder.cards["Guardian"]);
            ClassesRegistry.Register(CardHolder.cards["Harvester"],         CardType.SubClass,  new CardInfo[] { CardHolder.cards["Bigger Barriers"], CardHolder.cards["Twin Scythes"] });
            ClassesRegistry.Register(CardHolder.cards["Harvest Sickle"],    CardType.Gate, CardHolder.cards["Harvester"]);
            ClassesRegistry.Register(CardHolder.cards["Dark Harvest"],      CardType.Card, CardHolder.cards["Harvest Sickle"]);
            yield return null;
        }
    }
}