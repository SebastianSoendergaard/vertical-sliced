# vertical-sliced
A simple TODO app with Vertical Sliced Architecture 

The solution uses .NET 8 and minimal API.

Commands and Queries are implemented without any dependencies to external libraries.

Validation is done using ValueObjects and state is stored as Aggregates.

As the only public API to the application layer is through commands and queries all unit tests, tests the entire slice.

