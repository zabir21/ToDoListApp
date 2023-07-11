using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using ToDoListApp.DAL.Entity.Identity;
using ToDoListApp.Enum;

namespace ToDoListApp.DAL.Entity
{
    public class UserTask
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public DateTime Deadline { get; set; }

        public Priority Priority { get; set; }


        [ForeignKey("User")]
        public Guid UserId { get; set; }
        //[JsonIgnore]
        public User Users { get; set; } = new User();
    }
}
