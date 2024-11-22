using ClassesManagerReborn;
using System.Collections;

namespace RSClasses.Cards.Astronomer
{
    class AstronomerClass : ClassHandler
    {
        public override IEnumerator Init()
        {
            ClassesRegistry.Register(CardHolder.cards["Astronomer"],        CardType.Entry);
            ClassesRegistry.Register(CardHolder.cards["Domain Extension"],  CardType.Card, CardHolder.cards["Astronomer"]);
            ClassesRegistry.Register(CardHolder.cards["Faster Barriers"],   CardType.Card, CardHolder.cards["Astronomer"]);
            ClassesRegistry.Register(CardHolder.cards["Gravity Well"],      CardType.Card, CardHolder.cards["Astronomer"]);
            ClassesRegistry.Register(CardHolder.cards["Sharper Scythes"],   CardType.Card, CardHolder.cards["Astronomer"]);
            ClassesRegistry.Register(CardHolder.cards["Double Barriers"],   CardType.Gate, CardHolder.cards["Astronomer"]);
            ClassesRegistry.Register(CardHolder.cards["Twin Scythes"],      CardType.Gate, CardHolder.cards["Astronomer"]);
            ClassesRegistry.Register(CardHolder.cards["Guardian"],          CardType.SubClass,  new CardInfo[] { CardHolder.cards["Double Barriers"], CardHolder.cards["Twin Scythes"] });
            ClassesRegistry.Register(CardHolder.cards["Perfect Guard"],     CardType.Card, CardHolder.cards["Guardian"]);
            ClassesRegistry.Register(CardHolder.cards["Shield Spikes"],     CardType.Card, CardHolder.cards["Guardian"]);
            ClassesRegistry.Register(CardHolder.cards["Harvester"],         CardType.SubClass,  new CardInfo[] { CardHolder.cards["Double Barriers"], CardHolder.cards["Twin Scythes"] });
            ClassesRegistry.Register(CardHolder.cards["Harvest Sickle"],    CardType.Gate, CardHolder.cards["Harvester"]);
            ClassesRegistry.Register(CardHolder.cards["Dark Harvest"],      CardType.Card, CardHolder.cards["Harvest Sickle"]);
            yield return null;
        }
    }
}
