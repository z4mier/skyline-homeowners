namespace SkylineHOA.Models
{
    public class BillWithUser
    {
        public int BillId { get; set; }
        public string AmenityName { get; set; }
        public string Status { get; set; }
        public DateTime BookingDate { get; set; }
        public string PaymentMethod { get; set; }
        public string UserName { get; set; }
        public string FullName => UserName; 

    }


}
