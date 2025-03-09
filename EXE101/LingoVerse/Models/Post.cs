using System;
using System.Collections.Generic;

namespace LingoVerse.Models;

public partial class Post
{
    public int PostId { get; set; }

    public int AuthorId { get; set; }

    public int LanguageId { get; set; }

    public int LevelId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Status { get; set; }

    public string? Image { get; set; }

    public virtual User Author { get; set; } = null!;

    public virtual Language Language { get; set; } = null!;

    public virtual Level Level { get; set; } = null!;

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}
