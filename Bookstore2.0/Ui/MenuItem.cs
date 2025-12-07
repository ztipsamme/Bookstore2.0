namespace Bookstore2._0;

public record class MenuItem
{
    public string Name { get; set; }
    public Func<Task> Method { get; set; }

    public MenuItem(string name, Func<Task> method)
    {
        Name = name;
        Method = method;
    }
}
