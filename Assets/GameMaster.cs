using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EVMC4U;
using System.IO;
using System.Threading;
using System.Text;
using Unity.IO.LowLevel.Unsafe;
using VRM;
using Unity.VisualScripting;
using UnityEngine.XR.Interaction.Toolkit;

public class GameMaster : MonoBehaviour
{
    public ExternalReceiver externalReceiver;
    public CommunicationValidator communicationValidator;
    public Transform left;
    public Transform right;
    string loadStatus = "Wait...";
    bool isLoaded = false;
    void Start()
    {
        Application.targetFrameRate = 90;
    }
    void Update()
    {
        if (!isLoaded) {
            string loadPath = "";
            if (File.Exists(@"VMCPViewerVR.vrm"))
            {
                // カレントフォルダ
                loadPath = @"VMCPViewerVR.vrm";
            }
            else if (File.Exists(@"C:\VMCPViewerVR.vrm"))
            {
                // Cドライブ
                loadPath = @"C:\VMCPViewerVR.vrm";
            }
            else if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\VMCPViewerVR.vrm"))
            {
                // Cドキュメントフォルダ
                loadPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\VMCPViewerVR.vrm";
            }
            else if (File.Exists(@"default.vrm"))
            {
                // カレントフォルダ
                loadPath = @"default.vrm";
            }
            else if (File.Exists(@"C:\default.vrm"))
            {
                // Cドライブ
                loadPath = @"C:\default.vrm";
            }
            else if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\default.vrm"))
            {
                // Cドキュメントフォルダ
                loadPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\default.vrm";
            }
            else
            {
                loadPath = "";
            }

            if (loadPath != "")
            {
                loadStatus = "Load: " + loadPath;
                externalReceiver.LoadVRM(loadPath);
            }
            else
            {
                loadStatus = @"default.vrm is not found. Put in C:\ or Document or Current Directory.";
            }
            isLoaded = true;
        }

        // 入力
        bool leftTriggerPressed = false;
        bool rightTriggerPressed = false;
        bool leftGripPressed = false;
        bool rightGripPressed = false;

        var leftHandDevices = new List<UnityEngine.XR.InputDevice>();
        var rightHandDevices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.LeftHand, leftHandDevices);
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.RightHand, rightHandDevices);

        if (leftHandDevices.Count == 1)
        {
            UnityEngine.XR.InputDevice device = leftHandDevices[0];

            device.IsPressed(InputHelpers.Button.Trigger, out leftTriggerPressed);
            device.IsPressed(InputHelpers.Button.Grip, out leftGripPressed);
        }

        if (rightHandDevices.Count == 1)
        {
            UnityEngine.XR.InputDevice device = rightHandDevices[0];

            device.IsPressed(InputHelpers.Button.Trigger, out rightTriggerPressed);
            device.IsPressed(InputHelpers.Button.Grip, out rightGripPressed);
        }

        if (leftTriggerPressed)
        {
            externalReceiver.transform.position = left.position;
            externalReceiver.transform.rotation = left.rotation;
        }
        if (rightTriggerPressed)
        {
            externalReceiver.transform.position = right.position;
            externalReceiver.transform.rotation = right.rotation;
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 20 * 0, 800, 50), "LOAD STATUS (EXR) : " + loadStatus);
        GUI.Label(new Rect(10, 20 * 1, 800, 50), "LOADED VRM (EXR) : "+externalReceiver.loadedVRMName);
        GUI.Label(new Rect(10, 20 * 2, 800, 50), "AVAILABLE (COMM) : " + communicationValidator.Available);
        GUI.Label(new Rect(10, 20 * 3, 800, 50), "TIME      (COMM) : " + communicationValidator.time);

        if (GUI.Button(new Rect(10, 20 * 4, 120, 20), "[ON] Freeze"))
        {
            externalReceiver.Freeze = true;
        }
        if (GUI.Button(new Rect(10 + 120 + 5, 20 * 4, 120, 20), "[OFF] Freeze"))
        {
            externalReceiver.Freeze = false;
        }
        if (GUI.Button(new Rect(10, 20 * 5, 120, 20), "[ON] Spring"))
        {
            foreach (var c in externalReceiver.Model.GetComponentsInChildren<VRMSpringBone>())
            {
                c.enabled = true;
            }
        }
        if (GUI.Button(new Rect(10 + 120 + 5, 20 * 5, 120, 20), "[OFF] Spring"))
        {
            foreach (var c in externalReceiver.Model.GetComponentsInChildren<VRMSpringBone>())
            {
                c.enabled = false;
            }
        }
    }
}
