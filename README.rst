################################################################################
                                   YSL-Forth
################################################################################

A Forth_ system implemented in `Yeti's Simple Language`_. It's based on
the `Forth 2012 standard`_, but it doesn't adhere to it in some places so if you
want to use it make sure to check out the glossary_.

.. _glossary: ./glossary.rst
.. Most hyperlink targets can be found at the bottom of the document.

-------
 Usage
-------

To use YSL Forth:

1. Install YSL_, its host language;
2. Call ``ysl core.ysl`` in command line;
3. You should be greeted with the magic number 2137. You can now write Forth!


----------------------
 State of the project
----------------------

Currently there are two files: ``core.ysl`` and ``kernel.fs``. The first one is
the main YSL program that sets up the Forth environment and interpreter and then
reads and interprets the second one to declare more complex and higher-level
words. It's efficient to put a word definition in ``core.ysl``, but it's more
convenient to put them in ``kernel.fs``. At the moment ``core.ysl`` is capable
of interpreting the basic defintion words including ``:`` and ``code``,
so ``kernel.fs`` can contain ``code`` words with YSL bodies.

Many words are already implemented, including stack shuffling, control flow and
parsing/defining, but some areas (notably maths) are still lacking.


----------------------------------
 Main differences from Forth-2012
----------------------------------

YSL Forth is not trying to be a Forth-2012 Standard System. Most notably it
tries to minimize the explicit use of the return stack, and implicit use of
jumps for control flow. Instead it implements *quotations*: anonymous code
snippets compiled with ``{ }`` which push their execution tokens on the stack,
and *combinators*: words that work with execution tokens. All control flow in
YSL Forth is implemented with quotations, and a *tail call optimisation* system
is implemented to avoid unnecessary return stack growth.

Some snippets showcasing translation of common phrases from Forth-2012 to
YSL Forth::

 ( Executing code below an item on top of stack )
 >r some code r>   ( Forth-2012 )
 { some code } dip ( YSL Forth  )
 
 ( Conditional branch )
 predicate if body-then else body-else then ( Forth-2012 )
 predicate { body-then } { body-else } if   ( YSL Forth  )

 ( While loop )
 begin predicate while body repeat ( Forth-2012 )
 { predicate } { body } while      ( YSL Forth  )


-------------------
 How to contribute
-------------------

**PLEASE** fix any spelling, stylistic or grammar mistakes you find in the docs
(which for now includes just this ``README.rst``). I will accept any and all doc
fixing pull requests with minor scrutiny.

If you want something or implemented open an issue so we can talk about it and
then make a pull request resolving it. You can also find me
on `Esolangs Discord`_, I'm ``olus2000#5728``.

:wq


.. Hyperlinks:

.. _Forth: https://en.wikipedia.org/wiki/Forth_(programming_language)
.. _Yeti's Simple Language: https://github.com/yeti0904/ysl
.. _YSL: `Yeti's Simple Language`_
.. _Forth 2012 standard: https://forth-standard.org
.. _Esolangs Discord: https://discord.gg/3UXSK5p
