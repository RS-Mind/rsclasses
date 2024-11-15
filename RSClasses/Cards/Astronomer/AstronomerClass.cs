using ClassesManagerReborn;
using System.Collections;

namespace RSClasses.Cards.Astronomer
{
    class AstronomerClass : ClassHandler
    {
        internal static string name = "<color=#ffff00>Astro</color>";
        internal static string nameGuardian = "<color=#66ffff>Astro</color>";
        internal static string nameHarvester = "<color=#b300ff>Astro</color>";

        public override IEnumerator Init()
        {
            CardInfo classCard = null;
            while (!(Astronomer.Card && DarkHarvest.Card && DomainExtension.Card && DualShields.Card && FasterShields.Card && GravityWell.Card && Guardian.Card && Harvester.Card && HarvestSickle.Card && PerfectGuard.Card && SharperScythes.Card && TwinScythes.Card)) yield return null;
            ClassesRegistry.Register(Astronomer.Card,       CardType.Entry);
            ClassesRegistry.Register(DomainExtension.Card,  CardType.Card,      Astronomer.Card);
            ClassesRegistry.Register(FasterShields.Card,    CardType.Card,      Astronomer.Card);
            ClassesRegistry.Register(GravityWell.Card,      CardType.Card,      Astronomer.Card);
            ClassesRegistry.Register(SharperScythes.Card,   CardType.Card,      Astronomer.Card);
            ClassesRegistry.Register(DualShields.Card,      CardType.Gate,      Astronomer.Card);
            ClassesRegistry.Register(TwinScythes.Card,      CardType.Gate,      Astronomer.Card);
            ClassesRegistry.Register(Guardian.Card,         CardType.SubClass,  new CardInfo[] { DualShields.Card, TwinScythes.Card });
            ClassesRegistry.Register(PerfectGuard.Card,     CardType.Card,      Guardian.Card);
            ClassesRegistry.Register(ShieldSpikes.Card,     CardType.Card,      Guardian.Card);
            ClassesRegistry.Register(Harvester.Card,        CardType.SubClass,  new CardInfo[] { DualShields.Card, TwinScythes.Card });
            ClassesRegistry.Register(HarvestSickle.Card,    CardType.Card,      Harvester.Card);
            ClassesRegistry.Register(DarkHarvest.Card,      CardType.Card,      HarvestSickle.Card);
        }

        public override IEnumerator PostInit()
        {
            ClassesRegistry.Get(FasterShields.Card).Blacklist(Harvester.Card);
            ClassesRegistry.Get(ShieldSpikes.Card).Blacklist(Harvester.Card);
            ClassesRegistry.Get(SharperScythes.Card).Blacklist(Guardian.Card);
            yield break;
        }
    }
}
