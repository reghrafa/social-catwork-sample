using System;
using System.ComponentModel.DataAnnotations;

namespace Catwork.Api.DbModels
{
    public class CatworkUser
    {
        [Key]
        public string UserId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }
        public string ProfilePicture { get; set; }
    }
}
