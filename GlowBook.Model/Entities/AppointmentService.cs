

namespace GlowBook.Model.Entities
{
    public class AppointmentService: BaseEntity
    {
        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; } = null!;

        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!;

        public int Qty { get; set; } = 1;
    }
}
