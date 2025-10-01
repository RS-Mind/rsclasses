using RSClasses.Utilities;
using Sonigon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnboundLib;
using UnboundLib.GameModes;
using UnityEngine;

// Handles the Astronomer's barriers

namespace RSClasses.MonoBehaviours
{
    class Barrier : MonoBehaviour // The individual barriers
    {
        public bool initialized = false;
        private Transform origin;
        private Animator anim;
        private Player player;
        private BarrierCollider barrierCollider;
        private GameObject barrier;
        private void Start()
        {
            player = GetComponentInParent<Player>(); // Get player

            barrier = Instantiate(RSClasses.assets.LoadAsset<GameObject>("Barrier"), player.transform); // Create the barrier
            barrier.SetActive(true);
            barrierCollider = barrier.transform.GetChild(0).gameObject.GetOrAddComponent<BarrierCollider>(); // Make sure the barrier has a collider and store it
            anim = barrier.GetComponent<Animator>(); // Get animator
            origin = barrier.GetComponentsInChildren<Transform>().Last(); //Store center position
            SetColor(player.GetComponent<Barrier_Mono>().color);
            SetScale(player.data.GetAdditionalData().orbitalRadius * 0.11875f);
            initialized = true;
        }

        private void Update()
        {
            barrierCollider.gameObject.SetActive(!player.data.dead); // Disable this object if the player is dead
        }

        public void UpdatePos(double angle, float radius)
        {
            double angle_radians = (angle * Math.PI) / 180; // Convert from degrees to radians
            Vector3 position = new Vector3((float)(radius * Math.Sin(angle_radians)), // Create a position based on the calculated angle
                (float)((radius * Math.Cos(angle_radians))), 0);
            Quaternion currentRotation = new Quaternion(); // Handle the rotation
            currentRotation.eulerAngles = new Vector3(0, 0, -(float)angle);
            barrier.transform.localPosition = position; // Assign the calculated values
            barrier.transform.rotation = currentRotation;
            barrierCollider.transform.position = barrier.transform.position; // Keep collider synced
            barrierCollider.transform.rotation = barrier.transform.rotation;
            barrierCollider.transform.localScale = Vector3.Scale(barrier.transform.localScale, player.transform.localScale);
        }

        public void DoHit() // Triggered by the Shield Spikes card
        {
            anim.SetTrigger("OnBlock"); // Trigger the animation
            var radius = barrier.transform.lossyScale.y * 5;
            var hits = Physics2D.OverlapCircleAll(origin.transform.position, radius); // Determine hit players
            if (player.data.view.IsMine) // Only trigger once per server
            {
                foreach (var hit in hits) // For each hit player
                {
                    var damageable = hit.gameObject.GetComponent<Damagable>(); // Grab the damageable object, if any
                    var healthHandler = hit.gameObject.GetComponent<HealthHandler>(); // Grab the player's health handler, if any
                    if (!damageable) // Skip non-damageable objects (e.g. terrain)
                    {
                        continue;
                    }
                    if (healthHandler)
                    {
                        Player hitPlayer = ((Player)healthHandler.GetFieldValue("player"));
                        SoundManager.Instance.PlayAtPosition(healthHandler.soundBounce, origin.transform, damageable.transform); // Play hit sound
                        healthHandler.CallTakeForce(((Vector2)hitPlayer.transform.position - (Vector2)player.transform.position).normalized * 5000, ForceMode2D.Impulse, true); // Apply knockbacl
                        if (((Player)healthHandler.GetFieldValue("player")).GetComponent<Block>().blockedThisFrame) { continue; } // Skip damage if the player blocked
                    }
                    damageable.CallTakeDamage(((Vector2)damageable.transform.position - (Vector2)origin.transform.position).normalized * ((0.10f * player.data.maxHealth) + 10f), // Deal damage, scaled off of max health
                        (Vector2)origin.transform.position, barrier.gameObject, player);
                }
            }
        }

        public void SetColor(Color color)
        {
            barrier.GetComponent<SpriteRenderer>().color = color; // Set the color of the barrier
            barrier.GetComponentsInChildren<SpriteRenderer>().Last().color = color; // And the spike
        }

        public void SetScale(float scale) // Set the barrier's scale. I could 
        {
            barrier.transform.localScale = new Vector3(scale, scale, scale);
        }

        private void OnDestroy()
        {
            // Get rid of the barrier objects
            Destroy(barrier);
            Destroy(barrierCollider);
        }
    }

    class BarrierCollider : MonoBehaviour // A barrier's collider
    {
        Player player;

        void Start()
        {
            gameObject.layer = LayerMask.NameToLayer("Projectile"); // Move the barrier collider to the projectile layer, allowing it to interact with bullets
            player = GetComponentInParent<Player>();
            gameObject.transform.SetParent(null, true); // Set the parent to the scene. Not doing this makes the barrier an additional hurtbox for the player
        }

        public void Update()
        {
            gameObject.SetActive(!player.data.dead); // Disable this object if the player is dead
        }
    }

    public class Barrier_Mono : MonoBehaviour // Holds all the barriers for a player
    {
        private Block block;
        private double angle = 0;
        public Color color = new Color(1f, 1f, 0.7411765f);
        private List<Barrier> barriers = new List<Barrier>();
        private Player player;
        private void Start()
        {
            player = GetComponentInParent<Player>(); // Get player
            block = GetComponentInParent<CharacterData>().block; // Get player's block
			block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Combine(block.BlockAction, new Action<BlockTrigger.BlockTriggerType>(OnBlock)); // Add a block action (for Shield Spikes)

            GameModeManager.AddHook(GameModeHooks.HookPointStart, PointStart); // Add a point start hook
        }

        private void FixedUpdate()
        {
            angle = (angle - (player.data.GetAdditionalData().barrierSpeed * TimeHandler.deltaTime)) % 360; // Update rotation

            int index = 0;
            foreach (Barrier barrier in barriers) // Tell each barrier what poisition to be at
            {
                double thisAngle = (angle + ((index%2*180) + (45*(index/2)))) % 360f; // Place the barriers in opposing pairs, filling in as 2 pieces
                barrier.UpdatePos(thisAngle, player.data.GetAdditionalData().orbitalRadius * 0.0125f); // Tell the barrier what angle and scale to exist at
                index++;
            }
        }

        private void OnBlock(BlockTrigger.BlockTriggerType blockTrigger) // On block effect
        {
            if (player.data.currentCards.Contains(CardHolder.cards["Shield Spikes"])) // If the player has the Shield Spikes card
            {
                foreach (Barrier barrier in barriers)
                {
                    barrier.DoHit(); // Perform the hit function
                }
            }
        }

        public void UpdateStats() // After each pick phase, update the barrier stats
        {
            while (barriers.Count() < player.data.GetAdditionalData().barrierCount) // Add more barriers as needed
            {
                GameObject barrier = new GameObject("Barrier", typeof(Barrier)); // Create a clone of the barrier prefab
                barrier.transform.SetParent(player.transform); // Bind it to the player
                barriers.Add(barrier.GetComponent<Barrier>()); // Add it to the list of barriers
            }
            while (barriers.Count() > Math.Max(player.data.GetAdditionalData().barrierCount, 0)) // Remove barriers as needed
            {
                Destroy(barriers[0]);
                barriers.Remove(barriers[0]); // Don't forget to remove barriers from the list
            }


            if (player.data.currentCards.Contains(CardHolder.cards["Guardian"])) color = new Color(0.4f, 1f, 1f); // Change color based on subclass
            else if (player.data.currentCards.Contains(CardHolder.cards["Stargazer"])) color = player.GetTeamColors().color * 1.75f;
            else if (player.data.currentCards.Contains(CardHolder.cards["Harvester"])) color = new Color(178f / 255f, 0f, 1f);

            foreach (Barrier barrier in barriers)
            {
                RSClasses.instance.ExecuteAfterSeconds(1f, () =>
                { // Update the color and scale
                    if (barrier.initialized)
                    {
                        barrier.GetComponent<Barrier>().SetColor(color);
                        barrier.SetScale(player.data.GetAdditionalData().orbitalRadius * 0.11875f); // I guess and checked this value
                    }
                });
            }
        }

        private void OnDestroy()
        {
            GameModeManager.RemoveHook(GameModeHooks.HookPointStart, PointStart); // Remove the point start hook

            // Remove the block action (Shield Spikes)
            block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Remove(block.BlockAction, new Action<BlockTrigger.BlockTriggerType>(OnBlock));

            while (barriers.Count() > 0) // Destroy all barriers
            {
                Destroy(barriers[0]);
                barriers.Remove(barriers[0]);
            }
        }

        IEnumerator PointStart(IGameModeHandler gm) // At start of point, reset the angle to help maintain sync and update the stats while we have the chance
        {
            angle = 0.0;
            UpdateStats();
            yield break;
        }
    }
}