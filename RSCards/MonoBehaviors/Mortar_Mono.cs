using UnityEngine;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Text;

namespace RSCards.MonoBehaviors
{
    public class Mortar_Mono : MonoBehaviour
    {
		private void OnDestroy()
		{
			
		}

		private void Start()
		{
			this.childRPC = base.GetComponentInParent<ChildRPC>();
			this.view = base.GetComponentInParent<PhotonView>();
			this.hit = base.GetComponentInParent<ProjectileHit>();
			this.move = base.GetComponentInParent<MoveTransform>();
            this.spawned = base.GetComponentInParent<SpawnedAttack>();
            this.startVelocity = this.move.velocity.magnitude;
            base.GetComponentInParent<SyncProjectile>().active = true;
        }

		private void Update()
		{
            try
            { // I know this is bad practice but I cannot tell what is null reference excepting when. The code seems to work despite the null references, so this is just to avoid spamming console.
                if (!this.view.IsMine)
                {
                    return;
                }
                Vector3 vector = Vector3.zero;
                if (this.spawned.spawner.data.playerActions.Device != null)
                {
                    vector = this.spawned.spawner.data.input.aimDirection;
                }
                else
                {
                    vector = MainCam.instance.cam.ScreenToWorldPoint(Input.mousePosition) - base.transform.position;
                    vector.z = 0f;
                    vector.Normalize();
                }

                this.trigger = this.spawned.spawner.data.weaponHandler.gun.ReadyAmount() < 0.01f;

                vector += Vector3.Cross(Vector3.forward, vector) * this.move.selectedSpread;
                this.c += TimeHandler.deltaTime;
                if (this.trigger)
                {
                    if (this.snap)
                    {
                        if (this.spawned.spawner.data.block.blockedThisFrame)
                        {
                            this.move.velocity = this.move.velocity * -1f;
                            base.enabled = false;
                            return;
                        }
                    }
                    else
                    {
                        if (vector.magnitude > 0.2f && this.hit.sinceReflect > 2f)
                        {
                            //Rotate Bullet
                            this.move.velocity = Vector3.RotateTowards(this.move.velocity, vector.normalized * this.startVelocity, this.rotateSpeed * TimeHandler.deltaTime, this.rotateSpeed * TimeHandler.deltaTime * 10f);
                            if (this.c > 0.1f)
                            {
                                this.c = 0f;
                            }
                            if (!this.isOn)
                            {
                                this.move.simulateGravity++;
                            }
                            this.isOn = true;
                            return;
                        }
                        if (this.isOn)
                        {
                            this.move.simulateGravity--;
                        }
                        this.isOn = false;
                    }
                }
            }
            catch { }
        }

		public bool snap;
        public bool trigger;
		public float rotateSpeed = 1000f;
		private SpawnedAttack spawned;
		private MoveTransform move;
		private float startVelocity;
		private bool isOn;
		private ProjectileHit hit;
		private float c;
		private PhotonView view;
		private ChildRPC childRPC;
	}
}
