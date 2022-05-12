using Sonigon;
using Sonigon.Internal;
using UnboundLib;
using UnityEngine;

namespace RSClasses.MonoBehaviours
{
    internal class ShatterMono : MonoBehaviour
    {
        private void Awake()
        {
            player = gameObject.GetComponent<Player>();
        }

        private void Start()
        {
            AudioClip shatterSound = RSClasses.ArtAssets.LoadAsset<AudioClip>("shatter.ogg");
            SoundContainer shatterSoundContainer = ScriptableObject.CreateInstance<SoundContainer>();
            shatterSoundContainer.setting.volumeIntensityEnable = true;
            shatterSoundContainer.audioClip[0] = shatterSound;
            shatter = ScriptableObject.CreateInstance<SoundEvent>();
            shatter.soundContainerArray[0] = shatterSoundContainer;

            AudioClip reflectSound = RSClasses.ArtAssets.LoadAsset<AudioClip>("reflect.ogg");
            SoundContainer reflectSoundContainer = ScriptableObject.CreateInstance<SoundContainer>();
            reflectSoundContainer.setting.volumeIntensityEnable = true;
            reflectSoundContainer.audioClip[0] = reflectSound;
            reflect = ScriptableObject.CreateInstance<SoundEvent>();
            reflect.soundContainerArray[0] = reflectSoundContainer;

            if (player.data.view.IsMine)
            {
                reflection = GameObject.Instantiate(RSClasses.ArtAssets.LoadAsset<GameObject>("Reflection"), player.transform);
                reflection.SetActive(true);
                reflection.GetComponent<SpriteRenderer>().color = player.GetTeamColors().color;
            }
        }

        public void Update()
        {
            cooldown = cooldown - TimeHandler.deltaTime < 0f ? 0f : cooldown - TimeHandler.deltaTime;
            if (player.data.view.IsMine)
            {
                if (shatterVisual)
                {
                    var hits = Physics2D.OverlapCircleAll(shatterVisual.transform.position, 3.75f);
                    foreach (var hit in hits)
                    {
                        var healthHandler = hit.gameObject.GetComponent<HealthHandler>();
                        if (healthHandler)
                        {
                            Player hitPlayer = ((Player)healthHandler.GetFieldValue("player"));
                            if (hitPlayer.playerID != player.playerID) healthHandler.CallTakeDamage(TimeHandler.deltaTime * 55 * Vector3.up,
                                (Vector2)transform.position, gameObject, player);
                        }
                    }
                }
                reflection.transform.SetPositionAndRotation(new Vector3(-player.transform.position.x, player.transform.position.y, player.transform.position.z), player.transform.rotation);
            }
        }

        public void TriggerReflect()
        {
            if (cooldown == 0)
            {
                SoundManager.Instance.PlayAtPosition(reflect, player.transform, player.transform, new SoundParameterBase[]
                {
                    soundParameterIntensity
                });

                player.GetComponent<PlayerCollision>().IgnoreWallForFrames(2);
                player.transform.SetPositionAndRotation(new Vector3(-player.transform.position.x, player.transform.position.y, player.transform.position.z), player.transform.rotation);
                cooldown = 1f;
            }
        }

        public void TriggerShatter()
        {
            if (cooldown == 0)
            {
                RSClasses.instance.ExecuteAfterSeconds(1f, () => Destroy(shatterVisual));

                soundParameterIntensity.intensity = Optionshandler.vol_Sfx / 1f * Optionshandler.vol_Master;
                SoundManager.Instance.PlayAtPosition(shatter, player.transform, player.transform, new SoundParameterBase[]
                {
                    soundParameterIntensity
                });

                shatterVisual = Instantiate(RSClasses.ArtAssets.LoadAsset<GameObject>("Shatter"));
                shatterVisual.transform.SetPositionAndRotation(player.transform.position, player.transform.rotation);
                shatterVisual.GetComponent<Canvas>().sortingLayerName = "MostFront";
            }
        }

        private void OnDestroy()
        {
            Destroy(shatterVisual);
        }

        GameObject reflection;
        GameObject shatterVisual;
        SoundEvent reflect;
        SoundEvent shatter;
        private float cooldown = 0f;
        private Player player;
        private SoundParameterIntensity soundParameterIntensity = new SoundParameterIntensity(0f, UpdateMode.Continuous);
    }

    internal class ShatterTrigger : WasDealtDamageTrigger
    {
        public ShatterMono mono;
        public bool shatter = false;
        public override void WasDealtDamage(Vector2 damage, bool selfDamage)
        {
            if (shatter) mono.TriggerShatter();
            mono.TriggerReflect();
        }

        private void OnDestroy()
        {
            Destroy(mono);
        }
    }
}