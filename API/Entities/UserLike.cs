
namespace API.Entities
{
    public class UserLike
    {
        public AppUser SourceUser {get;set;}

        public int SourceId {get;set;}

        public AppUser TargetUser {get;set;}

        public int TargetId {get;set;}

    }
}