using System;
using System.Collections.Generic;

public class Category
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public Type type { get; set; } = typeof(Category);
    public required string Description { get; set; }


    public Category() { }


    public Category(int id, string name, Type type, string description)
    {
        Id = id;
        Name = name;
        this.type = type;
        Description = description;
    }

}
