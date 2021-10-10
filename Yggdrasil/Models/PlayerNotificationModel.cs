using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yggdrasil.Models
{
    public class PlayerNotificationModel
    {
        public string RecipientProfileId { get; set; }
        public string Content { get; set; }
        public bool IsOffline { get; set; }
    }
}
