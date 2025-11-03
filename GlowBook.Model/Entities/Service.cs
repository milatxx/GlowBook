using System.Collections.Generic;


namespace GlowBook.Model.Entities
{
    public class Service: BaseEntity
    {
        public string Name { get; set; } = "";
        public int DurationMin { get; set; }
        public decimal Price { get; set; }

        public ICollection<AppointmentService> AppointmentServices { get; set; } = new List<AppointmentService>();
    }
}
