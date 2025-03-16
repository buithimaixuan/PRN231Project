using System;
using System.Collections.Generic;

namespace Client.Models;

public partial class AccountConversation
{
    public int AccountId { get; set; }

    public int ConversationId { get; set; }

    public bool? IsOut { get; set; }

    public DateOnly? OutAt { get; set; }

    public virtual Conversation Conversation { get; set; }
}
