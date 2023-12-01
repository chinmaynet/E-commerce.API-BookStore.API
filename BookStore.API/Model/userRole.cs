using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BookStore.API.Model
{
    public class userRole
    {
        public Guid UserRoleId { get; set; }

        public Guid UId { get; set; }
        [ForeignKey("UId")]
        //[JsonIgnore] 
        public user User { get; set; }

        public Guid RId { get; set; }

        [ForeignKey("RId")]
        public role Role { get; set; }

    }
}
