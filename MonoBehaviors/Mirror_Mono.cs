using UnityEngine;
using Photon.Pun;
using SimulationChamber;
using System.Linq;
using RSClasses.Extensions;

namespace RSClasses.MonoBehaviours
{
    public class Mirror_Mono : MonoBehaviour
    {
        Player player;
        Gun gun;

        public SimulatedGun[] savedGuns = new SimulatedGun[4];

        public static GameObject _stopRecursionObj = null;
        public static GameObject _PoisonObj = null;
        public static GameObject _DazzleObj = null;
        public static GameObject _ColdObj = null;

        public static GameObject StopRecursionObj
        {
            get
            {
                if (_stopRecursionObj == null)
                {
                    _stopRecursionObj = new GameObject("A_StopRecursion", typeof(StopRecursion));
                    DontDestroyOnLoad(_stopRecursionObj);
                }
                return _stopRecursionObj;
            }
        }

        public static ObjectsToSpawn[] StopRecursionSpawn
        {
            get
            {
                return new ObjectsToSpawn[] { new ObjectsToSpawn() { AddToProjectile = StopRecursionObj } };
            }
        }

        public static GameObject PoisonObj
        {
            get
            {
                if (_PoisonObj == null)
                {
                    _PoisonObj = new GameObject("A_Poison", typeof(RayHitPoison));
                    DontDestroyOnLoad(_PoisonObj);
                }
                return _PoisonObj;
            }
        }

        public static ObjectsToSpawn[] PoisonSpawn
        {
            get
            {
                return new ObjectsToSpawn[] { new ObjectsToSpawn() { AddToProjectile = PoisonObj } };
            }
        }

        public static GameObject DazzleObj
        {
            get
            {
                if (_DazzleObj == null)
                {
                    _DazzleObj = new GameObject("A_Dazzle", typeof(RayHitBash));
                    DontDestroyOnLoad(_DazzleObj);
                }
                return _DazzleObj;
            }
        }

        public static ObjectsToSpawn[] DazzleSpawn
        {
            get
            {
                return new ObjectsToSpawn[] { new ObjectsToSpawn() { AddToProjectile = DazzleObj } };
            }
        }

        public static GameObject ColdObj
        {
            get
            {
                if (_ColdObj == null)
                {
                    _ColdObj = new GameObject("A_Cold", typeof(ChillingTouch));
                    DontDestroyOnLoad(_ColdObj);
                }
                return _ColdObj;
            }
        }

        public static ObjectsToSpawn[] ColdSpawn
        {
            get
            {
                return new ObjectsToSpawn[] { new ObjectsToSpawn() { AddToProjectile = ColdObj } };
            }
        }

        public void Start()
        {
            // Get Player
            player = GetComponentInParent<Player>();
            // Get Gun
            gun = player.data.weaponHandler.gun;
            // Add action
            gun.ShootPojectileAction += OnShootProjectileAction;

            // Checks to see if we have a saved gun already, if not, make one.
            if (savedGuns[0] == null)
            {
                savedGuns[0] = new GameObject("Mirror Gun").AddComponent<SimulatedGun>();
            }

            // Checks to see if we have a second saved gun already, if not, make one.
            if (savedGuns[1] == null)
            {
                savedGuns[1] = new GameObject("Sapphire Gun").AddComponent<SimulatedGun>();
            }

            // Checks to see if we have a second saved gun already, if not, make one.
            if (savedGuns[2] == null)
            {
                savedGuns[2] = new GameObject("Ruby Gun").AddComponent<SimulatedGun>();
            }

            // Checks to see if we have a second saved gun already, if not, make one.
            if (savedGuns[3] == null)
            {
                savedGuns[3] = new GameObject("Emerald Gun").AddComponent<SimulatedGun>();
            }
        }

        public void OnShootProjectileAction(GameObject obj)
        {
            // If the bullet has the StopRecursion component in it somewhere, we don't want to trigger.
            if (obj.GetComponentsInChildren<StopRecursion>().Length > 0)
            {
                return;
            }
            
            // Mirrored X/Kaleidoscope sapphire
            SimulatedGun sapphireGun = savedGuns[0];

            // Copy gun stats, including actions
            sapphireGun.CopyGunStatsExceptActions(gun);
            sapphireGun.CopyAttackAction(gun);
            sapphireGun.CopyShootProjectileAction(gun);
            sapphireGun.ShootPojectileAction -= OnShootProjectileAction;

            // Only fire 1 bullet per bullet
            sapphireGun.numberOfProjectiles = 1;
            sapphireGun.bursts = 0;
            sapphireGun.spread = 0f;
            sapphireGun.evenSpread = 0f;
            sapphireGun.objectsToSpawn = sapphireGun.objectsToSpawn.Concat(StopRecursionSpawn).ToArray();

            if (player.data.currentCards.Contains(CardHolder.cards["Sapphire Shards"]))
            {
                sapphireGun.slow = 0.7f;
                sapphireGun.projectileColor = new Color(0, 172, 191);
            }

            // Mirrored Y/Kaledoscope no effect
            SimulatedGun mirrorGun = savedGuns[1];

            // Copy gun stats, including actions
            mirrorGun.CopyGunStatsExceptActions(gun);
            mirrorGun.CopyAttackAction(gun);
            mirrorGun.CopyShootProjectileAction(gun);
            mirrorGun.ShootPojectileAction -= OnShootProjectileAction;

            // Invert gravity, only fire 1 bullet per bullet
            mirrorGun.numberOfProjectiles = 1;
            mirrorGun.bursts = 0;
            mirrorGun.spread = 0f;
            mirrorGun.evenSpread = 0f;
            mirrorGun.gravity *= -1f;
            mirrorGun.objectsToSpawn = mirrorGun.objectsToSpawn.Concat(StopRecursionSpawn).ToArray();

            // Kaleidoscope emerald
            SimulatedGun emeraldGun = savedGuns[2];

            // Copy gun stats, including actions
            emeraldGun.CopyGunStatsExceptActions(gun);
            emeraldGun.CopyAttackAction(gun);
            emeraldGun.CopyShootProjectileAction(gun);
            emeraldGun.ShootPojectileAction -= OnShootProjectileAction;

            // Only fire 1 bullet per bullet
            emeraldGun.numberOfProjectiles = 1;
            emeraldGun.bursts = 0;
            emeraldGun.spread = 0f;
            emeraldGun.evenSpread = 0f;
            emeraldGun.objectsToSpawn = emeraldGun.objectsToSpawn.Concat(StopRecursionSpawn).ToArray();

            if (player.data.currentCards.Contains(CardHolder.cards["Emerald Glitter"]))
            {
                emeraldGun.damage *= 1.25f;
                emeraldGun.objectsToSpawn = emeraldGun.objectsToSpawn.Concat(PoisonSpawn).ToArray();
                emeraldGun.projectileColor = Color.green;
            }

            // Kaleidoscope ruby
            SimulatedGun rubyGun = savedGuns[3];

            // Copy gun stats, including actions
            rubyGun.CopyGunStatsExceptActions(gun);
            rubyGun.CopyAttackAction(gun);
            rubyGun.CopyShootProjectileAction(gun);
            rubyGun.ShootPojectileAction -= OnShootProjectileAction;

            // Only fire 1 bullet per bullet
            rubyGun.numberOfProjectiles = 1;
            rubyGun.bursts = 0;
            rubyGun.spread = 0f;
            rubyGun.evenSpread = 0f;
            rubyGun.objectsToSpawn = rubyGun.objectsToSpawn.Concat(StopRecursionSpawn).ToArray();

            if (player.data.currentCards.Contains(CardHolder.cards["Ruby Dust"]))
            {
                rubyGun.objectsToSpawn = rubyGun.objectsToSpawn.Concat(DazzleSpawn).ToArray();
                rubyGun.projectileColor = Color.magenta;
            }

            // Make sure to not fire for each player in the lobby
            if (!(player.data.view.IsMine || PhotonNetwork.OfflineMode))
            {
                return;
            }

            // Opposite on X
            sapphireGun.SimulatedAttack(player.playerID, new Vector3(obj.transform.position.x * -1f, obj.transform.position.y, 0), obj.transform.forward - (2 * Vector3.Dot(obj.transform.forward, Vector3.left) * Vector3.left), 1f, 1);
            // Only do these if the player has Prism
            if (player.data.currentCards.Contains(CardHolder.cards["Prism"]))
            {
                // Opposite on Y
                if (!player.data.currentCards.Contains(CardHolder.cards["Kaleido Witch"]))
                {
                    mirrorGun.SimulatedAttack(player.playerID, new Vector3(obj.transform.position.x, obj.transform.position.y * -1f, 0), obj.transform.forward - (2 * Vector3.Dot(obj.transform.forward, Vector3.up) * Vector3.up), 1f, 1);
                }
                // Opposite X & Y
                mirrorGun.SimulatedAttack(player.playerID, new Vector3(obj.transform.position.x * -1f, obj.transform.position.y * -1f, 0), new Vector3(obj.transform.forward.x * -1, obj.transform.forward.y * -1f, 0f), 1f, 1);
            }
            if (player.data.currentCards.Contains(CardHolder.cards["Kaleido Witch"]))
            {
                // Opposite on Y (prepped to be cold)
                sapphireGun.SimulatedAttack(player.playerID, new Vector3(obj.transform.position.x, obj.transform.position.y * -1f, 0), obj.transform.forward - (2 * Vector3.Dot(obj.transform.forward, Vector3.up) * Vector3.up), 1f, 1);
                // idk how to talk about these
                Vector3 normalDir = Quaternion.AngleAxis(45, Vector3.forward) * Vector3.up;
                emeraldGun.SimulatedAttack(player.playerID, new Vector3(obj.transform.position.y, obj.transform.position.x, 0), obj.transform.forward - (2 * Vector3.Dot(obj.transform.forward, normalDir) * normalDir), 1f, 1);
                normalDir = Quaternion.AngleAxis(45, Vector3.forward) * Vector3.left;
                emeraldGun.SimulatedAttack(player.playerID, new Vector3(obj.transform.position.y * -1f, obj.transform.position.x * -1f, 0), obj.transform.forward - (2 * Vector3.Dot(obj.transform.forward, normalDir) * normalDir), 1f, 1);
                rubyGun.SimulatedAttack(player.playerID, new Vector3(obj.transform.position.y * -1f, obj.transform.position.x, 0), Quaternion.AngleAxis(90, Vector3.forward) * obj.transform.forward, 1f, 1);
                rubyGun.SimulatedAttack(player.playerID, new Vector3(obj.transform.position.y, obj.transform.position.x * -1f, 0), Quaternion.AngleAxis(270, Vector3.forward) * obj.transform.forward, 1f, 1);
            }
        }

        public void OnDestroy()
        {
            // Remove action when the mono is removed
            gun.ShootPojectileAction -= OnShootProjectileAction;

            Destroy(savedGuns[0]);
            Destroy(savedGuns[1]);
            Destroy(savedGuns[2]);
            Destroy(savedGuns[3]);
            Destroy(savedGuns[4]);
            Destroy(savedGuns[5]);
            Destroy(savedGuns[6]);
        }
    }
}