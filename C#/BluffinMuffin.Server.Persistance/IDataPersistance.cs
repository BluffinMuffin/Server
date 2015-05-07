using BluffinMuffin.Server.DataTypes;

namespace BluffinMuffin.Server.Persistance
{
    public interface IDataPersistance
    {
        bool IsUsernameExist(string username);
        bool IsDisplayNameExist(string displayName);
        void Register(UserInfo u);
        UserInfo Get(string username);
        UserInfo Authenticate(string username, string password);
    }
}
