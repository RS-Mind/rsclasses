using ClassesManagerReborn;
using System.Collections;

namespace RSClasses.Cards.MirrorMage
{
    class MirrorMageClass : ClassHandler
    {
        internal static string name = "Mirror";

        public override IEnumerator Init()
        {
            CardInfo classCard = null;
            while (!(MirrorMage.Card && MirrorMind.Card && PolishedMirror.Card && Prism.Card && ReflectionReplacement.Card && Shatter.Card)) yield return null;
            ClassesRegistry.Register(MirrorMage.Card, CardType.Entry);
            ClassesRegistry.Register(MirrorMind.Card, CardType.Card, ReflectionReplacement.Card);
            ClassesRegistry.Register(PolishedMirror.Card, CardType.Card, MirrorMage.Card);
            ClassesRegistry.Register(Prism.Card, CardType.Card, MirrorMage.Card);
            ClassesRegistry.Register(ReflectionReplacement.Card, CardType.Gate, MirrorMage.Card);
            ClassesRegistry.Register(Shatter.Card, CardType.Card, ReflectionReplacement.Card);
            // Card that causes your gravity to face the prism line
        }
    }
}
