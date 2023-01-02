# YSL-Forth

A [Forth](https://en.wikipedia.org/wiki/Forth_(programming_language))
system implemented in [Yeti's Simple Language](https://github.com/yeti0904/ysl).
It's based on the [Forth 2012 standard](https://forth-standard.org),
but it doesn't adhere to it in some places so if you want to use it
make sure to check out the glossary (not yet existent).


### Usage

To use YSL Forth wait until I finish it. Otherwise you may want to:

1. Install [YSL](https://github.com/yeti0904/ysl), its host language;
2. Call `YSL core.ysl` in command line;
3. Write Forth!


### State of the project

Currently there are two files: `core.ysl` and `kernel.fs`.
The first one is the main YSL program that sets up the Forth environment
and interpreter and then reads and interprets the second one to declare
more complex and higher-level words. It's efficient to put a word
definition in `core.ysl`, but it's more convenient to put it in
`kernel.fs`. At the moment `core.ysl` is capable of interpreting
most defintion words including `:` and `code`, so `kernel.fs` can
contain `code` words.

Some random words like `*`, `+` and `,` are also implemented, and the
repl works, but it's not useful unless you define more `code` words.


### How to contribute

**PLEASE** fix any spelling, stylistic or grammar mistakes you find in
the docs (which for now includes just this `README.md`). I will accept
any and all doc fixing pull requests with minor scrutiny.

If you want something or implemented open an issue so we can talk about
it and then make a pull request resolving it. You can also find me on
[Esolangs Discord](https://discord.gg/3UXSK5p), I'm `olus2000#5728`.

:wq