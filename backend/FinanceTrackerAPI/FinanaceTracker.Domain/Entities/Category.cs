using System;
using System.Collections.Generic;

public class Category {
    public required int Id { get; set;}
    public required string Name { get; set;}
    public Type type{ get; set;} = typeof(Category);
}