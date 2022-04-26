using ClassesManagerReborn;
using System.Collections;
using UnboundLib.Cards;

namespace RSClasses.Cards.Astronomer
{
    class AstronomerClass : ClassHandler
    {
        internal static string name = "Astro";

        public override IEnumerator Init()
        {
            CardInfo classCard = null;
            CustomCard.BuildCard<Astronomer>((card) => { ClassesRegistry.Register(card, CardType.Entry); classCard = card; });
            while (classCard == null) yield return null;
            CustomCard.BuildCard<DomainExtension>((card) => ClassesRegistry.Register(card, CardType.Card, classCard));
            CustomCard.BuildCard<FasterShields>((card) => ClassesRegistry.Register(card, CardType.Card, classCard));
            CustomCard.BuildCard<GravityWell>((card) => ClassesRegistry.Register(card, CardType.Card, classCard));
            CustomCard.BuildCard<SharperScythes>((card) => ClassesRegistry.Register(card, CardType.Card, classCard));
            CardInfo dualShields = null;
            CardInfo twinScythes = null;
            CustomCard.BuildCard<DualShields>((card) => { ClassesRegistry.Register(card, CardType.Branch, classCard, 1); dualShields = card; });
            CustomCard.BuildCard<TwinScythes>((card) => { ClassesRegistry.Register(card, CardType.Branch, classCard, 1); twinScythes = card; });
            while (dualShields == null || twinScythes == null) yield return null;
            CardInfo[] coreCards = new CardInfo[2] { dualShields, twinScythes };
            CardInfo guardian = null;
            CustomCard.BuildCard<Guardian>((card) => { ClassesRegistry.Register(card, CardType.SubClass, coreCards, 1); guardian = card; });
            while (guardian == null) yield return null;
            CustomCard.BuildCard<PerfectGuard>((card) => ClassesRegistry.Register(card, CardType.Card, guardian, 1));
            CardInfo harvester = null;
            CustomCard.BuildCard<Harvester>((card) => { ClassesRegistry.Register(card, CardType.SubClass, coreCards, 1); harvester = card; });
            while (harvester == null) yield return null;
            CustomCard.BuildCard<DarkHarvest>((card) => ClassesRegistry.Register(card, CardType.Card, harvester, 1));
        }
    }
}
