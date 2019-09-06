/*
 * Tencent is pleased to support the open source community by making InjectFix available.
 * Copyright (C) 2019 THL A29 Limited, a Tencent company.  All rights reserved.
 * InjectFix is licensed under the MIT License, except for the third-party components listed in the file 'LICENSE.txt' which may be subject to their corresponding license terms. 
 * This file is subject to the terms and conditions defined in file 'LICENSE.txt', which is part of this source code package.
 */

//!!�����򵥵Ŀ�tcp�����յ�����û��У���ֱ��ִ��
//!!�����м������������ƽʱ��������ʹ��
//!!������Ϸ���������ɾ��
#warning "Please remove PatchReceiver.cs from release package."

using UnityEngine;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.IO;
using IFix.Core;

namespace IFix
{
    public class PatchReceiver : MonoBehaviour
    {
        //Ĭ�ϵĲ����ļ�������
        const string PERSISTENT_FILE_NAME = "__LAST_RUN_SAVED_PATCH";

        //���ջ������Ĵ�С
        const int BUFFER_SIZE = 1024;

        Stream patch = null;

        //��Ҫ��LiveDotNet.cs�Ķ˿�����
        public int Port = 8080;

        //�������Ϊtrue�Ļ��������ļ��ᱣ�浽�ļ�������Ӧ�ú���Ȼ��Ч
        public bool Persistent = false;

        Socket listener = null;

        string lastRunSavePath;

        //1������
        //2��accpet�����Ӻ󣬽��ո�������������
        //3����������Ϊ����������
        void ReceivePatch()
        {
            byte[] bytes = new byte[BUFFER_SIZE];

            //�������е�ַ
            IPAddress ipAddress = IPAddress.Parse("0.0.0.0");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, Port);

            listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                while (true)
                {
                    Socket handler = listener.Accept();

                    MemoryStream ms = new MemoryStream();

                    while (true)
                    {
                        int bytesRec = handler.Receive(bytes);
                        if (bytesRec == 0)
                        {
                            break;
                        }
                        ms.Write(bytes, 0, bytesRec);
                    }

                    if (Persistent)
                    {
                        File.WriteAllBytes(lastRunSavePath, ms.GetBuffer());
                    }

                    ms.Position = 0;

                    Debug.Log("Patch Size: " + ms.Length);

                    lock (this)
                    {
                        patch = ms;
                    }

                    try
                    {
                        handler.Shutdown(SocketShutdown.Both);
                    }
                    catch { }
                    finally
                    {
                        handler.Close();
                    }
                }

            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        //��������˳־û�����Awakeʱ�����ϴα���Ĳ����ļ�
        void Awake()
        {
            lastRunSavePath = Application.persistentDataPath + Path.DirectorySeparatorChar + PERSISTENT_FILE_NAME;
            if (Persistent && File.Exists(lastRunSavePath))
            {
                using (var fs = File.Open(lastRunSavePath, FileMode.Open))
                {
                    PatchManager.Load(fs);
                }
            }
            DontDestroyOnLoad(gameObject);
            //�����߳������ղ������������̣߳�����ͨ��patch��������������
            new Thread(ReceivePatch).Start();
        }

        void Update()
        {
            Stream ms = null;
            lock (this)
            {
                ms = patch;
                patch = null;
            }
            if (ms != null)
            {

                PatchManager.Load(ms);
            }
        }

        void OnDestroy()
        {
            listener.Close();
        }
    }
}
