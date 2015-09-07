namespace BluffinMuffin.Server.Persistance
{
    public static class DataManager
    {
        public static IDataPersistance Persistance { get; } = new DummyPersistance();
    }
}
