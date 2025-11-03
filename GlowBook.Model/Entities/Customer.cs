using System.Collections.Generic;

namespace GlowBook.Model.Entities
{
    public class Customer: BaseEntity
    {
        public string Name { get; set; } = "";
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Notes { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    }
}
