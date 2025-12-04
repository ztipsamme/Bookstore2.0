namespace Bookstore2._0;

public record class Menu
{
    public string Name { get; set; }
    public Action Method { get; set; }

    public Menu(string name, Action method)
    {
        Name = name;
        Method = method;
    }
}
