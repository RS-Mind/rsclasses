using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using RSClasses.Utilities;
using Sonigon;
using Sonigon.Internal;
using System.Collections.Generic;
using UnboundLib;
using UnityEngine;
using UnityEngine.UI;
using TabInfo.Extensions;

namespace RSClasses
{
    public class SovereignBladeMono : MonoBehaviour, IOnEventCallback
    {
        private List<Collider2D> hitTargets = new List<Collider2D>();
        private ParticleSystem chargeParticles;
        internal PhotonView view;
        private GameObject blade;
        private Player player;
        private float targetRot = 0f;
        private float targetSpin = 0f;
        private float rotVel = 0f;
        private float spinVel = 0f;
        private float currentRot = 0f;
        private float currentSpin = 0f;
        private BladeState status = BladeState.Idle;
        public float spring = 25f;
        public float drag = 25f;
        private Vector2 targetPos;
        private Vector3 posVel = Vector3.zero;
        internal static byte InitEventCode = 65;

        private SoundParameterIntensity soundParameterIntensity = new SoundParameterIntensity(0f, UpdateMode.Continuous);

        enum BladeState
        {
            Idle,
            Preparing,
            Attacking,
            Returning
        }

        void Start()
        {
            PhotonNetwork.AddCallbackTarget(this);
            player = GetComponentInParent<Player>();
            if (player.data.view.IsMine)
            {
                view = gameObject.GetComponent<PhotonView>();
                if (PhotonNetwork.AllocateViewID(view))
                {
                    object[] data = new object[]
                    {
                        player.playerID,
                        view.ViewID
                    };
                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions
                    {
                        Receivers = ReceiverGroup.Others,
                        CachingOption = EventCaching.AddToRoomCache
                    };

                    SendOptions sendOptions = new SendOptions { Reliability = true };

                    RSClasses.instance.ExecuteAfterSeconds(1f, () => PhotonNetwork.RaiseEvent(InitEventCode, data, raiseEventOptions, sendOptions));
                    PhotonNetwork.RemoveCallbackTarget(this);
                }
                else
                {
                    UnityEngine.Debug.LogError("Failed to allocate a ViewId to Sovereign Blade.");
                }
            }
            blade = transform.GetChild(0).gameObject;
            blade.transform.GetChild(0).GetComponent<Image>().color = player.GetTeamColors().particleEffect;
            blade.transform.parent = null;
            chargeParticles = blade.GetComponent<ParticleSystem>();

            if (UnboundLib.Extensions.PlayerExtensions.GetAdditionalData(player).colorID == 165)
            {
                blade.transform.GetComponent<Image>().color = player.GetTeamColors().color;
                ParticleSystem.MainModule main = chargeParticles.main;
                ParticleSystem.MinMaxGradient startColor = main.startColor;
                startColor.colorMin = player.GetTeamColors().backgroundColor;
                startColor.colorMax = player.GetTeamColors().color;
                main.startColor = startColor;
            }

            player.data.block.BlockAction += OnBlock;
        }

        void OnDestroy()
        {
            player.data.block.BlockAction -= OnBlock;
            Destroy(blade);
        }

        public void OnEvent(EventData photonEvent)
        {
            byte eventCode = photonEvent.Code;
            if (eventCode != SovereignBladeMono.InitEventCode)
                return;

            object[] data = (object[])photonEvent.CustomData;
            if (player.playerID != (int)data[0])
                return;

            view = gameObject.GetComponent<PhotonView>();
            view.ViewID = (int)data[1];
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        void FixedUpdate()
        {

            if (!player.data.view.IsMine || status != BladeState.Attacking) // Only run on the card holder's client
            {
                hitTargets.Clear();
                return;
            }

            var hits = Physics2D.OverlapBoxAll(blade.transform.position, new Vector2(2.25f * player.data.GetAdditionalData().bladeSize,
                0.55f * player.data.GetAdditionalData().bladeSize), blade.transform.rotation.eulerAngles.z);
            foreach (var hit in hits) // For each target
            {
                if (hitTargets.Contains(hit))
                    continue;
                // A target should only be hit once
                hitTargets.Add(hit);

                var damageable = hit.gameObject.GetComponent<Damagable>(); // Grab the damageable object, if any
                var healthHandler = hit.gameObject.GetComponent<HealthHandler>(); // Grab the opponent's health handler, if any

                float damage = player.data.weaponHandler.gun.damage * 55f;

                if (healthHandler) // If the target is a player basically
                {
                    Player hitPlayer = ((Player)healthHandler.GetFieldValue("player"));
                    if (hitPlayer == player)
                        continue;
                    SoundManager.Instance.PlayAtPosition(healthHandler.soundBounce, this.transform, damageable.transform); // Play sfx
                    healthHandler.CallTakeForce(blade.transform.right.normalized * 2500, ForceMode2D.Impulse, true); // Apply knockback
                    if (((Player)healthHandler.GetFieldValue("player")).GetComponent<Block>().blockedThisFrame) { continue; } // Skip everything else if they blocked
                }
                if (damageable) // If the target can take damage
                {
                    damageable.CallTakeDamage(((Vector2)damageable.transform.position - (Vector2)blade.transform.position).normalized * damage,
                        (Vector2)blade.transform.position, blade, player); // Apply damage
                }
            }
        }

        void OnEnable()
        {
            blade.SetActive(true);
        }

        void OnDisable()
        {
            blade.SetActive(false);
        }

        void Update()
        {
            var offset = transform.up * 3;
            switch (status)
            {
                case (BladeState.Idle):
                    transform.Rotate(Vector3.forward, -Time.deltaTime * 60);
                    blade.transform.position = transform.position + offset;

                    bool flag = player.data.aimDirection.x > 0;
                    targetRot = flag ? 0f : 180f;
                    targetSpin = 0f;
                    break;
                case (BladeState.Preparing):
                    if (targetRot == 0f)
                        targetSpin = Vector2.SignedAngle(Vector2.left, (Vector2)blade.transform.position - targetPos);
                    else
                        targetSpin = -Vector2.SignedAngle(Vector2.right, (Vector2)blade.transform.position - targetPos);
                    break;
                case (BladeState.Attacking):
                    posVel = FRILerp.Lerp(posVel, ((Vector3)targetPos - blade.transform.position) * spring, 50f);
                    blade.transform.position += posVel * TimeHandler.deltaTime;
                    break;
                case (BladeState.Returning):
                    posVel = FRILerp.Lerp(posVel, (transform.position + offset - blade.transform.position) * spring, drag);
                    blade.transform.position += posVel * TimeHandler.deltaTime;

                    bool flag2 = player.data.aimDirection.x > 0;
                    targetRot = flag2 ? 0f : 180f;
                    targetSpin = 0f;
                    break;
            }
            rotVel = FRILerp.Lerp(rotVel, (targetRot - currentRot) * spring, drag);
            spinVel = FRILerp.Lerp(spinVel, (targetSpin - currentSpin) * spring, drag);
            currentRot += rotVel * TimeHandler.deltaTime;
            currentSpin += spinVel * TimeHandler.deltaTime;
            blade.transform.localEulerAngles = new Vector3(0f, currentRot, currentSpin);
            blade.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f) * player.data.GetAdditionalData().bladeSize;
        }

        [PunRPC]
        public void StartBladeAttack(Vector2 pos)
        {
            targetPos = pos;
            status = BladeState.Preparing;
            chargeParticles.Play();
            soundParameterIntensity.intensity = (Optionshandler.vol_Sfx / 1f) * Optionshandler.vol_Master; // Play sfx
            SoundManager.Instance.PlayAtPosition(RSClasses.sovereignBladeChargeSound, transform, transform, new SoundParameterBase[] { soundParameterIntensity });
            RSClasses.instance.ExecuteAfterSeconds(0.5f, () => status = BladeState.Attacking);
            RSClasses.instance.ExecuteAfterSeconds(0.6f, () => {
                GamefeelManager.GameFeel(blade.transform.right * 10f);
                SoundManager.Instance.PlayAtPosition(RSClasses.sovereignBladeStrikeSound, transform, transform, new SoundParameterBase[] { soundParameterIntensity });
                });
            RSClasses.instance.ExecuteAfterSeconds(1.5f, () => status = BladeState.Returning);
            RSClasses.instance.ExecuteAfterSeconds(1.75f, () => status = BladeState.Idle);
        }

        void OnBlock(BlockTrigger.BlockTriggerType trigger)
        {
            if (trigger != BlockTrigger.BlockTriggerType.Default || !player.data.view.IsMine || status != BladeState.Idle)
                return;
            if (player.data.input.inputType == GeneralInput.InputType.Keyboard)
            {
                targetPos = MainCam.instance.cam.ScreenToWorldPoint(Input.mousePosition);
            }
            else
            {
                targetPos = transform.position + (player.data.aimDirection * 30f);
            }
            view.RPC("StartBladeAttack", RpcTarget.All, targetPos);
        }
    }
}