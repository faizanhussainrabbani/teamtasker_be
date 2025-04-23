ö
‚/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.Domain/ValueObjects/Address.cs
	namespace 	

TeamTasker
 
. 
Domain 
. 
ValueObjects (
{ 
public		 

class		 
Address		 
:		 
ValueObject		 &
{

 
public 
string 
Street 
{ 
get "
;" #
private$ +
set, /
;/ 0
}1 2
public 
string 
City 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
public 
string 
State 
{ 
get !
;! "
private# *
set+ .
;. /
}0 1
public 
string 
Country 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 
string 
ZipCode 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
private 
Address 
( 
) 
{ 
} 
public 
Address 
( 
string 
street $
,$ %
string& ,
city- 1
,1 2
string3 9
state: ?
,? @
stringA G
countryH O
,O P
stringQ W
zipCodeX _
)_ `
{ 	
Street 
= 
street 
; 
City 
= 
city 
; 
State 
= 
state 
; 
Country 
= 
country 
; 
ZipCode 
= 
zipCode 
; 
} 	
	protected 
override 
IEnumerable &
<& '
object' -
>- .!
GetEqualityComponents/ D
(D E
)E F
{ 	
yield 
return 
Street 
;  
yield 
return 
City 
; 
yield   
return   
State   
;   
yield!! 
return!! 
Country!!  
;!!  !
yield"" 
return"" 
ZipCode""  
;""  !
}## 	
}$$ 
}%% ¨
ˆ/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.Domain/Interfaces/IUserRepository.cs
	namespace 	

TeamTasker
 
. 
Domain 
. 

Interfaces &
{ 
public 

	interface 
IUserRepository $
:% &
IRepository' 2
<2 3
User3 7
>7 8
{ 
Task 
< 
User 
> 
GetByEmailAsync "
(" #
string# )
email* /
,/ 0
CancellationToken1 B
cancellationTokenC T
=U V
defaultW ^
)^ _
;_ `
Task 
< 
User 
> 
GetByUsernameAsync %
(% &
string& ,
username- 5
,5 6
CancellationToken7 H
cancellationTokenI Z
=[ \
default] d
)d e
;e f
} 
} Ž

ˆ/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.Domain/Interfaces/ITaskRepository.cs
	namespace 	

TeamTasker
 
. 
Domain 
. 

Interfaces &
{ 
public 

	interface 
ITaskRepository $
:% &
IRepository' 2
<2 3
Entities3 ;
.; <
Task< @
>@ A
{ 
Task 
< 
IEnumerable 
< 
Entities !
.! "
Task" &
>& '
>' (!
GetTasksByUserIdAsync) >
(> ?
int? B
userIdC I
,I J
CancellationTokenK \
cancellationToken] n
=o p
defaultq x
)x y
;y z
Task 
< 
IEnumerable 
< 
Entities !
.! "
Task" &
>& '
>' ($
GetTasksByProjectIdAsync) A
(A B
intB E
	projectIdF O
,O P
CancellationTokenQ b
cancellationTokenc t
=u v
defaultw ~
)~ 
;	 €
} 
} ù
‹/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.Domain/Interfaces/IProjectRepository.cs
	namespace 	

TeamTasker
 
. 
Domain 
. 

Interfaces &
{ 
public 

	interface 
IProjectRepository '
:( )
IRepository* 5
<5 6
Project6 =
>= >
{ 
Task 
< 
IEnumerable 
< 
Project  
>  !
>! "$
GetProjectsByUserIdAsync# ;
(; <
int< ?
userId@ F
,F G
CancellationTokenH Y
cancellationTokenZ k
=l m
defaultn u
)u v
;v w
Task 
< 
Project 
> $
GetProjectWithTasksAsync .
(. /
int/ 2
	projectId3 <
,< =
CancellationToken> O
cancellationTokenP a
=b c
defaultd k
)k l
;l m
} 
} Û
/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.Domain/Events/UserEvents.cs
	namespace 	

TeamTasker
 
. 
Domain 
. 
Events "
{ 
public 

class 
UserCreatedEvent !
:" #
INotification$ 1
{ 
public 
User 
User 
{ 
get 
; 
}  !
public

 
UserCreatedEvent

 
(

  
User

  $
user

% )
)

) *
{ 	
User 
= 
user 
; 
} 	
} 
public 

class 
UserUpdatedEvent !
:" #
INotification$ 1
{ 
public 
User 
User 
{ 
get 
; 
}  !
public 
UserUpdatedEvent 
(  
User  $
user% )
)) *
{ 	
User 
= 
user 
; 
} 	
} 
public 

class #
UserAddressUpdatedEvent (
:) *
INotification+ 8
{ 
public 
User 
User 
{ 
get 
; 
}  !
public #
UserAddressUpdatedEvent &
(& '
User' +
user, 0
)0 1
{ 	
User   
=   
user   
;   
}!! 	
}"" 
public$$ 

class$$  
UserDeactivatedEvent$$ %
:$$& '
INotification$$( 5
{%% 
public&& 
User&& 
User&& 
{&& 
get&& 
;&& 
}&&  !
public((  
UserDeactivatedEvent(( #
(((# $
User(($ (
user(() -
)((- .
{)) 	
User** 
=** 
user** 
;** 
}++ 	
},, 
public.. 

class.. 
UserActivatedEvent.. #
:..$ %
INotification..& 3
{// 
public00 
User00 
User00 
{00 
get00 
;00 
}00  !
public22 
UserActivatedEvent22 !
(22! "
User22" &
user22' +
)22+ ,
{33 	
User44 
=44 
user44 
;44 
}55 	
}66 
}77 ” 
/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.Domain/Events/TaskEvents.cs
	namespace 	

TeamTasker
 
. 
Domain 
. 
Events "
{ 
public 

class 
TaskCreatedEvent !
:" #
INotification$ 1
{ 
public 
Entities 
. 
Task 
TaskItem %
{& '
get( +
;+ ,
}- .
public

 
TaskCreatedEvent

 
(

  
Entities

  (
.

( )
Task

) -
task

. 2
)

2 3
{ 	
TaskItem 
= 
task 
; 
} 	
} 
public 

class 
TaskUpdatedEvent !
:" #
INotification$ 1
{ 
public 
Entities 
. 
Task 
TaskItem %
{& '
get( +
;+ ,
}- .
public 
TaskUpdatedEvent 
(  
Entities  (
.( )
Task) -
task. 2
)2 3
{ 	
TaskItem 
= 
task 
; 
} 	
} 
public 

class "
TaskStatusUpdatedEvent '
:( )
INotification* 7
{ 
public 
Entities 
. 
Task 
TaskItem %
{& '
get( +
;+ ,
}- .
public "
TaskStatusUpdatedEvent %
(% &
Entities& .
.. /
Task/ 3
task4 8
)8 9
{ 	
TaskItem   
=   
task   
;   
}!! 	
}"" 
public$$ 

class$$ 
TaskAssignedEvent$$ "
:$$# $
INotification$$% 2
{%% 
public&& 
Entities&& 
.&& 
Task&& 
TaskItem&& %
{&&& '
get&&( +
;&&+ ,
}&&- .
public'' 
int'' 
UserId'' 
{'' 
get'' 
;''  
}''! "
public)) 
TaskAssignedEvent))  
())  !
Entities))! )
.))) *
Task))* .
task))/ 3
,))3 4
int))5 8
userId))9 ?
)))? @
{** 	
TaskItem++ 
=++ 
task++ 
;++ 
UserId,, 
=,, 
userId,, 
;,, 
}-- 	
}.. 
public00 

class00 
TaskUnassignedEvent00 $
:00% &
INotification00' 4
{11 
public22 
Entities22 
.22 
Task22 
TaskItem22 %
{22& '
get22( +
;22+ ,
}22- .
public44 
TaskUnassignedEvent44 "
(44" #
Entities44# +
.44+ ,
Task44, 0
task441 5
)445 6
{55 	
TaskItem66 
=66 
task66 
;66 
}77 	
}88 
public:: 

class:: #
TaskAddedToProjectEvent:: (
:::) *
INotification::+ 8
{;; 
public<< 
Entities<< 
.<< 
Task<< 
TaskItem<< %
{<<& '
get<<( +
;<<+ ,
}<<- .
public== 
Project== 
Project== 
{==  
get==! $
;==$ %
}==& '
public?? #
TaskAddedToProjectEvent?? &
(??& '
Entities??' /
.??/ 0
Task??0 4
task??5 9
,??9 :
Project??; B
project??C J
)??J K
{@@ 	
TaskItemAA 
=AA 
taskAA 
;AA 
ProjectBB 
=BB 
projectBB 
;BB 
}CC 	
}DD 
}EE ë
‚/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.Domain/Events/ProjectEvents.cs
	namespace 	

TeamTasker
 
. 
Domain 
. 
Events "
{ 
public 

class 
ProjectCreatedEvent $
:% &
INotification' 4
{ 
public 
Project 
Project 
{  
get! $
;$ %
}& '
public

 
ProjectCreatedEvent

 "
(

" #
Project

# *
project

+ 2
)

2 3
{ 	
Project 
= 
project 
; 
} 	
} 
public 

class 
ProjectUpdatedEvent $
:% &
INotification' 4
{ 
public 
Project 
Project 
{  
get! $
;$ %
}& '
public 
ProjectUpdatedEvent "
(" #
Project# *
project+ 2
)2 3
{ 	
Project 
= 
project 
; 
} 	
} 
public 

class %
ProjectStatusUpdatedEvent *
:+ ,
INotification- :
{ 
public 
Project 
Project 
{  
get! $
;$ %
}& '
public %
ProjectStatusUpdatedEvent (
(( )
Project) 0
project1 8
)8 9
{ 	
Project   
=   
project   
;   
}!! 	
}"" 
}## Ž'
{/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.Domain/Entities/User.cs
	namespace 	

TeamTasker
 
. 
Domain 
. 
Entities $
{ 
public 

class 
User 
: 

BaseEntity "
{ 
private 
User 
( 
) 
{ 
} 
public 
User 
( 
string 
	firstName $
,$ %
string& ,
lastName- 5
,5 6
string7 =
email> C
,C D
stringE K
usernameL T
)T U
{ 	
	FirstName 
= 
	firstName !
;! "
LastName 
= 
lastName 
;  
Email 
= 
email 
; 
Username 
= 
username 
;  
Status 
= 

UserStatus 
.  
Active  &
;& '
CreatedDate 
= 
DateTime "
." #
UtcNow# )
;) *
AddDomainEvent 
( 
new 
UserCreatedEvent /
(/ 0
this0 4
)4 5
)5 6
;6 7
} 	
public 
string 
	FirstName 
{  !
get" %
;% &
private' .
set/ 2
;2 3
}4 5
public 
string 
LastName 
{  
get! $
;$ %
private& -
set. 1
;1 2
}3 4
public 
string 
Email 
{ 
get !
;! "
private# *
set+ .
;. /
}0 1
public 
string 
Username 
{  
get! $
;$ %
private& -
set. 1
;1 2
}3 4
public 

UserStatus 
Status  
{! "
get# &
;& '
private( /
set0 3
;3 4
}5 6
public   
DateTime   
CreatedDate   #
{  $ %
get  & )
;  ) *
private  + 2
set  3 6
;  6 7
}  8 9
public!! 
Address!! 
Address!! 
{!!  
get!!! $
;!!$ %
private!!& -
set!!. 1
;!!1 2
}!!3 4
public## 
string## 
FullName## 
=>## !
$"##" $
{##$ %
	FirstName##% .
}##. /
$str##/ 0
{##0 1
LastName##1 9
}##9 :
"##: ;
;##; <
public%% 
void%% 
UpdateProfile%% !
(%%! "
string%%" (
	firstName%%) 2
,%%2 3
string%%4 :
lastName%%; C
,%%C D
string%%E K
email%%L Q
)%%Q R
{&& 	
	FirstName'' 
='' 
	firstName'' !
;''! "
LastName(( 
=(( 
lastName(( 
;((  
Email)) 
=)) 
email)) 
;)) 
AddDomainEvent++ 
(++ 
new++ 
UserUpdatedEvent++ /
(++/ 0
this++0 4
)++4 5
)++5 6
;++6 7
},, 	
public.. 
void.. 
UpdateAddress.. !
(..! "
Address.." )
address..* 1
)..1 2
{// 	
Address00 
=00 
address00 
;00 
AddDomainEvent22 
(22 
new22 #
UserAddressUpdatedEvent22 6
(226 7
this227 ;
)22; <
)22< =
;22= >
}33 	
public55 
void55 

Deactivate55 
(55 
)55  
{66 	
Status77 
=77 

UserStatus77 
.77  
Inactive77  (
;77( )
AddDomainEvent99 
(99 
new99  
UserDeactivatedEvent99 3
(993 4
this994 8
)998 9
)999 :
;99: ;
}:: 	
public<< 
void<< 
Activate<< 
(<< 
)<< 
{== 	
Status>> 
=>> 

UserStatus>> 
.>>  
Active>>  &
;>>& '
AddDomainEvent@@ 
(@@ 
new@@ 
UserActivatedEvent@@ 1
(@@1 2
this@@2 6
)@@6 7
)@@7 8
;@@8 9
}AA 	
}BB 
publicDD 

enumDD 

UserStatusDD 
{EE 
ActiveFF 
,FF 
InactiveGG 
}HH 
}II ù/
{/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.Domain/Entities/Task.cs
	namespace 	

TeamTasker
 
. 
Domain 
. 
Entities $
{ 
public

 

class

 
Task

 
:

 

BaseEntity

 "
{ 
private 
Task 
( 
) 
{ 
} 
public 
Task 
( 
string 
title  
,  !
string" (
description) 4
,4 5
DateTime6 >
dueDate? F
,F G
TaskPriorityH T
priorityU ]
,] ^
int_ b
	projectIdc l
)l m
{ 	
Title 
= 
title 
; 
Description 
= 
description %
;% &
DueDate 
= 
dueDate 
; 
Priority 
= 
priority 
;  
Status 
= 

TaskStatus 
.  
ToDo  $
;$ %
	ProjectId 
= 
	projectId !
;! "
CreatedDate 
= 
DateTime "
." #
UtcNow# )
;) *
AddDomainEvent 
( 
new 
TaskCreatedEvent /
(/ 0
this0 4
)4 5
)5 6
;6 7
} 	
public 
string 
Title 
{ 
get !
;! "
private# *
set+ .
;. /
}0 1
public 
string 
Description !
{" #
get$ '
;' (
private) 0
set1 4
;4 5
}6 7
public 
DateTime 
DueDate 
{  !
get" %
;% &
private' .
set/ 2
;2 3
}4 5
public 
TaskPriority 
Priority $
{% &
get' *
;* +
private, 3
set4 7
;7 8
}9 :
public 

TaskStatus 
Status  
{! "
get# &
;& '
private( /
set0 3
;3 4
}5 6
public   
int   
	ProjectId   
{   
get   "
;  " #
private  $ +
set  , /
;  / 0
}  1 2
public!! 
int!! 
?!! 
AssignedToUserId!! $
{!!% &
get!!' *
;!!* +
private!!, 3
set!!4 7
;!!7 8
}!!9 :
public"" 
DateTime"" 
CreatedDate"" #
{""$ %
get""& )
;"") *
private""+ 2
set""3 6
;""6 7
}""8 9
public## 
DateTime## 
?## 
CompletedDate## &
{##' (
get##) ,
;##, -
private##. 5
set##6 9
;##9 :
}##; <
public%% 
void%% 
UpdateDetails%% !
(%%! "
string%%" (
title%%) .
,%%. /
string%%0 6
description%%7 B
,%%B C
DateTime%%D L
dueDate%%M T
,%%T U
TaskPriority%%V b
priority%%c k
)%%k l
{&& 	
Title'' 
='' 
title'' 
;'' 
Description(( 
=(( 
description(( %
;((% &
DueDate)) 
=)) 
dueDate)) 
;)) 
Priority** 
=** 
priority** 
;**  
AddDomainEvent,, 
(,, 
new,, 
TaskUpdatedEvent,, /
(,,/ 0
this,,0 4
),,4 5
),,5 6
;,,6 7
}-- 	
public// 
void// 
UpdateStatus//  
(//  !

TaskStatus//! +
status//, 2
)//2 3
{00 	
if11 
(11 
status11 
==11 

TaskStatus11 $
.11$ %
Done11% )
&&11* ,
Status11- 3
!=114 6

TaskStatus117 A
.11A B
Done11B F
)11F G
{22 
CompletedDate33 
=33 
DateTime33  (
.33( )
UtcNow33) /
;33/ 0
}44 
Status66 
=66 
status66 
;66 
AddDomainEvent88 
(88 
new88 "
TaskStatusUpdatedEvent88 5
(885 6
this886 :
)88: ;
)88; <
;88< =
}99 	
public;; 
void;; 
AssignToUser;;  
(;;  !
int;;! $
userId;;% +
);;+ ,
{<< 	
AssignedToUserId== 
=== 
userId== %
;==% &
AddDomainEvent?? 
(?? 
new?? 
TaskAssignedEvent?? 0
(??0 1
this??1 5
,??5 6
userId??7 =
)??= >
)??> ?
;??? @
}@@ 	
publicBB 
voidBB 
RemoveAssignmentBB $
(BB$ %
)BB% &
{CC 	
AssignedToUserIdDD 
=DD 
nullDD #
;DD# $
AddDomainEventFF 
(FF 
newFF 
TaskUnassignedEventFF 2
(FF2 3
thisFF3 7
)FF7 8
)FF8 9
;FF9 :
}GG 	
}HH 
publicJJ 

enumJJ 

TaskStatusJJ 
{KK 
ToDoLL 
,LL 

InProgressMM 
,MM 
DoneNN 
,NN 
BlockedOO 
}PP 
publicRR 

enumRR 
TaskPriorityRR 
{SS 
LowTT 
,TT 
MediumUU 
,UU 
HighVV 
,VV 
CriticalWW 
}XX 
}YY ™*
~/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.Domain/Entities/Project.cs
	namespace 	

TeamTasker
 
. 
Domain 
. 
Entities $
{ 
public 

class 
Project 
: 

BaseEntity %
{ 
private 
Project 
( 
) 
{ 
} 
public 
Project 
( 
string 
name "
," #
string$ *
description+ 6
,6 7
DateTime8 @
	startDateA J
,J K
DateTimeL T
?T U
endDateV ]
)] ^
{ 	
Name 
= 
name 
; 
Description 
= 
description %
;% &
	StartDate 
= 
	startDate !
;! "
EndDate 
= 
endDate 
; 
Status 
= 
ProjectStatus "
." #

NotStarted# -
;- .
Tasks 
= 
new 
List 
< 
Entities %
.% &
Task& *
>* +
(+ ,
), -
;- .
AddDomainEvent 
( 
new 
ProjectCreatedEvent 2
(2 3
this3 7
)7 8
)8 9
;9 :
} 	
public 
string 
Name 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
public 
string 
Description !
{" #
get$ '
;' (
private) 0
set1 4
;4 5
}6 7
public 
DateTime 
	StartDate !
{" #
get$ '
;' (
private) 0
set1 4
;4 5
}6 7
public 
DateTime 
? 
EndDate  
{! "
get# &
;& '
private( /
set0 3
;3 4
}5 6
public 
ProjectStatus 
Status #
{$ %
get& )
;) *
private+ 2
set3 6
;6 7
}8 9
public   
List   
<   
Entities   
.   
Task   !
>  ! "
Tasks  # (
{  ) *
get  + .
;  . /
private  0 7
set  8 ;
;  ; <
}  = >
public"" 
void"" 
UpdateDetails"" !
(""! "
string""" (
name"") -
,""- .
string""/ 5
description""6 A
,""A B
DateTime""C K
	startDate""L U
,""U V
DateTime""W _
?""_ `
endDate""a h
)""h i
{## 	
Name$$ 
=$$ 
name$$ 
;$$ 
Description%% 
=%% 
description%% %
;%%% &
	StartDate&& 
=&& 
	startDate&& !
;&&! "
EndDate'' 
='' 
endDate'' 
;'' 
AddDomainEvent)) 
()) 
new)) 
ProjectUpdatedEvent)) 2
())2 3
this))3 7
)))7 8
)))8 9
;))9 :
}** 	
public,, 
void,, 
UpdateStatus,,  
(,,  !
ProjectStatus,,! .
status,,/ 5
),,5 6
{-- 	
Status.. 
=.. 
status.. 
;.. 
AddDomainEvent00 
(00 
new00 %
ProjectStatusUpdatedEvent00 8
(008 9
this009 =
)00= >
)00> ?
;00? @
}11 	
public33 
Entities33 
.33 
Task33 
AddTask33 $
(33$ %
string33% +
title33, 1
,331 2
string333 9
description33: E
,33E F
DateTime33G O
dueDate33P W
,33W X
TaskPriority33Y e
priority33f n
)33n o
{44 	
var55 
task55 
=55 
new55 
Entities55 #
.55# $
Task55$ (
(55( )
title55) .
,55. /
description550 ;
,55; <
dueDate55= D
,55D E
priority55F N
,55N O
this55P T
.55T U
Id55U W
)55W X
;55X Y
Tasks66 
.66 
Add66 
(66 
task66 
)66 
;66 
AddDomainEvent88 
(88 
new88 #
TaskAddedToProjectEvent88 6
(886 7
task887 ;
,88; <
this88= A
)88A B
)88B C
;88C D
return:: 
task:: 
;:: 
};; 	
}<< 
public>> 

enum>> 
ProjectStatus>> 
{?? 

NotStarted@@ 
,@@ 

InProgressAA 
,AA 
	CompletedBB 
,BB 
OnHoldCC 
,CC 
	CancelledDD 
}EE 
}FF ¨
t/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Core/TeamTasker.Domain/Class1.cs
	namespace 	

TeamTasker
 
. 
Domain 
; 
public 
class 
Class1 
{ 
} 