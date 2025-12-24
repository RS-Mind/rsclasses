using Photon.Pun;
using RSClasses.Utilities;
using Sonigon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnityEngine;

namespace RSClasses.MonoBehaviors
{

    class Comet : MonoBehaviourPun // An individual Comet
    {
        internal Vector3 velocity = new Vector3(0, 0, 0);
        private bool icemelt = false;
        private bool stardust = false;
        private bool stellarImpact = false;
        private float baseScale = 1f;
        private float dustTimer = 0f;
        private float cometSpeed = 2f;
        private Player player;
        private TrailRenderer trailRenderer;
        private Dictionary<int, float> hitPlayers = new Dictionary<int, float>();
        private System.Random rand = new System.Random(DateTime.Now.Millisecond); // Only used cosmetically, no syncing necessary

        private void Start()
        {
            player = GetComponentInParent<Player>(); // Get player
        }

        public void DoHit()
        {
            if (stardust) // Spawn stardust
            {
                dustTimer += Time.fixedDeltaTime;
                while (dustTimer > 0.15f)
                {
                    var newDust = PhotonNetwork.Instantiate("Stardust", gameObject.transform.position, new Quaternion(0, 0, rand.Next() % 360, rand.Next() % 360)); // They have random angles
                    newDust.GetComponent<PhotonView>().RPC("SetValues", RpcTarget.All, new object[] { player.playerID, rand.Next(-120, 120) }); // Set starting values
                    RSClasses.instance.ExecuteAfterSeconds(2f, () => PhotonNetwork.Destroy(newDust)); // They only last 2 seconds
                    dustTimer -= 0.15f;
                }
            }

            float damageMult = 1;
            if (stellarImpact) // Calculate damage multiplier from current velocity
                Mathf.Clamp((velocity.magnitude / 5 - 1) * player.data.GetAdditionalData().cometSpeed, 1, 1.5f * player.data.GetAdditionalData().cometSpeed);
            var Keys = hitPlayers.Keys.ToArray(); // Update the list of recently hit players
            foreach (int Key in Keys)
            {
                hitPlayers[Key] -= TimeHandler.deltaTime;
                if (hitPlayers[Key] <= 0f) hitPlayers.Remove(Key);
            }

            var radius = transform.lossyScale.y * 0.9f;
            var hits = Physics2D.OverlapCircleAll(gameObject.transform.position, radius); // Get potential hits
            foreach (var hit in hits)
            {
                var damageable = hit.gameObject.GetComponent<Damagable>(); // Get the damageable object, if possible
                var healthHandler = hit.gameObject.GetComponent<HealthHandler>(); // Get the player's health handler, if it exists
                if (healthHandler)
                {
                    Player hitPlayer = ((Player)healthHandler.GetFieldValue("player"));
                    if (hitPlayer == player || hitPlayers.ContainsKey(hitPlayer.playerID)) continue; // Check if the hit player was recently hit or is the owner. If so, skip the rest
                    SoundManager.Instance.PlayAtPosition(healthHandler.soundBounce, this.transform, damageable.transform); // Play hit sound
                    healthHandler.CallTakeForce(((Vector2)hitPlayer.transform.position - (Vector2)gameObject.transform.position).normalized * 2500, ForceMode2D.Impulse, true); // Apply knockback
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

        public void UpdatePos(Vector3 playerPos)
        {
            Vector3 directionToPlayer = (playerPos - gameObject.transform.position).normalized; // Get the direction to the player

            float distance = Vector3.Distance(gameObject.transform.position, playerPos) / 2; // Calculate the distance to the player

            float adjustedScale = baseScale;
            if (icemelt) // If the player has Icemelt, adjust the scale of the comet (don't ask me about this formula, I guessed until it looked good)
                adjustedScale *= distance < 7.5 ? -(distance / 7.5f) + 2f : 1;
            trailRenderer.widthMultiplier = adjustedScale; // Apply the scale changes (if any) to the trail and comet
            gameObject.transform.localScale = new Vector3(adjustedScale, adjustedScale, adjustedScale);

            float gPull = (float)(100f / distance); // Calculate the gravitational force
            velocity *= (float)Math.Pow(0.95f, Time.fixedDeltaTime); // Apply drag (helps keep the comet on-screen)
            velocity += new Vector3(directionToPlayer.x * gPull, directionToPlayer.y * gPull) * Time.fixedDeltaTime; // Add the force from this frame to the velocity
            if (velocity.magnitude > 15) velocity *= 15f / velocity.magnitude; // Limit the velocity to 15

            Vector3 calculatedMotion = velocity * Time.fixedDeltaTime * cometSpeed;

            if (Vector3.Distance(gameObject.transform.position + calculatedMotion, playerPos) <= transform.lossyScale.y * 0.9f) // If the comet would collide with the player, reflect it
            {
                Vector3 normal = Vector3.Normalize(playerPos - gameObject.transform.position); // Normal vector from the player to the comet
                calculatedMotion = calculatedMotion - (2 * Vector3.Dot(calculatedMotion, normal) * normal); // Reflection formula
            }

            gameObject.transform.position += calculatedMotion; // Move comet based on current velocity

            Quaternion currentRotation = new Quaternion(); // Rotate the comet to match the current movement direction
            float rotation = (float)Math.Atan2(velocity.normalized.x, -velocity.normalized.y);
            currentRotation.eulerAngles = new Vector3(0, 0, (float)(rotation * (180 / Math.PI)) - 90);
            gameObject.transform.rotation = currentRotation;
            if (photonView.IsMine)
            {
                photonView.RPC("SyncVelocity", RpcTarget.Others, new object[] { velocity });
            }
        }

        [PunRPC]
        public void SyncVelocity(Vector3 syncVel)
        {
            velocity = syncVel;
        }

        [PunRPC]
        public void SetAttributes(int ownerID, float scale)
        {
            player = PlayerManager.instance.players.Find(p => p.playerID == ownerID);
            name = "Comet " + player.playerID;

            cometSpeed = player.data.GetAdditionalData().cometSpeed;

            icemelt = player.data.currentCards.Contains(CardHolder.cards["Icemelt"]);
            stellarImpact = player.data.currentCards.Contains(CardHolder.cards["Stellar Impact"]);
            stardust = player.data.currentCards.Contains(CardHolder.cards["Stardust"]);

            baseScale = scale;

            trailRenderer = gameObject.GetComponentsInChildren<TrailRenderer>().Last(); // Get the trail renderer

            gameObject.GetComponentInChildren<SpriteRenderer>().color = player.GetTeamColors().color * 1.75f; // set the color
            trailRenderer.material.SetColor(Shader.PropertyToID("_Color"), player.GetTeamColors().color);
            trailRenderer.material.SetColor(Shader.PropertyToID("_EmissionColor"), player.GetTeamColors().color * 1.75f);
        }

        public void FixedUpdate()
        {
            //gameObject.SetActive(!player.data.dead); // Disable this object if the player is dead
        }
    }

}