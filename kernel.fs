: dummy 2134 . ;


code memory-dump
memory-dump:
    var x = $mp
    .loop:
        var w f mem $x
        print $w
        print " "
        var x - 1
        gt $x -1
        goto_if *.loop
    goto *next
;


code >r
    var r-stack a $t
    var rp + 1
    var t f d-stack $dp
    var d-stack r $dp 1
    var dp - 1
    goto *next
;


code r>
    var d-stack a $t
    var dp + 1
    var t f r-stack $rp
    var r-stack r $rp 1
    var rp - 1
    goto *next
;


code *
    var x f d-stack $dp
    var d-stack r $dp 1
    var dp - 1
    var t * $x
    goto *next
;


code depth
    var d-stack a $t
    var dp + 1
    var t = $dp
    var t + 1
    goto *next
;


code '
':
    gosub *parse-word
    gosub *_find
    goto_if *.found
    println "Error when executing ': word not found."
    exit
    .found:
    var d-stack a $t
    var dp + 1
    var t = $x
    goto *next
;


2137 .
