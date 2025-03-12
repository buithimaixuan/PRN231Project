using System;
using System.Collections.Generic;

namespace CommunicateService.Models;

public partial class Message
{
    public int MessageId { get; set; }

    public int ConversationId { get; set; }

    public int SenderId { get; set; }

    public DateOnly CreateAt { get; set; }

    public string Content { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual Conversation Conversation { get; set; }
}
