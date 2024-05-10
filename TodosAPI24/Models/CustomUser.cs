using Microsoft.AspNetCore.Identity;

namespace TodosAPI24.Models
{
    public class CustomUser : IdentityUser
    {
        public ICollection<Todo> Todos { get; set; } = new List<Todo>();
    }
}
