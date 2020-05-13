## What is this for?

My personal goal is to implement a Lox interpreter in C# and later a VM in C++ (sorry but I have changed my mind a bit after finding out [Rust](https://www.rust-lang.org)...), following this fantastic book [Crafting Interpreters](https://craftinginterpreters.com/). I never managed to finish a compiler book before, but Bob's book is the best!

## How do I Run?

It is an interpreter,

```bash
# this will compile and run it
dotnet run
# or run a script
dotnet run ./scripts/hello.lox
```

## Devlog

* 2019.12.25 Scanner.
* 2020.4.12 Expressions.
* 2020.4.16 Control flows.
* 2020.5.14 Functions.

## License

I know [MIT](./LICENSE) is good so I use it.