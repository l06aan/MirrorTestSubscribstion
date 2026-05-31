using System.Collections;
using Mirror;
using UnityEngine;

namespace TestTask.Networking.Demo
{
    public sealed class AutoStartHostForTest : MonoBehaviour
    {
        [SerializeField] private bool autoStartHost = true;

        private IEnumerator Start()
        {
            yield return null;

            if (!autoStartHost)
            {
                yield break;
            }

            if (NetworkManager.singleton == null)
            {
                Debug.LogError("[AutoStartHostForTest] NetworkManager.singleton is null.");
                yield break;
            }

            if (NetworkServer.active || NetworkClient.active)
            {
                Debug.Log("[AutoStartHostForTest] Network is already active.");
                yield break;
            }

            Debug.Log("[AutoStartHostForTest] Starting Host...");

            NetworkManager.singleton.StartHost();
        }
    }
}