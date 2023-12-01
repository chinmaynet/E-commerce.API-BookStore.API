using System.ComponentModel;

namespace BookStore.API.Model
{
    public class user
    {   
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }
        public string UserPhone { get; set; }

        [DefaultValue("active")]
        public string ActivityStatus { get; set; }
        public virtual ICollection<userRole> UserRoles { get; set; }
    }
}
