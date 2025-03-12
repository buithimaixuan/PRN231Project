using System;
using System.Collections.Generic;

namespace CommunicateService.Models;

public partial class Conversation
{
    public int ConversationId { get; set; }

    public string ConversationName { get; set; }

    public DateOnly CreateAt { get; set; }

    public int? CreatorId { get; set; }

    public int? MemberCount { get; set; }

    public bool? IsGroup { get; set; }

    public bool? IsDeleted { get; set; }

    public DateOnly? DeleteAt { get; set; }

    public virtual ICollection<AccountConversation> AccountConversations { get; set; } = new List<AccountConversation>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
