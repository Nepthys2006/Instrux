namespace Instrux.Domain.Models;

public class TodoItem
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Priority { get; set; } = "Medium";
    public bool IsCompleted { get; set; } = false;
    public DateTime? DueDate { get; set; }
}
