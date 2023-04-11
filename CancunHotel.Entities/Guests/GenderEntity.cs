namespace CancunHotel.Entities.Guests
{
    public class Gender
    {
        public int IdGender { get; set; }
        public string GenderName { get; set; }

        public Gender()
        {
            this.GenderName = string.Empty;
        }
    }
}
