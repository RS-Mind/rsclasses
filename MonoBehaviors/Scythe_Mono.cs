using RSClasses.Utilities;
using Sonigon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnboundLib.GameModes;
using UnityEngine;

namespace RSClasses.MonoBehaviours
{
    class Scythe : MonoBehaviour // An individual scythe
    {
        public bool initialized = false;
        public bool active = true;
        public bool ableToHit = true;
        private Player player;
        private GameObject scythe;

        private void Start()
        {
            player = GetComponentInParent<Player>(); // Get player

            scythe = Instantiate(RSClasses.assets.LoadAsset<GameObject>("Scythe"), player.transform); // Load the scythe prefab
            scythe.SetActive(true);

            SetColor(player.GetComponent<Scythe_Mono>().color);
            SetScale(player.data.GetAdditionalData().orbitalRadius * 0.15625f);
            initialized = true;
        }

        public void DoHit() // Hit players
        {
            var radius = transform.localScale.y; // This is here in case I need to tweak the radius compared to the local scale
            var hits = Physics2D.OverlapCircleAll(scythe.transform.position, radius); // Get all targets in range

            if (player.data.view.IsMine) // Only run on the card holder's client
            {
                foreach (var hit in hits) // For each target
                {
                    var damageable = hit.gameObject.GetComponent<Damagable>(); // Grab the damageable object, if any
                    var healthHandler = hit.gameObject.GetComponent<HealthHandler>(); // Grab the opponent's health handler, if any
                    float bonusDamage = 0f;
                    if (player.data.currentCards.Contains(CardHolder.cards["Dark Harvest"])) // If the player has Dark Harvest, add the bonus damage from life steal
                    {
                        bonusDamage = (player.data.stats.lifeSteal * 50f);
                    }

                    float damage = player.data.GetAdditionalData().scytheDamage + bonusDamage;

                    if (healthHandler) // If the target is a player basically
                    {
                        Player hitPlayer = ((Player)healthHandler.GetFieldValue("player"));
                        SoundManager.Instance.PlayAtPosition(healthHandler.soundBounce, this.transform, damageable.transform); // Play sfx
                        healthHandler.CallTakeForce(((Vector2)hitPlayer.transform.position - (Vector2)scythe.transform.position).normalized * 2500, ForceMode2D.Impulse, true); // Apply knockback
                        this.ableToHit = false; // Disable the scythe for the rest of the rotation
                        if (((Player)healthHandler.GetFieldValue("player")).GetComponent<Block>().blockedThisFrame) { continue; } // Skip everything else if they blocked

                        if (player.data.currentCards.Contains(CardHolder.cards["Death's Blade"])) // If the player has Death's Blade, set minimum damge.
                        {
                            damage = Math.Max(hitPlayer.data.maxHealth * 0.15f, damage);
                        }
                    }
                    if (damageable) // If the target can take damage
                    {
                        damageable.CallTakeDamage(((Vector2)damageable.transform.position - (Vector2)this.transform.position).normalized * damage,
                            (Vector2)this.transform.position, this.gameObject, player); // Apply damage
                    }
                }
            }
        }

        public void UpdatePos(double angle, float rotation, float radius) // Update the position
        {
            double angle_radians = (angle * Math.PI) / 180; // Convert degrees to radians
            Vector3 position = new Vector3((float)(radius * Math.Sin(angle_radians)), // Convert angle to position
                (float)((radius * Math.Cos(angle_radians))), 0);
            Quaternion currentRotation = new Quaternion();
            currentRotation.eulerAngles = new Vector3(0, 0, rotation); // Rotate the scythe
            scythe.transform.localPosition = position; // Write values
            scythe.transform.rotation = currentRotation;
        }

        public void SetColor(Color color)
        {
            scythe.GetComponent<SpriteRenderer>().color = color; // set the color
        }

        public void SetScale(float scale)
        {
            scythe.transform.localScale = new Vector3(scale, scale, scale); // set the scale
        }

        private void OnDestroy()
        {
            Destroy(scythe); // Delete the scythe
        }
    }

    public class Scythe_Mono : MonoBehaviour // Holds all the scythes on a player
    {
        private double angle = 0;
        private float rotation = 0;
        private bool active = false;
        public Color color = new Color(1f, 1f, 0.7411765f);
        internal List<Scythe> scythes = new List<Scythe>();
        private Player player;

        private void Start()
        {
            player = GetComponentInParent<Player>(); // Get player

            GameModeManager.AddHook(GameModeHooks.HookPointStart, PointStart); // Add hooks
            GameModeManager.AddHook(GameModeHooks.HookRoundStart, RoundStart);
            GameModeManager.AddHook(GameModeHooks.HookPointEnd, PointEnd);
        }

        private void FixedUpdate()
        {
            angle += player.data.GetAdditionalData().scytheSpeed * TimeHandler.deltaTime; // Update rotation
            if (angle > 360) // After a full rotation, re-enable all scythes
            {
                foreach (Scythe scythe in scythes)
                {
                    scythe.ableToHit = true;
                }
                angle -= 360;
            }
            rotation = (rotation - (1200 * TimeHandler.deltaTime)) % 360;

            int index = 0;
            foreach (Scythe scythe in scythes) // Tell each scythe where it belongs
            {
                double thisAngle = angle + (360f / scythes.Count() * index);
                scythe.UpdatePos(thisAngle, rotation, player.data.GetAdditionalData().orbitalRadius * 2.5f);
                if (active && scythe.ableToHit) // Trigger scythe hits
                {
                    scythe.DoHit();
                }
                if (scythe.ableToHit) // Update opacity to reflect whether the scythe is active or not
                {
                    Color setColor = color;
                    setColor.a = 1;
                    scythe.SetColor(setColor);
                }
                else
                {
                    Color setColor = color;
                    setColor.a = 0.5f;
                    scythe.SetColor(setColor);
                }
                index++;
            }
        }

        public void UpdateStats()
        {
            while (scythes.Count() < player.data.GetAdditionalData().scytheCount) // Create scythes as needed
            {
                GameObject scythe = new GameObject("Scythe", typeof(Scythe));
                scythe.transform.SetParent(player.transform);
                scythes.Add(scythe.GetComponent<Scythe>());
            }
            while (scythes.Count() > Math.Max(player.data.GetAdditionalData().scytheCount, 0)) // Delete scythes as needed
            {
                Destroy(scythes[0]);
                scythes.Remove(scythes[0]);
            }

            if (player.data.currentCards.Contains(CardHolder.cards["Harvester"])) color = new Color(178f / 255f, 0f, 1f); // Update colors based on subclass
            else if (player.data.currentCards.Contains(CardHolder.cards["Stargazer"])) color = player.GetTeamColors().color * 1.75f;
            else if (player.data.currentCards.Contains(CardHolder.cards["Guardian"])) color = new Color(0.4f, 1f, 1f);

            foreach (Scythe scythe in scythes)
            {
                RSClasses.instance.ExecuteAfterSeconds(1f, () => { // Tell scythes their new color and scale
                    if (scythe.initialized)
                    {
                        scythe.GetComponent<Scythe>().SetColor(color);
                        scythe.SetScale(player.data.GetAdditionalData().orbitalRadius * 0.15625f); // This number is 5 / 32
                    }
                });
            }
        }

        private void OnDestroy()
        {
            GameModeManager.RemoveHook(GameModeHooks.HookPointStart, PointStart); // Remove hooks
            GameModeManager.RemoveHook(GameModeHooks.HookPointEnd, PointEnd);

            while (scythes.Count() > 0) // Destroy all the scythes
            {
                Destroy(scythes[0]);
                scythes.Remove(scythes[0]);
            }
        }

        IEnumerator RoundStart(IGameModeHandler gm) // At the start of battle, reset rotations to help maintain sync
        {
            UpdateStats();
            yield break;
        }
        IEnumerator PointStart(IGameModeHandler gm) // At the start of battle, reset rotations to help maintain sync
        {
            active = true;
            rotation = 0f;
            angle = 0.0;
            yield break;
        }

        IEnumerator PointEnd(IGameModeHandler gm) // Disable the scythes when not in a match
        {
            active = false;
            yield break;
        }
    }
}