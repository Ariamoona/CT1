using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    [Header("Network Settings")]
    public string serverIP = "127.0.0.1";
    public int port = 8888;
    public bool isServer = false;

    [Header("Game References")]
    public GameObject playerPrefab;
    public Transform[] spawnPoints;

    private TcpListener tcpListener;
    private TcpClient tcpClient;
    private NetworkStream networkStream;
    private Thread networkThread;
    private bool isConnected = false;

    private Dictionary<int, PlayerData> players = new Dictionary<int, PlayerData>();
    private int localPlayerId = -1;

    void Start()
    {
        Application.runInBackground = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            StartHost();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            ConnectToServer();
        }
    }

    void OnApplicationQuit()
    {
        Disconnect();
    }

    public void StartHost()
    {
        isServer = true;
        StartServer();
    }

    public void ConnectToServer()
    {
        isServer = false;
        StartCoroutine(ConnectAsClientCoroutine());
    }

    private void StartServer()
    {
        try
        {
            tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start();
            Debug.Log($"Server started on port {port}");

            networkThread = new Thread(new ThreadStart(AcceptClients));
            networkThread.IsBackground = true;
            networkThread.Start();

            CreateLocalPlayer(0);
        }
        catch (Exception e)
        {
            Debug.LogError($"Server error: {e.Message}");
        }
    }

    private IEnumerator ConnectAsClientCoroutine()
    {
        bool connectionSuccess = false;
        Exception connectionException = null;

        Thread connectThread = new Thread(() =>
        {
            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connect(serverIP, port);
                networkStream = tcpClient.GetStream();
                isConnected = true;
                connectionSuccess = true;

                Debug.Log("Connected to server");

                byte[] buffer = new byte[1024];
                int bytesRead = networkStream.Read(buffer, 0, buffer.Length);
                string message = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);

                if (message.StartsWith("PLAYER_ID:"))
                {
                    localPlayerId = int.Parse(message.Split(':')[1]);
                    Debug.Log($"Assigned player ID: {localPlayerId}");
                }

                networkThread = new Thread(new ThreadStart(ReceiveData));
                networkThread.IsBackground = true;
                networkThread.Start();
            }
            catch (Exception e)
            {
                connectionSuccess = false;
                connectionException = e;
            }
        });

        connectThread.IsBackground = true;
        connectThread.Start();

        while (connectThread.IsAlive)
        {
            yield return null;
        }

        if (connectionSuccess)
        {
            CreateLocalPlayer(localPlayerId);
        }
        else
        {
            Debug.LogError($"Connection error: {connectionException?.Message}");
        }
    }

    private void AcceptClients()
    {
        int playerId = 0;

        while (true)
        {
            try
            {
                TcpClient client = tcpListener.AcceptTcpClient();
                NetworkStream clientStream = client.GetStream();

                playerId++;
                Debug.Log($"Client connected, assigned ID: {playerId}");

                string idMessage = $"PLAYER_ID:{playerId}";
                byte[] idData = System.Text.Encoding.UTF8.GetBytes(idMessage);
                clientStream.Write(idData, 0, idData.Length);

                CreateRemotePlayer(playerId, client, clientStream);

                SendAllPlayersToClient(clientStream);

                BroadcastMessage($"PLAYER_JOINED:{playerId}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Accept clients error: {e.Message}");
                break;
            }
        }
    }

    private void ReceiveData()
    {
        byte[] buffer = new byte[1024];

        while (isConnected)
        {
            try
            {
                if (networkStream != null && networkStream.DataAvailable)
                {
                    int bytesRead = networkStream.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string message = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        ProcessNetworkMessage(message);
                    }
                }
                Thread.Sleep(10);
            }
            catch (Exception e)
            {
                Debug.LogError($"Receive data error: {e.Message}");
                break;
            }
        }
    }

    private void ProcessNetworkMessage(string message)
    {
        string[] parts = message.Split(':');
        if (parts.Length < 2) return;

        string command = parts[0];
        int playerId = int.Parse(parts[1]);

        switch (command)
        {
            case "POSITION":
                if (parts.Length >= 3)
                {
                    Vector3 position = ParseVector2(parts[2]);
                    UpdatePlayerPosition(playerId, position);
                }
                break;

            case "COLOR":
                if (parts.Length >= 3)
                {
                    Color color = ParseColor(parts[2]);
                    UpdatePlayerColor(playerId, color);
                }
                break;

            case "PLAYER_JOINED":
                if (isServer) break;
                CreateRemotePlayer(playerId);
                break;

            case "PLAYER_LEFT":
                RemovePlayer(playerId);
                break;
        }
    }

    public void SendPosition(Vector2 position)
    {
        if (!isConnected && !isServer) return;

        string message = $"POSITION:{localPlayerId}:{position.x},{position.y}";
        SendNetworkMessage(message);
    }

    public void SendColorChange(int playerId, Color color)
    {
        string message = $"COLOR:{playerId}:{color.r},{color.g},{color.b}";
        SendNetworkMessage(message);
    }

    private void SendNetworkMessage(string message)
    {
        try
        {
            if (isServer)
            {
                BroadcastMessage(message);
            }
            else if (isConnected && networkStream != null)
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
                networkStream.Write(data, 0, data.Length);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Send message error: {e.Message}");
        }
    }

    private void BroadcastMessage(string message)
    {
        if (!isServer) return;

        try
        {
            foreach (var player in players.Values)
            {
                if (player.isRemote && player.stream != null)
                {
                    byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
                    player.stream.Write(data, 0, data.Length);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Broadcast message error: {e.Message}");
        }
    }

    private void CreateLocalPlayer(int id)
    {
        if (players.ContainsKey(id)) return;

        Vector3 spawnPos = spawnPoints.Length > 0 ?
            spawnPoints[id % spawnPoints.Length].position :
            new Vector3(id * 2, 0, 0);

        GameObject playerObj = Instantiate(playerPrefab, spawnPos, Quaternion.identity);

        PlayerController playerController = playerObj.GetComponent<PlayerController>();
        playerController.Initialize(id, true, this);

        players[id] = new PlayerData
        {
            id = id,
            gameObject = playerObj,
            controller = playerController,
            isLocal = true
        };

        localPlayerId = id;

        Debug.Log($"Local player {id} created");
    }

    private void CreateRemotePlayer(int id, TcpClient client = null, NetworkStream stream = null)
    {
        if (players.ContainsKey(id)) return;

        Vector3 spawnPos = spawnPoints.Length > 0 ?
            spawnPoints[id % spawnPoints.Length].position :
            new Vector3(id * 2, 0, 0);

        GameObject playerObj = Instantiate(playerPrefab, spawnPos, Quaternion.identity);

        PlayerController playerController = playerObj.GetComponent<PlayerController>();
        playerController.Initialize(id, false, this);

        players[id] = new PlayerData
        {
            id = id,
            gameObject = playerObj,
            controller = playerController,
            isLocal = false,
            client = client,
            stream = stream,
            isRemote = true
        };

        Debug.Log($"Remote player {id} created");
    }

    private void UpdatePlayerPosition(int playerId, Vector3 position)
    {
        if (players.ContainsKey(playerId) && !players[playerId].isLocal)
        {
            players[playerId].gameObject.transform.position = position;
        }
    }

    private void UpdatePlayerColor(int playerId, Color color)
    {
        if (players.ContainsKey(playerId))
        {
            players[playerId].controller.SetColor(color);
        }
    }

    private void RemovePlayer(int playerId)
    {
        if (players.ContainsKey(playerId))
        {
            Destroy(players[playerId].gameObject);
            players.Remove(playerId);
            Debug.Log($"Player {playerId} removed");
        }
    }

    private void SendAllPlayersToClient(NetworkStream clientStream)
    {
        try
        {
            foreach (var player in players.Values)
            {
                if (player.isLocal)
                {
                    string message = $"PLAYER_JOINED:{player.id}";
                    byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
                    clientStream.Write(data, 0, data.Length);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Send all players error: {e.Message}");
        }
    }

    private void Disconnect()
    {
        isConnected = false;

        if (localPlayerId != -1)
        {
            SendNetworkMessage($"PLAYER_LEFT:{localPlayerId}");
        }

        if (networkThread != null && networkThread.IsAlive)
        {
            networkThread.Abort();
        }

        networkStream?.Close();
        tcpClient?.Close();
        tcpListener?.Stop();

        Debug.Log("Disconnected from network");
    }

    private Vector2 ParseVector2(string data) 
    {
        try
        {
            string[] parts = data.Split(',');
            return new Vector2(float.Parse(parts[0]), float.Parse(parts[1]));
        }
        catch
        {
            return Vector2.zero;
        }
    }
    private Color ParseColor(string data)
    {
        try
        {
            string[] parts = data.Split(',');
            return new Color(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]));
        }
        catch
        {
            return Color.white;
        }
    }
}

[System.Serializable]
public class PlayerData
{
    public int id;
    public GameObject gameObject;
    public PlayerController controller;
    public bool isLocal;
    public bool isRemote;
    public TcpClient client;
    public NetworkStream stream;
}