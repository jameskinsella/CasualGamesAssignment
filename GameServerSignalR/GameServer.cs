using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace GameServerSignalR
{
    [HubName("GameServerHub")]
    public class GameServer : Hub
    {
        static Timer t;
        string PlayerID;
        public GameServer() : base()
        {
            //Create Timer of 3 seconds
            t = new Timer(8000);
            t.AutoReset = false;
            t.Elapsed += T_Elapsed;
            t.Start();
        }
        private void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            PlayerID = "James";
            Clients.All.ChangePoint(PlayerID);
            t.Stop();
            t.Start();
        }
    }
}