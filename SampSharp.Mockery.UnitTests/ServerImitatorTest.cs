using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SampSharp.GameMode.API;

namespace SampSharp.Mockery.UnitTests
{
    [TestClass]
    public class ServerImitatorTest
    {
        [TestMethod]
        public void TestCanBoot()
        {
            ServerImitator.Boot<TestGameMode>();
        }

        [TestMethod]
        public void TestSomething()
        {
            // Setup
            var srv = ServerImitator.Boot<TestGameMode>();

            // Connect
            var playerid = srv.ConnectPlayer("John Doe", "127.0.0.1");

            // Check name
            var args = new object[2];
            args[0] = playerid;

            Native.Load("GetPlayerName", typeof(int), typeof(string).MakeByRefType()).Invoke(args);

            // Assert
            Assert.AreEqual("John Doe", args[1]);
        }
    }
}
