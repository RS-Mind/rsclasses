using UnityEngine;
using Photon.Pun;
using SimulationChamber;
using System.Linq;
using RSClasses.Utilities;

namespace RSClasses.MonoBehaviours
{
    public class Voidseer_Mono : MonoBehaviour
    {
        Player player;

        public void Start()
        {
            player = GetComponentInParent<Player>();
        }

        public void Trigger()
        {
            player.GetComponent<Damagable>().CallTakeDamage(Vector2.up * 1, (Vector2)player.transform.position, null, player); // Give the player the self-damage trigger
        }
    }
}