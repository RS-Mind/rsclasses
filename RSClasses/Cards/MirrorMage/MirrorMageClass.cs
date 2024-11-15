using ClassesManagerReborn;
using RSClasses.Cards.Astronomer;
using System.Collections;

namespace RSClasses.Cards.MirrorMage
{
    class MirrorMageClass : ClassHandler
    {
        internal static string name = "<color=#00ff00>Mirror</color>";
        internal static string nameKaleido = "<color=#ff00ff>Mirror</color>";
        internal static string nameVoidseer = "<color=#0055ff>Mirror</color>";

        public override IEnumerator Init()
        {
            CardInfo classCard = null;
            while (!(MirrorMage.Card && MirrorMind.Card && PolishedMirror.Card && Prism.Card && ReflectionReplacement.Card && Fracture.Card && Voidseer.Card && Shatter.Card && WeakenedMirror.Card 
                && ForcedReflection.Card && ForcedRefraction.Card && KaleidoWitch.Card && EmeraldGlitter.Card && RubyDust.Card && SapphireShards.Card && KaleidoParty.Card)) yield return null;
            ClassesRegistry.Register(MirrorMage.Card,               CardType.Entry);
            ClassesRegistry.Register(PolishedMirror.Card,           CardType.Card,      MirrorMage.Card);
            ClassesRegistry.Register(Prism.Card,                    CardType.Gate,      MirrorMage.Card);
            ClassesRegistry.Register(ReflectionReplacement.Card,    CardType.Gate,      MirrorMage.Card);
            ClassesRegistry.Register(MirrorMind.Card,               CardType.Card,      ReflectionReplacement.Card);
            ClassesRegistry.Register(Fracture.Card,                 CardType.Gate,      ReflectionReplacement.Card);
            ClassesRegistry.Register(Voidseer.Card,                 CardType.SubClass,  Fracture.Card);
            ClassesRegistry.Register(Shatter.Card,                  CardType.Card,      Voidseer.Card);
            ClassesRegistry.Register(WeakenedMirror.Card,           CardType.Card,      Voidseer.Card);
            ClassesRegistry.Register(ForcedReflection.Card,         CardType.Card,      Voidseer.Card);
            ClassesRegistry.Register(ForcedRefraction.Card,         CardType.Card, new CardInfo[] { Voidseer.Card, Prism.Card });
            ClassesRegistry.Register(KaleidoWitch.Card,             CardType.SubClass,  Prism.Card);
            ClassesRegistry.Register(EmeraldGlitter.Card,           CardType.Card,      KaleidoWitch.Card);
            ClassesRegistry.Register(RubyDust.Card,                 CardType.Card,      KaleidoWitch.Card);
            ClassesRegistry.Register(SapphireShards.Card,           CardType.Card,      KaleidoWitch.Card);
            ClassesRegistry.Register(KaleidoParty.Card,             CardType.Card,      KaleidoWitch.Card);
        }
    }
}
