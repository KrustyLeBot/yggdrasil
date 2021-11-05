namespace Yggdrasil.Models
{
    public class PlayerNotificationModel
    {
        public string RecipientProfileId { get; set; }
        public string Content { get; set; }
    }

    public class DBPlayerNotification
    {
        public string SenderProfileId { get; set; }
        public string Content { get; set; }
    }
}
