using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ToDoListApp.DAL.Entity.Identity;
using ToDoListApp.Enums;

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

        public Enums.TaskStatus StatusTask { get; set; }

        public DateTime AcceptanceDate { get; set; }

        [ForeignKey(nameof(Users))]
        public Guid UserId { get; set; }

        public User Users { get; set; }
    }
}
