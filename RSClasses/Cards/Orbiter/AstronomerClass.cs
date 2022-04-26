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
            while (!(Astronomer.Card && DarkHarvest.Card && DomainExtension.Card && DualShields.Card && FasterShields.Card && GravityWell.Card && Guardian.Card && Harvester.Card && PerfectGuard.Card && SharperScythes.Card && TwinScythes.Card)) yield return null;
            ClassesRegistry.Register(Astronomer.Card, CardType.Entry, 1);
            ClassesRegistry.Register(DomainExtension.Card, CardType.Card, Astronomer.Card);
            ClassesRegistry.Register(FasterShields.Card, CardType.Card, Astronomer.Card);
            ClassesRegistry.Register(GravityWell.Card, CardType.Card, Astronomer.Card);
            ClassesRegistry.Register(SharperScythes.Card, CardType.Card, Astronomer.Card);
            ClassesRegistry.Register(DualShields.Card, CardType.Branch, Astronomer.Card);
            ClassesRegistry.Register(TwinScythes.Card, CardType.Branch, Astronomer.Card);
            ClassesRegistry.Register(Guardian.Card, CardType.SubClass, new CardInfo[] { DualShields.Card, TwinScythes.Card, Astronomer.Card });
            ClassesRegistry.Register(PerfectGuard.Card, CardType.Card, new CardInfo[] {DualShields.Card, TwinScythes.Card, Astronomer.Card, Guardian.Card }, 1);
            ClassesRegistry.Register(Harvester.Card, CardType.SubClass, new CardInfo[] { DualShields.Card, TwinScythes.Card, Astronomer.Card });
            ClassesRegistry.Register(DarkHarvest.Card, CardType.Card, new CardInfo[] { DualShields.Card, TwinScythes.Card, Astronomer.Card, Harvester.Card }, 1);

        }
    }
}
