using Photon.Pun;
using System;
using UnboundLib;
using UnityEngine;

namespace RSClasses
{
    public class KaleidoBounce : MonoBehaviour
    {
        private float sinceBounce = 1f;

        private Camera mainCam;

        private RayHitReflect reflect;

        private Vector2 lastNormal;

        private ProjectileHit projHit;

        private RayHitBulletSound bulletSound;

        private PhotonView view;

        private bool positive;
        private bool positive2;

        private void Start()
        {
            GetComponentInParent<ChildRPC>().childRPCsVector2Vector2IntInt.Add("KaleidoBounce", DoHit);
            view = GetComponentInParent<PhotonView>();
            bulletSound = GetComponentInParent<RayHitBulletSound>();
            projHit = GetComponentInParent<ProjectileHit>();
            MirrorBounce[] componentsInChildren = base.transform.root.GetComponentsInChildren<MirrorBounce>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                if (i > 0)
                {
                    UnityEngine.Object.Destroy(componentsInChildren[i]);
                }
            }
            mainCam = MainCam.instance.transform.GetComponent<Camera>();
            reflect = GetComponentInParent<RayHitReflect>();
            positive = this.transform.position.x + this.transform.position.y > 0f;
            positive2 = this.transform.position.x - this.transform.position.y > 0f;
        }

        private void FixedUpdate()
        {
            sinceBounce += Time.fixedDeltaTime;
            if (!view.IsMine || sinceBounce < 0.1f)
                return;
            if (this.transform.position.x + this.transform.position.y != 0 && this.transform.position.x + this.transform.position.y > 0f == positive // Check if the bullet is still on the correct side of the screen
                && this.transform.position.x - this.transform.position.y != 0 && this.transform.position.x - this.transform.position.y > 0f == positive2)
                return;

            RaycastHit2D raycastHit2D = default(RaycastHit2D);
            raycastHit2D.normal = new Vector2(1f, -1f).normalized;
            if (Math.Abs(this.transform.position.x + this.transform.position.y) < Math.Abs(this.transform.position.x - this.transform.position.y))
                raycastHit2D.normal = new Vector2(1f, 1f).normalized;
            raycastHit2D.point = transform.position;
            int num = -1;
            if ((bool)raycastHit2D.transform)
            {
                PhotonView component = raycastHit2D.transform.root.GetComponent<PhotonView>();
                if ((bool)component)
                {
                    num = component.ViewID;
                }
            }
            int intData = -1;
            if (num == -1)
            {
                Collider2D[] componentsInChildren = MapManager.instance.currentMap.Map.GetComponentsInChildren<Collider2D>();
                for (int i = 0; i < componentsInChildren.Length; i++)
                {
                    if (componentsInChildren[i] == raycastHit2D.collider)
                    {
                        intData = i;
                    }
                }
            }
            GetComponentInParent<ChildRPC>().CallFunction("KaleidoBounce", raycastHit2D.point, raycastHit2D.normal, num, intData);
            sinceBounce = 0f;
        }

        private void DoHit(Vector2 hitPos, Vector2 hitNormal, int viewID = -1, int colliderID = -1)
        {
            HitInfo hitInfo = new HitInfo();
            hitInfo.point = hitPos;
            hitInfo.normal = hitNormal;
            hitInfo.collider = null;
            if (viewID != -1)
            {
                PhotonView photonView = PhotonNetwork.GetPhotonView(viewID);
                hitInfo.collider = photonView.GetComponentInChildren<Collider2D>();
                hitInfo.transform = photonView.transform;
            }
            else if (colliderID != -1)
            {
                hitInfo.collider = MapManager.instance.currentMap.Map.GetComponentsInChildren<Collider2D>()[colliderID];
                hitInfo.transform = hitInfo.collider.transform;
            }
            DynamicParticles.instance.PlayBulletHit(projHit.damage, base.transform, hitInfo, projHit.projectileColor);
            bulletSound.DoHitEffect(hitInfo);
            if (reflect != null)
                reflect.DoHitEffect(hitInfo);
            if (reflect == null || reflect.reflects <= 0)
                projHit.InvokeMethod("DestroyMe");
        }
    }
}