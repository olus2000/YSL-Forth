( --- Tests for core and kernel of YSL Forth --- )

forgetter forget-tests

t{ -> }t 'EMPTY TEST'
t{ -> }t 'ANOTHER EMPTY TEST'
t{ 1 -> 1 }t 'IDENTITY TEST'
t{ 0 ->   }t 'INCORRECT DEPTH TEST'
t{ 1 -> 0 }t 'INCORRECT VALUE TEST'

( --- Numbers --- )

t{ #2137     ->    2137 }t 'DECIMAL'
t{ #-2137    ->   -2137 }t 'NEGATIVE DECIMAL'
t{ $AcAb4    ->  707252 }t 'HEX'
t{ $-AcAb4   -> -707252 }t 'NEGATIVE HEX'
t{ %1011001  ->      89 }t 'BINARY'
t{ %-1011001 ->     -89 }t 'NEGATIVE BINARY'
t{ 'o        ->     111 }t 'CHARACTER NUMBER'


( --- Simple definitions --- )

t{ : TS1 1 ; ->   }t 'TS1 DEFINITION'
t{ TS1       -> 1 }t 'SIMPLE DEFINITION'

t{ 10 : TS2 literal ; ->    }t 'TS2 DEFINITION'
t{ TS2                -> 10 }t 'LITERAL'

t{ : TS3 17 ; immediate ->    }t 'TS3 DEFINITION'
t{ : TS4 TS3 ;          -> 17 }t 'IMMEDIATE IS IMMEDIATE'
t{ TS4                  ->    }t 'IMMEDIATE IS NOT COMPILED'


( --- Stack shuffling --- )

t{ 1 drop -> }t 'DROP'
t{ 1 2 drop -> 1 }t 'DROP ONLY AFFECTS ONE VALUE'

t{ 1 dup -> 1 1 }t 'DUP'

t{ 1 2 swap -> 2 1 }t 'SWAP'

t{ 1 2 over -> 1 2 1 }t 'OVER'

t{ 1 2 3 rot -> 2 3 1 }t 'ROT'

\ ``depth`` tests assume starting depth is zero.
t{     depth -> 0     }t 'EMTPY DEPTH'
t{   0 depth -> 0 1   }t 'SINGLE DEPTH'
t{ 0 1 depth -> 0 1 2 }t 'DOUBLE DEPTH'

t{ 11 >h    h>      -> 11 }t '>H H>'
t{ 13 >h h@ h> drop -> 13 }t 'H@'

t{ : TR1 >r    r> nop  ; -> }t 'TR1 DEFINITION'
t{ : TR2 >r r@ r> drop ; -> }t 'TR2 DEFINITION'
t{ 11 TR1 -> 11 }t '>R R>'
t{ 13 TR2 -> 13 }t 'R@'


( --- Maths and logic --- )

t{ 3 4 + ->  7 }t 'ADD'

t{ 3 4 - -> true }t 'SUB'

t{ 3 4 * -> 12 }t 'MULTIPLY'

t{ -1 0= -> false }t 'TRUE IS NOT ZERO'
t{  0 0= -> true  }t 'ZERO IS ZERO'
t{ 69 0= -> false }t 'NUMBER IS NOT ZERO'

t{ 53 53 = -> true  }t 'EQUAL'
t{ 42 95 = -> false }t 'NOT EQUAL'

t{ 53 53 < -> false }t 'EQUAL NOT LESS THAN'
t{ 36 53 < -> true  }t 'LESS THAN'
t{ 95 53 < -> false }t 'GREATER THAN NOT LESS THAN'

t{ 53 53 > -> false }t 'EQUAL NOT GREATER THAN'
t{ 36 53 > -> false }t 'LESS THAN NOT GREATER THAN'
t{ 95 53 > -> true  }t 'GREATER THAN'

t{ true  true  and -> true  }t 'TRUE AND TRUE'
t{ false true  and -> false }t 'FALSE AND TRUE'
t{ true  false and -> false }t 'TRUE AND FALSE'
t{ false false and -> false }t 'FALSE AND FALSE'


( --- Memory access --- )

t{ here 21 , @           -> 21 }t 'HERE , @'

t{ here 21 , 37 over ! @ -> 37 }t '!'

t{ next-tmp-buffer next-tmp-buffer = -> false }t
   'NEXT-TMP-BUFFER DIFFERENT BUFFERS'

t{ next-tmp-buffer 5 + 42 over ! @ -> 42 }t 'TMP BUFFER READ WRITE'

t{ next-tmp-buffer 21 over ! 16 over !+ @ -> 37 }t '!+'

t{ state @ -> 0 }t 'INTERPRETATION STATE'

t{ : TM1 @ ; immediate -> }t 'TM1 DEFINITION'
t{ state ] TM1 [ -> -1 }t 'COMPILATION STATE'


( --- Parsing --- )

t{ : TP1 $ 'af' ;                ->           }t 'TP1 DEFINITION'
t{ TP1    swap dup 1 + @ swap @  -> 2 102 97  }t 'COMPILED STRING'
t{ $ 'ct' swap dup 1 + @ swap @  -> 2 116 99  }t 'TMP STRING'
t{ $ 'q'swap drop                -> 1         }t 'WORD IMMEDIATELY AFTER STRING'
t{ $ 'a' swap $ 'b' swap @ rot @ -> 1 1 98 97 }t 'TWO TMP STRINGS'
t{ $ '' swap drop                -> 0         }t 'EMPTY STRING'
t{ $ 'asdf'                     -$> 'asdf'    }t 'STRING IDENTITY'
t{ 1                            -$> 'a'       }t 'STRING WRONG NUMBER OF ITEMS'
t{ $ 'a'                        -$> 'as'      }t 'STRING WRONG LENGTH'
t{ $ 'a'                        -$> 'b'       }t 'STRING WRONG CONTENT'

t{ $ $\][/.,';-=)(*&^%#@!~`|}{><?":+ _$ -$>
     Y\][/.,';-=)(*&^%#@!~`|}{><?":+ _Y }t
     'TMP STRING WITH SPECIAL CHARACTERS'

t{ ( comment ) -> }t '(COMMENT)'

t{ \ line comment
   -> }t '\COMMENT'

t{ 'Q parse some words Q -$> 'some words ' }t 'PARSE TEST'

t{ parse-string 'some words' -$> 'some words' }t 'PARSE-STRING'

\ ``compile`` will not be tested. If the system works it probably works.

t{ here : TP2 literal ; -> }t 'TP2 DEFINITION'
t{ ' TP2 -> TP2 }t "'"

t{ : TP3 10 ;                     ->    }t 'TP3 DEFINITION'
t{ : TP4 postpone TP3 ; immediate ->    }t 'POSTPONE NONIMMEDIATE'
t{ : TP5 TP4 ; immediate          ->    }t 'TP5 DEFINITION'
t{ TP5                            -> 10 }t 'POSTPONED NONIMMEDIATE COMPILES'
t{ : TP6 postpone TP5 ;           ->    }t 'POSTPONE IMMEDIATE'
t{ TP6                            -> 10 }t 'POSTPONED IMMEDIATE GETS EXECUTED'


( --- Definitions c.d. --- )

t{ define TD1 enter-action , ] 8 exit [ -> }t 'TD1 DEFINITION'
t{ TD1 -> 8 }t 'MANUAL DEFINITIONS'

t{ create TD2 42 , ->    }t 'TD2 DEFINITION'
t{ TD2 @           -> 42 }t 'CREATE'

\ ``code`` will not be tested, see ``compile``

t{ variable TD3   ->    }t 'TD3 DEFINITION'
t{ 21 TD3 !       ->    }t 'VARIABLE ASSIGNMENT'
t{ TD3 @          -> 21 }t 'VARIABLE FETCHING'
t{ 37 TD3 ! TD3 @ -> 37 }t 'VARABLE REASSIGNMENT'

t{ 21 constant TD4 ->    }t 'TD4 DEFINITION'
t{ TD4             -> 21 }t 'CONSTANT'

\ ``;code`` will not be tested, see ``compile``

t{          latest -> ' TD4 }t 'LATEST'
t{ shadow   latest -> ' TD3 }t 'SHADOW'
t{ unshadow latest -> ' TD4 }t 'UNSHADOW'

\ ``exit`` was tested in TD4 definition

t{ here 10 literal ' exit , : TD5 jump ; -> }t 'TD5 DEFINITION'
t{ TD5 -> 10 }t 'JUMP'


( --- Combinators --- )

t{ : TC1 { 11 dup } ; ->       }t 'TC1 DEFINITION'
t{ TC1 execute        -> 11 11 }t 'QUOTATION AND EXECUTE'

t{ : TC2 this ; ->      }t 'TC2 DEFINITION'
t{ TC2 ' TC2 =  -> true }t 'THIS'

t{ $ '13 dup' evaluate -> 13 13 }t 'EVALUATE'

\ ``recurse`` will be tested with control flow

t{ 13 TC1 dip -> 11 11 13 }t 'DIP'

t{ 13 TC1 keep -> 13 11 11 13 }t 'KEEP'


( --- Control Flow --- )

t{ : TF1 21 ; -> }t 'TF1 DEFINITION'

t{ true  ' TF1 when -> 21 }t 'WHEN TRUE'
t{ false ' TF1 when ->    }t 'WHEN FALSE'

t{ true  ' TF1 unless ->    }t 'UNLESS TRUE'
t{ false ' TF1 unless -> 21 }t 'UNLESS FALSE'

t{ : TF2 37 ; -> }t 'TF2 DEFINITION'

t{ true  ' TF1 ' TF2 if -> 21 }t 'IF TRUE'
t{ false ' TF1 ' TF2 if -> 37 }t 'IF FALSE'

t{ : TF3 dup 1 - dup ; -> }t 'TF3 DEFINITION'

t{ 1 ' TF3 loop ->         1 0 }t 'LOOP SINGLE'
t{ 5 ' TF3 loop -> 5 4 3 2 1 0 }t 'LOOP MULTIPLE'

t{ : TF4 dup 1 - ; -> }t 'TF4 DEFINITION'

t{ 0 ' dup ' TF4 while ->           0 }t 'WHILE NO ITERATION'
t{ 5 ' dup ' TF4 while -> 5 4 3 2 1 0 }t 'WHILE MULTIPLE'

t{ : TF5 dup 5 < ; -> }t 'TF5 DEFINITION'

t{ 4  ' TF5 ' TF4 until ->              4 }t 'UNTIL NO ITERATION'
t{ 10 ' TF5 ' TF4 until -> 10 9 8 7 6 5 4 }t 'UTNIL MULTIPLE'

t{ : TF6 dup { dup 1 - recurse } when ; -> }t 'TF6 DEFINITION'
t{ 5 TF6 -> 5 4 3 2 1 0 }t 'RECURSE'

t{ 0  ' TF1 times ->                }t 'TIMES ZERO'
t{ 5  ' TF1 times -> 21 21 21 21 21 }t 'TIMES MULTPILE'
t{ -4 ' TF1 times ->                }t 'TIMES NEGATIVE'

t{ : TF7 i^          @ ; -> }t 'TF7 DEFINITION'
t{ : TF8 times.bound @ ; -> }t 'TF8 DEFINITION'
t{ : TF9 i             ; -> }t 'TF9 DEFINITION'

t{ 5 ' TF7 times -> 5 4 3 2 1 }t 'I^'
t{ 5 ' TF8 times -> 5 5 5 5 5 }t 'TIMES.BOUND'
t{ 5 ' TF9 times -> 0 1 2 3 4 }t 'I'


( --- Double --- )

t{ 1 2 2drop -> }t '2DROP'

t{ 1 2 2dup -> 1 2 1 2 }t '2DUP'

t{ 1 2 3 4 2swap -> 3 4 1 2 }t '2SWAP'


( --- Cleanup --- )

forget-tests
