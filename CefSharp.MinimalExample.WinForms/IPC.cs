using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;

// ITestContract.cs
using System.ServiceModel;

namespace IPCTestServer
{
    public class DownloadData
    {
        public string URL { get; set; }
        public string Folder { get; set; }
        public string Title { get; set; }
        public string Protocol { get; set; }
        public string Extension { get; set; }

        public string RTMP_T { get; set; }
        public string RTMP_Y { get; set; }
    }

    [ServiceContract]
    interface ITestContract
    {
        [OperationContract]
        void Add(DownloadData data);


        [OperationContract]
        int Subtract(int a, int b);
    }



    class MyClient
    {
        ITestContract channel;

        public MyClient()
        {
            Connect();
        }

        public void Connect()
        {

            try
            {
                string address = "net.pipe://localhost/IPCTest";


                NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
                EndpointAddress ep = new EndpointAddress(address);
                channel = ChannelFactory<ITestContract>.CreateChannel(binding, ep);

            }
            catch (Exception e)
            {
                return;
            }
        }

        public void Add(DownloadData data)
        {
            try
            {
                Connect();
                channel.Add(data);

            }
            catch (Exception e)
            {
                return;
            }
        }
    }
}

namespace IpcSample
{
    class IpcServer
    {
        public IpcRemoteObject RemoteObject { get; set; }

        public IpcServer()
        {
            // Create the server channel.
            IpcServerChannel channel = new IpcServerChannel("ipcSample");

            // Register the server channel.
            ChannelServices.RegisterChannel(channel, true);

            // Create an instance of the remote object.
            RemoteObject = new IpcRemoteObject();

            RemoteObject.URL = "URL";
            RemotingServices.Marshal(RemoteObject, "test", typeof(IpcRemoteObject));
        }
    }

    class IpcClient
    {
        public IpcRemoteObject RemoteObject { get; set; }

        public IpcClient()
        {
            // Create the channel.
            IpcClientChannel channel = new IpcClientChannel();

            // Register the channel.
            ChannelServices.RegisterChannel(channel, true);

            // Get an instance of the remote object.
            RemoteObject = Activator.GetObject(typeof(IpcRemoteObject), "ipc://ipcSample/test") as IpcRemoteObject;

            Console.WriteLine(RemoteObject.URL);
            return;
        }
    }

    public class IpcRemoteObject : MarshalByRefObject
    {
        public string URL { get; set; }

        /// <summary>
        /// 自動的に切断されるのを回避する
        /// </summary>
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}