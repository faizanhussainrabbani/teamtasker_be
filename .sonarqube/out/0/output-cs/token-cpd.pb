Û
/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.SharedKernel/ValueObject.cs
	namespace 	

TeamTasker
 
. 
SharedKernel !
{ 
public

 

abstract

 
class

 
ValueObject

 %
{ 
	protected 
static 
bool 
EqualOperator +
(+ ,
ValueObject, 7
left8 <
,< =
ValueObject> I
rightJ O
)O P
{ 	
if 
( 
left 
is 
null 
^ 
right $
is% '
null( ,
), -
{ 
return 
false 
; 
} 
return 
left 
? 
. 
Equals 
(  
right  %
)% &
!=' )
false* /
;/ 0
} 	
	protected 
static 
bool 
NotEqualOperator .
(. /
ValueObject/ :
left; ?
,? @
ValueObjectA L
rightM R
)R S
{ 	
return 
! 
EqualOperator !
(! "
left" &
,& '
right( -
)- .
;. /
} 	
	protected 
abstract 
IEnumerable &
<& '
object' -
>- .!
GetEqualityComponents/ D
(D E
)E F
;F G
public 
override 
bool 
Equals #
(# $
object$ *
obj+ .
). /
{ 	
if 
( 
obj 
== 
null 
|| 
obj "
." #
GetType# *
(* +
)+ ,
!=- /
GetType0 7
(7 8
)8 9
)9 :
{   
return!! 
false!! 
;!! 
}"" 
var$$ 
other$$ 
=$$ 
($$ 
ValueObject$$ $
)$$$ %
obj$$% (
;$$( )
return%% !
GetEqualityComponents%% (
(%%( )
)%%) *
.%%* +
SequenceEqual%%+ 8
(%%8 9
other%%9 >
.%%> ?!
GetEqualityComponents%%? T
(%%T U
)%%U V
)%%V W
;%%W X
}&& 	
public(( 
override(( 
int(( 
GetHashCode(( '
(((' (
)((( )
{)) 	
return** !
GetEqualityComponents** (
(**( )
)**) *
.++ 
Select++ 
(++ 
x++ 
=>++ 
x++ 
!=++ !
null++" &
?++' (
x++) *
.++* +
GetHashCode+++ 6
(++6 7
)++7 8
:++9 :
$num++; <
)++< =
.,, 
	Aggregate,, 
(,, 
(,, 
x,, 
,,, 
y,,  
),,  !
=>,," $
x,,% &
^,,' (
y,,) *
),,* +
;,,+ ,
}-- 	
}.. 
}// ◊
ç/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.SharedKernel/Interfaces/ISpecification.cs
	namespace 	

TeamTasker
 
. 
SharedKernel !
.! "

Interfaces" ,
{ 
public 

	interface 
ISpecification #
<# $
T$ %
>% &
{ 

Expression 
< 
Func 
< 
T 
, 
bool 
>  
>  !
Criteria" *
{+ ,
get- 0
;0 1
}2 3
List 
< 

Expression 
< 
Func 
< 
T 
, 
object  &
>& '
>' (
>( )
Includes* 2
{3 4
get5 8
;8 9
}: ;
List 
< 
string 
> 
IncludeStrings #
{$ %
get& )
;) *
}+ ,

Expression 
< 
Func 
< 
T 
, 
object !
>! "
>" #
OrderBy$ +
{, -
get. 1
;1 2
}3 4

Expression 
< 
Func 
< 
T 
, 
object !
>! "
>" #
OrderByDescending$ 5
{6 7
get8 ;
;; <
}= >

Expression 
< 
Func 
< 
T 
, 
object !
>! "
>" #
GroupBy$ +
{, -
get. 1
;1 2
}3 4
int 
Take 
{ 
get 
; 
} 
int 
Skip 
{ 
get 
; 
} 
bool 
IsPagingEnabled 
{ 
get "
;" #
}$ %
} 
} ÷
ä/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.SharedKernel/Interfaces/IRepository.cs
	namespace 	

TeamTasker
 
. 
SharedKernel !
.! "

Interfaces" ,
{ 
public 

	interface 
IRepository  
<  !
T! "
>" #
where$ )
T* +
:, -

BaseEntity. 8
{ 
Task 
< 
T 
> 
GetByIdAsync 
( 
int  
id! #
,# $
CancellationToken% 6
cancellationToken7 H
=I J
defaultK R
)R S
;S T
Task 
< 
IReadOnlyList 
< 
T 
> 
> 
ListAllAsync +
(+ ,
CancellationToken, =
cancellationToken> O
=P Q
defaultR Y
)Y Z
;Z [
Task 
< 
IReadOnlyList 
< 
T 
> 
> 
	ListAsync (
(( )
ISpecification) 7
<7 8
T8 9
>9 :
spec; ?
,? @
CancellationTokenA R
cancellationTokenS d
=e f
defaultg n
)n o
;o p
Task 
< 
T 
> 
AddAsync 
( 
T 
entity !
,! "
CancellationToken# 4
cancellationToken5 F
=G H
defaultI P
)P Q
;Q R
Task 
UpdateAsync 
( 
T 
entity !
,! "
CancellationToken# 4
cancellationToken5 F
=G H
defaultI P
)P Q
;Q R
Task 
DeleteAsync 
( 
T 
entity !
,! "
CancellationToken# 4
cancellationToken5 F
=G H
defaultI P
)P Q
;Q R
Task 
< 
int 
> 

CountAsync 
( 
ISpecification +
<+ ,
T, -
>- .
spec/ 3
,3 4
CancellationToken5 F
cancellationTokenG X
=Y Z
default[ b
)b c
;c d
Task 
< 
T 
> 
FirstOrDefaultAsync #
(# $
ISpecification$ 2
<2 3
T3 4
>4 5
spec6 :
,: ;
CancellationToken< M
cancellationTokenN _
=` a
defaultb i
)i j
;j k
} 
} ô
ï/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.SharedKernel/Interfaces/IDomainEventDispatcher.cs
	namespace 	

TeamTasker
 
. 
SharedKernel !
.! "

Interfaces" ,
{ 
public		 

	interface		 "
IDomainEventDispatcher		 +
{

 
Task 
DispatchEventsAsync  
(  !

BaseEntity! +
entity, 2
,2 3
CancellationToken4 E
cancellationTokenF W
=X Y
defaultZ a
)a b
;b c
} 
} ¥
z/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.SharedKernel/Class1.cs
	namespace 	

TeamTasker
 
. 
SharedKernel !
;! "
public 
class 
Class1 
{ 
} Â0
Ö/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.SharedKernel/BaseSpecification.cs
	namespace 	

TeamTasker
 
. 
SharedKernel !
{ 
public 

abstract 
class 
BaseSpecification +
<+ ,
T, -
>- .
:/ 0
ISpecification1 ?
<? @
T@ A
>A B
{ 
	protected 
BaseSpecification #
(# $

Expression$ .
<. /
Func/ 3
<3 4
T4 5
,5 6
bool7 ;
>; <
>< =
criteria> F
)F G
{ 	
Criteria 
= 
criteria 
;  
} 	
	protected 
BaseSpecification #
(# $
)$ %
{ 	
Criteria 
= 
_ 
=> 
true  
;  !
} 	
public 

Expression 
< 
Func 
< 
T  
,  !
bool" &
>& '
>' (
Criteria) 1
{2 3
get4 7
;7 8
}9 :
public 
List 
< 

Expression 
< 
Func #
<# $
T$ %
,% &
object' -
>- .
>. /
>/ 0
Includes1 9
{: ;
get< ?
;? @
}A B
=C D
newE H
ListI M
<M N

ExpressionN X
<X Y
FuncY ]
<] ^
T^ _
,_ `
objecta g
>g h
>h i
>i j
(j k
)k l
;l m
public 
List 
< 
string 
> 
IncludeStrings *
{+ ,
get- 0
;0 1
}2 3
=4 5
new6 9
List: >
<> ?
string? E
>E F
(F G
)G H
;H I
public 

Expression 
< 
Func 
< 
T  
,  !
object" (
>( )
>) *
OrderBy+ 2
{3 4
get5 8
;8 9
private: A
setB E
;E F
}G H
public 

Expression 
< 
Func 
< 
T  
,  !
object" (
>( )
>) *
OrderByDescending+ <
{= >
get? B
;B C
privateD K
setL O
;O P
}Q R
public 

Expression 
< 
Func 
< 
T  
,  !
object" (
>( )
>) *
GroupBy+ 2
{3 4
get5 8
;8 9
private: A
setB E
;E F
}G H
public 
int 
Take 
{ 
get 
; 
private &
set' *
;* +
}, -
public   
int   
Skip   
{   
get   
;   
private   &
set  ' *
;  * +
}  , -
public!! 
bool!! 
IsPagingEnabled!! #
{!!$ %
get!!& )
;!!) *
private!!+ 2
set!!3 6
;!!6 7
}!!8 9
=!!: ;
false!!< A
;!!A B
	protected## 
virtual## 
void## 

AddInclude## )
(##) *

Expression##* 4
<##4 5
Func##5 9
<##9 :
T##: ;
,##; <
object##= C
>##C D
>##D E
includeExpression##F W
)##W X
{$$ 	
Includes%% 
.%% 
Add%% 
(%% 
includeExpression%% *
)%%* +
;%%+ ,
}&& 	
	protected(( 
virtual(( 
void(( 

AddInclude(( )
((() *
string((* 0
includeString((1 >
)((> ?
{)) 	
IncludeStrings** 
.** 
Add** 
(** 
includeString** ,
)**, -
;**- .
}++ 	
	protected-- 
virtual-- 
void-- 
ApplyPaging-- *
(--* +
int--+ .
skip--/ 3
,--3 4
int--5 8
take--9 =
)--= >
{.. 	
Skip// 
=// 
skip// 
;// 
Take00 
=00 
take00 
;00 
IsPagingEnabled11 
=11 
true11 "
;11" #
}22 	
	protected44 
virtual44 
void44 
ApplyOrderBy44 +
(44+ ,

Expression44, 6
<446 7
Func447 ;
<44; <
T44< =
,44= >
object44? E
>44E F
>44F G
orderByExpression44H Y
)44Y Z
{55 	
OrderBy66 
=66 
orderByExpression66 '
;66' (
}77 	
	protected99 
virtual99 
void99 "
ApplyOrderByDescending99 5
(995 6

Expression996 @
<99@ A
Func99A E
<99E F
T99F G
,99G H
object99I O
>99O P
>99P Q'
orderByDescendingExpression99R m
)99m n
{:: 	
OrderByDescending;; 
=;; '
orderByDescendingExpression;;  ;
;;;; <
}<< 	
	protected>> 
virtual>> 
void>> 
ApplyGroupBy>> +
(>>+ ,

Expression>>, 6
<>>6 7
Func>>7 ;
<>>; <
T>>< =
,>>= >
object>>? E
>>>E F
>>>F G
groupByExpression>>H Y
)>>Y Z
{?? 	
GroupBy@@ 
=@@ 
groupByExpression@@ '
;@@' (
}AA 	
}BB 
}CC Á
~/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.SharedKernel/BaseEntity.cs
	namespace 	

TeamTasker
 
. 
SharedKernel !
{ 
public		 

abstract		 
class		 

BaseEntity		 $
{

 
public 
int 
Id 
{ 
get 
; 
set  
;  !
}" #
private 
List 
< 
INotification "
>" #
_domainEvents$ 1
=2 3
new4 7
List8 <
<< =
INotification= J
>J K
(K L
)L M
;M N
public 
IReadOnlyCollection "
<" #
INotification# 0
>0 1
DomainEvents2 >
=>? A
_domainEventsB O
.O P

AsReadOnlyP Z
(Z [
)[ \
;\ ]
public 
void 
AddDomainEvent "
(" #
INotification# 0
domainEvent1 <
)< =
{ 	
_domainEvents 
. 
Add 
( 
domainEvent )
)) *
;* +
} 	
public 
void 
RemoveDomainEvent %
(% &
INotification& 3
domainEvent4 ?
)? @
{ 	
_domainEvents 
. 
Remove  
(  !
domainEvent! ,
), -
;- .
} 	
public 
void 
ClearDomainEvents %
(% &
)& '
{ 	
_domainEvents 
. 
Clear 
(  
)  !
;! "
} 	
} 
} 