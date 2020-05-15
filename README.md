## What is this for?

My personal goal is to implement a Lox interpreter in C# and later a VM in C++ (I may also want to do it in [Rust](https://www.rust-lang.org)...), following the fantastic book [Crafting Interpreters](https://craftinginterpreters.com/). I never managed to finish a compiler book, but Bob's book is the best!

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
* 2020.5.16 Anonymous functions & Can assign var using statements.

## Want to do

* exception handling.
* more native functions.

## License

I know [MIT](./LICENSE) is good so I use it.