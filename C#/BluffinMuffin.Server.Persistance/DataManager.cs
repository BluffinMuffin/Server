namespace BluffinMuffin.Server.Persistance
{
    public static class DataManager
    {
        private static readonly IDataPersistance m_Persistance = new DummyPersistance();
        public static IDataPersistance Persistance { get { return m_Persistance; } }
    }
}
