using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RSClasses.MonoBehaviours
{
    internal class MirrorMageVisualizer_Mono : MonoBehaviour // Handles the reflections of opposing players
    {
        Dictionary<int, List<GameObject>> reflections = new Dictionary<int, List<GameObject>>();
        private Player player;

        public void Awake()
        {
            player = GetComponentInParent<Player>(); // Get player
        }

        public void Update()
        {
            if (player.data.view.IsMine) // Only run on card owner's client
            {
                foreach (Player other in PlayerManager.instance.players.Where(p => p.playerID != player.playerID)) // For each player besides yourself
                {
                    if (other.data.currentCards.Contains(CardHolder.cards["Mirror Mage"]))
                    {
                        if (!reflections.ContainsKey(other.playerID)) // If no reflections, make them
                        {
                            List<GameObject> reflectionList = new List<GameObject>();
                            var reflection = GameObject.Instantiate(RSClasses.assets.LoadAsset<GameObject>("Reflection"), other.transform);
                            reflection.transform.SetPositionAndRotation(new Vector3(1000, 1000, 1000), other.transform.rotation); // Set the position way offscreen until needed
                            reflection.SetActive(true);
                            reflection.GetComponent<SpriteRenderer>().color = other.GetTeamColors().color;
                            reflectionList.Add(reflection);
                            for (int i = 0; i < 6; i++)
                                reflectionList.Add(Instantiate(reflection, other.transform));
                            reflections[other.playerID] = reflectionList;
                        }
                        for (int i = 0; i < 7; i++)
                            reflections[other.playerID][i].SetActive(other.data.dead);
                        // Set reflection's position to be opposite opponent's
                        reflections[other.playerID][0].transform.SetPositionAndRotation(new Vector3(-other.transform.position.x, other.transform.position.y, other.transform.position.z), other.transform.rotation);

                        if (other.data.currentCards.Contains(CardHolder.cards["Prism"])) // Add extra reflections for Prism
                        {
                            reflections[other.playerID][1].transform.SetPositionAndRotation(new Vector3(other.transform.position.x, -other.transform.position.y, other.transform.position.z), other.transform.rotation);
                            reflections[other.playerID][2].transform.SetPositionAndRotation(new Vector3(-other.transform.position.x, -other.transform.position.y, other.transform.position.z), other.transform.rotation);
                        }
                        if (other.data.currentCards.Contains(CardHolder.cards["Kaleido Witch"])) // Add extra reflections for Kaleido Witch
                        {
                            reflections[other.playerID][3].transform.SetPositionAndRotation(new Vector3(other.transform.position.y, other.transform.position.x, other.transform.position.z), other.transform.rotation);
                            reflections[other.playerID][4].transform.SetPositionAndRotation(new Vector3(-other.transform.position.y, other.transform.position.x, other.transform.position.z), other.transform.rotation);
                            reflections[other.playerID][5].transform.SetPositionAndRotation(new Vector3(other.transform.position.y, -other.transform.position.x, other.transform.position.z), other.transform.rotation);
                            reflections[other.playerID][6].transform.SetPositionAndRotation(new Vector3(-other.transform.position.y, -other.transform.position.x, other.transform.position.z), other.transform.rotation);
                        }
                    }
                }
            }
        }

        private void OnDestroy()
        {
            foreach (KeyValuePair<int, List<GameObject>> item in reflections) // Destroy all the reflections
                foreach (GameObject reflection in item.Value) Destroy(reflection);
        }
    }
}