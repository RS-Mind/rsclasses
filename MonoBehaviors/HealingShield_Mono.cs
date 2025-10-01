using RSClasses.Utilities;
using System;
using TabInfo.Extensions;
using UnboundLib;
using UnboundLib.Networking;
using UnityEngine;

namespace RSClasses.MonoBehaviours
{
    class HealingShield_Mono : MonoBehaviour // An individual scythe
    {
        private Player player;
        private AttackLevel attackLevel;
        private Transform timerVisualizer;
        private float healAmount = 10f;
        private float timer = 0f;
        private float scaleFactor = 3f;
        
        private void Awake()
        {
            player = GetComponentInParent<Player>();
            attackLevel = GetComponent<AttackLevel>();
            timerVisualizer = transform.GetChild(1);
        }

        private void FixedUpdate()
        {
            transform.localScale = new Vector3(player.data.GetAdditionalData().orbitalRadius * scaleFactor, player.data.GetAdditionalData().orbitalRadius * scaleFactor);
            timer += Time.deltaTime;
            float scale = (float)Math.Sqrt(-Math.Pow(timer,2) + 2 * timer);
            timerVisualizer.localScale = new Vector3(scale, scale);
            if (timer >= 1f)
            {
                if (player.data.view.IsMine) // Only run on the card holder's client
                {
                    Heal();
                }
                timer -= 1f;
            }

        }

        private void Heal()
        {
            var hits = Physics2D.OverlapCircleAll(transform.position, transform.lossyScale.y / 2); // Get all targets in range
            foreach (var hit in hits) // For each target
            {
                HealthHandler healthHandler = hit.gameObject.GetComponent<HealthHandler>(); // Grab the opponent's health handler, if any

                if (healthHandler) // If the target is a player basically
                {
                    Player target = healthHandler.GetComponent<Player>();
                    if (target.teamID == player.teamID)
                        NetworkingManager.RPC(typeof(HealingShield_Mono), nameof(RPCA_Heal), target.playerID, healAmount * attackLevel.LevelScale());
                }
            }
        }

        [UnboundRPC]
        static void RPCA_Heal(int playerID, float healAmount)
        {
            HealthHandler healthHandler = PlayerManager.instance.GetPlayerWithID(playerID).GetComponent<HealthHandler>();
            healthHandler.Heal(healAmount);
        }
    }
}