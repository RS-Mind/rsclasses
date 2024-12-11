using Photon.Compression;
using RSClasses.Extensions;
using Sonigon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnboundLib;
using UnboundLib.Extensions;
using UnboundLib.GameModes;
using UnityEngine;
using static UnityEngine.UI.Image;

namespace RSClasses.MonoBehaviours
{
    class Comet : MonoBehaviour // An individual Comet
    {
        public Vector3 velocity = new Vector3(0, 0, 0);
        public bool active = true;
        private float baseScale = 1f;
        private float dustTimer = 0f;
        public Player player;
        public GameObject comet;
        private GameObject stardust;
        private TrailRenderer trailRenderer;
        private Dictionary<int, float> hitPlayers = new Dictionary<int, float>();
        System.Random rand = new System.Random(DateTime.Now.Millisecond);

        private void Awake()
        {
            player = GetComponentInParent<Player>(); // Get player
            comet = Instantiate(RSClasses.assets.LoadAsset<GameObject>("Comet"), null); // Make a comet
            stardust = Instantiate(RSClasses.assets.LoadAsset<GameObject>("Stardust"), null); // Make a stardust for copying later
            stardust.SetPosition(new Vector3(100000, 100000, 0), IncludedAxes.XY); // Get it way offscreen
        }

        private void Start()
        {
            comet.SetActive(true); // Make sure the comet is active
            trailRenderer = comet.GetComponentsInChildren<TrailRenderer>().Last(); // Get the trail renderer

            comet.GetComponentInChildren<SpriteRenderer>().color = player.GetTeamColors().color * 1.75f; // set the color
            trailRenderer.material.SetColor(Shader.PropertyToID("_Color"), player.GetTeamColors().color);
            trailRenderer.material.SetColor(Shader.PropertyToID("_EmissionColor"), player.GetTeamColors().color * 1.75f);

            stardust.GetComponent<Stardust_Mono>().player = player; // Set stardust player and color
            stardust.GetComponent<SpriteRenderer>().color = player.GetTeamColors().color * 1.75f;
        }

        public void DoHit()
        {
            if (player.data.currentCards.Contains(CardHolder.cards["Stardust"])) // Spawn stardust
            {
                dustTimer += Time.deltaTime;
                while (dustTimer > 0.15f)
                {
                    var newDust = Instantiate(stardust, comet.transform.position, new Quaternion(0, 0, rand.Next() % 360, rand.Next() % 360)); // They have random angles
                    newDust.GetComponent<Stardust_Mono>().rotationDirection = rand.Next(-120, 120); // And random rotation directions
                    RSClasses.instance.ExecuteAfterSeconds(2f, () => Destroy(newDust)); // They only last 2 seconds
                    dustTimer -= 0.15f;
                }
            }

            float damageMult = 1;
            if (player.data.currentCards.Contains(CardHolder.cards["Stellar Impact"])) // Calculate damage multiplier from current velocity
            {
                Mathf.Clamp((velocity.magnitude / 5 - 1) * player.data.GetAdditionalData().cometSpeed, 1, 3 * player.data.GetAdditionalData().cometSpeed);
            }
            if (player.data.view.IsMine) // Only run on the owner's client
            {
                var Keys = hitPlayers.Keys.ToArray(); // Update the list of recently hit players
                foreach (int Key in Keys)
                {
                    hitPlayers[Key] -= TimeHandler.deltaTime;
                    if (hitPlayers[Key] <= 0f) hitPlayers.Remove(Key);
                }

                var radius = transform.lossyScale.y * 0.9f;
                var hits = Physics2D.OverlapCircleAll(comet.transform.position, radius); // Get potential hits
                foreach (var hit in hits)
                {
                    var damageable = hit.gameObject.GetComponent<Damagable>(); // Get the damageable object, if possible
                    var healthHandler = hit.gameObject.GetComponent<HealthHandler>(); // Get the player's health handler, if it exists
                    if (healthHandler)
                    {
                        Player hitPlayer = ((Player)healthHandler.GetFieldValue("player"));
                        if (hitPlayer == player || hitPlayers.ContainsKey(hitPlayer.playerID)) continue; // Check if the hit player was recently hit or is the owner. If so, skip the rest
                        SoundManager.Instance.PlayAtPosition(healthHandler.soundBounce, this.transform, damageable.transform); // Play hit sound
                        healthHandler.CallTakeForce(((Vector2)hitPlayer.transform.position - (Vector2)comet.transform.position).normalized * 2500, ForceMode2D.Impulse, true); // Apply knockback
                        hitPlayers[hitPlayer.playerID] = 1f; // Add the player to the list of recently hit players
                        if (((Player)healthHandler.GetFieldValue("player")).GetComponent<Block>().blockedThisFrame) { continue; } // If the player blocked, don't deal damage
                    }
                    if (damageable)
                    {
                        damageable.CallTakeDamage(((Vector2)damageable.transform.position - (Vector2)this.transform.position).normalized * (player.data.GetAdditionalData().cometDamage * damageMult),
                            (Vector2)this.transform.position, this.gameObject, player); // Apply damage
                    }
                }
            }
        }

        public void UpdatePos(Vector3 playerPos)
        {
            Vector3 directionToPlayer = (playerPos - comet.transform.position).normalized; // Get the direction to the player

            float distance = Vector3.Distance(comet.transform.position, playerPos) / 2; // Calculate the distance to the player

            float adjustedScale = baseScale;
            if (player.data.currentCards.Contains(CardHolder.cards["Icemelt"])) adjustedScale *= distance < 7.5 ? -(distance / 7.5f) + 2f : 1; // If the player has Icemelt, adjust the scale of the comet (don't ask me about this formula, I guessed until it looked good)
            trailRenderer.widthMultiplier = adjustedScale; // Apply the scale changes (if any) to the trail and comet
            comet.transform.localScale = new Vector3(adjustedScale, adjustedScale, adjustedScale);

            float gPull = (float)(100f / distance); // Calculate the gravitational force
            velocity *= (float)Math.Pow(0.95f, Time.deltaTime); // Apply drag (helps keep the comet on-screen)
            velocity += new Vector3(directionToPlayer.x * gPull, directionToPlayer.y * gPull) * Time.deltaTime; // Add the force from this frame to the velocity
            if (velocity.magnitude > 15) velocity *= 15f / velocity.magnitude; // Limit the velocity to 15

            comet.transform.position += velocity * Time.deltaTime * player.data.GetAdditionalData().cometSpeed; // Move comet based on current velocity

            Quaternion currentRotation = new Quaternion(); // Rotate the comet to match the current movement direction
            float rotation = (float)Math.Atan2(velocity.normalized.x, -velocity.normalized.y);
            currentRotation.eulerAngles = new Vector3(0, 0, (float)(rotation * (180/Math.PI)) - 90);
            comet.transform.rotation = currentRotation;
        }

        public void SetScale(float scale)
        {
            baseScale = scale;
        }

        private void OnDestroy()
        {
            Destroy(comet);
        }
    }

    public class Comet_Mono : MonoBehaviour
    {
        private double angle = 0;
        private float rotation = 0;
        private bool active = false;
        List<Comet> comets = new List<Comet>();
        Player player;
        private void Start()
        {
            player = GetComponentInParent<Player>();

            GameModeManager.AddHook(GameModeHooks.HookPickEnd, PickEnd);
            GameModeManager.AddHook(GameModeHooks.HookBattleStart, BattleStart);
            GameModeManager.AddHook(GameModeHooks.HookPointStart, PointStart);
            GameModeManager.AddHook(GameModeHooks.HookPointEnd, PointEnd);
        }

        private void FixedUpdate()
        {
            if (active)
            {
                foreach (Comet comet in comets)
                {
                    comet.UpdatePos(player.transform.position);
                    comet.DoHit();
                }
            }
        }

        public void UpdateStats()
        {
            while (comets.Count() < player.data.GetAdditionalData().cometCount)
            {
                GameObject comet = new GameObject("Comet", typeof(Comet));
                comet.transform.SetParent(player.transform);
                comets.Add(comet.GetComponent<Comet>());
                comet.GetComponent<Comet>().player = player;
            }
            while (comets.Count() > Math.Max(player.data.GetAdditionalData().cometCount, 0))
            {
                Destroy(comets[0]);
                comets.Remove(comets[0]);
            }

            foreach (Comet comet in comets)
            {
                RSClasses.instance.ExecuteAfterSeconds(1f, () => comet.SetScale((player.data.GetAdditionalData().orbitalRadius * 0.5f) + 0.5f));
            }
        }

        private void OnDestroy()
        {
            GameModeManager.RemoveHook(GameModeHooks.HookPickEnd, PickEnd);
            GameModeManager.RemoveHook(GameModeHooks.HookBattleStart, BattleStart);
            GameModeManager.RemoveHook(GameModeHooks.HookPointStart, PointStart);
            GameModeManager.RemoveHook(GameModeHooks.HookPointEnd, PointEnd);

            while (comets.Count() > 0)
            {
                Destroy(comets[0]);
                comets.Remove(comets[0]);
            }
        }

        IEnumerator PickEnd(IGameModeHandler gm)
        {
            UpdateStats();
            yield break;
        }
        IEnumerator PointStart(IGameModeHandler gm)
        {
            rotation = 0f;
            angle = 0.0;
            int index = 0;
            foreach (Comet comet in comets)
            {
                comet.comet.transform.position = player.transform.position + new Vector3(0, 7.5f - (15*index), 0);
                comet.velocity = new Vector3(0, 0, 0);
                index++;
            }
            yield break;
        }IEnumerator BattleStart(IGameModeHandler gm)
        {
            active = true;
            yield break;
        }

        IEnumerator PointEnd(IGameModeHandler gm)
        {
            active = false;
            yield break;
        }
    }
}