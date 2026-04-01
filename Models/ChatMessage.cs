namespace WEBCOMIC_FINALPROJECT_.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ChatMessages")] // Ép tên bảng chính xác là ChatMessages
    public class ChatMessage
    {
        public int Id { get; set; }
        public string User { get; set; }
        public string Message { get; set; }
        public DateTime SentAt { get; set; } = DateTime.Now;
    }
}
