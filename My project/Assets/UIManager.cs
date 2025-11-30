using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject connectionPanel;
    public InputField ipInputField;
    public Text connectionStatusText;
    public Button hostButton;
    public Button connectButton;

    private NetworkManager networkManager;

    void Start()
    {
        networkManager = FindObjectOfType<NetworkManager>();

        hostButton.onClick.AddListener(StartHost);
        connectButton.onClick.AddListener(ConnectToServer);

        UpdateConnectionStatus("Disconnected");
    }

    void Update()
    {
    }

    public void StartHost()
    {
        networkManager.StartHost();
        connectionPanel.SetActive(false);
        UpdateConnectionStatus("Hosting on port " + networkManager.port);
    }

    public void ConnectToServer()
    {
        if (!string.IsNullOrEmpty(ipInputField.text))
        {
            networkManager.serverIP = ipInputField.text;
        }

        networkManager.ConnectToServer();
        connectionPanel.SetActive(false);
        UpdateConnectionStatus("Connecting...");
    }

    public void ShowConnectionPanel()
    {
        connectionPanel.SetActive(true);
    }

    public void UpdateConnectionStatus(string status)
    {
        if (connectionStatusText != null)
            connectionStatusText.text = status;
    }
}