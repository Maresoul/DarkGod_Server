using System.Threading;

namespace Server
{
    class ServerStart
    {
        static void Main(string[] args)
        {
            ServerRoot.Instance.Init();

            while (true)
            {
                ServerRoot.Instance.Update();

                //提高性能
                Thread.Sleep(20);
            }
        }
    }
}
