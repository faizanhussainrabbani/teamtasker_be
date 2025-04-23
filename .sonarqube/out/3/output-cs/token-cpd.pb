â
ö/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.Application/Projects/Queries/GetProjects/ProjectDto.cs
	namespace 	

TeamTasker
 
. 
Application  
.  !
Projects! )
.) *
Queries* 1
.1 2
GetProjects2 =
{ 
public 

class 

ProjectDto 
{		 
public

 
int

 
Id

 
{

 
get

 
;

 
set

  
;

  !
}

" #
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Description !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 
DateTime 
	StartDate !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 
DateTime 
? 
EndDate  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 
string 
Status 
{ 
get "
;" #
set$ '
;' (
}) *
} 
} ‹3
†/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.Application/Projects/Queries/GetProjects/GetProjectsQuery.cs
	namespace 	

TeamTasker
 
. 
Application  
.  !
Projects! )
.) *
Queries* 1
.1 2
GetProjects2 =
{ 
public 

class 
GetProjectsQuery !
:" #
IRequest$ ,
<, -
PaginatedList- :
<: ;

ProjectDto; E
>E F
>F G
{ 
public 
int 

PageNumber 
{ 
get  #
;# $
set% (
;( )
}* +
=, -
$num. /
;/ 0
public 
int 
PageSize 
{ 
get !
;! "
set# &
;& '
}( )
=* +
$num, .
;. /
public 
string 

SearchTerm  
{! "
get# &
;& '
set( +
;+ ,
}- .
public$$ 
string$$ 
Status$$ 
{$$ 
get$$ "
;$$" #
set$$$ '
;$$' (
}$$) *
}%% 
public'' 

class'' #
GetProjectsQueryHandler'' (
:'') *
IRequestHandler''+ :
<'': ;
GetProjectsQuery''; K
,''K L
PaginatedList''M Z
<''Z [

ProjectDto''[ e
>''e f
>''f g
{(( 
private)) 
readonly)) 
IProjectRepository)) +
_projectRepository)), >
;))> ?
private** 
readonly** 
ICurrentUserService** ,
_currentUserService**- @
;**@ A
private++ 
readonly++ !
IApplicationDbContext++ .

_dbContext++/ 9
;++9 :
public-- #
GetProjectsQueryHandler-- &
(--& '
IProjectRepository.. 
projectRepository.. 0
,..0 1
ICurrentUserService// 
currentUserService//  2
,//2 3!
IApplicationDbContext00 !
	dbContext00" +
)00+ ,
{11 	
_projectRepository22 
=22  
projectRepository22! 2
;222 3
_currentUserService33 
=33  !
currentUserService33" 4
;334 5

_dbContext44 
=44 
	dbContext44 "
;44" #
}55 	
public77 
async77 
Task77 
<77 
PaginatedList77 '
<77' (

ProjectDto77( 2
>772 3
>773 4
Handle775 ;
(77; <
GetProjectsQuery77< L
request77M T
,77T U
CancellationToken77V g
cancellationToken77h y
)77y z
{88 	
var:: 
query:: 
=:: 

_dbContext:: "
.::" #
Projects::# +
.::+ ,
AsQueryable::, 7
(::7 8
)::8 9
;::9 :
if== 
(== 
!== 
string== 
.== 
IsNullOrWhiteSpace== *
(==* +
request==+ 2
.==2 3

SearchTerm==3 =
)=== >
)==> ?
{>> 
var?? 

searchTerm?? 
=??  
request??! (
.??( )

SearchTerm??) 3
.??3 4
Trim??4 8
(??8 9
)??9 :
.??: ;
ToLower??; B
(??B C
)??C D
;??D E
query@@ 
=@@ 
query@@ 
.@@ 
Where@@ #
(@@# $
p@@$ %
=>@@& (
pAA 
.AA 
NameAA 
.AA 
ToLowerAA "
(AA" #
)AA# $
.AA$ %
ContainsAA% -
(AA- .

searchTermAA. 8
)AA8 9
||AA: <
pBB 
.BB 
DescriptionBB !
.BB! "
ToLowerBB" )
(BB) *
)BB* +
.BB+ ,
ContainsBB, 4
(BB4 5

searchTermBB5 ?
)BB? @
)BB@ A
;BBA B
}CC 
ifEE 
(EE 
!EE 
stringEE 
.EE 
IsNullOrWhiteSpaceEE *
(EE* +
requestEE+ 2
.EE2 3
StatusEE3 9
)EE9 :
)EE: ;
{FF 
ifGG 
(GG 
SystemGG 
.GG 
EnumGG 
.GG  
TryParseGG  (
<GG( )
DomainGG) /
.GG/ 0
EntitiesGG0 8
.GG8 9
ProjectStatusGG9 F
>GGF G
(GGG H
requestGGH O
.GGO P
StatusGGP V
,GGV W
trueGGX \
,GG\ ]
outGG^ a
varGGb e
statusGGf l
)GGl m
)GGm n
{HH 
queryII 
=II 
queryII !
.II! "
WhereII" '
(II' (
pII( )
=>II* ,
pII- .
.II. /
StatusII/ 5
==II6 8
statusII9 ?
)II? @
;II@ A
}JJ 
}KK 
varNN 
projectDtosQueryNN  
=NN! "
queryNN# (
.NN( )
SelectNN) /
(NN/ 0
pNN0 1
=>NN2 4
newNN5 8

ProjectDtoNN9 C
{OO 
IdPP 
=PP 
pPP 
.PP 
IdPP 
,PP 
NameQQ 
=QQ 
pQQ 
.QQ 
NameQQ 
,QQ 
DescriptionRR 
=RR 
pRR 
.RR  
DescriptionRR  +
,RR+ ,
	StartDateSS 
=SS 
pSS 
.SS 
	StartDateSS '
,SS' (
EndDateTT 
=TT 
pTT 
.TT 
EndDateTT #
,TT# $
StatusUU 
=UU 
pUU 
.UU 
StatusUU !
.UU! "
ToStringUU" *
(UU* +
)UU+ ,
}VV 
)VV 
;VV 
returnYY 
awaitYY 
PaginatedListYY &
<YY& '

ProjectDtoYY' 1
>YY1 2
.YY2 3
CreateAsyncYY3 >
(YY> ?
projectDtosQueryZZ  
,ZZ  !
request[[ 
.[[ 

PageNumber[[ "
,[[" #
request\\ 
.\\ 
PageSize\\  
,\\  !
cancellationToken]] !
)]]! "
;]]" #
}^^ 	
}__ 
}`` ü
£/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.Application/Projects/Queries/GetProjectById/ProjectDetailDto.cs
	namespace 	

TeamTasker
 
. 
Application  
.  !
Projects! )
.) *
Queries* 1
.1 2
GetProjectById2 @
{ 
public		 

class		 
ProjectDetailDto		 !
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
}" #
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Description !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 
DateTime 
	StartDate !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 
DateTime 
? 
EndDate  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 
string 
Status 
{ 
get "
;" #
set$ '
;' (
}) *
public 
List 
< 
TaskDto 
> 
Tasks "
{# $
get% (
;( )
set* -
;- .
}/ 0
=1 2
new3 6
List7 ;
<; <
TaskDto< C
>C D
(D E
)E F
;F G
} 
public 

class 
TaskDto 
{ 
public 
int 
Id 
{ 
get 
; 
set  
;  !
}" #
public 
string 
Title 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
Description !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 
DateTime 
DueDate 
{  !
get" %
;% &
set' *
;* +
}, -
public 
string 
Status 
{ 
get "
;" #
set$ '
;' (
}) *
public 
string 
Priority 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 
int 
? 
AssignedToUserId $
{% &
get' *
;* +
set, /
;/ 0
}1 2
} 
} ◊#
¶/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.Application/Projects/Queries/GetProjectById/GetProjectByIdQuery.cs
	namespace 	

TeamTasker
 
. 
Application  
.  !
Projects! )
.) *
Queries* 1
.1 2
GetProjectById2 @
{		 
public 

class 
GetProjectByIdQuery $
:% &
IRequest' /
</ 0
ProjectDetailDto0 @
>@ A
{ 
public 
int 
Id 
{ 
get 
; 
set  
;  !
}" #
} 
public 

class &
GetProjectByIdQueryHandler +
:, -
IRequestHandler. =
<= >
GetProjectByIdQuery> Q
,Q R
ProjectDetailDtoS c
>c d
{ 
private 
readonly 
IProjectRepository +
_projectRepository, >
;> ?
public &
GetProjectByIdQueryHandler )
() *
IProjectRepository* <
projectRepository= N
)N O
{ 	
_projectRepository 
=  
projectRepository! 2
;2 3
} 	
public 
async 
Task 
< 
ProjectDetailDto *
>* +
Handle, 2
(2 3
GetProjectByIdQuery3 F
requestG N
,N O
CancellationTokenP a
cancellationTokenb s
)s t
{ 	
var 
project 
= 
await 
_projectRepository  2
.2 3$
GetProjectWithTasksAsync3 K
(K L
requestL S
.S T
IdT V
,V W
cancellationTokenX i
)i j
;j k
if 
( 
project 
== 
null 
)  
{   
throw!! 
new!! 
NotFoundException!! +
(!!+ ,
nameof!!, 2
(!!2 3
Project!!3 :
)!!: ;
,!!; <
request!!= D
.!!D E
Id!!E G
)!!G H
;!!H I
}"" 
var$$ 

projectDto$$ 
=$$ 
new$$  
ProjectDetailDto$$! 1
{%% 
Id&& 
=&& 
project&& 
.&& 
Id&& 
,&&  
Name'' 
='' 
project'' 
.'' 
Name'' #
,''# $
Description(( 
=(( 
project(( %
.((% &
Description((& 1
,((1 2
	StartDate)) 
=)) 
project)) #
.))# $
	StartDate))$ -
,))- .
EndDate** 
=** 
project** !
.**! "
EndDate**" )
,**) *
Status++ 
=++ 
project++  
.++  !
Status++! '
.++' (
ToString++( 0
(++0 1
)++1 2
},, 
;,, 
foreach.. 
(.. 
var.. 
task.. 
in..  
project..! (
...( )
Tasks..) .
)... /
{// 

projectDto00 
.00 
Tasks00  
.00  !
Add00! $
(00$ %
new00% (
TaskDto00) 0
{11 
Id22 
=22 
task22 
.22 
Id22  
,22  !
Title33 
=33 
task33  
.33  !
Title33! &
,33& '
Description44 
=44  !
task44" &
.44& '
Description44' 2
,442 3
DueDate55 
=55 
task55 "
.55" #
DueDate55# *
,55* +
Status66 
=66 
task66 !
.66! "
Status66" (
.66( )
ToString66) 1
(661 2
)662 3
,663 4
Priority77 
=77 
task77 #
.77# $
Priority77$ ,
.77, -
ToString77- 5
(775 6
)776 7
,777 8
AssignedToUserId88 $
=88% &
task88' +
.88+ ,
AssignedToUserId88, <
}99 
)99 
;99 
}:: 
return<< 

projectDto<< 
;<< 
}== 	
}>> 
}?? ∑
∞/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.Application/Projects/Commands/UpdateProject/UpdateProjectCommandValidator.cs
	namespace 	

TeamTasker
 
. 
Application  
.  !
Projects! )
.) *
Commands* 2
.2 3
UpdateProject3 @
{ 
public 

class )
UpdateProjectCommandValidator .
:/ 0
AbstractValidator1 B
<B C 
UpdateProjectCommandC W
>W X
{		 
public

 )
UpdateProjectCommandValidator

 ,
(

, -
)

- .
{ 	
RuleFor 
( 
v 
=> 
v 
. 
Id 
) 
. 
NotEmpty 
( 
) 
. 
WithMessage '
(' (
$str( 9
)9 :
;: ;
RuleFor 
( 
v 
=> 
v 
. 
Name 
)  
. 
NotEmpty 
( 
) 
. 
WithMessage '
(' (
$str( ;
); <
. 
MaximumLength 
( 
$num "
)" #
.# $
WithMessage$ /
(/ 0
$str0 V
)V W
;W X
RuleFor 
( 
v 
=> 
v 
. 
Description &
)& '
. 
MaximumLength 
( 
$num "
)" #
.# $
WithMessage$ /
(/ 0
$str0 ]
)] ^
;^ _
RuleFor 
( 
v 
=> 
v 
. 
	StartDate $
)$ %
. 
NotEmpty 
( 
) 
. 
WithMessage '
(' (
$str( A
)A B
;B C
RuleFor 
( 
v 
=> 
v 
. 
EndDate "
)" #
.  
GreaterThanOrEqualTo %
(% &
v& '
=>( *
v+ ,
., -
	StartDate- 6
)6 7
. 
When 
( 
v 
=> 
v 
. 
EndDate $
.$ %
HasValue% -
)- .
. 
WithMessage 
( 
$str T
)T U
;U V
} 	
} 
} å
ß/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.Application/Projects/Commands/UpdateProject/UpdateProjectCommand.cs
	namespace

 	

TeamTasker


 
.

 
Application

  
.

  !
Projects

! )
.

) *
Commands

* 2
.

2 3
UpdateProject

3 @
{ 
public 

class  
UpdateProjectCommand %
:& '
IRequest( 0
{ 
public 
int 
Id 
{ 
get 
; 
set  
;  !
}" #
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Description !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 
DateTime 
	StartDate !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 
DateTime 
? 
EndDate  
{! "
get# &
;& '
set( +
;+ ,
}- .
} 
public 

class '
UpdateProjectCommandHandler ,
:- .
IRequestHandler/ >
<> ? 
UpdateProjectCommand? S
>S T
{ 
private 
readonly 
IProjectRepository +
_projectRepository, >
;> ?
public '
UpdateProjectCommandHandler *
(* +
IProjectRepository+ =
projectRepository> O
)O P
{ 	
_projectRepository 
=  
projectRepository! 2
;2 3
} 	
public!! 
async!! 
Task!! 
Handle!!  
(!!  ! 
UpdateProjectCommand!!! 5
request!!6 =
,!!= >
CancellationToken!!? P
cancellationToken!!Q b
)!!b c
{"" 	
var## 
project## 
=## 
await## 
_projectRepository##  2
.##2 3
GetByIdAsync##3 ?
(##? @
request##@ G
.##G H
Id##H J
,##J K
cancellationToken##L ]
)##] ^
;##^ _
if%% 
(%% 
project%% 
==%% 
null%% 
)%%  
{&& 
throw'' 
new'' 
NotFoundException'' +
(''+ ,
nameof'', 2
(''2 3
Project''3 :
)'': ;
,''; <
request''= D
.''D E
Id''E G
)''G H
;''H I
}(( 
project** 
.** 
UpdateDetails** !
(**! "
request++ 
.++ 
Name++ 
,++ 
request,, 
.,, 
Description,, #
,,,# $
request-- 
.-- 
	StartDate-- !
,--! "
request.. 
... 
EndDate.. 
)..  
;..  !
await00 
_projectRepository00 $
.00$ %
UpdateAsync00% 0
(000 1
project001 8
,008 9
cancellationToken00: K
)00K L
;00L M
}11 	
}22 
}33 ä
ß/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.Application/Projects/Commands/DeleteProject/DeleteProjectCommand.cs
	namespace		 	

TeamTasker		
 
.		 
Application		  
.		  !
Projects		! )
.		) *
Commands		* 2
.		2 3
DeleteProject		3 @
{

 
public 

class  
DeleteProjectCommand %
:& '
IRequest( 0
{ 
public 
int 
Id 
{ 
get 
; 
set  
;  !
}" #
} 
public 

class '
DeleteProjectCommandHandler ,
:- .
IRequestHandler/ >
<> ? 
DeleteProjectCommand? S
>S T
{ 
private 
readonly 
IProjectRepository +
_projectRepository, >
;> ?
public '
DeleteProjectCommandHandler *
(* +
IProjectRepository+ =
projectRepository> O
)O P
{ 	
_projectRepository 
=  
projectRepository! 2
;2 3
} 	
public 
async 
Task 
Handle  
(  ! 
DeleteProjectCommand! 5
request6 =
,= >
CancellationToken? P
cancellationTokenQ b
)b c
{ 	
var 
project 
= 
await 
_projectRepository  2
.2 3
GetByIdAsync3 ?
(? @
request@ G
.G H
IdH J
,J K
cancellationTokenL ]
)] ^
;^ _
if   
(   
project   
==   
null   
)    
{!! 
throw"" 
new"" 
NotFoundException"" +
(""+ ,
nameof"", 2
(""2 3
Project""3 :
)"": ;
,""; <
request""= D
.""D E
Id""E G
)""G H
;""H I
}## 
await%% 
_projectRepository%% $
.%%$ %
DeleteAsync%%% 0
(%%0 1
project%%1 8
,%%8 9
cancellationToken%%: K
)%%K L
;%%L M
}&& 	
}'' 
}(( ç
∞/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.Application/Projects/Commands/CreateProject/CreateProjectCommandValidator.cs
	namespace 	

TeamTasker
 
. 
Application  
.  !
Projects! )
.) *
Commands* 2
.2 3
CreateProject3 @
{ 
public		 

class		 )
CreateProjectCommandValidator		 .
:		/ 0
AbstractValidator		1 B
<		B C 
CreateProjectCommand		C W
>		W X
{

 
public )
CreateProjectCommandValidator ,
(, -
)- .
{ 	
RuleFor 
( 
v 
=> 
v 
. 
Name 
)  
. 
NotEmpty 
( 
) 
. 
WithMessage '
(' (
$str( ;
); <
. 
MaximumLength 
( 
$num "
)" #
.# $
WithMessage$ /
(/ 0
$str0 V
)V W
;W X
RuleFor 
( 
v 
=> 
v 
. 
Description &
)& '
. 
MaximumLength 
( 
$num "
)" #
.# $
WithMessage$ /
(/ 0
$str0 ]
)] ^
;^ _
RuleFor 
( 
v 
=> 
v 
. 
	StartDate $
)$ %
. 
NotEmpty 
( 
) 
. 
WithMessage '
(' (
$str( A
)A B
;B C
RuleFor 
( 
v 
=> 
v 
. 
EndDate "
)" #
.  
GreaterThanOrEqualTo %
(% &
v& '
=>( *
v+ ,
., -
	StartDate- 6
)6 7
. 
When 
( 
v 
=> 
v 
. 
EndDate $
.$ %
HasValue% -
)- .
. 
WithMessage 
( 
$str T
)T U
;U V
} 	
} 
}  
ß/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.Application/Projects/Commands/CreateProject/CreateProjectCommand.cs
	namespace 	

TeamTasker
 
. 
Application  
.  !
Projects! )
.) *
Commands* 2
.2 3
CreateProject3 @
{		 
public 

class  
CreateProjectCommand %
:& '
IRequest( 0
<0 1
int1 4
>4 5
{ 
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Description !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 
DateTime 
	StartDate !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 
DateTime 
? 
EndDate  
{! "
get# &
;& '
set( +
;+ ,
}- .
} 
public 

class '
CreateProjectCommandHandler ,
:- .
IRequestHandler/ >
<> ? 
CreateProjectCommand? S
,S T
intU X
>X Y
{ 
private 
readonly 
IProjectRepository +
_projectRepository, >
;> ?
public '
CreateProjectCommandHandler *
(* +
IProjectRepository+ =
projectRepository> O
)O P
{ 	
_projectRepository 
=  
projectRepository! 2
;2 3
} 	
public 
async 
Task 
< 
int 
> 
Handle %
(% & 
CreateProjectCommand& :
request; B
,B C
CancellationTokenD U
cancellationTokenV g
)g h
{ 	
var   
project   
=   
new   
Project   %
(  % &
request!! 
.!! 
Name!! 
,!! 
request"" 
."" 
Description"" #
,""# $
request## 
.## 
	StartDate## !
,##! "
request$$ 
.$$ 
EndDate$$ 
)$$  
;$$  !
await&& 
_projectRepository&& $
.&&$ %
AddAsync&&% -
(&&- .
project&&. 5
,&&5 6
cancellationToken&&7 H
)&&H I
;&&I J
return(( 
project(( 
.(( 
Id(( 
;(( 
})) 	
}** 
}++ ∂
Ü/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.Application/DependencyInjection.cs
	namespace 	

TeamTasker
 
. 
Application  
{ 
public 

static 
class 
DependencyInjection +
{ 
public 
static 
IServiceCollection (
AddApplication) 7
(7 8
this8 <
IServiceCollection= O
servicesP X
)X Y
{ 	
services 
. 

AddMediatR 
(  
cfg  #
=>$ &
cfg' *
.* +(
RegisterServicesFromAssembly+ G
(G H
AssemblyH P
.P Q 
GetExecutingAssemblyQ e
(e f
)f g
)g h
)h i
;i j
services 
. %
AddValidatorsFromAssembly .
(. /
Assembly/ 7
.7 8 
GetExecutingAssembly8 L
(L M
)M N
)N O
;O P
services 
. 
AddTransient !
(! "
typeof" (
(( )
IPipelineBehavior) :
<: ;
,; <
>< =
)= >
,> ?
typeof@ F
(F G
ValidationBehaviorG Y
<Y Z
,Z [
>[ \
)\ ]
)] ^
;^ _
services 
. 
AddTransient !
(! "
typeof" (
(( )
IPipelineBehavior) :
<: ;
,; <
>< =
)= >
,> ?
typeof@ F
(F G
LoggingBehaviorG V
<V W
,W X
>X Y
)Y Z
)Z [
;[ \
return 
services 
; 
} 	
} 
} ‹
ë/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.Application/Common/Models/PaginationParams.cs
	namespace 	

TeamTasker
 
. 
Application  
.  !
Common! '
.' (
Models( .
{ 
public 

class 
PaginationParams !
{ 
private 
const 
int 
MaxPageSize %
=& '
$num( *
;* +
private		 
int		 
	_pageSize		 
=		 
$num		  "
;		" #
private

 
int

 
_pageNumber

 
=

  !
$num

" #
;

# $
public 
int 

PageNumber 
{ 	
get 
=> 
_pageNumber 
; 
set 
=> 
_pageNumber 
=  
value! &
<' (
$num) *
?+ ,
$num- .
:/ 0
value1 6
;6 7
} 	
public 
int 
PageSize 
{ 	
get 
=> 
	_pageSize 
; 
set 
=> 
	_pageSize 
= 
value $
>% &
MaxPageSize' 2
?3 4
MaxPageSize5 @
:A B
valueC H
<I J
$numK L
?M N
$numO P
:Q R
valueS X
;X Y
} 	
} 
} Ä(
é/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.Application/Common/Models/PaginatedList.cs
	namespace 	

TeamTasker
 
. 
Application  
.  !
Common! '
.' (
Models( .
{		 
public 

class 
PaginatedList 
< 
T  
>  !
{ 
public 
List 
< 
T 
> 
Items 
{ 
get "
;" #
}$ %
public 
int 

PageNumber 
{ 
get  #
;# $
}% &
public 
int 
PageSize 
{ 
get !
;! "
}# $
public"" 
int"" 

TotalCount"" 
{"" 
get""  #
;""# $
}""% &
public'' 
int'' 

TotalPages'' 
{'' 
get''  #
;''# $
}''% &
public,, 
bool,, 
HasPreviousPage,, #
=>,,$ &

PageNumber,,' 1
>,,2 3
$num,,4 5
;,,5 6
public11 
bool11 
HasNextPage11 
=>11  "

PageNumber11# -
<11. /

TotalPages110 :
;11: ;
public33 
PaginatedList33 
(33 
List33 !
<33! "
T33" #
>33# $
items33% *
,33* +
int33, /
count330 5
,335 6
int337 :

pageNumber33; E
,33E F
int33G J
pageSize33K S
)33S T
{44 	

PageNumber55 
=55 

pageNumber55 #
;55# $
PageSize66 
=66 
pageSize66 
;66  

TotalCount77 
=77 
count77 
;77 

TotalPages88 
=88 
(88 
int88 
)88 
Math88 "
.88" #
Ceiling88# *
(88* +
count88+ 0
/881 2
(883 4
double884 :
)88: ;
pageSize88; C
)88C D
;88D E
Items99 
=99 
items99 
;99 
}:: 	
public?? 
static?? 
async?? 
Task??  
<??  !
PaginatedList??! .
<??. /
T??/ 0
>??0 1
>??1 2
CreateAsync??3 >
(??> ?

IQueryable@@ 
<@@ 
T@@ 
>@@ 
source@@  
,@@  !
intAA 

pageNumberAA 
,AA 
intBB 
pageSizeBB 
,BB 
CancellationTokenCC 
cancellationTokenCC /
=CC0 1
defaultCC2 9
)CC9 :
{DD 	
varEE 
countEE 
=EE 
awaitEE 
sourceEE $
.EE$ %

CountAsyncEE% /
(EE/ 0
cancellationTokenEE0 A
)EEA B
;EEB C
varGG 
itemsGG 
=GG 
awaitGG 
sourceGG $
.HH 
SkipHH 
(HH 
(HH 

pageNumberHH !
-HH" #
$numHH$ %
)HH% &
*HH' (
pageSizeHH) 1
)HH1 2
.II 
TakeII 
(II 
pageSizeII 
)II 
.JJ 
ToListAsyncJJ 
(JJ 
cancellationTokenJJ .
)JJ. /
;JJ/ 0
returnLL 
newLL 
PaginatedListLL $
<LL$ %
TLL% &
>LL& '
(LL' (
itemsLL( -
,LL- .
countLL/ 4
,LL4 5

pageNumberLL6 @
,LL@ A
pageSizeLLB J
)LLJ K
;LLK L
}MM 	
publicRR 
staticRR 
PaginatedListRR #
<RR# $
TRR$ %
>RR% &
CreateRR' -
(RR- .
ListSS 
<SS 
TSS 
>SS 
sourceSS 
,SS 
intTT 

pageNumberTT 
,TT 
intUU 
pageSizeUU 
)UU 
{VV 	
varWW 
countWW 
=WW 
sourceWW 
.WW 
CountWW $
;WW$ %
varYY 
itemsYY 
=YY 
sourceYY 
.ZZ 
SkipZZ 
(ZZ 
(ZZ 

pageNumberZZ !
-ZZ" #
$numZZ$ %
)ZZ% &
*ZZ' (
pageSizeZZ) 1
)ZZ1 2
.[[ 
Take[[ 
([[ 
pageSize[[ 
)[[ 
.\\ 
ToList\\ 
(\\ 
)\\ 
;\\ 
return^^ 
new^^ 
PaginatedList^^ $
<^^$ %
T^^% &
>^^& '
(^^' (
items^^( -
,^^- .
count^^/ 4
,^^4 5

pageNumber^^6 @
,^^@ A
pageSize^^B J
)^^J K
;^^K L
}__ 	
}`` 
}aa Ö
ò/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.Application/Common/Interfaces/ICurrentUserService.cs
	namespace 	

TeamTasker
 
. 
Application  
.  !
Common! '
.' (

Interfaces( 2
{ 
public 

	interface 
ICurrentUserService (
{ 
int 
? 
UserId 
{ 
get 
; 
} 
string		 
Username		 
{		 
get		 
;		 
}		  
bool

 
IsAuthenticated

 
{

 
get

 "
;

" #
}

$ %
} 
} ñ	
ö/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.Application/Common/Interfaces/IApplicationDbContext.cs
	namespace 	

TeamTasker
 
. 
Application  
.  !
Common! '
.' (

Interfaces( 2
{ 
public 

	interface !
IApplicationDbContext *
{ 
DbSet 
< 
Project 
> 
Projects 
{  !
get" %
;% &
}' (
DbSet 
< 
Domain 
. 
Entities 
. 
Task "
>" #
Tasks$ )
{* +
get, /
;/ 0
}1 2
DbSet 
< 
User 
> 
Users 
{ 
get 
;  
}! "
Task 
< 
int 
> 
SaveChangesAsync "
(" #
CancellationToken# 4
cancellationToken5 F
)F G
;G H
} 
} £
ò/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.Application/Common/Exceptions/ValidationException.cs
	namespace 	

TeamTasker
 
. 
Application  
.  !
Common! '
.' (

Exceptions( 2
{ 
public 

class 
ValidationException $
:% &
	Exception' 0
{ 
public 
ValidationException "
(" #
)# $
: 
base 
( 
$str C
)C D
{ 	
Errors 
= 
new 

Dictionary #
<# $
string$ *
,* +
string, 2
[2 3
]3 4
>4 5
(5 6
)6 7
;7 8
} 	
public 
ValidationException "
(" #
IEnumerable# .
<. /
ValidationFailure/ @
>@ A
failuresB J
)J K
: 
this 
( 
) 
{ 	
Errors 
= 
failures 
. 
GroupBy 
( 
e 
=> 
e 
.  
PropertyName  ,
,, -
e. /
=>0 2
e3 4
.4 5
ErrorMessage5 A
)A B
. 
ToDictionary 
( 
failureGroup *
=>+ -
failureGroup. :
.: ;
Key; >
,> ?
failureGroup@ L
=>M O
failureGroupP \
.\ ]
ToArray] d
(d e
)e f
)f g
;g h
} 	
public 
IDictionary 
< 
string !
,! "
string# )
[) *
]* +
>+ ,
Errors- 3
{4 5
get6 9
;9 :
}; <
} 
} ã
ñ/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.Application/Common/Exceptions/NotFoundException.cs
	namespace 	

TeamTasker
 
. 
Application  
.  !
Common! '
.' (

Exceptions( 2
{ 
public 

class 
NotFoundException "
:# $
	Exception% .
{		 
public

 
NotFoundException

  
(

  !
)

! "
: 
base 
( 
) 
{ 	
} 	
public 
NotFoundException  
(  !
string! '
message( /
)/ 0
: 
base 
( 
message 
) 
{ 	
} 	
public 
NotFoundException  
(  !
string! '
message( /
,/ 0
	Exception1 :
innerException; I
)I J
: 
base 
( 
message 
, 
innerException *
)* +
{ 	
} 	
public 
NotFoundException  
(  !
string! '
name( ,
,, -
object. 4
key5 8
)8 9
: 
base 
( 
$" 
$str 
{ 
name #
}# $
$str$ (
{( )
key) ,
}, -
$str- =
"= >
)> ?
{ 	
} 	
} 
} Ñ

ù/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.Application/Common/Exceptions/ForbiddenAccessException.cs
	namespace 	

TeamTasker
 
. 
Application  
.  !
Common! '
.' (

Exceptions( 2
{ 
public 

class $
ForbiddenAccessException )
:* +
	Exception, 5
{		 
public

 $
ForbiddenAccessException

 '
(

' (
)

( )
:

* +
base

, 0
(

0 1
$str

1 f
)

f g
{ 	
} 	
public $
ForbiddenAccessException '
(' (
string( .
message/ 6
)6 7
:8 9
base: >
(> ?
message? F
)F G
{ 	
} 	
public $
ForbiddenAccessException '
(' (
string( .
message/ 6
,6 7
	Exception8 A
innerExceptionB P
)P Q
:R S
baseT X
(X Y
messageY `
,` a
innerExceptionb p
)p q
{ 	
} 	
} 
} ù
ñ/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.Application/Common/Exceptions/ConflictException.cs
	namespace 	

TeamTasker
 
. 
Application  
.  !
Common! '
.' (

Exceptions( 2
{ 
public 

class 
ConflictException "
:# $
	Exception% .
{		 
public

 
ConflictException

  
(

  !
)

! "
:

# $
base

% )
(

) *
$str

* @
)

@ A
{ 	
} 	
public 
ConflictException  
(  !
string! '
message( /
)/ 0
:1 2
base3 7
(7 8
message8 ?
)? @
{ 	
} 	
public 
ConflictException  
(  !
string! '
message( /
,/ 0
	Exception1 :
innerException; I
)I J
:K L
baseM Q
(Q R
messageR Y
,Y Z
innerException[ i
)i j
{ 	
} 	
public 
ConflictException  
(  !
string! '
name( ,
,, -
object. 4
key5 8
)8 9
:: ;
base< @
(@ A
$"A C
$strC L
{L M
nameM Q
}Q R
$strR V
{V W
keyW Z
}Z [
$str[ l
"l m
)m n
{ 	
} 	
} 
} Î	
ò/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.Application/Common/Exceptions/BadRequestException.cs
	namespace 	

TeamTasker
 
. 
Application  
.  !
Common! '
.' (

Exceptions( 2
{ 
public 

class 
BadRequestException $
:% &
	Exception' 0
{		 
public

 
BadRequestException

 "
(

" #
)

# $
:

% &
base

' +
(

+ ,
$str

, E
)

E F
{ 	
} 	
public 
BadRequestException "
(" #
string# )
message* 1
)1 2
:3 4
base5 9
(9 :
message: A
)A B
{ 	
} 	
public 
BadRequestException "
(" #
string# )
message* 1
,1 2
	Exception3 <
innerException= K
)K L
:M N
baseO S
(S T
messageT [
,[ \
innerException] k
)k l
{ 	
} 	
} 
} £
ñ/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.Application/Common/Behaviors/ValidationBehavior.cs
	namespace 	

TeamTasker
 
. 
Application  
.  !
Common! '
.' (
	Behaviors( 1
{		 
public 

class 
ValidationBehavior #
<# $
TRequest$ ,
,, -
	TResponse. 7
>7 8
:9 :
IPipelineBehavior; L
<L M
TRequestM U
,U V
	TResponseW `
>` a
where 
TRequest 
: 
IRequest !
<! "
	TResponse" +
>+ ,
{ 
private 
readonly 
IEnumerable $
<$ %

IValidator% /
</ 0
TRequest0 8
>8 9
>9 :
_validators; F
;F G
public 
ValidationBehavior !
(! "
IEnumerable" -
<- .

IValidator. 8
<8 9
TRequest9 A
>A B
>B C

validatorsD N
)N O
{ 	
_validators 
= 

validators $
;$ %
} 	
public 
async 
Task 
< 
	TResponse #
># $
Handle% +
(+ ,
TRequest, 4
request5 <
,< ="
RequestHandlerDelegate> T
<T U
	TResponseU ^
>^ _
next` d
,d e
CancellationTokenf w
cancellationToken	x â
)
â ä
{ 	
if 
( 
_validators 
. 
Any 
(  
)  !
)! "
{ 
var 
context 
= 
new !
ValidationContext" 3
<3 4
TRequest4 <
>< =
(= >
request> E
)E F
;F G
var 
validationResults %
=& '
await( -
Task. 2
.2 3
WhenAll3 :
(: ;
_validators   
.    
Select    &
(  & '
v  ' (
=>  ) +
v  , -
.  - .
ValidateAsync  . ;
(  ; <
context  < C
,  C D
cancellationToken  E V
)  V W
)  W X
)  X Y
;  Y Z
var"" 
failures"" 
="" 
validationResults"" 0
.## 

SelectMany## 
(##  
r##  !
=>##" $
r##% &
.##& '
Errors##' -
)##- .
.$$ 
Where$$ 
($$ 
f$$ 
=>$$ 
f$$  !
!=$$" $
null$$% )
)$$) *
.%% 
ToList%% 
(%% 
)%% 
;%% 
if'' 
('' 
failures'' 
.'' 
Count'' "
!=''# %
$num''& '
)''' (
{(( 
throw)) 
new)) 
ValidationException)) 1
())1 2
failures))2 :
))): ;
;)); <
}** 
}++ 
return-- 
await-- 
next-- 
(-- 
)-- 
;--  
}.. 	
}// 
}00 €$
ì/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.Application/Common/Behaviors/LoggingBehavior.cs
	namespace		 	

TeamTasker		
 
.		 
Application		  
.		  !
Common		! '
.		' (
	Behaviors		( 1
{

 
public 

class 
LoggingBehavior  
<  !
TRequest! )
,) *
	TResponse+ 4
>4 5
:6 7
IPipelineBehavior8 I
<I J
TRequestJ R
,R S
	TResponseT ]
>] ^
where 
TRequest 
: 
IRequest !
<! "
	TResponse" +
>+ ,
{ 
private 
readonly 
ILogger  
<  !
LoggingBehavior! 0
<0 1
TRequest1 9
,9 :
	TResponse; D
>D E
>E F
_loggerG N
;N O
private 
readonly 
ICurrentUserService ,
_currentUserService- @
;@ A
public 
LoggingBehavior 
( 
ILogger &
<& '
LoggingBehavior' 6
<6 7
TRequest7 ?
,? @
	TResponseA J
>J K
>K L
loggerM S
,S T
ICurrentUserServiceU h
currentUserServicei {
){ |
{ 	
_logger 
= 
logger 
; 
_currentUserService 
=  !
currentUserService" 4
;4 5
} 	
public 
async 
Task 
< 
	TResponse #
># $
Handle% +
(+ ,
TRequest, 4
request5 <
,< ="
RequestHandlerDelegate> T
<T U
	TResponseU ^
>^ _
next` d
,d e
CancellationTokenf w
cancellationToken	x â
)
â ä
{ 	
var 
requestName 
= 
typeof $
($ %
TRequest% -
)- .
.. /
Name/ 3
;3 4
var 
userId 
= 
_currentUserService ,
., -
UserId- 3
??4 6
$num7 8
;8 9
var   
userName   
=   
_currentUserService   .
.  . /
Username  / 7
??  8 :
$str  ; F
;  F G
var!! 
	requestId!! 
=!! 
Guid!!  
.!!  !
NewGuid!!! (
(!!( )
)!!) *
.!!* +
ToString!!+ 3
(!!3 4
)!!4 5
;!!5 6
_logger## 
.## 
LogInformation## "
(##" #
$str### a
,##a b
	requestId$$ 
,$$ 
requestName$$ &
,$$& '
userId$$( .
,$$. /
userName$$0 8
)$$8 9
;$$9 :
var&& 
	stopwatch&& 
=&& 
	Stopwatch&& %
.&&% &
StartNew&&& .
(&&. /
)&&/ 0
;&&0 1
try(( 
{)) 
var** 
response** 
=** 
await** $
next**% )
(**) *
cancellationToken*** ;
)**; <
;**< =
	stopwatch,, 
.,, 
Stop,, 
(,, 
),,  
;,,  !
_logger.. 
... 
LogInformation.. &
(..& '
$str..' 
,	.. Ä
	requestId// 
,// 
requestName// *
,//* +
userId//, 2
,//2 3
userName//4 <
,//< =
	stopwatch//> G
.//G H
ElapsedMilliseconds//H [
)//[ \
;//\ ]
return11 
response11 
;11  
}22 
catch33 
(33 
	Exception33 
ex33 
)33  
{44 
	stopwatch55 
.55 
Stop55 
(55 
)55  
;55  !
_logger77 
.77 
LogError77  
(77  !
ex77! #
,77# $
$str	77% á
,
77á à
	requestId88 
,88 
requestName88 *
,88* +
userId88, 2
,882 3
userName884 <
,88< =
	stopwatch88> G
.88G H
ElapsedMilliseconds88H [
)88[ \
;88\ ]
throw:: 
;:: 
};; 
}<< 	
}== 
}>> ≤
y/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.Application/Class1.cs
	namespace 	

TeamTasker
 
. 
Application  
;  !
public 
class 
Class1 
{ 
} 