using HarmonyLib;
using Photon.Pun;
using System;
using System.Linq;
using UnboundLib;
using UnityEngine;
using WeaponsManager;

namespace RSClasses.MonoBehaviours
{
    public class Mirror_Mono : MonoBehaviour // All bullet reflection effects (i.e. Mirror Mage, Prism, Kaleido Witch, and the glitter cards)
    {
        private Player player;
        public Gun[] mirrorGuns;
        public GameObject[] gameObjects;
        private static GameObject _stopRecursionObj = null;
        private static GameObject _PoisonObj = null;
        private static GameObject _DazzleObj = null;
        private static GameObject _ColdObj = null;

        public static GameObject StopRecursionObj // Added to a projectile to stop bullets spawned by this effect from retriggering the effect
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

        public static ObjectsToSpawn[] StopRecursionSpawn // Adds the stop recursion object to a bullet
        {
            get
            {
                return new ObjectsToSpawn[] { new ObjectsToSpawn() { AddToProjectile = StopRecursionObj } };
            }
        }

        public static GameObject PoisonObj // Poison effect for Emerald Glitter
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

        public static ObjectsToSpawn[] PoisonSpawn // Adds the poison object to a bullet
        {
            get
            {
                return new ObjectsToSpawn[] { new ObjectsToSpawn() { AddToProjectile = PoisonObj } };
            }
        }

        public static GameObject DazzleObj // Dazzle effect for Ruby Dust
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

        public static ObjectsToSpawn[] DazzleSpawn // Adds the dazzle object to a bullet
        {
            get
            {
                return new ObjectsToSpawn[] { new ObjectsToSpawn() { AddToProjectile = DazzleObj } };
            }
        }

        public static GameObject ColdObj // Cold effect for Sapphire Shards
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

        public static ObjectsToSpawn[] ColdSpawn // Adds the cold object to a bullet
        {
            get
            {
                return new ObjectsToSpawn[] { new ObjectsToSpawn() { AddToProjectile = ColdObj } };
            }
        }

        public void Start()
        {
            player = GetComponentInParent<Player>(); // Get player and gun
            var manager = player.GetComponent<WeaponManager>();
            foreach (Gun gun in manager.weapons)
            {
                if (gun != null && gun.gameObject.activeSelf)
                {
                    gun.ShootPojectileAction += OnShootProjectileAction; // Add the shoot action to each of a player's guns
                }
            }

            foreach (Player other in PlayerManager.instance.players.Where(p => p.playerID != player.playerID)) // For each player besides yourself
            {
                other.gameObject.GetOrAddComponent<MirrorMageVisualizer_Mono>();
            }
        }

        public void Update()
        {
            var gun = player.data.weaponHandler.gun;

            // Opposite on X
            gameObjects[0].transform.position = Vector3.Scale(player.transform.position, new Vector3(-1, 1, 1));
            gameObjects[0].transform.rotation = new Quaternion(-gun.shootPosition.rotation.x, gun.shootPosition.rotation.y, gun.shootPosition.rotation.z, -gun.shootPosition.rotation.w);
            // Opposite on Y
            gameObjects[1].transform.position = Vector3.Scale(player.transform.position, new Vector3(1, -1, 1));
            gameObjects[1].transform.rotation = new Quaternion(gun.shootPosition.rotation.x, -gun.shootPosition.rotation.y, gun.shootPosition.rotation.z, -gun.shootPosition.rotation.w);
            //Opposite on X & Y
            gameObjects[2].transform.position = Vector3.Scale(player.transform.position, new Vector3(-1, -1, 1));
            gameObjects[2].transform.rotation = new Quaternion(-gun.shootPosition.rotation.x, -gun.shootPosition.rotation.y, gun.shootPosition.rotation.z, gun.shootPosition.rotation.w);

            Vector3 normalDir = Quaternion.AngleAxis(45, Vector3.forward) * Vector3.up;
            gameObjects[3].transform.position = new Vector3(player.transform.position.y, player.transform.position.x, 0);
            gameObjects[3].transform.rotation = Quaternion.LookRotation(Vector3.Reflect(gun.shootPosition.forward, normalDir), gun.shootPosition.up);

            gameObjects[5].transform.position = new Vector3(player.transform.position.y * -1f, player.transform.position.x, 0);
            gameObjects[5].transform.rotation = Quaternion.LookRotation(Vector3.Reflect(gameObjects[3].transform.forward, Vector3.right), gameObjects[3].transform.up);

            normalDir = Quaternion.AngleAxis(-45, Vector3.forward) * Vector3.up;
            gameObjects[4].transform.position = new Vector3(player.transform.position.y * -1f, player.transform.position.x * -1f, 0);
            gameObjects[4].transform.rotation = Quaternion.LookRotation(Vector3.Reflect(gun.shootPosition.forward, normalDir), gun.shootPosition.up);

            gameObjects[6].transform.position = new Vector3(player.transform.position.y, player.transform.position.x * -1f, 0);
            gameObjects[6].transform.rotation = Quaternion.LookRotation(Vector3.Reflect(gameObjects[4].transform.forward, Vector3.right), gameObjects[3].transform.up);
        }

        public void OnShootProjectileAction(GameObject obj)
        {
            // If the bullet has the StopRecursion component in it somewhere, do not trigger
            if (obj.GetComponentsInChildren<StopRecursion>().Length > 0)
            {
                return;
            }

            var gun = player.data.weaponHandler.gun;

            foreach (var mirrorGun in mirrorGuns)
            {
                CopyGunStatsExceptActions(gun, mirrorGun);
                CopyAttackAction(gun, mirrorGun);
                CopyShootProjectileAction(gun, mirrorGun);
                mirrorGun.ShootPojectileAction -= OnShootProjectileAction;

                mirrorGun.numberOfProjectiles = 1;
                mirrorGun.bursts = 0;
                mirrorGun.spread = 0f;
                mirrorGun.evenSpread = 0f;
                mirrorGun.objectsToSpawn = mirrorGun.objectsToSpawn.Concat(StopRecursionSpawn).ToArray();
            }

            mirrorGuns[1].gravity *= -1f;
            mirrorGuns[2].gravity *= -1f;

            if (player.data.currentCards.Contains(CardHolder.cards["Sapphire Shards"]))
            {
                mirrorGuns[0].slow = 0.7f;
                mirrorGuns[0].projectileColor = new Color(0, 172, 191);

                mirrorGuns[1].slow = 0.7f;
                mirrorGuns[1].projectileColor = new Color(0, 172, 191);
            }

            if (player.data.currentCards.Contains(CardHolder.cards["Emerald Glitter"]))
            {
                mirrorGuns[3].damage *= 1.25f;
                mirrorGuns[3].objectsToSpawn = mirrorGuns[3].objectsToSpawn.Concat(PoisonSpawn).ToArray();
                mirrorGuns[3].projectileColor = Color.green;

                mirrorGuns[4].damage *= 1.25f;
                mirrorGuns[4].objectsToSpawn = mirrorGuns[4].objectsToSpawn.Concat(PoisonSpawn).ToArray();
                mirrorGuns[4].projectileColor = Color.green;
            }

            if (player.data.currentCards.Contains(CardHolder.cards["Ruby Dust"]))
            {
                mirrorGuns[5].objectsToSpawn = mirrorGuns[5].objectsToSpawn.Concat(DazzleSpawn).ToArray();
                mirrorGuns[5].projectileColor = Color.magenta;

                mirrorGuns[6].objectsToSpawn = mirrorGuns[6].objectsToSpawn.Concat(DazzleSpawn).ToArray();
                mirrorGuns[6].projectileColor = Color.magenta;
            }

            // Make sure to not fire for each player in the lobby
            if (!(player.data.view.IsMine || PhotonNetwork.OfflineMode))
            {
                return;
            }

            mirrorGuns[0].Attack(gun.currentCharge, true, 1f, 1f, true); // Sapphire

            // Only do these if the player has Prism
            if (player.data.currentCards.Contains(CardHolder.cards["Prism"]))
            {
                mirrorGuns[1].Attack(gun.currentCharge, true, 1f, 1f, true); // Sapphire
                mirrorGuns[2].Attack(gun.currentCharge, true, 1f, 1f, true); // Mirror
            }
            if (player.data.currentCards.Contains(CardHolder.cards["Kaleido Witch"]))
            {
                mirrorGuns[3].Attack(gun.currentCharge, true, 1f, 1f, true); // Emerald
                mirrorGuns[4].Attack(gun.currentCharge, true, 1f, 1f, true); // Emerald
                mirrorGuns[5].Attack(gun.currentCharge, true, 1f, 1f, true); // Ruby
                mirrorGuns[6].Attack(gun.currentCharge, true, 1f, 1f, true); // Ruby
            }
        }

        public void OnDestroy()
        {
            // Remove action when the mono is removed
            var manager = player.GetComponent<WeaponManager>();
            foreach (Gun gun in manager.weapons)
            {
                if (gun != null && gun.gameObject.activeSelf)
                {
                    gun.ShootPojectileAction -= OnShootProjectileAction; // Add the shoot action to each of a player's guns
                }
            }
        }

        public void CopyGunStatsExceptActions(Gun copyFromGun, Gun copyToGun)
        {
            copyToGun.ammo = copyFromGun.ammo;
            copyToGun.ammoReg = copyFromGun.ammoReg;
            copyToGun.attackID = copyFromGun.attackID;
            copyToGun.attackSpeed = copyFromGun.attackSpeed;
            copyToGun.attackSpeedMultiplier = copyFromGun.attackSpeedMultiplier;
            copyToGun.bodyRecoil = copyFromGun.bodyRecoil;
            copyToGun.bulletDamageMultiplier = copyFromGun.bulletDamageMultiplier;
            copyToGun.bulletPortal = copyFromGun.bulletPortal;
            copyToGun.bursts = copyFromGun.bursts;
            copyToGun.chargeDamageMultiplier = copyFromGun.chargeDamageMultiplier;
            copyToGun.chargeEvenSpreadTo = copyFromGun.chargeEvenSpreadTo;
            copyToGun.chargeNumberOfProjectilesTo = copyFromGun.chargeNumberOfProjectilesTo;
            copyToGun.chargeRecoilTo = copyFromGun.chargeRecoilTo;
            copyToGun.chargeSpeedTo = copyFromGun.chargeSpeedTo;
            copyToGun.chargeSpreadTo = copyFromGun.chargeSpreadTo;
            copyToGun.cos = copyFromGun.cos;
            copyToGun.currentCharge = copyFromGun.currentCharge;
            copyToGun.damage = copyFromGun.damage;
            copyToGun.damageAfterDistanceMultiplier = copyFromGun.damageAfterDistanceMultiplier;
            copyToGun.defaultCooldown = copyFromGun.defaultCooldown;
            copyToGun.destroyBulletAfter = copyFromGun.destroyBulletAfter;
            copyToGun.dmgMOnBounce = copyFromGun.dmgMOnBounce;
            copyToGun.dontAllowAutoFire = copyFromGun.dontAllowAutoFire;
            copyToGun.drag = copyFromGun.drag;
            copyToGun.dragMinSpeed = copyFromGun.dragMinSpeed;
            copyToGun.evenSpread = copyFromGun.evenSpread;
            copyToGun.explodeNearEnemyDamage = copyFromGun.explodeNearEnemyDamage;
            copyToGun.forceSpecificAttackSpeed = copyFromGun.forceSpecificAttackSpeed;
            copyToGun.forceSpecificShake = copyFromGun.forceSpecificShake;
            copyToGun.gravity = copyFromGun.gravity;
            copyToGun.hitMovementMultiplier = copyFromGun.hitMovementMultiplier;
            copyToGun.ignoreWalls = copyFromGun.ignoreWalls;
            copyToGun.isProjectileGun = copyFromGun.isProjectileGun;
            copyToGun.isReloading = copyFromGun.isReloading;
            copyToGun.knockback = copyFromGun.knockback;
            copyToGun.lockGunToDefault = copyFromGun.lockGunToDefault;
            copyToGun.multiplySpread = copyFromGun.multiplySpread;
            copyToGun.numberOfProjectiles = copyFromGun.numberOfProjectiles;
            copyToGun.objectsToSpawn = copyFromGun.objectsToSpawn.ToArray();
            copyToGun.overheatMultiplier = copyFromGun.overheatMultiplier;
            copyToGun.percentageDamage = copyFromGun.percentageDamage;
            copyToGun.player = copyFromGun.player;
            copyToGun.projectielSimulatonSpeed = copyFromGun.projectielSimulatonSpeed;
            copyToGun.projectileColor = copyFromGun.projectileColor;
            copyToGun.projectiles = copyFromGun.projectiles.ToArray();
            copyToGun.projectileSize = copyFromGun.projectileSize;
            copyToGun.projectileSpeed = copyFromGun.projectileSpeed;
            copyToGun.randomBounces = copyFromGun.randomBounces;
            copyToGun.recoil = copyFromGun.recoil;
            copyToGun.recoilMuiltiplier = copyFromGun.recoilMuiltiplier;
            copyToGun.reflects = copyFromGun.reflects;
            copyToGun.reloadTime = copyFromGun.reloadTime;
            copyToGun.reloadTimeAdd = copyFromGun.reloadTimeAdd;
            copyToGun.shake = copyFromGun.shake;
            copyToGun.shakeM = copyFromGun.shakeM;
            copyToGun.size = copyFromGun.size;
            copyToGun.slow = copyFromGun.slow;
            copyToGun.smartBounce = copyFromGun.smartBounce;
            copyToGun.soundDisableRayHitBulletSound = copyFromGun.soundDisableRayHitBulletSound;
            copyToGun.soundGun = copyFromGun.soundGun;
            copyToGun.soundImpactModifier = copyFromGun.soundImpactModifier;
            copyToGun.soundShotModifier = copyFromGun.soundShotModifier;
            copyToGun.spawnSkelletonSquare = copyFromGun.spawnSkelletonSquare;
            copyToGun.speedMOnBounce = copyFromGun.speedMOnBounce;
            copyToGun.spread = copyFromGun.spread;
            copyToGun.teleport = copyFromGun.teleport;
            copyToGun.timeBetweenBullets = copyFromGun.timeBetweenBullets;
            copyToGun.timeToReachFullMovementMultiplier = copyFromGun.timeToReachFullMovementMultiplier;
            copyToGun.unblockable = copyFromGun.unblockable;
            copyToGun.useCharge = copyFromGun.useCharge;
            copyToGun.waveMovement = copyFromGun.waveMovement;
        }

        public void CopyAttackAction(Gun copyFromGun, Gun copyToGun)
        {
            if ((Action)Traverse.Create(copyFromGun).Field("attackAction").GetValue() == null)
            {
                return;
            }

            copyToGun.attackAction = (Action)(((Action)Traverse.Create(copyFromGun).Field("attackAction").GetValue()).Clone());
        }

        public void CopyShootProjectileAction(Gun copyFromGun, Gun copyToGun)
        {
            if (copyFromGun.ShootPojectileAction == null)
            {
                return;
            }

            copyToGun.ShootPojectileAction = (Action<GameObject>)(copyFromGun.ShootPojectileAction.Clone());
        }
    }
}