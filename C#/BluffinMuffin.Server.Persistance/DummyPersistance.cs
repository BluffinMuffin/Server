using BluffinMuffin.Server.DataTypes;
using System.Collections.Generic;

namespace BluffinMuffin.Server.Persistance
{
    public class DummyPersistance : IDataPersistance
    {
        private readonly Dictionary<string, UserInfo> m_UsersByUsername = new Dictionary<string, UserInfo>();
        private readonly Dictionary<string, UserInfo> m_UsersByDisplayname = new Dictionary<string, UserInfo>();

        #region IDataPersistance Members

        public bool IsUsernameExist(string username)
        {
            return m_UsersByUsername.ContainsKey(username.ToLower());
        }

        public bool IsDisplayNameExist(string displayName)
        {
            return m_UsersByDisplayname.ContainsKey(displayName.ToLower());
        }

        public void Register(UserInfo u)
        {
            m_UsersByUsername.Add(u.Username.ToLower(), u);
            m_UsersByDisplayname.Add(u.DisplayName.ToLower(), u);
        }

        public UserInfo Get(string name)
        {
            if (!IsUsernameExist(name))
            {
                if (!IsDisplayNameExist(name))
                    return null;
                return m_UsersByDisplayname[name.ToLower()];
            }
            return m_UsersByUsername[name.ToLower()];
        }

        public UserInfo Authenticate(string username, string password)
        {
            var u = Get(username);
            if (u != null && u.Password == password)
                return u;
            return null;
        }

        #endregion
    }
}
