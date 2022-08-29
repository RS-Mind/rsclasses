using System;
using UnboundLib;
using UnityEngine;
using HarmonyLib;
using Photon.Pun;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace RSClasses.MonoBehaviours
{
    public class MirrorAssets
    {
        private static GameObject _mirror = null;

        internal static GameObject mirror
        {
            get
            {
                if (_mirror != null) { return _mirror; }
                else
                {
                    _mirror = new GameObject("RSC_Mirror", typeof(MirrorEffect), typeof(PhotonView));
                    UnityEngine.Object.DontDestroyOnLoad(_mirror);

                    return _mirror;
                }
            }
            set { }
        }
    }
    public class MirrorSpawner : MonoBehaviour
    {
        private static bool Initialized = false;



        void Awake()
        {
            if (!Initialized)
            {
                PhotonNetwork.PrefabPool.RegisterPrefab(MirrorAssets.mirror.name, MirrorAssets.mirror);
            }
        }

        void Start()
        {
            if (!Initialized)
            {
                Initialized = true;
                return;
            }

            if (!PhotonNetwork.OfflineMode && !this.gameObject.transform.parent.GetComponent<ProjectileHit>().ownPlayer.data.view.IsMine) return;


            PhotonNetwork.Instantiate(
                MirrorAssets.mirror.name,
                transform.position,
                transform.rotation,
                0,
                new object[] { this.gameObject.transform.parent.GetComponent<PhotonView>().ViewID }
            );
        }
    }
    [RequireComponent(typeof(PhotonView))]
    public class MirrorEffect : MonoBehaviour, IPunInstantiateMagicCallback
    {
        private Player player;
        private Gun gun;
        private Gun mirrorGun;
        private Gun realGun;
        private ProjectileHit projectile;

        public void OnPhotonInstantiate(Photon.Pun.PhotonMessageInfo info)
        {
            object[] instantiationData = info.photonView.InstantiationData;

            GameObject parent = PhotonView.Find((int)instantiationData[0]).gameObject;

            gameObject.transform.SetParent(parent.transform);

            player = parent.GetComponent<ProjectileHit>().ownPlayer;
            gun = player.GetComponent<Holding>().holdable.GetComponent<Gun>();
        }

        void Awake()
        {

        }
        void Start()
        {
            // get the projectile, player, and gun this is attached to
            projectile = gameObject.transform.parent.GetComponent<ProjectileHit>();
            player = projectile.ownPlayer;
            gun = player.GetComponent<Holding>().holdable.GetComponent<Gun>();

            // create a new gun for the spawnbulletseffect
            mirrorGun = player.gameObject.AddComponent<MirrorGun>();

            SpawnBulletsEffect mirrorEffect = player.gameObject.AddComponent<SpawnBulletsEffect>();

            // set the position and direction to fire
            Quaternion rotation = projectile.transform.rotation;
            rotation.x *= -1;
            rotation.w *= -1;
            mirrorEffect.SetDirection(rotation * Vector3.forward);

            mirrorEffect.SetPosition(Vector3.Scale(projectile.transform.position, new Vector3(-1, 1, 1)));
            mirrorEffect.SetNumBullets(1);
            mirrorEffect.SetTimeBetweenShots(0f);
            mirrorEffect.SetInitialDelay(0f);

            // copy gun stats over
            SpawnBulletsEffect.CopyGunStats(gun, mirrorGun);
            mirrorGun.objectsToSpawn = mirrorGun.objectsToSpawn.Where(obj => obj.AddToProjectile.GetComponent<MirrorSpawner>() == null).ToArray();
            mirrorGun.bursts = 1;
            mirrorGun.numberOfProjectiles = 1;
            mirrorGun.spread = 0;

            // set the gun of the spawnbulletseffect
            mirrorEffect.SetGun(mirrorGun);



            // create a new gun for the spawnbulletseffect
            realGun = player.gameObject.AddComponent<RealGun>();

            SpawnBulletsEffect realEffect = player.gameObject.AddComponent<SpawnBulletsEffect>();
            // set the position and direction to fire

            realEffect.SetDirection(projectile.transform.rotation * Vector3.forward);

            realEffect.SetPosition(projectile.transform.position);
            realEffect.SetNumBullets(1);
            realEffect.SetTimeBetweenShots(0f);
            realEffect.SetInitialDelay(0f);

            // copy gun stats over
            SpawnBulletsEffect.CopyGunStats(gun, realGun); ;
            realGun.objectsToSpawn = realGun.objectsToSpawn.Where(obj => obj.AddToProjectile.GetComponent<MirrorSpawner>() == null).ToArray();
            realGun.bursts = 1;
            realGun.numberOfProjectiles = 1;
            realGun.spread = 0;

            // set the gun of the spawnbulletseffect
            realEffect.SetGun(realGun);

            projectile.deathEvent.Invoke();
            Destroy(projectile.gameObject);
        }
    }
    class MirrorGun : Gun
    { }
    class RealGun : Gun
    { }
}