Ò
ž/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Infrastructure/TeamTasker.Infrastructure/Services/DomainEventDispatcher.cs
	namespace 	

TeamTasker
 
. 
Infrastructure #
.# $
Services$ ,
{ 
public 

class !
DomainEventDispatcher &
:' ("
IDomainEventDispatcher) ?
{ 
private 
readonly 
	IMediator "
	_mediator# ,
;, -
public !
DomainEventDispatcher $
($ %
	IMediator% .
mediator/ 7
)7 8
{ 	
	_mediator 
= 
mediator  
;  !
} 	
public 
async 
Task 
DispatchEventsAsync -
(- .

BaseEntity. 8
entity9 ?
,? @
CancellationTokenA R
cancellationTokenS d
=e f
defaultg n
)n o
{ 	
foreach 
( 
var 
domainEvent $
in% '
entity( .
.. /
DomainEvents/ ;
); <
{ 
await 
	_mediator 
.  
Publish  '
(' (
domainEvent( 3
,3 4
cancellationToken5 F
)F G
;G H
} 
entity 
. 
ClearDomainEvents $
($ %
)% &
;& '
} 	
} 
} Î
›/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Infrastructure/TeamTasker.Infrastructure/Services/CurrentUserService.cs
	namespace 	

TeamTasker
 
. 
Infrastructure #
.# $
Services$ ,
{ 
public 

class 
CurrentUserService #
:$ %
ICurrentUserService& 9
{ 
private 
readonly  
IHttpContextAccessor - 
_httpContextAccessor. B
;B C
public 
CurrentUserService !
(! " 
IHttpContextAccessor" 6
httpContextAccessor7 J
)J K
{ 	 
_httpContextAccessor  
=! "
httpContextAccessor# 6
;6 7
} 	
public 
int 
? 
UserId 
{ 	
get 
{ 
var 
userId 
=  
_httpContextAccessor 1
.1 2
HttpContext2 =
?= >
.> ?
User? C
?C D
.D E
ClaimsE K
.K L
FirstOrDefaultL Z
(Z [
c[ \
=>] _
c` a
.a b
Typeb f
==g i

ClaimTypesj t
.t u
NameIdentifier	u ƒ
)
ƒ „
?
„ …
.
… †
Value
† ‹
;
‹ Œ
return 
! 
string 
. 
IsNullOrEmpty ,
(, -
userId- 3
)3 4
?5 6
int7 :
.: ;
Parse; @
(@ A
userIdA G
)G H
:I J
nullK O
;O P
} 
} 	
public 
string 
Username 
=> ! 
_httpContextAccessor" 6
.6 7
HttpContext7 B
?B C
.C D
UserD H
?H I
.I J
ClaimsJ P
.P Q
FirstOrDefaultQ _
(_ `
c` a
=>b d
ce f
.f g
Typeg k
==l n

ClaimTypeso y
.y z
Namez ~
)~ 
?	 €
.
€ 
Value
 †
;
† ‡
public   
bool   
IsAuthenticated   #
=>  $ & 
_httpContextAccessor  ' ;
.  ; <
HttpContext  < G
?  G H
.  H I
User  I M
?  M N
.  N O
Identity  O W
?  W X
.  X Y
IsAuthenticated  Y h
??  i k
false  l q
;  q r
}!! 
}"" ˜
›/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Infrastructure/TeamTasker.Infrastructure/Repositories/UserRepository.cs
	namespace 	

TeamTasker
 
. 
Infrastructure #
.# $
Repositories$ 0
{		 
public 

class 
UserRepository 
:  !
EfRepository" .
<. /
User/ 3
>3 4
,4 5
IUserRepository6 E
{ 
public 
UserRepository 
(  
ApplicationDbContext 2
	dbContext3 <
)< =
:> ?
base@ D
(D E
	dbContextE N
)N O
{ 	
} 	
public 
async 
Task 
< 
User 
> 
GetByEmailAsync  /
(/ 0
string0 6
email7 <
,< =
CancellationToken> O
cancellationTokenP a
=b c
defaultd k
)k l
{ 	
return 
await 

_dbContext #
.# $
Users$ )
. 
FirstOrDefaultAsync $
($ %
u% &
=>' )
u* +
.+ ,
Email, 1
==2 4
email5 :
,: ;
cancellationToken< M
)M N
;N O
} 	
public 
async 
Task 
< 
User 
> 
GetByUsernameAsync  2
(2 3
string3 9
username: B
,B C
CancellationTokenD U
cancellationTokenV g
=h i
defaultj q
)q r
{ 	
return 
await 

_dbContext #
.# $
Users$ )
. 
FirstOrDefaultAsync $
($ %
u% &
=>' )
u* +
.+ ,
Username, 4
==5 7
username8 @
,@ A
cancellationTokenB S
)S T
;T U
} 	
} 
} Ô
›/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Infrastructure/TeamTasker.Infrastructure/Repositories/TaskRepository.cs
	namespace		 	

TeamTasker		
 
.		 
Infrastructure		 #
.		# $
Repositories		$ 0
{

 
public 

class 
TaskRepository 
:  !
EfRepository" .
<. /
Domain/ 5
.5 6
Entities6 >
.> ?
Task? C
>C D
,D E
ITaskRepositoryF U
{ 
public 
TaskRepository 
(  
ApplicationDbContext 2
	dbContext3 <
)< =
:> ?
base@ D
(D E
	dbContextE N
)N O
{ 	
} 	
public 
async 
Task 
< 
IEnumerable %
<% &
Domain& ,
., -
Entities- 5
.5 6
Task6 :
>: ;
>; <!
GetTasksByUserIdAsync= R
(R S
intS V
userIdW ]
,] ^
CancellationToken_ p
cancellationToken	q ‚
=
ƒ „
default
… Œ
)
Œ 
{ 	
return 
await 

_dbContext #
.# $
Tasks$ )
. 
Where 
( 
t 
=> 
t 
. 
AssignedToUserId .
==/ 1
userId2 8
)8 9
. 
ToListAsync 
( 
cancellationToken .
). /
;/ 0
} 	
public 
async 
Task 
< 
IEnumerable %
<% &
Domain& ,
., -
Entities- 5
.5 6
Task6 :
>: ;
>; <$
GetTasksByProjectIdAsync= U
(U V
intV Y
	projectIdZ c
,c d
CancellationTokene v
cancellationToken	w ˆ
=
‰ Š
default
‹ ’
)
’ “
{ 	
return 
await 

_dbContext #
.# $
Tasks$ )
. 
Where 
( 
t 
=> 
t 
. 
	ProjectId '
==( *
	projectId+ 4
)4 5
. 
ToListAsync 
( 
cancellationToken .
). /
;/ 0
}   	
}!! 
}"" ï!
ž/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Infrastructure/TeamTasker.Infrastructure/Repositories/ProjectRepository.cs
	namespace 	

TeamTasker
 
. 
Infrastructure #
.# $
Repositories$ 0
{ 
public 

class 
ProjectRepository "
:# $
EfRepository% 1
<1 2
Project2 9
>9 :
,: ;
IProjectRepository< N
{ 
private 
readonly 
ILogger  
<  !
ProjectRepository! 2
>2 3
_logger4 ;
;; <
public 
ProjectRepository  
(  ! 
ApplicationDbContext! 5
	dbContext6 ?
,? @
ILoggerA H
<H I
ProjectRepositoryI Z
>Z [
logger\ b
)b c
:d e
basef j
(j k
	dbContextk t
)t u
{ 	
_logger 
= 
logger 
; 
} 	
public 
async 
Task 
< 
IEnumerable %
<% &
Project& -
>- .
>. /$
GetProjectsByUserIdAsync0 H
(H I
intI L
userIdM S
,S T
CancellationTokenU f
cancellationTokeng x
=y z
default	{ ‚
)
‚ ƒ
{ 	
_logger 
. 
LogInformation "
(" #
$str# G
,G H
userIdI O
)O P
;P Q
var 
projects 
= 
await  

_dbContext! +
.+ ,
Projects, 4
.   
Where   
(   
p   
=>   
p   
.   
Tasks   #
.  # $
Any  $ '
(  ' (
t  ( )
=>  * ,
t  - .
.  . /
AssignedToUserId  / ?
==  @ B
userId  C I
)  I J
)  J K
.!! 
ToListAsync!! 
(!! 
cancellationToken!! .
)!!. /
;!!/ 0
_logger## 
.## 
LogInformation## "
(##" #
$str### Q
,##Q R
projects##S [
.##[ \
Count##\ a
,##a b
userId##c i
)##i j
;##j k
return%% 
projects%% 
;%% 
}&& 	
public(( 
async(( 
Task(( 
<(( 
Project(( !
>((! "$
GetProjectWithTasksAsync((# ;
(((; <
int((< ?
	projectId((@ I
,((I J
CancellationToken((K \
cancellationToken((] n
=((o p
default((q x
)((x y
{)) 	
_logger** 
.** 
LogInformation** "
(**" #
$str**# K
,**K L
	projectId**M V
)**V W
;**W X
var,, 
project,, 
=,, 
await,, 

_dbContext,,  *
.,,* +
Projects,,+ 3
.-- 
Include-- 
(-- 
p-- 
=>-- 
p-- 
.--  
Tasks--  %
)--% &
... 
FirstOrDefaultAsync.. $
(..$ %
p..% &
=>..' )
p..* +
...+ ,
Id.., .
==../ 1
	projectId..2 ;
,..; <
cancellationToken..= N
)..N O
;..O P
if00 
(00 
project00 
==00 
null00 
)00  
{11 
_logger22 
.22 

LogWarning22 "
(22" #
$str22# B
,22B C
	projectId22D M
)22M N
;22N O
return33 
null33 
;33 
}44 
_logger66 
.66 
LogInformation66 "
(66" #
$str66# Y
,66Y Z
	projectId66[ d
,66d e
project66f m
.66m n
Tasks66n s
.66s t
Count66t y
)66y z
;66z {
return88 
project88 
;88 
}99 	
}:: 
};; âr
§/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Infrastructure/TeamTasker.Infrastructure/Migrations/20250423082445_InitialCreate.cs
	namespace 	

TeamTasker
 
. 
Infrastructure #
.# $

Migrations$ .
{ 
public		 

partial		 
class		 
InitialCreate		 &
:		' (
	Migration		) 2
{

 
	protected 
override 
void 
Up  "
(" #
MigrationBuilder# 3
migrationBuilder4 D
)D E
{ 	
migrationBuilder 
. 
CreateTable (
(( )
name 
: 
$str  
,  !
columns 
: 
table 
=> !
new" %
{ 
Id 
= 
table 
. 
Column %
<% &
int& )
>) *
(* +
type+ /
:/ 0
$str1 :
,: ;
nullable< D
:D E
falseF K
)K L
. 

Annotation #
(# $
$str$ :
,: ;
true< @
)@ A
,A B
Name 
= 
table  
.  !
Column! '
<' (
string( .
>. /
(/ 0
type0 4
:4 5
$str6 <
,< =
	maxLength> G
:G H
$numI L
,L M
nullableN V
:V W
falseX ]
)] ^
,^ _
Description 
=  !
table" '
.' (
Column( .
<. /
string/ 5
>5 6
(6 7
type7 ;
:; <
$str= C
,C D
	maxLengthE N
:N O
$numP S
,S T
nullableU ]
:] ^
false_ d
)d e
,e f
	StartDate 
= 
table  %
.% &
Column& ,
<, -
DateTime- 5
>5 6
(6 7
type7 ;
:; <
$str= C
,C D
nullableE M
:M N
falseO T
)T U
,U V
EndDate 
= 
table #
.# $
Column$ *
<* +
DateTime+ 3
>3 4
(4 5
type5 9
:9 :
$str; A
,A B
nullableC K
:K L
trueM Q
)Q R
,R S
Status 
= 
table "
." #
Column# )
<) *
int* -
>- .
(. /
type/ 3
:3 4
$str5 >
,> ?
nullable@ H
:H I
falseJ O
)O P
} 
, 
constraints 
: 
table "
=># %
{ 
table 
. 

PrimaryKey $
($ %
$str% 2
,2 3
x4 5
=>6 8
x9 :
.: ;
Id; =
)= >
;> ?
} 
) 
; 
migrationBuilder 
. 
CreateTable (
(( )
name   
:   
$str   
,   
columns!! 
:!! 
table!! 
=>!! !
new!!" %
{"" 
Id## 
=## 
table## 
.## 
Column## %
<##% &
int##& )
>##) *
(##* +
type##+ /
:##/ 0
$str##1 :
,##: ;
nullable##< D
:##D E
false##F K
)##K L
.$$ 

Annotation$$ #
($$# $
$str$$$ :
,$$: ;
true$$< @
)$$@ A
,$$A B
	FirstName%% 
=%% 
table%%  %
.%%% &
Column%%& ,
<%%, -
string%%- 3
>%%3 4
(%%4 5
type%%5 9
:%%9 :
$str%%; A
,%%A B
	maxLength%%C L
:%%L M
$num%%N P
,%%P Q
nullable%%R Z
:%%Z [
false%%\ a
)%%a b
,%%b c
LastName&& 
=&& 
table&& $
.&&$ %
Column&&% +
<&&+ ,
string&&, 2
>&&2 3
(&&3 4
type&&4 8
:&&8 9
$str&&: @
,&&@ A
	maxLength&&B K
:&&K L
$num&&M O
,&&O P
nullable&&Q Y
:&&Y Z
false&&[ `
)&&` a
,&&a b
Email'' 
='' 
table'' !
.''! "
Column''" (
<''( )
string'') /
>''/ 0
(''0 1
type''1 5
:''5 6
$str''7 =
,''= >
	maxLength''? H
:''H I
$num''J M
,''M N
nullable''O W
:''W X
false''Y ^
)''^ _
,''_ `
Username(( 
=(( 
table(( $
.(($ %
Column((% +
<((+ ,
string((, 2
>((2 3
(((3 4
type((4 8
:((8 9
$str((: @
,((@ A
	maxLength((B K
:((K L
$num((M O
,((O P
nullable((Q Y
:((Y Z
false(([ `
)((` a
,((a b
Status)) 
=)) 
table)) "
.))" #
Column))# )
<))) *
int))* -
>))- .
()). /
type))/ 3
:))3 4
$str))5 >
,))> ?
nullable))@ H
:))H I
false))J O
)))O P
,))P Q
CreatedDate** 
=**  !
table**" '
.**' (
Column**( .
<**. /
DateTime**/ 7
>**7 8
(**8 9
type**9 =
:**= >
$str**? E
,**E F
nullable**G O
:**O P
false**Q V
)**V W
,**W X
Street++ 
=++ 
table++ "
.++" #
Column++# )
<++) *
string++* 0
>++0 1
(++1 2
type++2 6
:++6 7
$str++8 >
,++> ?
	maxLength++@ I
:++I J
$num++K N
,++N O
nullable++P X
:++X Y
false++Z _
)++_ `
,++` a
City,, 
=,, 
table,,  
.,,  !
Column,,! '
<,,' (
string,,( .
>,,. /
(,,/ 0
type,,0 4
:,,4 5
$str,,6 <
,,,< =
	maxLength,,> G
:,,G H
$num,,I K
,,,K L
nullable,,M U
:,,U V
false,,W \
),,\ ]
,,,] ^
State-- 
=-- 
table-- !
.--! "
Column--" (
<--( )
string--) /
>--/ 0
(--0 1
type--1 5
:--5 6
$str--7 =
,--= >
	maxLength--? H
:--H I
$num--J L
,--L M
nullable--N V
:--V W
false--X ]
)--] ^
,--^ _
Country.. 
=.. 
table.. #
...# $
Column..$ *
<..* +
string..+ 1
>..1 2
(..2 3
type..3 7
:..7 8
$str..9 ?
,..? @
	maxLength..A J
:..J K
$num..L N
,..N O
nullable..P X
:..X Y
false..Z _
).._ `
,..` a
ZipCode// 
=// 
table// #
.//# $
Column//$ *
<//* +
string//+ 1
>//1 2
(//2 3
type//3 7
://7 8
$str//9 ?
,//? @
	maxLength//A J
://J K
$num//L N
,//N O
nullable//P X
://X Y
false//Z _
)//_ `
}00 
,00 
constraints11 
:11 
table11 "
=>11# %
{22 
table33 
.33 

PrimaryKey33 $
(33$ %
$str33% /
,33/ 0
x331 2
=>333 5
x336 7
.337 8
Id338 :
)33: ;
;33; <
}44 
)44 
;44 
migrationBuilder66 
.66 
CreateTable66 (
(66( )
name77 
:77 
$str77 
,77 
columns88 
:88 
table88 
=>88 !
new88" %
{99 
Id:: 
=:: 
table:: 
.:: 
Column:: %
<::% &
int::& )
>::) *
(::* +
type::+ /
:::/ 0
$str::1 :
,::: ;
nullable::< D
:::D E
false::F K
)::K L
.;; 

Annotation;; #
(;;# $
$str;;$ :
,;;: ;
true;;< @
);;@ A
,;;A B
Title<< 
=<< 
table<< !
.<<! "
Column<<" (
<<<( )
string<<) /
><</ 0
(<<0 1
type<<1 5
:<<5 6
$str<<7 =
,<<= >
	maxLength<<? H
:<<H I
$num<<J M
,<<M N
nullable<<O W
:<<W X
false<<Y ^
)<<^ _
,<<_ `
Description== 
===  !
table==" '
.==' (
Column==( .
<==. /
string==/ 5
>==5 6
(==6 7
type==7 ;
:==; <
$str=== C
,==C D
	maxLength==E N
:==N O
$num==P S
,==S T
nullable==U ]
:==] ^
false==_ d
)==d e
,==e f
DueDate>> 
=>> 
table>> #
.>># $
Column>>$ *
<>>* +
DateTime>>+ 3
>>>3 4
(>>4 5
type>>5 9
:>>9 :
$str>>; A
,>>A B
nullable>>C K
:>>K L
false>>M R
)>>R S
,>>S T
Priority?? 
=?? 
table?? $
.??$ %
Column??% +
<??+ ,
int??, /
>??/ 0
(??0 1
type??1 5
:??5 6
$str??7 @
,??@ A
nullable??B J
:??J K
false??L Q
)??Q R
,??R S
Status@@ 
=@@ 
table@@ "
.@@" #
Column@@# )
<@@) *
int@@* -
>@@- .
(@@. /
type@@/ 3
:@@3 4
$str@@5 >
,@@> ?
nullable@@@ H
:@@H I
false@@J O
)@@O P
,@@P Q
	ProjectIdAA 
=AA 
tableAA  %
.AA% &
ColumnAA& ,
<AA, -
intAA- 0
>AA0 1
(AA1 2
typeAA2 6
:AA6 7
$strAA8 A
,AAA B
nullableAAC K
:AAK L
falseAAM R
)AAR S
,AAS T
AssignedToUserIdBB $
=BB% &
tableBB' ,
.BB, -
ColumnBB- 3
<BB3 4
intBB4 7
>BB7 8
(BB8 9
typeBB9 =
:BB= >
$strBB? H
,BBH I
nullableBBJ R
:BBR S
trueBBT X
)BBX Y
,BBY Z
CreatedDateCC 
=CC  !
tableCC" '
.CC' (
ColumnCC( .
<CC. /
DateTimeCC/ 7
>CC7 8
(CC8 9
typeCC9 =
:CC= >
$strCC? E
,CCE F
nullableCCG O
:CCO P
falseCCQ V
)CCV W
,CCW X
CompletedDateDD !
=DD" #
tableDD$ )
.DD) *
ColumnDD* 0
<DD0 1
DateTimeDD1 9
>DD9 :
(DD: ;
typeDD; ?
:DD? @
$strDDA G
,DDG H
nullableDDI Q
:DDQ R
trueDDS W
)DDW X
}EE 
,EE 
constraintsFF 
:FF 
tableFF "
=>FF# %
{GG 
tableHH 
.HH 

PrimaryKeyHH $
(HH$ %
$strHH% /
,HH/ 0
xHH1 2
=>HH3 5
xHH6 7
.HH7 8
IdHH8 :
)HH: ;
;HH; <
tableII 
.II 

ForeignKeyII $
(II$ %
nameJJ 
:JJ 
$strJJ ;
,JJ; <
columnKK 
:KK 
xKK  !
=>KK" $
xKK% &
.KK& '
	ProjectIdKK' 0
,KK0 1
principalTableLL &
:LL& '
$strLL( 2
,LL2 3
principalColumnMM '
:MM' (
$strMM) -
,MM- .
onDeleteNN  
:NN  !
ReferentialActionNN" 3
.NN3 4
CascadeNN4 ;
)NN; <
;NN< =
}OO 
)OO 
;OO 
migrationBuilderQQ 
.QQ 
CreateIndexQQ (
(QQ( )
nameRR 
:RR 
$strRR *
,RR* +
tableSS 
:SS 
$strSS 
,SS 
columnTT 
:TT 
$strTT #
)TT# $
;TT$ %
migrationBuilderVV 
.VV 
CreateIndexVV (
(VV( )
nameWW 
:WW 
$strWW &
,WW& '
tableXX 
:XX 
$strXX 
,XX 
columnYY 
:YY 
$strYY 
,YY  
uniqueZZ 
:ZZ 
trueZZ 
)ZZ 
;ZZ 
migrationBuilder\\ 
.\\ 
CreateIndex\\ (
(\\( )
name]] 
:]] 
$str]] )
,]]) *
table^^ 
:^^ 
$str^^ 
,^^ 
column__ 
:__ 
$str__ "
,__" #
unique`` 
:`` 
true`` 
)`` 
;`` 
}aa 	
	protecteddd 
overridedd 
voiddd 
Downdd  $
(dd$ %
MigrationBuilderdd% 5
migrationBuilderdd6 F
)ddF G
{ee 	
migrationBuilderff 
.ff 
	DropTableff &
(ff& '
namegg 
:gg 
$strgg 
)gg 
;gg 
migrationBuilderii 
.ii 
	DropTableii &
(ii& '
namejj 
:jj 
$strjj 
)jj 
;jj 
migrationBuilderll 
.ll 
	DropTablell &
(ll& '
namemm 
:mm 
$strmm  
)mm  !
;mm! "
}nn 	
}oo 
}pp ž
“/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Infrastructure/TeamTasker.Infrastructure/DependencyInjection.cs
	namespace 	

TeamTasker
 
. 
Infrastructure #
{ 
public 

static 
class 
DependencyInjection +
{ 
public 
static 
IServiceCollection (
AddInfrastructure) :
(: ;
this; ?
IServiceCollection@ R
servicesS [
,[ \
IConfiguration] k
configurationl y
)y z
{ 	
services 
. 
AddDbContext !
<! " 
ApplicationDbContext" 6
>6 7
(7 8
options8 ?
=>@ B
options 
. 
	UseSqlite !
(! "
configuration !
.! "
GetConnectionString" 5
(5 6
$str6 I
)I J
,J K
b 
=> 
b 
. 
MigrationsAssembly -
(- .
typeof. 4
(4 5 
ApplicationDbContext5 I
)I J
.J K
AssemblyK S
.S T
FullNameT \
)\ ]
)] ^
)^ _
;_ `
services 
. 
	AddScoped 
< !
IApplicationDbContext 4
>4 5
(5 6
provider6 >
=>? A
providerB J
.J K
GetRequiredServiceK ]
<] ^ 
ApplicationDbContext^ r
>r s
(s t
)t u
)u v
;v w
services 
. 
	AddScoped 
< "
IDomainEventDispatcher 5
,5 6!
DomainEventDispatcher7 L
>L M
(M N
)N O
;O P
services 
. 
	AddScoped 
( 
typeof %
(% &
IRepository& 1
<1 2
>2 3
)3 4
,4 5
typeof6 <
(< =
EfRepository= I
<I J
>J K
)K L
)L M
;M N
services 
. 
	AddScoped 
< 
IProjectRepository 1
,1 2
ProjectRepository3 D
>D E
(E F
)F G
;G H
services   
.   
	AddScoped   
<   
ITaskRepository   .
,  . /
TaskRepository  0 >
>  > ?
(  ? @
)  @ A
;  A B
services!! 
.!! 
	AddScoped!! 
<!! 
IUserRepository!! .
,!!. /
UserRepository!!0 >
>!!> ?
(!!? @
)!!@ A
;!!A B
services$$ 
.$$ 
	AddScoped$$ 
<$$ 
ICurrentUserService$$ 2
,$$2 3
CurrentUserService$$4 F
>$$F G
($$G H
)$$H I
;$$I J
return&& 
services&& 
;&& 
}'' 	
}(( 
})) Á
›/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Infrastructure/TeamTasker.Infrastructure/Data/SpecificationEvaluator.cs
	namespace 	

TeamTasker
 
. 
Infrastructure #
.# $
Data$ (
{ 
public 

class "
SpecificationEvaluator '
<' (
T( )
>) *
where+ 0
T1 2
:3 4

BaseEntity5 ?
{ 
public 
static 

IQueryable  
<  !
T! "
>" #
GetQuery$ ,
(, -

IQueryable- 7
<7 8
T8 9
>9 :

inputQuery; E
,E F
ISpecificationG U
<U V
TV W
>W X
specificationY f
)f g
{ 	
var 
query 
= 

inputQuery "
;" #
if 
( 
specification 
. 
Criteria &
!=' )
null* .
). /
{ 
query 
= 
query 
. 
Where #
(# $
specification$ 1
.1 2
Criteria2 :
): ;
;; <
} 
query 
= 
specification !
.! "
Includes" *
.* +
	Aggregate+ 4
(4 5
query5 :
,: ;
( 
current 
, 
include !
)! "
=># %
current& -
.- .
Include. 5
(5 6
include6 =
)= >
)> ?
;? @
query 
= 
specification !
.! "
IncludeStrings" 0
.0 1
	Aggregate1 :
(: ;
query; @
,@ A
( 
current 
, 
include !
)! "
=># %
current& -
.- .
Include. 5
(5 6
include6 =
)= >
)> ?
;? @
if!! 
(!! 
specification!! 
.!! 
OrderBy!! %
!=!!& (
null!!) -
)!!- .
{"" 
query## 
=## 
query## 
.## 
OrderBy## %
(##% &
specification##& 3
.##3 4
OrderBy##4 ;
)##; <
;##< =
}$$ 
else%% 
if%% 
(%% 
specification%% "
.%%" #
OrderByDescending%%# 4
!=%%5 7
null%%8 <
)%%< =
{&& 
query'' 
='' 
query'' 
.'' 
OrderByDescending'' /
(''/ 0
specification''0 =
.''= >
OrderByDescending''> O
)''O P
;''P Q
}(( 
if++ 
(++ 
specification++ 
.++ 
GroupBy++ %
!=++& (
null++) -
)++- .
{,, 
query-- 
=-- 
query-- 
.-- 
GroupBy-- %
(--% &
specification--& 3
.--3 4
GroupBy--4 ;
)--; <
.--< =

SelectMany--= G
(--G H
x--H I
=>--J L
x--M N
)--N O
;--O P
}.. 
if11 
(11 
specification11 
.11 
IsPagingEnabled11 -
)11- .
{22 
query33 
=33 
query33 
.33 
Skip33 "
(33" #
specification33# 0
.330 1
Skip331 5
)335 6
.44 
Take44 
(44 
specification44 '
.44' (
Take44( ,
)44, -
;44- .
}55 
return77 
query77 
;77 
}88 	
}99 
}:: ¨8
‘/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Infrastructure/TeamTasker.Infrastructure/Data/EfRepository.cs
	namespace		 	

TeamTasker		
 
.		 
Infrastructure		 #
.		# $
Data		$ (
{

 
public 

class 
EfRepository 
< 
T 
>  
:! "
IRepository# .
<. /
T/ 0
>0 1
where2 7
T8 9
:: ;

BaseEntity< F
{ 
	protected 
readonly  
ApplicationDbContext /

_dbContext0 :
;: ;
public 
EfRepository 
(  
ApplicationDbContext 0
	dbContext1 :
): ;
{ 	

_dbContext 
= 
	dbContext "
;" #
} 	
public 
virtual 
async 
Task !
<! "
T" #
># $
GetByIdAsync% 1
(1 2
int2 5
id6 8
,8 9
CancellationToken: K
cancellationTokenL ]
=^ _
default` g
)g h
{ 	
return 
await 

_dbContext #
.# $
Set$ '
<' (
T( )
>) *
(* +
)+ ,
., -
	FindAsync- 6
(6 7
new7 :
object; A
[A B
]B C
{D E
idF H
}I J
,J K
cancellationTokenL ]
)] ^
;^ _
} 	
public 
async 
Task 
< 
IReadOnlyList '
<' (
T( )
>) *
>* +
ListAllAsync, 8
(8 9
CancellationToken9 J
cancellationTokenK \
=] ^
default_ f
)f g
{ 	
return 
await 

_dbContext #
.# $
Set$ '
<' (
T( )
>) *
(* +
)+ ,
., -
ToListAsync- 8
(8 9
cancellationToken9 J
)J K
;K L
}   	
public"" 
async"" 
Task"" 
<"" 
IReadOnlyList"" '
<""' (
T""( )
>"") *
>""* +
	ListAsync"", 5
(""5 6
ISpecification""6 D
<""D E
T""E F
>""F G
spec""H L
,""L M
CancellationToken""N _
cancellationToken""` q
=""r s
default""t {
)""{ |
{## 	
return$$ 
await$$ 
ApplySpecification$$ +
($$+ ,
spec$$, 0
)$$0 1
.$$1 2
ToListAsync$$2 =
($$= >
cancellationToken$$> O
)$$O P
;$$P Q
}%% 	
public'' 
async'' 
Task'' 
<'' 
T'' 
>'' 
AddAsync'' %
(''% &
T''& '
entity''( .
,''. /
CancellationToken''0 A
cancellationToken''B S
=''T U
default''V ]
)''] ^
{(( 	
await)) 

_dbContext)) 
.)) 
Set))  
<))  !
T))! "
>))" #
())# $
)))$ %
.))% &
AddAsync))& .
()). /
entity))/ 5
,))5 6
cancellationToken))7 H
)))H I
;))I J
await** 

_dbContext** 
.** 
SaveChangesAsync** -
(**- .
cancellationToken**. ?
)**? @
;**@ A
return,, 
entity,, 
;,, 
}-- 	
public// 
async// 
Task// 
UpdateAsync// %
(//% &
T//& '
entity//( .
,//. /
CancellationToken//0 A
cancellationToken//B S
=//T U
default//V ]
)//] ^
{00 	

_dbContext11 
.11 
Entry11 
(11 
entity11 #
)11# $
.11$ %
State11% *
=11+ ,
EntityState11- 8
.118 9
Modified119 A
;11A B
await22 

_dbContext22 
.22 
SaveChangesAsync22 -
(22- .
cancellationToken22. ?
)22? @
;22@ A
}33 	
public55 
async55 
Task55 
DeleteAsync55 %
(55% &
T55& '
entity55( .
,55. /
CancellationToken550 A
cancellationToken55B S
=55T U
default55V ]
)55] ^
{66 	

_dbContext77 
.77 
Set77 
<77 
T77 
>77 
(77 
)77 
.77  
Remove77  &
(77& '
entity77' -
)77- .
;77. /
await88 

_dbContext88 
.88 
SaveChangesAsync88 -
(88- .
cancellationToken88. ?
)88? @
;88@ A
}99 	
public;; 
async;; 
Task;; 
<;; 
int;; 
>;; 

CountAsync;; )
(;;) *
ISpecification;;* 8
<;;8 9
T;;9 :
>;;: ;
spec;;< @
,;;@ A
CancellationToken;;B S
cancellationToken;;T e
=;;f g
default;;h o
);;o p
{<< 	
return== 
await== 
ApplySpecification== +
(==+ ,
spec==, 0
)==0 1
.==1 2

CountAsync==2 <
(==< =
cancellationToken=== N
)==N O
;==O P
}>> 	
public@@ 
async@@ 
Task@@ 
<@@ 
T@@ 
>@@ 
FirstOrDefaultAsync@@ 0
(@@0 1
ISpecification@@1 ?
<@@? @
T@@@ A
>@@A B
spec@@C G
,@@G H
CancellationToken@@I Z
cancellationToken@@[ l
=@@m n
default@@o v
)@@v w
{AA 	
returnBB 
awaitBB 
ApplySpecificationBB +
(BB+ ,
specBB, 0
)BB0 1
.BB1 2
FirstOrDefaultAsyncBB2 E
(BBE F
cancellationTokenBBF W
)BBW X
;BBX Y
}CC 	
privateEE 

IQueryableEE 
<EE 
TEE 
>EE 
ApplySpecificationEE 0
(EE0 1
ISpecificationEE1 ?
<EE? @
TEE@ A
>EEA B
specEEC G
)EEG H
{FF 	
returnGG "
SpecificationEvaluatorGG )
<GG) *
TGG* +
>GG+ ,
.GG, -
GetQueryGG- 5
(GG5 6

_dbContextGG6 @
.GG@ A
SetGGA D
<GGD E
TGGE F
>GGF G
(GGG H
)GGH I
.GGI J
AsQueryableGGJ U
(GGU V
)GGV W
,GGW X
specGGY ]
)GG] ^
;GG^ _
}HH 	
}II 
}JJ Ž*
¥/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Infrastructure/TeamTasker.Infrastructure/Data/Configurations/UserConfiguration.cs
	namespace 	

TeamTasker
 
. 
Infrastructure #
.# $
Data$ (
.( )
Configurations) 7
{ 
public 

class 
UserConfiguration "
:# $$
IEntityTypeConfiguration% =
<= >
User> B
>B C
{ 
public 
void 
	Configure 
( 
EntityTypeBuilder /
</ 0
User0 4
>4 5
builder6 =
)= >
{ 	
builder 
. 
HasKey 
( 
u 
=> 
u  !
.! "
Id" $
)$ %
;% &
builder 
. 
Property 
( 
u 
=> !
u" #
.# $
	FirstName$ -
)- .
. 

IsRequired 
( 
) 
. 
HasMaxLength 
( 
$num  
)  !
;! "
builder 
. 
Property 
( 
u 
=> !
u" #
.# $
LastName$ ,
), -
. 

IsRequired 
( 
) 
. 
HasMaxLength 
( 
$num  
)  !
;! "
builder 
. 
Property 
( 
u 
=> !
u" #
.# $
Email$ )
)) *
. 

IsRequired 
( 
) 
. 
HasMaxLength 
( 
$num !
)! "
;" #
builder 
. 
HasIndex 
( 
u 
=> !
u" #
.# $
Email$ )
)) *
. 
IsUnique 
( 
) 
; 
builder   
.   
Property   
(   
u   
=>   !
u  " #
.  # $
Username  $ ,
)  , -
.!! 

IsRequired!! 
(!! 
)!! 
."" 
HasMaxLength"" 
("" 
$num""  
)""  !
;""! "
builder$$ 
.$$ 
HasIndex$$ 
($$ 
u$$ 
=>$$ !
u$$" #
.$$# $
Username$$$ ,
)$$, -
.%% 
IsUnique%% 
(%% 
)%% 
;%% 
builder'' 
.'' 
Property'' 
('' 
u'' 
=>'' !
u''" #
.''# $
Status''$ *
)''* +
.(( 

IsRequired(( 
((( 
)(( 
;(( 
builder** 
.** 
Property** 
(** 
u** 
=>** !
u**" #
.**# $
CreatedDate**$ /
)**/ 0
.++ 

IsRequired++ 
(++ 
)++ 
;++ 
builder-- 
.-- 
OwnsOne-- 
(-- 
u-- 
=>--  
u--! "
.--" #
Address--# *
,--* +
a--, -
=>--. 0
{.. 
a// 
.// 
Property// 
(// 
p// 
=>// 
p//  !
.//! "
Street//" (
)//( )
.00 
HasMaxLength00 !
(00! "
$num00" %
)00% &
.11 
HasColumnName11 "
(11" #
$str11# +
)11+ ,
;11, -
a33 
.33 
Property33 
(33 
p33 
=>33 
p33  !
.33! "
City33" &
)33& '
.44 
HasMaxLength44 !
(44! "
$num44" $
)44$ %
.55 
HasColumnName55 "
(55" #
$str55# )
)55) *
;55* +
a77 
.77 
Property77 
(77 
p77 
=>77 
p77  !
.77! "
State77" '
)77' (
.88 
HasMaxLength88 !
(88! "
$num88" $
)88$ %
.99 
HasColumnName99 "
(99" #
$str99# *
)99* +
;99+ ,
a;; 
.;; 
Property;; 
(;; 
p;; 
=>;; 
p;;  !
.;;! "
Country;;" )
);;) *
.<< 
HasMaxLength<< !
(<<! "
$num<<" $
)<<$ %
.== 
HasColumnName== "
(==" #
$str==# ,
)==, -
;==- .
a?? 
.?? 
Property?? 
(?? 
p?? 
=>?? 
p??  !
.??! "
ZipCode??" )
)??) *
.@@ 
HasMaxLength@@ !
(@@! "
$num@@" $
)@@$ %
.AA 
HasColumnNameAA "
(AA" #
$strAA# ,
)AA, -
;AA- .
}BB 
)BB 
;BB 
}CC 	
}DD 
}EE ·
¥/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Infrastructure/TeamTasker.Infrastructure/Data/Configurations/TaskConfiguration.cs
	namespace 	

TeamTasker
 
. 
Infrastructure #
.# $
Data$ (
.( )
Configurations) 7
{ 
public		 

class		 
TaskConfiguration		 "
:		# $$
IEntityTypeConfiguration		% =
<		= >
Domain		> D
.		D E
Entities		E M
.		M N
Task		N R
>		R S
{

 
public 
void 
	Configure 
( 
EntityTypeBuilder /
</ 0
Domain0 6
.6 7
Entities7 ?
.? @
Task@ D
>D E
builderF M
)M N
{ 	
builder 
. 
HasKey 
( 
t 
=> 
t  !
.! "
Id" $
)$ %
;% &
builder 
. 
Property 
( 
t 
=> !
t" #
.# $
Title$ )
)) *
. 

IsRequired 
( 
) 
. 
HasMaxLength 
( 
$num !
)! "
;" #
builder 
. 
Property 
( 
t 
=> !
t" #
.# $
Description$ /
)/ 0
. 
HasMaxLength 
( 
$num !
)! "
;" #
builder 
. 
Property 
( 
t 
=> !
t" #
.# $
Status$ *
)* +
. 

IsRequired 
( 
) 
; 
builder 
. 
Property 
( 
t 
=> !
t" #
.# $
Priority$ ,
), -
. 

IsRequired 
( 
) 
; 
builder 
. 
Property 
( 
t 
=> !
t" #
.# $
CreatedDate$ /
)/ 0
. 

IsRequired 
( 
) 
; 
} 	
} 
}   È
¨/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Infrastructure/TeamTasker.Infrastructure/Data/Configurations/ProjectConfiguration.cs
	namespace 	

TeamTasker
 
. 
Infrastructure #
.# $
Data$ (
.( )
Configurations) 7
{ 
public

 

class

  
ProjectConfiguration

 %
:

& '$
IEntityTypeConfiguration

( @
<

@ A
Project

A H
>

H I
{ 
public 
void 
	Configure 
( 
EntityTypeBuilder /
</ 0
Project0 7
>7 8
builder9 @
)@ A
{ 	
builder 
. 
HasKey 
( 
p 
=> 
p  !
.! "
Id" $
)$ %
;% &
builder 
. 
Property 
( 
p 
=> !
p" #
.# $
Name$ (
)( )
. 

IsRequired 
( 
) 
. 
HasMaxLength 
( 
$num !
)! "
;" #
builder 
. 
Property 
( 
p 
=> !
p" #
.# $
Description$ /
)/ 0
. 
HasMaxLength 
( 
$num !
)! "
;" #
builder 
. 
Property 
( 
p 
=> !
p" #
.# $
Status$ *
)* +
. 

IsRequired 
( 
) 
; 
builder 
. 
HasMany 
( 
p 
=>  
p! "
." #
Tasks# (
)( )
. 
WithOne 
( 
) 
. 
HasForeignKey 
( 
t  
=>! #
t$ %
.% &
	ProjectId& /
)/ 0
. 
OnDelete 
( 
DeleteBehavior (
.( )
Cascade) 0
)0 1
;1 2
} 	
} 
}   …
™/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Infrastructure/TeamTasker.Infrastructure/Data/ApplicationDbContext.cs
	namespace 	

TeamTasker
 
. 
Infrastructure #
.# $
Data$ (
{ 
public 

class  
ApplicationDbContext %
:& '
	DbContext( 1
,1 2!
IApplicationDbContext3 H
{ 
private 
readonly "
IDomainEventDispatcher /
_dispatcher0 ;
;; <
public  
ApplicationDbContext #
(# $
DbContextOptions 
<  
ApplicationDbContext 1
>1 2
options3 :
,: ;"
IDomainEventDispatcher "

dispatcher# -
)- .
:/ 0
base1 5
(5 6
options6 =
)= >
{ 	
_dispatcher 
= 

dispatcher $
;$ %
} 	
public 
DbSet 
< 
Project 
> 
Projects &
{' (
get) ,
;, -
set. 1
;1 2
}3 4
public 
DbSet 
< 
Domain 
. 
Entities $
.$ %
Task% )
>) *
Tasks+ 0
{1 2
get3 6
;6 7
set8 ;
;; <
}= >
public 
DbSet 
< 
User 
> 
Users  
{! "
get# &
;& '
set( +
;+ ,
}- .
	protected 
override 
void 
OnModelCreating  /
(/ 0
ModelBuilder0 <
modelBuilder= I
)I J
{   	
base!! 
.!! 
OnModelCreating!!  
(!!  !
modelBuilder!!! -
)!!- .
;!!. /
modelBuilder## 
.## +
ApplyConfigurationsFromAssembly## 8
(##8 9
typeof##9 ?
(##? @ 
ProjectConfiguration##@ T
)##T U
.##U V
Assembly##V ^
)##^ _
;##_ `
}$$ 	
public&& 
override&& 
async&& 
Task&& "
<&&" #
int&&# &
>&&& '
SaveChangesAsync&&( 8
(&&8 9
CancellationToken&&9 J
cancellationToken&&K \
=&&] ^
default&&_ f
)&&f g
{'' 	
foreach)) 
()) 
var)) 
entry)) 
in)) !
ChangeTracker))" /
.))/ 0
Entries))0 7
<))7 8

BaseEntity))8 B
>))B C
())C D
)))D E
)))E F
{** 
if++ 
(++ 
entry++ 
.++ 
Entity++  
.++  !
DomainEvents++! -
.++- .
Any++. 1
(++1 2
)++2 3
)++3 4
{,, 
await-- 
_dispatcher-- %
.--% &
DispatchEventsAsync--& 9
(--9 :
entry--: ?
.--? @
Entity--@ F
,--F G
cancellationToken--H Y
)--Y Z
;--Z [
}.. 
}// 
return11 
await11 
base11 
.11 
SaveChangesAsync11 .
(11. /
cancellationToken11/ @
)11@ A
;11A B
}22 	
}33 
}44 Ã
†/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Infrastructure/TeamTasker.Infrastructure/Class1.cs
	namespace 	

TeamTasker
 
. 
Infrastructure #
;# $
public 
class 
Class1 
{ 
} 