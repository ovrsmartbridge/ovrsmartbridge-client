using UnityEngine;
using TMPro;
using System;
using System.IO;
using WebSocketSharp;
using Valve.VR;
using UnityEngine.UI;

public class OVRSmartBridgeServer : MonoBehaviour
{
    public OVR_Handler ovrHandler;

    public TextMeshProUGUI textMesh;

    public Animator animator;
    public AudioSource source;

    public TMP_InputField ipInputField;
    public Button ipInputButton;

    public TextMeshProUGUI statusTextOVR;
    public TextMeshProUGUI statusTextHomeAssistant;

    WebSocket ws;

    public void IPSaveButtonClick()
    {
        PlayerPrefs.SetString("homeassistant_ip", ipInputField.text);
        PlayerPrefs.Save();
        WSDisconnect();
        WSConnect();
    }

    void WSDisconnect()
    {
        if (ws != null)
        {
            ws.Close();
        }
    }

    void WSConnect()
    {
        ws = new WebSocket("ws://" + PlayerPrefs.GetString("homeassistant_ip") + ":17825");

        ws.Connect();

        if (!ws.IsAlive)
        {
            Debug.LogWarning("Could not connect to websocket.");
            ws.Close();
            ws = null;
            return;
        }

        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("Message Received from " + ((WebSocket) sender).Url + ", Data : " + e.Data);
            ShowNotification(e.Data);
        };
    }

    void Start()
    {
        Application.targetFrameRate = 5;

        Debug.Log(PlayerPrefs.GetString("homeassistant_ip"));
        if (PlayerPrefs.GetString("homeassistant_ip") == "")
        {
            PlayerPrefs.SetString("homeassistant_ip", "127.0.0.1");
            PlayerPrefs.Save();
        }
        ipInputField.text = PlayerPrefs.GetString("homeassistant_ip");

        ovrHandler = OVR_Handler.instance;
        ovrHandler.onVREvent += MyVREventHandler;

        WSConnect();
    }

    public void SendWebsocketMessage(String message)
    {
        if (ws == null)
        {
            Debug.Log("Could not send message (websocket not connected): '" + message + "'");
            return;
        }

        ws.Send(message);
    }

    public void MyVREventHandler(VREvent_t e)
	{
        EVREventType type = (EVREventType) e.eventType;

        if (type == EVREventType.VREvent_ButtonPress && e.data.controller.button == (uint) EVRButtonId.k_EButton_ProximitySensor)
        {
            ws.Send("on");
        }

        if (type == EVREventType.VREvent_ButtonUnpress && e.data.controller.button == (uint) EVRButtonId.k_EButton_ProximitySensor)
        {
            ws.Send("off");
        }

		// Debug.Log("Captured VR Event: " + type.ToString());
	}

    float timerDefault = 6f;
    float timer = 0f;
    bool timerActive = false;

    void ShowNotification(String message)
    {
        Debug.Log("Displaying Notification: " + message);
        textTarget = message;
        animatorBoolTarget = true;
        updateComponents = true;
        timer = timerDefault;
        timerActive = true;
        source.Play();
    }

    bool animatorBoolTarget = false;
    String textTarget = "";
    bool updateComponents = false;

    bool autostart_commited = false;

    void Update()
    {
        if (ovrHandler.OpenVRConnected && !autostart_commited)
        {
            RegisterManifestAndAutostart();

            autostart_commited = true;
        }

        if (!ovrHandler.OpenVRConnected && autostart_commited)
        {
            Application.Quit(0);
        }

        if (ovrHandler.OpenVRConnected)
        {
            statusTextOVR.text = "OpenVR Handler: Connected";
            statusTextOVR.color = Color.green;
        }
        else 
        {
            statusTextOVR.text = "OpenVR Handler: Disconnected";
            statusTextOVR.color = Color.yellow;
        }

        if (ws != null && ws.IsAlive)
        {
            statusTextHomeAssistant.text = "Home Assistant Integration: Connected";
            statusTextHomeAssistant.color = Color.green;
        }
        else
        {
            statusTextHomeAssistant.text = "Home Assistant Integration: Disconnected";
            statusTextHomeAssistant.color = Color.yellow;
        }

        if (updateComponents)
        {
            updateComponents = false;
            animator.SetBool("NotificationVisible", animatorBoolTarget);
            textMesh.text = textTarget;
        }

        if (timerActive)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                timerActive = false;
                animatorBoolTarget = false;
                updateComponents = true;
            }
        }
    }

    void RegisterManifestAndAutostart()
    {
        string application_key = "ovrsmartbridge.ovrsmartbridgeclient";
        string manifest_path = Path.GetFullPath(Path.Combine(Application.dataPath, "../manifest.vrmanifest"));
        Debug.Log(manifest_path);
        Debug.Log("Is Application installed? : " + ovrHandler.Applications.IsApplicationInstalled(application_key));
        Debug.Log("Installing manifest...");
        EVRApplicationError manifest_error = ovrHandler.Applications.AddApplicationManifest(manifest_path, false);
        if (manifest_error != EVRApplicationError.None)
        {
            Debug.LogError("Could not add application manifest: " + ovrHandler.Applications.GetApplicationsErrorNameFromEnum(manifest_error));
        }
        else
        {
            Debug.Log("Successfully installed manifest with no errors: " + ovrHandler.Applications.GetApplicationsErrorNameFromEnum(manifest_error));

            Debug.Log("Is Application installed? : " + ovrHandler.Applications.IsApplicationInstalled(application_key));


            Debug.Log("Enabling overlay autostart...");
            EVRApplicationError autostart_error = ovrHandler.Applications.SetApplicationAutoLaunch(application_key, true);
            if (autostart_error != EVRApplicationError.None)
            {
                Debug.LogError("Could not enable autostart: " + ovrHandler.Applications.GetApplicationsErrorNameFromEnum(autostart_error));
            }
        }
    }
}
