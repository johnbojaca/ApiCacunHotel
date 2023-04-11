namespace CancunHotel.Entities.Guests
{
    public class IdType
    {
        public int Id { get; set; }
        public string IdTypeName { get; set; }
        public string Acronym { get; set; }

        public IdType()
        {
            this.IdTypeName = string.Empty;
            this.Acronym = string.Empty;
        }
    }
}
