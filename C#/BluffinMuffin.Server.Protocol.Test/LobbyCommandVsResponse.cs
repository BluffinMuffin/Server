using System.Linq;
using BluffinMuffin.Protocol;
using BluffinMuffin.Protocol.Lobby;
using BluffinMuffin.Protocol.Lobby.RegisteredMode;
using BluffinMuffin.Protocol.Lobby.QuickMode;
using BluffinMuffin.Server.Protocol.Test.Mocking;
using BluffinMuffin.Protocol.Tests.DataTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BluffinMuffin.Server.Protocol.Test
{
    [TestClass]
    public class LobbyCommandVsResponse
    {
        private void CheckIfResponseIs<T>(AbstractCommand c) where T : AbstractCommand
        {
            var server = new ServerMock();
            server.Send(c);
            server.LobbyCommands.CompleteAdding();
            var received = server.ServerSendedCommands.GetConsumingEnumerable().First();
            Assert.AreEqual(typeof(T), received.Command.GetType());
        }
        [TestMethod]
        public void AuthenticateUserCommand()
        {
            CheckIfResponseIs<AuthenticateUserResponse>(LobbyCommandMock.AuthenticateUserCommand());
        }
        [TestMethod]
        public void CheckDisplayExistCommand()
        {
            CheckIfResponseIs<CheckDisplayExistResponse>(LobbyCommandMock.CheckDisplayExistCommand());
        }
        [TestMethod]
        public void CheckUserExistCommand()
        {
            CheckIfResponseIs<CheckUserExistResponse>(LobbyCommandMock.CheckUserExistCommand());
        }
        [TestMethod]
        public void CreateTableCommand()
        {
            CheckIfResponseIs<CreateTableResponse>(LobbyCommandMock.CreateTableCommand());
        }
        [TestMethod]
        public void CreateUserCommand()
        {
            CheckIfResponseIs<CreateUserResponse>(LobbyCommandMock.CreateUserCommand());
        }
        [TestMethod]
        public void GetUserCommand()
        {
            CheckIfResponseIs<GetUserResponse>(LobbyCommandMock.GetUserCommand());
        }
        [TestMethod]
        public void IdentifyCommand()
        {
            CheckIfResponseIs<IdentifyResponse>(LobbyCommandMock.IdentifyCommand());
        }
        [TestMethod]
        public void JoinTableCommand()
        {
            CheckIfResponseIs<JoinTableResponse>(LobbyCommandMock.JoinTableCommand());
        }
        [TestMethod]
        public void ListTableCommand()
        {
            CheckIfResponseIs<ListTableResponse>(LobbyCommandMock.ListTableCommand());
        }
        [TestMethod]
        public void CheckCompatibilityCommand()
        {
            CheckIfResponseIs<CheckCompatibilityResponse>(LobbyCommandMock.CheckCompatibilityCommand());
        }
    }
}
