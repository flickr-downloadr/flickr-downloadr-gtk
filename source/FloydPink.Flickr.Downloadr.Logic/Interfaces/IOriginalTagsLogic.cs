using System.Threading.Tasks;
using FloydPink.Flickr.Downloadr.Model;

namespace FloydPink.Flickr.Downloadr.Logic.Interfaces {
    public interface IOriginalTagsLogic {
        Task<Photo> GetOriginalTagsTask(Photo photo);
    }
}
