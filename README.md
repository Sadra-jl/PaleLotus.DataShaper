# PaleLotus.DataShaper

**PaleLotus.DataShaper** is a .NET source generator designed to simplify and optimize data transformation. It eliminates the need for reflection and `ExpandoObject`, offering a type-safe and performant alternative for data shaping.

## What is Data Shaping?

Data shaping involves transforming complex data models into simpler structures, such as flattening nested objects or selecting specific fields for APIs.
By doing so it enables the consumer of the API to select(shape) the data by choosing the fields through the query string, So we can potentially reduce the stress on the API.

## Why using this library

Typical way of doing the data shaping consist of using reflection becuase we are returning an object based on the fields that are specified at runtime.
involving reflection in data shaping it not something every API needs.

#### there are multiple ways to achieving Data shaping without reflection.
- **Using Expression Trees**: Expression trees allow you to build expressions at runtime without reflection and offer better performance than reflection.
- **Manual String Parsing**:  You could manually parse a list of fields passed in the query string and build a query to select those fields.

As you can see using expression trees are better than reflection, but they can never have compiled time performance.
And Manual String Parsing is also time-consuming and may lead complexity, and having huge codes to maintain.

This library will take the manual approach so you don't have to write it yourself.
it will create DTOs of all combination of whole object's properties and uses a switch to determine the required fields
then maps object to required DTO and returns it.

## Features

- **No Reflection**: No runtime performance costs associated with reflection.
- **Type-Safe**: Compile-time generation ensures type safety.
- **All Combinations**: Automatically generates all possible shapes of your classes.
- **High Performance**: Optimized for fast data processing.

## Pros and Cons

### Pros:
- **Performance**: Faster and more memory-efficient than reflection.
- **Compile-Time Safety**: Reduces runtime errors.
- **Flexibility**: Generates a wide range of shapes for various use cases.

### Cons:
- **Build Time**: Large models can slightly increase compile time(based on models that use Shaper).
- **Static Nature**: Less flexible than dynamic runtime approaches for unforeseen use cases.
- **App Size**: Generating all combination of Properties Will Increase the App size. a class with less than 5 Property have over 100 combinations,
so compiling all those classes will make app size bigger.

## Benchmark Results (its dummy data Real data will be added.)

Benchmarking against reflection-based data shaping:

| Method                  | Execution Time (ns) | Memory Allocated (KB) |
|-------------------------|---------------------|-----------------------|
| **PaleLotus.DataShaper** | 500                 | 2                     |
| **Reflection-Based**    | 1500                | 20                    |

PaleLotus.DataShaper demonstrates a significant reduction in both execution time and memory usage, making it ideal for high-performance applications.

## Installation(Place Holder NO NUGET PACKAGE FOR NOW)

Install with:

```bash
dotnet add package PaleLotus.DataShaper
```

## Usage
1. **Build the Project to start the source generator**
2. **Add the Attribute ShapeAbleData to your Object**

```csharp
using PaleLotus.DataShaper;

[ShapeAbleData]
public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
    public Address Address { get; set; }
}

```
3. Use the shaper class generated.

```csharp

//dummy data
Person[] persons = [new Person { Name = "John", Age = 30, Address = new Address { City = "NY", Street = "5th Ave" } }];

// Use generated shaper
var personShaper = new PersonShaper(); //this will be replaced by shaper manager class in futer releases
var shapedData = personShaper.ShapeData(persons, "Name,Address");
```

## Use Cases

1. **API Responses**: Shape data for different API endpoints without exposing internal properties.
2. **DTO Creation**: Generate optimized DTOs for database interactions in compile time.

## Contributing

Contributions are welcome! Open issues or submit pull requests.

## License

Licensed under the MIT License.