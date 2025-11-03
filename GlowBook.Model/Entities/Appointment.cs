using System;
using System.Collections.Generic;

namespace GlowBook.Model.Entities
{
    public class Appointment: BaseEntity
    {
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public int StaffId { get; set; }
        public Staff Staff { get; set; } = null!;

        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Status { get; set; } = "Planned"; // Planned/Done/NoShow/Cancelled

        public ICollection<AppointmentService> AppointmentServices { get; set; } = new List<AppointmentService>();
    }
}

