using Photon.Pun;
using RSClasses.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

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
                    Object.DontDestroyOnLoad(_mirror);

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

            if (!PhotonNetwork.OfflineMode && !gameObject.transform.parent.GetComponent<ProjectileHit>().ownPlayer.data.view.IsMine) return;


            PhotonNetwork.Instantiate(
                MirrorAssets.mirror.name,
                transform.position,
                transform.rotation,
                0,
                new object[] { gameObject.transform.parent.GetComponent<PhotonView>().ViewID }
            );
        }
    }

    [RequireComponent(typeof(PhotonView))]
    public class MirrorEffect : MonoBehaviour, IPunInstantiateMagicCallback
    {
        private Player player;
        private Gun gun;
        private Gun newGun;
        private Gun newGun2;
        private ProjectileHit projectile;

        public void OnPhotonInstantiate(Photon.Pun.PhotonMessageInfo info)
        {
            object[] instantiationData = info.photonView.InstantiationData;

            GameObject parent = PhotonView.Find((int)instantiationData[0]).gameObject;

            gameObject.transform.SetParent(parent.transform);

            player = parent.GetComponent<ProjectileHit>().ownPlayer;
            gun = player.GetComponent<Holding>().holdable.GetComponent<Gun>();
        }

        void Start()
        {
            projectile = gameObject.transform.parent.GetComponent<ProjectileHit>();
            player = projectile.ownPlayer;
            gun = player.GetComponent<Holding>().holdable.GetComponent<Gun>();

            newGun = player.gameObject.AddComponent<MirrorGun>();

            SpawnBulletsEffect effect = player.gameObject.AddComponent<SpawnBulletsEffect>();

            effect.SetDirection(((Quaternion)typeof(Gun).InvokeMember("getShootRotation",
                BindingFlags.Instance | BindingFlags.InvokeMethod |
                BindingFlags.NonPublic, null, gun, new object[] { 0, 0, 0f })) * Vector3.forward);

            List<Vector3> positions = new List<Vector3>() { };
            positions.Add(Vector3.Scale(projectile.transform.position, new Vector3(-1, 1, 1)));
            positions.Add(projectile.transform.position);
            List<Vector3> directions = new List<Vector3>() { };
            Quaternion rotation = projectile.transform.rotation;
            rotation.x *= -1;
            rotation.w *= -1;
            directions.Add(rotation.eulerAngles);
            directions.Add(projectile.transform.rotation.eulerAngles);

            effect.SetDirections(directions);
            effect.SetPositions(positions);
            effect.SetNumBullets(2);
            effect.SetTimeBetweenShots(0f);
            effect.SetInitialDelay(0.1f);

            SpawnBulletsEffect.CopyGunStats(gun, newGun);
            newGun.objectsToSpawn = newGun.objectsToSpawn.Where(obj => obj.AddToProjectile.GetComponent<MirrorSpawner>() == null).ToArray();
            newGun.bursts = 1;
            newGun.numberOfProjectiles = 1;

            effect.SetGun(newGun);

            if (player.data.GetAdditionalData().prism)
            {
                // Upside down

                newGun2 = player.gameObject.AddComponent<PrismGun>();

                SpawnBulletsEffect effect2 = player.gameObject.AddComponent<SpawnBulletsEffect>();

                effect2.SetDirection(((Quaternion)typeof(Gun).InvokeMember("getShootRotation",
                    BindingFlags.Instance | BindingFlags.InvokeMethod |
                    BindingFlags.NonPublic, null, gun, new object[] { 0, 0, 0f })) * Vector3.forward);

                List<Vector3> positions2 = new List<Vector3>() { };
                positions2.Add(Vector3.Scale(projectile.transform.position, new Vector3(-1, -1, 1)));
                positions2.Add(Vector3.Scale(projectile.transform.position, new Vector3(1, -1, 1)));
                List<Vector3> directions2 = new List<Vector3>() { };
                Quaternion rotation3 = projectile.transform.rotation;
                rotation3.y *= -1;
                rotation3.x *= -1;
                directions2.Add(rotation3.eulerAngles);
                Quaternion rotation2 = projectile.transform.rotation;
                rotation2.y *= -1;
                rotation2.w *= -1;
                directions2.Add(rotation2.eulerAngles);

                effect2.SetDirections(directions2);
                effect2.SetPositions(positions2);
                effect2.SetNumBullets(2);
                effect2.SetTimeBetweenShots(0f);
                effect2.SetInitialDelay(0.1f);

                SpawnBulletsEffect.CopyGunStats(gun, newGun2);
                newGun2.objectsToSpawn = newGun2.objectsToSpawn.Where(obj => obj.AddToProjectile.GetComponent<MirrorSpawner>() == null).ToArray();
                newGun2.bursts = 1;
                newGun2.numberOfProjectiles = 1;
                newGun2.gravity = -1f;

                effect2.SetGun(newGun2);
            }

            Destroy(projectile.gameObject);
        }
    }
    class MirrorGun : Gun
    { }

    class PrismGun : Gun
    { }

    public class spawnBulletMono : MonoBehaviour
    {

        [PunRPC]
        public static void RPCA_Shoot(int bulletViewID, int numProj, float dmgM, float seed, Gun gunToShootFrom)
        {
            GameObject bulletObj = PhotonView.Find(bulletViewID).gameObject;
            gunToShootFrom.BulletInit(bulletObj, numProj, dmgM, seed, true);
        }
    }
}