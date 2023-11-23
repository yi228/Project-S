using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Google.Protobuf;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class NetworkManager
{
    ServerSession _session = new ServerSession();
    private string urlValue;

    public void Send(IMessage packet)
    {
        _session.Send(packet);
    }

    public class URLData
    {
        public string url;
    }
    public IEnumerator CoDownloadServerURL()
    {
        UnityWebRequest www = UnityWebRequest.Get(Managers.URL.Ec2Url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error: " + www.error);
        }
        else
        {
            URLData urlData = JsonConvert.DeserializeObject<URLData>(www.downloadHandler.text);
            urlValue = urlData.url;
            Init();
        }
    }
    public void Init()
    {
        // DNS (Domain Name System)
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = IPAddress.Parse(urlValue); // for ec2
        //IPAddress ipAddr = ipHost.AddressList[0]; // for local test
        //IPAddress ipAddr = ipHost.AddressList[1]; // for local test
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

        Connector connector = new Connector();

        connector.Connect(endPoint,
           () => { return _session; },
           1);
    }

    public void Update()
    {
        List<PacketMessage> list = PacketQueue.Instance.PopAll();
        foreach (PacketMessage packet in list)
        {
            Action<PacketSession, IMessage> handler = PacketManager.Instance.GetPacketHandler(packet.Id);
            if (handler != null)
                handler.Invoke(_session, packet.Message);
        }
    }
}