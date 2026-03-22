namespace WEBCOMIC_FINALPROJECT_.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int PointsAdded { get; set; }
        public string TransactionCode { get; set; } // Mã giao dịch
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
