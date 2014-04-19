namespace FloydPink.Flickr.Downloadr.Repository {
    public interface IRepository<T> {
        T Get();
        void Save(T value);
        void Delete();
    }
}
