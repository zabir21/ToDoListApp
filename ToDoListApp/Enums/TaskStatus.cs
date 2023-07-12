using System.ComponentModel.DataAnnotations;

namespace ToDoListApp.Enums
{
    public enum TaskStatus
    {
        [Display(Name = "Не начата")]
        NotAtWork = 0,

        [Display(Name = "В работе")]
        InWork = 1,

        [Display(Name = "Завершена")]
        Сompleted = 2
    }
}

