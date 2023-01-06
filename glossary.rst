################################################################################
                               YSL Forth Glossary
################################################################################

name : ( arguments "parsed" -- outputs ) : [ immediate ] : standard/YSL
    *name* is the name of the word. *arguments* are values it expects on the
    stack starting from the bottommost one. *parsed* indicates what the word
    parses. *outputs* are values it leaves, replacing *arguments*; also from the
    bottommost one. Existence of *immediate* indicates that the world will be
    executed no matter the interpreter state. *standard/YSL* indicates whether
    a word behaves according to the `Forth 2012 standard`_ or is it specific to
    the YSL Forth system.

.. _Forth 2012 standard: https://forth-standard.org

--------------------------------------------------------------------------------
                         System variables and constants
--------------------------------------------------------------------------------

``ysl-here`` : ( -- x ) : YSL
    Push the line number at which the next line of YSL code will be compiled.


--------------------------------------------------------------------------------
                                Stack shuffling
--------------------------------------------------------------------------------

``drop`` : ( x -- ) : standard
    Remove a value from the top of the stack.

``dup`` : ( x -- x x ) : standard
    Duplicate the top value on the stack.

``swap`` : ( a b -- b a ) : standard
    Swap top two values on the stack.

``depth`` : ( -- n ) : standard
    Push the number of items on the stack before executing *depth*.

``>r`` : ( x -- ) ( R: -- x ) : standard
    Put the top value from the data stack on the return stack.

``r>`` : ( -- x ) ( R: x -- ) : standard
    Put the top value from the return stack on the data stack.


--------------------------------------------------------------------------------
                                     Maths
--------------------------------------------------------------------------------

``+`` : ( n n -- n ) : standard
    Add top two values from the stack.

``*`` : ( n n -- n ) : standard
    Multiply top two values from the stack.


--------------------------------------------------------------------------------
                                    Parsing
--------------------------------------------------------------------------------

``(`` : ( "ccc)" -- ) : standard
    Parse until ``)`` is encountered or until the end of the parse area.

``\\`` : ( "ccc<newline>" -- ) : standard
    Parse until *newline* is encountered or until the end of the parse area.

``parse`` : ( char "ccc<char>" -- addr n ) : standard
    Parse until *char* is encountered or until the end of the parse area.
    Return starting address of the parsed text and its length.


``compile`` : ( "YSL<newline>;" -- ) : YSL
    Compile YSL code until a line startign with ``;`` is encountered.

``'`` : ( "<spaces>name<space>" -- xt ) : standard
    Push the execution token corresponding to *name*.

``postpone`` : ( "<spaces>name<space>" -- ) : immediate : standard
    Compile the compilation semantics of the word corresponding to *name*.

``$`` : ( "<spaces><char>ccc<char>" -- ) comp: ( -- addr n ) : immediate : YSL
    Parse a string between two delimiting characters. *char* can be any
    character not present in the string. Compile the string along with code that
    pushes its address and length to the stack.


--------------------------------------------------------------------------------
                                    Defining
--------------------------------------------------------------------------------

``define`` : ( "<spaces>name<space>" -- ) : YSL
    Add *name* to the dictionary pointing at the next cell to be allocated.

``create`` : ( "<spaces>name<space>" -- ) : standard
    Define a word with *name* which pushes the address of its data field. Don't
    allocate any space in its data field.

``:`` : ( "<spaces>name<space>" -- ) : standard
    Define a word with *name* which run-time semantics is to execute the
    subsequently compiled code. Make the word invisible in the dictionary.
    Change the interpreter state to compilation.

``;`` : ( -- ) : immediate : standard
    Compile a return from word. Make the latest defined word visible in the
    dictionary. Change the interpreter state to interpretation.

``code`` : ( "<spaces>name<space>YSL<newline>;" -- ) : standard
    Define a word with *name* which run-time semantics is to call the following
    YSL code. Compile YSL code until a line starting with ``;`` is encountered.

``variable`` : ( "<spaces>name<space>" -- ) : standard
    Define a word with *name* which pushes the address of its data field.
    Allocate one cell in its data field.


``constant`` : ( x "<spaces>name<space>" -- ) : standard
    Define a word with *name* which pushes the value of its data field.
    Allocate one cell in its data field and set it to *x*.

``;code`` : ( "YSL<newline>;" -- ) : immediate : standard
    Compile the following run-time semantics: change the action of the latest
    defined word to the YSL code after this ``;code``. Compile return from word.
    Compile YSL code until a line starting with ``;``.

``latest`` : ( -- addr ) : YSL
    Push the address of the action field of the latest defined word.

``literal`` : ( x -- ) comp: ( -- x ) : immediate : standard
    Compile code that will push *x*.


--------------------------------------------------------------------------------
                                  Combinators
--------------------------------------------------------------------------------

``evaluate`` : ( x*i addr n -- x*j ) : standard
    Save parsing state and switch it to parsing the string at *addr* with length
    *n*. Interpret the string. Restore saved parsing state. Additional stack
    effect comes from interpreted string.

``execute`` : ( x*i xt -- x*j ) : standard
    Execute word identified by *xt* execution token. Additional stack effect
    comes from the word executed.


--------------------------------------------------------------------------------
                                 Memory access
--------------------------------------------------------------------------------

``@`` : ( addr -- x ) : standard
    Fetch the value from the address. Due to memory mapping of negative
    addresses this is not equal to YSL::

        var t f mem $t
        goto *next

``!`` : ( x addr -- ) : standard
    Set the value in memory at *addr* to *x*. Negative addresses can be memory
    mapped. Some portions of memory are read-only and writing them is ignored.

``,`` : ( n -- ) : standard
    Append *n* to the main memory block.


--------------------------------------------------------------------------------
                                      I/O
--------------------------------------------------------------------------------

``.`` : ( n -- ) : standard
    Display *n* in decimal (TODO: base dependent) with a trailing space.


--------------------------------------------------------------------------------
                                     Double
--------------------------------------------------------------------------------

--------------------------------------------------------------------------------
                                  File access
--------------------------------------------------------------------------------

``open-file`` : ( addr n -- addr n ) ( F: -- f ) : YSL
    Read file described by string at *addr* of length *n* and push its contents
    to the file stack. Return address of the top of the file stack and size of
    the opened file.

``drop-file`` : ( -- ) ( F: f -- ) : YSL
    Drop the top file from the file stack.

``included`` : ( x*i addr u -- x*j ) : standard
    Open the file described by string at *addr* with length *u* and push its
    contents to the file stack. Interpret the file and drop it from the file
    stack. Additional stack effect comes from the file interpreted.


--------------------------------------------------------------------------------
                                 Miscellaneous
--------------------------------------------------------------------------------

``[`` : ( -- ) : immediate : standard
    Change the interpreter state to interpretation.

``]`` : ( -- ) : standard
    Change the interpreter state to compilation.

``bye`` : ( -- ) : YSL
    Exit the program.

``immediate`` : ( -- ) : standard
    Make the latest defined word immediate.
