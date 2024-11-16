using System;
using System.ComponentModel.Design;
using BepInEx;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Xml.Linq;
using POpusCodec.Enums;
using System.Collections;



namespace GorillaUI
{
    [BepInIncompatibility("org.iidk.gorillatag.iimenu")]
    [BepInIncompatibility("com.goldentrophy.gorillatag.nametags")]
    [BepInIncompatibility("com.dedouwe26.gorillatag.cosmetx")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class GorillaUI : BaseUnityPlugin//oops
    {
        bool inRoom;
        bool GUIEnabled = false;
        bool isThirdPerson = true;
        bool GamemodeSelecterEnabled = false;
        bool GorillaMenuEnabled = false;
        bool NoClip = false;
        //bool NoClip2;
        bool DisabledLeaves = false;
        bool tped;
        private GameObject forest;
        private GameObject sky;
        private GameObject city1;
        private GameObject city2;
        private GameObject tree;
        private GameObject treeroom;
        private const string MODDEDCASUAL = "MODDED_CASUAL";
        private const string MODDEDINFECTION = "MODDED_INFECTION";
        string INFO = "https://github.com/Bobavrgt/GorillaUI/blob/master/README.md";

        int delay = 0;

        private string room = "";

        private MeshCollider[] MeshColliders;


        void Start()
        {
            forest = GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest");
            sky = GameObject.Find("Environment Objects/LocalObjects_Prefab/Standard Sky");
            city1 = GameObject.Find("Environment Objects/LocalObjects_Prefab/Vista_Prefab");
            city2 = GameObject.Find("Environment Objects/LocalObjects_Prefab/City_WorkingPrefab");
            tree = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables");
            treeroom = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom");
            MeshColliders = Resources.FindObjectsOfTypeAll<MeshCollider>();

        }

        void OnEnable()
        {
            HarmonyPatches.ApplyHarmonyPatches();
        }

        void OnDisable()
        {
            HarmonyPatches.RemoveHarmonyPatches();
        }

        void Update()
        {
            if(PhotonNetwork.InRoom == false)
            {
                OnLeave();
                
            }

            if (Keyboard.current.tabKey.wasPressedThisFrame)
            {
                GUIEnabled = !GUIEnabled;
            }


            if (Keyboard.current[Key.Backquote].wasPressedThisFrame)
            {
                GamemodeSelecterEnabled = !GamemodeSelecterEnabled;
            }

            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                isThirdPerson = !isThirdPerson;
                GorillaTagger.Instance.thirdPersonCamera.SetActive(isThirdPerson);
            }

            if (PhotonNetwork.CurrentRoom.CustomProperties["gameMode"].ToString().Contains("MODDED"))
            {
                OnJoin();
            }

            if (inRoom)
            {
                InRoom();
            }

            if (tped)
            {
                foreach (MeshCollider collider in MeshColliders)
                {
                    collider.enabled = NoClip;
                };
                SetEnvironment(true);
                tped = false;
            }

        }

        public void OnJoin()
        {
            inRoom = true;
        }


        private void OnLeave()
        {
            GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest").transform.GetChild(19).gameObject.SetActive(true);//leaf mod disable
            GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest").transform.GetChild(20).gameObject.SetActive(true);
            GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest").transform.GetChild(21).gameObject.SetActive(true);

            foreach (MeshCollider Coliders in MeshColliders)
            {
                Coliders.enabled = true;
            }
            GorillaMenuEnabled = false;
            inRoom = false;
        }

        private void SetEnvironment(bool enable)
        {

            forest.SetActive(enable);
            tree.SetActive(enable);
            treeroom.SetActive(enable);
            sky.SetActive(enable);
            city1.SetActive(false);
            city2.SetActive(false);
            
        }
        private void ThirdPerson()
        {
            isThirdPerson = !isThirdPerson;
            GorillaTagger.Instance.thirdPersonCamera.SetActive(isThirdPerson);
        }
        private void Noclip()
        {
            foreach (MeshCollider collider in MeshColliders)
            {
                collider.enabled = !NoClip;
            }
        }

        private void OnGUI()
        {
            if (GUIEnabled)
            {
                GorillaGUI();
            }

            if (GamemodeSelecterEnabled)
            {
                GamemodeSelector();
            }
            
            if (GorillaMenuEnabled)
            {
                GorillaMenu();
            }
            else
            {
                GorillaMenuEnabled = false;
            }
        }
        private void InRoom()
        {

            if (Keyboard.current.cKey.wasPressedThisFrame)
            {
                GorillaMenuEnabled = !GorillaMenuEnabled;
            }

            Noclip();


            if (DisabledLeaves)
            {
                ToggleLeaves(true);
            }
            else
            {
                ToggleLeaves(false);
            }


            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                NoClip = !NoClip;
            }

        }
        private void ToggleLeaves(bool enable)
        {
           forest.transform.GetChild(19).gameObject.SetActive(!DisabledLeaves);
           forest.transform.GetChild(20).gameObject.SetActive(!DisabledLeaves);
           forest.transform.GetChild(21).gameObject.SetActive(!DisabledLeaves);
        }
        private void JoinRoom()
        {
            if (!string.IsNullOrEmpty(room))
            {
                PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(room, JoinType.Solo);
            }
            else
            {
                Debug.Log("Room name is empty.");
            }
        }

        
        
            /*NoClip = true;
            GorillaLocomotion.Player.Instance.headCollider.transform.position = new Vector3(-68, 12, -83);

            // After the delay, disable NoClip
            NoClip = false;

            // Restore environment
            SetEnvironment(true);*/
        




        private void GorillaGUI()
        {
            GUI.Box(new Rect(10, 10, 150, 350), "GorillaUI");

            room = GUI.TextField(new Rect(15, 50, 140, 30), room, 25);

            if (GUI.Button(new Rect(15, 100, 140, 40), "Join Room"))
            {
                JoinRoom();
            }

            if (GUI.Button(new Rect(15, 150, 140, 40), "Disconnect"))
            {
                NetworkSystem.Instance.ReturnToSinglePlayer();
            }

            if (GUI.Button(new Rect(15, 200, 140, 40), "Quit Game"))
            {
                Application.Quit();
            }

            if (GUI.Button(new Rect(15, 250, 140, 40), isThirdPerson ? "FPC" : "3rd Person"))
            {
                {
                    ThirdPerson();
                }
            }

            if (GUI.Button(new Rect(15, 300, 140, 40), "Mod Info"))
            {
                {
                    Application.OpenURL(INFO);
                }
            }
        }

        private void GorillaMenu()
        {
            GUI.Box(new Rect(10, 400, 150, 250), "Gorilla Menu");

            if (GUI.Button(new Rect(15, 450, 140, 40), "NoClip"))
            {
                NoClip = !NoClip;
            }

            if (GUI.Button(new Rect(15, 500, 140, 40), "Toggle Leaves"))
            {
                DisabledLeaves = !DisabledLeaves;
            }

            if (GUI.Button(new Rect(15, 550, 140, 40), "Tp to stump"))
            {
                tped = true;
                foreach (MeshCollider collider in MeshColliders)
                {
                    collider.enabled = false;
                }
                GorillaLocomotion.Player.Instance.headCollider.transform.position = new Vector3(-68, 12, -83);
            }

            if (GUI.Button(new Rect(15, 600, 140, 40), "Placeholder"))
            {

            }
        }

        private void GamemodeSelector()
        {
            GUI.Box(new Rect(170, 10, 150, 250), "Gamemode selector");

            if (GUI.Button(new Rect(175, 50, 140, 40), "Set Casual"))
            {
                GorillaComputer.instance.currentGameMode.Value = "CASUAL";
                Debug.Log("Gamemode changed to CASUAL.");
            }

            if (GUI.Button(new Rect(175, 100, 140, 40), "Set Infection"))
            {
                GorillaComputer.instance.currentGameMode.Value = "INFECTION";
            }

            if (GUI.Button(new Rect(175, 150, 140, 40), "Set Modded Casual"))
            {
                GorillaComputer.instance.currentGameMode.Value = MODDEDCASUAL;
            }

            if (GUI.Button(new Rect(175, 200, 140, 40), "Set Modded"))
            {
                GorillaComputer.instance.currentGameMode.Value = MODDEDINFECTION;
            }
        }
    }
}