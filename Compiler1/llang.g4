grammar llang;

prog
	: toplevel+
	;

toplevel
    : libimport
	| globalVar
	| fundef
	| structdef
	| enumdef
	;

libimport
    : 'import' '"' Iden  '"' ';'
    ;

globalVar
	: 'global' varDeclStmt
	;

fundef
	: 'def' Iden '(' defargslist? ')' '->' typename block
	;

structdef
	: 'struct' Iden '{' structDeclList? '}'
	;

enumdef
	: 'enum' Iden '{' enumDeclList? '}'
	;

defargslist
	: defargitem (',' defargitem)*
	;

defargitem
	: Iden ':' typename
	;

structDeclList
	: structDecl+
	;
	
structDecl
	: Iden ':' typename ('=' expr)? ';'
	;

enumDeclList
	: enumDeclItem (',' enumDeclItem)*
	;

enumDeclItem
	: Iden
	| Iden '=' Number
	;

block
	: '{' stmt* '}'
	;

stmt
	: ';'
	| block
	| varDeclStmt
	| assignStmt
	| funcallStmt
	| flowstmt
	| retstmt
	| deferstmt
	;

flowstmt
	: whilestmt
	| forstmt
	| ifstmt
	;

whilestmt
	: 'while' '(' expr ')' stmt
	;

forstmt
	: 'for' '(' Iden 'in' expr ')' stmt
	;

ifstmt
	: 'if' '(' expr ')' stmt ('else' stmt)?
	;

retstmt
	: 'return' expr? ';'
	;

varDeclStmt
	: Iden ':=' expr ';'
	| Iden ':' typename ('=' expr)? ';'
	;

assignStmt
	: expr assignop expr ';'
	;

deferstmt
	: 'defer' '(' expr ')'
	;
	
assignop
	: '=' | '*=' | '/=' | '%=' | '+=' | '-=' | '<<=' | '>>=' | '&=' | '^=' | '|='
	;

funcallStmt
	: expr '(' argslist ')' ';'
	| expr ':' Iden '(' argslist ')'
	;

argslist
	: (expr (',' expr)*)?
	;

expr
	: Iden
	| Number
	| String
	| 'new' typename
	| '(' expr ')'
	| '[' varlist ']'
	| '[' elementlist? ']'
	| expr '[' expr ']'
	| expr '(' argslist ')'
	| expr '.' Iden
	| expr ':' Iden '(' argslist ')'
	| unaryop expr
	| expr multop expr
	| expr addop expr
	| expr shiftop expr
	| expr compop expr
	| expr eqop expr
	| expr bitwiseop expr
	| expr logicop expr
	;
	
varlist
	: expr '..' expr (',' expr)?
	;
	
elementlist
	: expr (',' expr)*
	;

unaryop
	: '-' | '!' | '~'
	;

multop
    : '*' | '/' | '%'
    ;

addop
    : '+' | '-'
    ;

shiftop
    : '>>' | '<<'
    ;

compop
    : '<' | '>' | '<=' | '>='
    ;

eqop
    : '==' | '!='
    ;

bitwiseop
	: '&' | '|' | '^'
	;

logicop
	: '&&' | '||'
	;

typename
	: ('void'
	| 'int'
	| 'float'
	| 'bool'
	| 'string' )
	| Iden
	| '[' typename ']'
	;


Iden : Alpha (Alpha | Digit)* ;

fragment Alpha : [a-zA-Z_] ;
fragment Digit : [0-9] ;


Number
	: HexNum
	| BinNum
	| IntNum
	| FloatNum
	;

fragment HexNum : '0' [xX] [0-9a-fA-F]+ ;
fragment BinNum : '0' [bB] [01]+ ;
fragment IntNum : [+-]? Digit+ ;
fragment FloatNum : [+-]? Digit+ '.' Digit+ ;

String : '"' StringChar* '"' ;

fragment StringChar
	: ~['\\\r\n]
	| EscapeSequence
	;

fragment EscapeSequence
 	: '\\' ['"?abfnrtv\\]
	;


Whitespace : [ \t\r\n]+ -> skip ;

Comment : '#' ~[\r\n]* -> skip ;