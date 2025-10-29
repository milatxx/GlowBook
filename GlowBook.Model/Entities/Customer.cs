

namespace GlowBook.Model.Entities
{
    public class Customer: BaseEntity
    {
        public string Name { get; set; } = "";
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Notes { get; set; }

    }
}
