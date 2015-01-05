namespace FloydPink.Flickr.Downloadr.Logic.Interfaces {
    using System.Threading.Tasks;
    using Model;

    public interface IOriginalTagsLogic {
        Task<Photo> GetOriginalTagsTask(Photo photo);
    }
}
