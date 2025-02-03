namespace StructsDemo;

public record Foo(string Name, string Status, DateTimeOffset ModifiedDate); // reference type

public record struct Bar(string Name, string Status, DateTimeOffset ModifiedDate); // value type
