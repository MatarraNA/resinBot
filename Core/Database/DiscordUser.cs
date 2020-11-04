
using System;
using System.Collections.Generic;
using System.Text;

namespace resinBot.Core.Database
{
    public class DiscordUser
    {
        [System.ComponentModel.DataAnnotations.Key]
        public ulong DiscId { get; set; }
        public long ResinSetTicks { get; set; }
        public int LastResin { get; set; }
    }
}
