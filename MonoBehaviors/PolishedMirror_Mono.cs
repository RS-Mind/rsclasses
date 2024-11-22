using RSClasses.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RSClasses.MonoBehaviours
{
    internal class PolishedMirror_Mono : MonoBehaviour
    {
        public void Awake()
        {
            player = GetComponentInParent<Player>();
        }

        public void Update()
        {
            if (player.data.view.IsMine)
            {
                foreach (Player other in PlayerManager.instance.players.Where(p => p.playerID != player.playerID))
                {
                    if (!reflections.ContainsKey(other.playerID))
                    {
                        List<GameObject> reflectionList = new List<GameObject>();
                        var reflection = GameObject.Instantiate(RSClasses.assets.LoadAsset<GameObject>("Reflection"), other.transform);
                        reflection.transform.SetPositionAndRotation(new Vector3(1000, 1000, 1000), other.transform.rotation);
                        reflection.SetActive(true);
                        reflection.GetComponent<SpriteRenderer>().color = other.GetTeamColors().color;
                        reflectionList.Add(reflection);
                        reflectionList.Add(Instantiate(reflection, other.transform));
                        reflectionList.Add(Instantiate(reflection, other.transform));
                        reflectionList.Add(Instantiate(reflection, other.transform));
                        reflectionList.Add(Instantiate(reflection, other.transform));
                        reflectionList.Add(Instantiate(reflection, other.transform));
                        reflectionList.Add(Instantiate(reflection, other.transform));
                        reflections[other.playerID] = reflectionList;
                    }
                    reflections[other.playerID][0].transform.SetPositionAndRotation(new Vector3(-other.transform.position.x, other.transform.position.y, other.transform.position.z), other.transform.rotation);

                    if (player.data.currentCards.Contains(CardHolder.cards["Prism"]))
                    {
                        reflections[other.playerID][1].transform.SetPositionAndRotation(new Vector3(other.transform.position.x, -other.transform.position.y, other.transform.position.z), other.transform.rotation);
                        reflections[other.playerID][2].transform.SetPositionAndRotation(new Vector3(-other.transform.position.x, -other.transform.position.y, other.transform.position.z), other.transform.rotation);
                    }
                    if (player.data.currentCards.Contains(CardHolder.cards["Kaleido Witch"]))
                    {
                        reflections[other.playerID][3].transform.SetPositionAndRotation(new Vector3(other.transform.position.y, other.transform.position.x, other.transform.position.z), other.transform.rotation);
                        reflections[other.playerID][4].transform.SetPositionAndRotation(new Vector3(-other.transform.position.y, other.transform.position.x, other.transform.position.z), other.transform.rotation);
                        reflections[other.playerID][5].transform.SetPositionAndRotation(new Vector3(other.transform.position.y, -other.transform.position.x, other.transform.position.z), other.transform.rotation);
                        reflections[other.playerID][6].transform.SetPositionAndRotation(new Vector3(-other.transform.position.y, -other.transform.position.x, other.transform.position.z), other.transform.rotation);
                    }
                }
            }
        }

        private void OnDestroy()
        {
            foreach (KeyValuePair<int, List<GameObject>> item in reflections)
                foreach (GameObject reflection in item.Value) Destroy(reflection);
        }

        Dictionary<int, List<GameObject>> reflections = new Dictionary<int, List<GameObject>>();
        private Player player;
    }
}