################################################################################
                               YSL Forth Glossary
################################################################################

.. Maximum width of this document should be kept at 80.

``name`` : ( arguments "parsed" -- outputs ) : [ immediate ] : standard/YSL
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

``here`` : ( -- addr ) : standard
    *addr* is the address of the next memory cell to be allocated.

``ysl-here`` : ( -- x ) : YSL
    *x* is the line number at which the next line of YSL code will be compiled.

``enter-action`` : ( -- x ) : YSL
    *x* is the YSL address of the action responsible for entering definitions.

``state`` : ( -- addr ) : YSL
    *addr* is the address of the interpreter state variable. Zero is
    interpretation state, -1 is compilation state. Setting it to any nonzero
    value will result in a value of -1.


--------------------------------------------------------------------------------
                                Stack shuffling
--------------------------------------------------------------------------------

``drop`` : ( x -- ) : standard
    Remove a value from the top of the stack.

``dup`` : ( x -- x x ) : standard
    Duplicate the top value on the stack.

``swap`` : ( a b -- b a ) : standard
    Swap top two values on the stack.

``over`` : ( a b -- a b a ) : standard
    Copy the second element from the stack to the top.

``rot`` : ( a b c -- b c a ) : standard
    Move the third element from the stack to the top.

``depth`` : ( -- n ) : standard
    Push the number of items on the stack before executing *depth*.

``>h`` : ( x -- ) ( H: -- x ) : YSL
    Put the top value from the data stack on the helper stack.

``h>`` : ( -- x ) ( H: x -- ) : YSL
    Put the top value from the helper stack on the data stack.

``h@`` : ( -- x ) ( H: x -- x ) : YSL
    Copy the top value from the helper stack on the data stack.

``>r`` : ( x -- ) ( R: -- x ) : YSL
    Put the top value from the data stack on the return stack. Explicit use of
    the return stack is discouraged. Consider using Combinators_ or helper stack
    instead.

``r>`` : ( -- x ) ( R: x -- ) : YSL
    Put the top value from the return stack on the data stack. Explicit use of
    the return stack is discouraged. Consider using Combinators_ or helper stack
    instead.

``r@`` : ( -- x ) ( R: x -- x ) : YSL
    Copy the top value from the return stack on the data stack. Explicit use of
    the return stack is discouraged. Consider using Combinators_ or helper stack
    instead.


--------------------------------------------------------------------------------
                                Maths and logic
--------------------------------------------------------------------------------

``+`` : ( n n -- n ) : standard
    Add top two values from the stack.

``-`` : ( a b -- n ) : standard
    Subtract *b* from *a*.

``*`` : ( n n -- n ) : standard
    Multiply top two values from the stack.

``true`` : ( -- -1 ) : standard
    Push the canonical true value to the stack.

``false`` : ( -- 0 ) : standard
    Push the canonical false value to the stack.

``0=`` : ( n -- ? ) : standard
    Return -1 if *n* is zero and 0 otherwise.

``=`` : ( a b -- ? ) : standard
    Return -1 if *a* and *b* are equal in value and 0 otherwise.

``>`` : ( a b -- ? ) : standard
    Return -1 if *a* is bigger than *b* and 0 otherwise.

``<`` : ( a b -- ? ) : standard
    Return -1 if *a* is smaller than *b* and 0 otherwise.

``and`` : ( a? b? -- c? ) : standard
    Return -1 if both *a?* and *b?* are nonzero and *0* otherwise.


--------------------------------------------------------------------------------
                                    Parsing
--------------------------------------------------------------------------------

``(`` : ( "ccc)" -- ) : standard
    Parse until ``)`` is encountered or until the end of the parse area.

``\`` : ( "ccc<newline>" -- ) : standard
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

``$`` : ( "<spaces><char>ccc<char>" -- addr n ) : immediate : YSL
    Parse a string between two delimiting characters. *char* can be any
    character not present in the string. If in compilation state compile the
    string along with code that pushes its address and length to the stack.
    Otherwise save the string in a temporary buffer and push its address and
    length on the stack. It may be invalidated by subsequent use of temporary
    buffers.

``parse-string`` : ( "<spaces><char>ccc<char>" -- addr n ) : YSL
    Parse a string between two delimiting characters. Push address in the
    input buffer and length of the parsed string on the stack.


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

``exit`` : ( -- ) ( R: x -- ) : standard
    Return from word.

``jump`` : ( addr -- ) : immediate : YSL
    Compile code that will jump to *addr*.

``literal`` : ( x -- ) comp: ( -- x ) : immediate : standard
    Compile code that will push *x*.

``shadow`` : ( -- ) : YSL
    Make the latest visible definition invisible to the interpreter.

``unshadow`` : ( -- ) : YSL
    Make the latest shadowed definition visible to the interpreter.


--------------------------------------------------------------------------------
                                  Combinators
--------------------------------------------------------------------------------

``{`` : ( -- orig ) comp: ( -- xt ) : immediate : YSL
    Compile code that will push the execution token of a following quotation and
    jump over it. Compile the runtime semantics to execute the quotation body at
    the execution token. Leave the data about incomplete jump on the stack.

``}`` : ( orig -- ) : immediate : YSL
    Compile a return from word and complete the jump semantics pointed to by
    *orig*.

``this`` : ( -- ) comp: ( -- xt ) : immediate : YSL
    Compile code that will push the execution token of the currently defined
    word. Assumes that currently defined word has been shadowed by ``:`` or
    equivalent.

``evaluate`` : ( A.. addr n -- B.. ) : standard
    Save parsing state and switch it to parsing the string at *addr* with length
    *n*. Interpret the string. Restore saved parsing state. Additional stack
    effect comes from interpreted string.

``execute`` : ( A.. xt:{ A.. -- B.. } -- B.. ) : standard
    Execute word identified by *xt* execution token. Additional stack effect
    comes from the word executed.

``recurse`` : ( -- ) : immediate : standard
    Compile semantics of a recursive call to the word currently being defined.
    Assumes the word has been shadowed by ``:`` or equivalent.

``dip`` : ( A.. x xt:{ A.. -- B.. } -- B.. x ) : YSL
    Execute *xt* below the top element of the stack.

``keep`` : ( A.. x xt:{ A.. x -- B.. } -- B.. x ) : YSL
    Execute *xt* and restore the top element of the stack from before execution.


--------------------------------------------------------------------------------
                                  Control flow
--------------------------------------------------------------------------------

``when`` : ( A.. ? xt:{ A.. -- B.. } -- B.. | A.. ) : YSL
    Execute *xt* if *?* is nonzero.

``unless`` : ( A.. ? xt:{ A.. -- B.. } -- A.. | B.. ) : YSL
    Execute *xt* unless *?* is nonzero.

``if`` : ( X.. ? a:{ X.. -- A.. } b:{ X.. -- B.. } -- A.. | B.. ) : YSL
    Execute *a* if *?* is nonzero, otherwise execute *b*.

``loop`` : ( A.. xt:{ A.. -- A.. ? } -- A.. ) : YSL
    Execute *xt* consuming *?* until *?* is zero.

``while`` : ( A.. pred:{ A.. -- B.. ? } body:{ B.. -- A.. } -- B.. ) : YSL
    Execute *pred*. Consume *?* and if it's nonzero execute *body* and repeat.

``until`` : ( A.. pred:{ A.. -- B.. ? } body:{ B.. -- A.. } -- B.. ) : YSL
    Execute *pred*. Consume *?* and if it's zero execute *body* and repeat.

``times`` : ( A.. n xt:{ A.. -- A.. } -- A.. ) : YSL
    Execute *xt* *n* times. Set ``i^`` and ``times.bound`` accordingly for each
    iteration. Restore previous ``i^`` and ``times.bound`` afterwards. *n* lower
    than zero results in no iteration.

``times.bound`` : ( -- addr ) : YSL
    Holds the initial value of the ``times`` iteration loop.

``i^`` : ( -- addr ) : YSL
    Holds the ``times`` iterator. Counts down from ``times.bound``.

``i`` : ( -- n ) : YSL
    Push the number of iteration on the stack. Starts from zero and counts
    towards ``times.bound``.


--------------------------------------------------------------------------------
                                 Memory access
--------------------------------------------------------------------------------

``@`` : ( addr -- x ) : standard
    Fetch the value from the address. Due to memory mapping of negative
    addresses this is **not** equal to YSL::

        var t f mem $t
        goto *next

``!`` : ( x addr -- ) : standard
    Set the value in memory at *addr* to *x*. Negative addresses can be memory
    mapped. Some portions of memory are read-only and writing them is ignored.

``!+`` : ( x addr -- ) : YSL
    Add the value of *x* to memory at *addr*.

``,`` : ( n -- ) : standard
    Append *n* to the main memory block.

``next-tmp-buffer`` : ( -- addr ) : YSL
    Push the addres of the next tmp buffer. Its previous contents are
    invalidated, and the buffer can serve as a random access storage for local
    purposes. It can be invalidated by use of tmp buffer words.


--------------------------------------------------------------------------------
                                      I/O
--------------------------------------------------------------------------------

``.`` : ( n -- ) : standard
    Print *n* in decimal (TODO: base dependent) with a trailing space.

``emit`` : ( c -- ) : standard
    Print character *c*.

``nl``/``cr`` : ( -- ) : YSL/standard
    Print newline.

``type`` : ( addr n -- ) : standard
    Print a counted string.

``say`` : ( "string" -- ) : immediate : YSL
    In interpretation state parse and print *string*. In compilation state
    compile semantics that print *string*.


--------------------------------------------------------------------------------
                                     Double
--------------------------------------------------------------------------------

``2drop`` : ( a b -- ) : standard
    Drop two items from the stack.

``2dup`` : ( a b -- a b a b ) : standard
    Duplicate two items on the stack.

``2swap`` : ( a b c d -- c d a b ) : standard
    Swap two pairs of items on the stack.


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
    Make the latest defined word immediate. It will be executed when encountered
    int both interpretation and compilation state.

``nop`` : ( -- ) : immediate : YSL
    Turn off tail call optimisation for this point. Use between a word that
    accesses the return stack and a returning word.


--------------------------------------------------------------------------------
                                    Testing
--------------------------------------------------------------------------------

This word set is defined in testing.fs_ and implements a simple testing toolset
along with some helper words used in the toolset definition.


Main words
----------

``t{`` : ( -- ) ( test-depth: depth ) : YSL
    Open the "actual" part of the testcase.

``->`` : ( x*i -- ) ( H: -- x*i ) ( test-quantity: i ) : YSL
    Close the "actual" part of the testcase and save the results. Open the
    "expected" part of the testcase.

    Tyical usage::

        t{ 1 dup -> 1 1 }t 'SAMPLE TEST'

``-$>`` : ( addr n "string" -- string ) ( H: -- c*n ) ( test-quantity: n ) : YSL
    Close the "actual" part of the testcase and interpret the results as
    a counted string. Parse a string. If there are more than two items in the
    "actual" part or *n* is different than length of *string* save the results
    of the "actual" part and leave the parsed string descriptor on stack.
    Otherwise save the contents if the counted string and put the contents of
    the parsed *string* on the stack.

    Typical usage::

        t{ $ !string! -$> |string| }t 'STRING TEST'

``}t`` : ( x*i "string" -- ) ( H: x*i -- ) : YSL
    Close the "expected" part of the test. *string* will be used as the test
    name for failure reporting. If there is a different number of items in the
    "expected" and "actual" part they differ output an adequate message along
    with both sets of items.

``forgetter`` : ( "word" -- ) word: ( -- ) : YSL
    Define *word* with the following semantics: remove dictionary definitions
    and memory allocated in the main memory block since the definition of *word*
    including the definition itslef. Allow reuse of YSL lines allocated since
    the definition of *word*.


Variables
---------

``test-depth`` : ( -- addr ) : YSL
    Holds the stack depth at which the testcase started.

``test-quantity`` : ( -- addr ) : YSL
    Holds the amount of items in the "actual" part of the test.


Helper words
------------

``pick`` : ( a x*n n -- x*n a ) : standard
    Move the value from below *n* values on the stack to the top.

``push-string`` : ( addr n -- c*n )
    Push *n* cells starting from *addr* to the stack.
