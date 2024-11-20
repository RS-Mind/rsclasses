using UnityEngine;
using Photon.Pun;

namespace RSCards.MonoBehaviors
{
    public class Repentence_Mono : MonoBehaviour
    {
        void Start()
        {
            Player player = GetComponentInParent<Player>();
            int repentenceCount = 0;
            foreach (CardInfo card in player.data.currentCards)
            {
                if (card.cardName == "Repentence") { repentenceCount++; }
            }
            if (repentenceCount == 9)
            {
                ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, ModdingUtils.Utils.Cards.instance.GetCardWithName("Repen10ce"), false, "", 0, 0);
            }
        }
	}
}
