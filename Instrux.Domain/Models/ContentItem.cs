namespace Instrux.Domain.Models;

public class ContentItem
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ClassId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public virtual SchoolClass? Class { get; set; }
}
