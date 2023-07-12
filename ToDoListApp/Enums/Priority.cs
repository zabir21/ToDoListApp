using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ToDoListApp.Enums
{
    public enum Priority
    {
        [Display(Name = "Простая")]
        Easy = 0,

        [Display(Name = "Важная")]
        Medium = 1,

        [Display(Name = "Критичная")]
        Hard = 2
    }
}
