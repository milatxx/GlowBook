using System.Collections.Generic;

namespace GlowBook.Model.Entities
{
    public class Staff: BaseEntity
    {
        public string Name { get; set; } = "";
        public string Role { get; set; } = "Employee";

        public bool IsActive { get; set; } = true;

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
