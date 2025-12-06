namespace Bookstore2._0;

public record class Menu
{
    public string Name { get; set; }
    public Func<Task> Method { get; set; }

    public Menu(string name, Func<Task> method)
    {
        Name = name;
        Method = method;
    }
}
