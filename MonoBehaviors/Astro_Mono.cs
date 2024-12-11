using UnboundLib;
using UnityEngine;

namespace RSClasses.MonoBehaviours
{
    class Astro_Mono : MonoBehaviour // Adds the scythe and barrier monobehaviors. Required since a card can only add 1 monobehavior to a player by default
    {
        private Player player;
        private Scythe_Mono scythes;
        private Barrier_Mono barriers;
        private void Start()
        {
            player = GetComponentInParent<Player>();
            scythes = player.gameObject.GetOrAddComponent<Scythe_Mono>();
            barriers = player.gameObject.GetOrAddComponent<Barrier_Mono>();
        }

        private void OnDestroy()
        {
            Destroy(scythes);
            Destroy(barriers);
        }
    }
}