using System;
using System.Collections.Generic;

namespace LingoVerse.Models;

public partial class Question
{
    public int QuestionId { get; set; }

    public int PostId { get; set; }

    public string QuestionText { get; set; } = null!;

    public string? Answer { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Post Post { get; set; } = null!;
}
