∑+
z/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Presentation/TeamTasker.API/Program.cs
var 
builder 
= 
WebApplication 
. 
CreateBuilder *
(* +
args+ /
)/ 0
;0 1
builder 
. 
Logging 
. 
ClearProviders 
( 
)  
;  !
builder 
. 
Logging 
. 

AddConsole 
( 
) 
; 
builder 
. 
Logging 
. 
AddDebug 
( 
) 
; 
builder 
. 
Logging 
. 
SetMinimumLevel 
(  
LogLevel  (
.( )
Information) 4
)4 5
;5 6
builder 
. 
Services 
. 
AddApplication 
(  
)  !
;! "
builder 
. 
Services 
. 
AddInfrastructure "
(" #
builder# *
.* +
Configuration+ 8
)8 9
;9 :
builder 
. 
Services 
. "
AddHttpContextAccessor '
(' (
)( )
;) *
builder 
. 
Services 
. 
AddControllers 
(  
options  '
=>( *
options 
. 
Filters 
. 
Add 
< '
ApiExceptionFilterAttribute 3
>3 4
(4 5
)5 6
)6 7
;7 8
builder"" 
."" 
Services"" 
."" #
AddEndpointsApiExplorer"" (
(""( )
)"") *
;""* +
builder## 
.## 
Services## 
.## 
AddSwaggerGen## 
(## 
c##  
=>##! #
{$$ 
c%% 
.%% 

SwaggerDoc%% 
(%% 
$str%% 
,%% 
new%% 
OpenApiInfo%% &
{&& 
Title'' 
='' 
$str''  
,''  !
Version(( 
=(( 
$str(( 
,(( 
Description)) 
=)) 
$str)) C
,))C D
Contact** 
=** 
new** 
OpenApiContact** $
{++ 	
Name,, 
=,, 
$str,, '
,,,' (
Email-- 
=-- 
$str-- ,
}.. 	
}// 
)// 
;// 
var22 
xmlFile22 
=22 
$"22 
{22 
Assembly22 
.22  
GetExecutingAssembly22 2
(222 3
)223 4
.224 5
GetName225 <
(22< =
)22= >
.22> ?
Name22? C
}22C D
$str22D H
"22H I
;22I J
var33 
xmlPath33 
=33 
Path33 
.33 
Combine33 
(33 

AppContext33 )
.33) *
BaseDirectory33* 7
,337 8
xmlFile339 @
)33@ A
;33A B
if44 
(44 
File44 
.44 
Exists44 
(44 
xmlPath44 
)44 
)44 
{55 
c66 	
.66	 

IncludeXmlComments66
 
(66 
xmlPath66 $
)66$ %
;66% &
}77 
}88 
)88 
;88 
builder;; 
.;; 
Services;; 
.;; 
AddCors;; 
(;; 
options;;  
=>;;! #
{<< 
options== 
.== 
	AddPolicy== 
(== 
$str==  
,==  !
builder==" )
=>==* ,
{>> 
builder?? 
.?? 
AllowAnyOrigin?? 
(?? 
)??  
.@@ 
AllowAnyMethod@@ 
(@@ 
)@@ 
.AA 
AllowAnyHeaderAA 
(AA 
)AA 
;AA 
}BB 
)BB 
;BB 
}CC 
)CC 
;CC 
varEE 
appEE 
=EE 	
builderEE
 
.EE 
BuildEE 
(EE 
)EE 
;EE 
ifHH 
(HH 
appHH 
.HH 
EnvironmentHH 
.HH 
IsDevelopmentHH !
(HH! "
)HH" #
)HH# $
{II 
appJJ 
.JJ 

UseSwaggerJJ 
(JJ 
)JJ 
;JJ 
appKK 
.KK 
UseSwaggerUIKK 
(KK 
)KK 
;KK 
}LL 
appNN 
.NN 
UseHttpsRedirectionNN 
(NN 
)NN 
;NN 
appPP 
.PP 
UseCorsPP 
(PP 
$strPP 
)PP 
;PP 
appSS 
.SS %
UseGlobalExceptionHandlerSS 
(SS 
)SS 
;SS  
appUU 
.UU 
UseAuthorizationUU 
(UU 
)UU 
;UU 
appWW 
.WW 
MapControllersWW 
(WW 
)WW 
;WW 
appYY 
.YY 
RunYY 
(YY 
)YY 	
;YY	 
ö2
ô/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Presentation/TeamTasker.API/Middleware/ExceptionHandlingMiddleware.cs
	namespace		 	

TeamTasker		
 
.		 
API		 
.		 

Middleware		 #
{

 
public 

class '
ExceptionHandlingMiddleware ,
{ 
private 
readonly 
RequestDelegate (
_next) .
;. /
private 
readonly 
ILogger  
<  !'
ExceptionHandlingMiddleware! <
>< =
_logger> E
;E F
public '
ExceptionHandlingMiddleware *
(* +
RequestDelegate+ :
next; ?
,? @
ILoggerA H
<H I'
ExceptionHandlingMiddlewareI d
>d e
loggerf l
)l m
{ 	
_next 
= 
next 
; 
_logger 
= 
logger 
; 
} 	
public 
async 
Task 
InvokeAsync %
(% &
HttpContext& 1
context2 9
)9 :
{ 	
try 
{ 
await 
_next 
( 
context #
)# $
;$ %
} 
catch 
( 
	Exception 
ex 
)  
{   
_logger!! 
.!! 
LogError!!  
(!!  !
ex!!! #
,!!# $
$str!!% f
)!!f g
;!!g h
await""  
HandleExceptionAsync"" *
(""* +
context""+ 2
,""2 3
ex""4 6
)""6 7
;""7 8
}## 
}$$ 	
private&& 
static&& 
async&& 
Task&& ! 
HandleExceptionAsync&&" 6
(&&6 7
HttpContext&&7 B
context&&C J
,&&J K
	Exception&&L U
	exception&&V _
)&&_ `
{'' 	
context(( 
.(( 
Response(( 
.(( 
ContentType(( (
=(() *
$str((+ =
;((= >
var** 

statusCode** 
=** 
GetStatusCode** *
(*** +
	exception**+ 4
)**4 5
;**5 6
var++ 
response++ 
=++ 
new++ 
{,, 
status-- 
=-- 

statusCode-- #
,--# $
title.. 
=.. 
GetTitle..  
(..  !
	exception..! *
)..* +
,..+ ,
detail// 
=// 
	exception// "
.//" #
Message//# *
,//* +
errors00 
=00 
	GetErrors00 "
(00" #
	exception00# ,
)00, -
}11 
;11 
context33 
.33 
Response33 
.33 

StatusCode33 '
=33( )

statusCode33* 4
;334 5
var55 
options55 
=55 
new55 !
JsonSerializerOptions55 3
{66  
PropertyNamingPolicy77 $
=77% &
JsonNamingPolicy77' 7
.777 8
	CamelCase778 A
}88 
;88 
var:: 
json:: 
=:: 
JsonSerializer:: %
.::% &
	Serialize::& /
(::/ 0
response::0 8
,::8 9
options::: A
)::A B
;::B C
await<< 
context<< 
.<< 
Response<< "
.<<" #

WriteAsync<<# -
(<<- .
json<<. 2
)<<2 3
;<<3 4
}== 	
private?? 
static?? 
int?? 
GetStatusCode?? (
(??( )
	Exception??) 2
	exception??3 <
)??< =
{@@ 	
returnAA 
	exceptionAA 
switchAA #
{BB 
ValidationExceptionCC #
=>CC$ &
(CC' (
intCC( +
)CC+ ,
HttpStatusCodeCC, :
.CC: ;

BadRequestCC; E
,CCE F
NotFoundExceptionDD !
=>DD" $
(DD% &
intDD& )
)DD) *
HttpStatusCodeDD* 8
.DD8 9
NotFoundDD9 A
,DDA B$
ForbiddenAccessExceptionEE (
=>EE) +
(EE, -
intEE- 0
)EE0 1
HttpStatusCodeEE1 ?
.EE? @
	ForbiddenEE@ I
,EEI J
BadRequestExceptionFF #
=>FF$ &
(FF' (
intFF( +
)FF+ ,
HttpStatusCodeFF, :
.FF: ;

BadRequestFF; E
,FFE F
ConflictExceptionGG !
=>GG" $
(GG% &
intGG& )
)GG) *
HttpStatusCodeGG* 8
.GG8 9
ConflictGG9 A
,GGA B
_HH 
=>HH 
(HH 
intHH 
)HH 
HttpStatusCodeHH (
.HH( )
InternalServerErrorHH) <
}II 
;II 
}JJ 	
privateLL 
staticLL 
stringLL 
GetTitleLL &
(LL& '
	ExceptionLL' 0
	exceptionLL1 :
)LL: ;
{MM 	
returnNN 
	exceptionNN 
switchNN #
{OO 
ValidationExceptionPP #
=>PP$ &
$strPP' 9
,PP9 :
NotFoundExceptionQQ !
=>QQ" $
$strQQ% 9
,QQ9 :$
ForbiddenAccessExceptionRR (
=>RR) +
$strRR, 7
,RR7 8
BadRequestExceptionSS #
=>SS$ &
$strSS' 4
,SS4 5
ConflictExceptionTT !
=>TT" $
$strTT% /
,TT/ 0
_UU 
=>UU 
$strUU #
}VV 
;VV 
}WW 	
privateYY 
staticYY 
objectYY 
	GetErrorsYY '
(YY' (
	ExceptionYY( 1
	exceptionYY2 ;
)YY; <
{ZZ 	
if[[ 
([[ 
	exception[[ 
is[[ 
ValidationException[[ 0
validationException[[1 D
)[[D E
{\\ 
return]] 
validationException]] *
.]]* +
Errors]]+ 1
;]]1 2
}^^ 
return`` 
null`` 
;`` 
}aa 	
}bb 
}cc íb
ñ/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Presentation/TeamTasker.API/Filters/ApiExceptionFilterAttribute.cs
	namespace		 	

TeamTasker		
 
.		 
API		 
.		 
Filters		  
{

 
public 

class '
ApiExceptionFilterAttribute ,
:- .$
ExceptionFilterAttribute/ G
{ 
private 
readonly 
IDictionary $
<$ %
Type% )
,) *
Action+ 1
<1 2
ExceptionContext2 B
>B C
>C D
_exceptionHandlersE W
;W X
private 
readonly 
ILogger  
<  !'
ApiExceptionFilterAttribute! <
>< =
_logger> E
;E F
public '
ApiExceptionFilterAttribute *
(* +
ILogger+ 2
<2 3'
ApiExceptionFilterAttribute3 N
>N O
loggerP V
=W X
nullY ]
)] ^
{ 	
_logger 
= 
logger 
; 
_exceptionHandlers 
=  
new! $

Dictionary% /
</ 0
Type0 4
,4 5
Action6 <
<< =
ExceptionContext= M
>M N
>N O
{ 
{ 
typeof 
( 
ValidationException ,
), -
,- .%
HandleValidationException/ H
}I J
,J K
{ 
typeof 
( 
NotFoundException *
)* +
,+ ,#
HandleNotFoundException- D
}E F
,F G
{ 
typeof 
( $
ForbiddenAccessException 1
)1 2
,2 3*
HandleForbiddenAccessException4 R
}S T
,T U
{ 
typeof 
( 
BadRequestException ,
), -
,- .%
HandleBadRequestException/ H
}I J
,J K
{ 
typeof 
( 
ConflictException *
)* +
,+ ,#
HandleConflictException- D
}E F
,F G
} 
; 
}   	
public"" 
override"" 
void"" 
OnException"" (
(""( )
ExceptionContext"") 9
context"": A
)""A B
{## 	
HandleException$$ 
($$ 
context$$ #
)$$# $
;$$$ %
base&& 
.&& 
OnException&& 
(&& 
context&& $
)&&$ %
;&&% &
}'' 	
private)) 
void)) 
HandleException)) $
())$ %
ExceptionContext))% 5
context))6 =
)))= >
{** 	
Type++ 
type++ 
=++ 
context++ 
.++  
	Exception++  )
.++) *
GetType++* 1
(++1 2
)++2 3
;++3 4
_logger.. 
?.. 
... 
LogError.. 
(.. 
context.. %
...% &
	Exception..& /
,../ 0
$str..1 g
,..g h
type..i m
...m n
Name..n r
)..r s
;..s t
if00 
(00 
_exceptionHandlers00 "
.00" #
ContainsKey00# .
(00. /
type00/ 3
)003 4
)004 5
{11 
_exceptionHandlers22 "
[22" #
type22# '
]22' (
.22( )
Invoke22) /
(22/ 0
context220 7
)227 8
;228 9
return33 
;33 
}44 
if66 
(66 
!66 
context66 
.66 

ModelState66 #
.66# $
IsValid66$ +
)66+ ,
{77 ,
 HandleInvalidModelStateException88 0
(880 1
context881 8
)888 9
;889 :
return99 
;99 
}:: "
HandleUnknownException<< "
(<<" #
context<<# *
)<<* +
;<<+ ,
}== 	
private?? 
void?? %
HandleValidationException?? .
(??. /
ExceptionContext??/ ?
context??@ G
)??G H
{@@ 	
varAA 
	exceptionAA 
=AA 
(AA 
ValidationExceptionAA 0
)AA0 1
contextAA1 8
.AA8 9
	ExceptionAA9 B
;AAB C
varCC 
detailsCC 
=CC 
newCC $
ValidationProblemDetailsCC 6
(CC6 7
	exceptionCC7 @
.CC@ A
ErrorsCCA G
)CCG H
{DD 
TypeEE 
=EE 
$strEE J
}FF 
;FF 
contextHH 
.HH 
ResultHH 
=HH 
newHH  "
BadRequestObjectResultHH! 7
(HH7 8
detailsHH8 ?
)HH? @
;HH@ A
contextJJ 
.JJ 
ExceptionHandledJJ $
=JJ% &
trueJJ' +
;JJ+ ,
}KK 	
privateMM 
voidMM ,
 HandleInvalidModelStateExceptionMM 5
(MM5 6
ExceptionContextMM6 F
contextMMG N
)MMN O
{NN 	
varOO 
detailsOO 
=OO 
newOO $
ValidationProblemDetailsOO 6
(OO6 7
contextOO7 >
.OO> ?

ModelStateOO? I
)OOI J
{PP 
TypeQQ 
=QQ 
$strQQ J
}RR 
;RR 
contextTT 
.TT 
ResultTT 
=TT 
newTT  "
BadRequestObjectResultTT! 7
(TT7 8
detailsTT8 ?
)TT? @
;TT@ A
contextVV 
.VV 
ExceptionHandledVV $
=VV% &
trueVV' +
;VV+ ,
}WW 	
privateYY 
voidYY #
HandleNotFoundExceptionYY ,
(YY, -
ExceptionContextYY- =
contextYY> E
)YYE F
{ZZ 	
var[[ 
	exception[[ 
=[[ 
([[ 
NotFoundException[[ .
)[[. /
context[[/ 6
.[[6 7
	Exception[[7 @
;[[@ A
var]] 
details]] 
=]] 
new]] 
ProblemDetails]] ,
(]], -
)]]- .
{^^ 
Type__ 
=__ 
$str__ J
,__J K
Title`` 
=`` 
$str`` ?
,``? @
Detailaa 
=aa 
	exceptionaa "
.aa" #
Messageaa# *
}bb 
;bb 
contextdd 
.dd 
Resultdd 
=dd 
newdd   
NotFoundObjectResultdd! 5
(dd5 6
detailsdd6 =
)dd= >
;dd> ?
contextff 
.ff 
ExceptionHandledff $
=ff% &
trueff' +
;ff+ ,
}gg 	
privateii 
voidii *
HandleForbiddenAccessExceptionii 3
(ii3 4
ExceptionContextii4 D
contextiiE L
)iiL M
{jj 	
varkk 
	exceptionkk 
=kk 
(kk $
ForbiddenAccessExceptionkk 5
)kk5 6
contextkk6 =
.kk= >
	Exceptionkk> G
;kkG H
varmm 
detailsmm 
=mm 
newmm 
ProblemDetailsmm ,
{nn 
Statusoo 
=oo 
StatusCodesoo $
.oo$ %
Status403Forbiddenoo% 7
,oo7 8
Titlepp 
=pp 
$strpp #
,pp# $
Typeqq 
=qq 
$strqq J
,qqJ K
Detailrr 
=rr 
	exceptionrr "
.rr" #
Messagerr# *
}ss 
;ss 
contextuu 
.uu 
Resultuu 
=uu 
newuu  
ObjectResultuu! -
(uu- .
detailsuu. 5
)uu5 6
{vv 

StatusCodeww 
=ww 
StatusCodesww (
.ww( )
Status403Forbiddenww) ;
}xx 
;xx 
contextzz 
.zz 
ExceptionHandledzz $
=zz% &
truezz' +
;zz+ ,
}{{ 	
private}} 
void}} %
HandleBadRequestException}} .
(}}. /
ExceptionContext}}/ ?
context}}@ G
)}}G H
{~~ 	
var 
	exception 
= 
( 
BadRequestException 0
)0 1
context1 8
.8 9
	Exception9 B
;B C
var
ÅÅ 
details
ÅÅ 
=
ÅÅ 
new
ÅÅ 
ProblemDetails
ÅÅ ,
{
ÇÇ 
Status
ÉÉ 
=
ÉÉ 
StatusCodes
ÉÉ $
.
ÉÉ$ %!
Status400BadRequest
ÉÉ% 8
,
ÉÉ8 9
Title
ÑÑ 
=
ÑÑ 
$str
ÑÑ %
,
ÑÑ% &
Type
ÖÖ 
=
ÖÖ 
$str
ÖÖ J
,
ÖÖJ K
Detail
ÜÜ 
=
ÜÜ 
	exception
ÜÜ "
.
ÜÜ" #
Message
ÜÜ# *
}
áá 
;
áá 
context
ââ 
.
ââ 
Result
ââ 
=
ââ 
new
ââ  $
BadRequestObjectResult
ââ! 7
(
ââ7 8
details
ââ8 ?
)
ââ? @
;
ââ@ A
context
ãã 
.
ãã 
ExceptionHandled
ãã $
=
ãã% &
true
ãã' +
;
ãã+ ,
}
åå 	
private
éé 
void
éé %
HandleConflictException
éé ,
(
éé, -
ExceptionContext
éé- =
context
éé> E
)
ééE F
{
èè 	
var
êê 
	exception
êê 
=
êê 
(
êê 
ConflictException
êê .
)
êê. /
context
êê/ 6
.
êê6 7
	Exception
êê7 @
;
êê@ A
var
íí 
details
íí 
=
íí 
new
íí 
ProblemDetails
íí ,
{
ìì 
Status
îî 
=
îî 
StatusCodes
îî $
.
îî$ %
Status409Conflict
îî% 6
,
îî6 7
Title
ïï 
=
ïï 
$str
ïï "
,
ïï" #
Type
ññ 
=
ññ 
$str
ññ J
,
ññJ K
Detail
óó 
=
óó 
	exception
óó "
.
óó" #
Message
óó# *
}
òò 
;
òò 
context
öö 
.
öö 
Result
öö 
=
öö 
new
öö  
ObjectResult
öö! -
(
öö- .
details
öö. 5
)
öö5 6
{
õõ 

StatusCode
úú 
=
úú 
StatusCodes
úú (
.
úú( )
Status409Conflict
úú) :
}
ùù 
;
ùù 
context
üü 
.
üü 
ExceptionHandled
üü $
=
üü% &
true
üü' +
;
üü+ ,
}
†† 	
private
¢¢ 
void
¢¢ $
HandleUnknownException
¢¢ +
(
¢¢+ ,
ExceptionContext
¢¢, <
context
¢¢= D
)
¢¢D E
{
££ 	
var
§§ 
details
§§ 
=
§§ 
new
§§ 
ProblemDetails
§§ ,
{
•• 
Status
¶¶ 
=
¶¶ 
StatusCodes
¶¶ $
.
¶¶$ %*
Status500InternalServerError
¶¶% A
,
¶¶A B
Title
ßß 
=
ßß 
$str
ßß J
,
ßßJ K
Type
®® 
=
®® 
$str
®® J
}
©© 
;
©© 
details
≠≠ 
.
≠≠ 
Detail
≠≠ 
=
≠≠ 
context
≠≠ $
.
≠≠$ %
	Exception
≠≠% .
.
≠≠. /
ToString
≠≠/ 7
(
≠≠7 8
)
≠≠8 9
;
≠≠9 :
context
∞∞ 
.
∞∞ 
Result
∞∞ 
=
∞∞ 
new
∞∞  
ObjectResult
∞∞! -
(
∞∞- .
details
∞∞. 5
)
∞∞5 6
{
±± 

StatusCode
≤≤ 
=
≤≤ 
StatusCodes
≤≤ (
.
≤≤( )*
Status500InternalServerError
≤≤) E
}
≥≥ 
;
≥≥ 
context
µµ 
.
µµ 
ExceptionHandled
µµ $
=
µµ% &
true
µµ' +
;
µµ+ ,
}
∂∂ 	
}
∑∑ 
}∏∏ —
í/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Presentation/TeamTasker.API/Extensions/MiddlewareExtensions.cs
	namespace 	

TeamTasker
 
. 
API 
. 

Extensions #
{ 
public		 

static		 
class		  
MiddlewareExtensions		 ,
{

 
public 
static 
IApplicationBuilder )%
UseGlobalExceptionHandler* C
(C D
thisD H
IApplicationBuilderI \
app] `
)` a
{ 	
return 
app 
. 
UseMiddleware $
<$ %'
ExceptionHandlingMiddleware% @
>@ A
(A B
)B C
;C D
} 	
} 
} ˇ
ê/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Presentation/TeamTasker.API/Controllers/ApiControllerBase.cs
	namespace 	

TeamTasker
 
. 
API 
. 
Controllers $
{ 
[

 
ApiController

 
]

 
[ 
Route 

(
 
$str 
) 
] 
public 

abstract 
class 
ApiControllerBase +
:, -
ControllerBase. <
{ 
private 
ISender 
	_mediator !
;! "
	protected 
ISender 
Mediator "
=># %
	_mediator& /
??=0 3
HttpContext4 ?
.? @
RequestServices@ O
.O P
GetRequiredServiceP b
<b c
ISenderc j
>j k
(k l
)l m
;m n
} 
} ·2
ë/Users/faizanhussain/Documents/Project/Practice/TeamTasker/02Code/teamtasker_be/src/Presentation/TeamTasker.API/Controllers/ProjectsController.cs
	namespace 	

TeamTasker
 
. 
API 
. 
Controllers $
{ 
public 

class 
ProjectsController #
:$ %
ApiControllerBase& 7
{ 
[ 	
HttpGet	 
] 
[ 	 
ProducesResponseType	 
( 
StatusCodes )
.) *
Status200OK* 5
)5 6
]6 7
public 
async 
Task 
< 
ActionResult &
<& '
PaginatedList' 4
<4 5

ProjectDto5 ?
>? @
>@ A
>A B
GetProjectsC N
(N O
[ 
	FromQuery 
] 
int 

pageNumber &
=' (
$num) *
,* +
[ 
	FromQuery 
] 
int 
pageSize $
=% &
$num' )
,) *
[   
	FromQuery   
]   
string   

searchTerm   )
=  * +
null  , 0
,  0 1
[!! 
	FromQuery!! 
]!! 
string!! 
status!! %
=!!& '
null!!( ,
)!!, -
{"" 	
return## 
await## 
Mediator## !
.##! "
Send##" &
(##& '
new##' *
GetProjectsQuery##+ ;
{$$ 

PageNumber%% 
=%% 

pageNumber%% '
,%%' (
PageSize&& 
=&& 
pageSize&& #
,&&# $

SearchTerm'' 
='' 

searchTerm'' '
,''' (
Status(( 
=(( 
status(( 
})) 
))) 
;)) 
}** 	
[11 	
HttpGet11	 
(11 
$str11 
)11 
]11 
[22 	 
ProducesResponseType22	 
(22 
StatusCodes22 )
.22) *
Status200OK22* 5
)225 6
]226 7
[33 	 
ProducesResponseType33	 
(33 
StatusCodes33 )
.33) *
Status404NotFound33* ;
)33; <
]33< =
public44 
async44 
Task44 
<44 
ActionResult44 &
<44& '
ProjectDetailDto44' 7
>447 8
>448 9

GetProject44: D
(44D E
int44E H
id44I K
)44K L
{55 	
return66 
await66 
Mediator66 !
.66! "
Send66" &
(66& '
new66' *
GetProjectByIdQuery66+ >
{66? @
Id66A C
=66D E
id66F H
}66I J
)66J K
;66K L
}77 	
[>> 	
HttpPost>>	 
]>> 
[?? 	 
ProducesResponseType??	 
(?? 
StatusCodes?? )
.??) *
Status201Created??* :
)??: ;
]??; <
[@@ 	 
ProducesResponseType@@	 
(@@ 
StatusCodes@@ )
.@@) *
Status400BadRequest@@* =
)@@= >
]@@> ?
publicAA 
asyncAA 
TaskAA 
<AA 
ActionResultAA &
<AA& '
intAA' *
>AA* +
>AA+ ,
CreateAA- 3
(AA3 4 
CreateProjectCommandAA4 H
commandAAI P
)AAP Q
{BB 	
varCC 
idCC 
=CC 
awaitCC 
MediatorCC #
.CC# $
SendCC$ (
(CC( )
commandCC) 0
)CC0 1
;CC1 2
returnEE 
CreatedAtActionEE "
(EE" #
nameofEE# )
(EE) *

GetProjectEE* 4
)EE4 5
,EE5 6
newEE7 :
{EE; <
idEE= ?
}EE@ A
,EEA B
idEEC E
)EEE F
;EEF G
}FF 	
[NN 	
HttpPutNN	 
(NN 
$strNN 
)NN 
]NN 
[OO 	 
ProducesResponseTypeOO	 
(OO 
StatusCodesOO )
.OO) *
Status204NoContentOO* <
)OO< =
]OO= >
[PP 	 
ProducesResponseTypePP	 
(PP 
StatusCodesPP )
.PP) *
Status400BadRequestPP* =
)PP= >
]PP> ?
[QQ 	 
ProducesResponseTypeQQ	 
(QQ 
StatusCodesQQ )
.QQ) *
Status404NotFoundQQ* ;
)QQ; <
]QQ< =
publicRR 
asyncRR 
TaskRR 
<RR 
ActionResultRR &
>RR& '
UpdateRR( .
(RR. /
intRR/ 2
idRR3 5
,RR5 6 
UpdateProjectCommandRR7 K
commandRRL S
)RRS T
{SS 	
ifTT 
(TT 
idTT 
!=TT 
commandTT 
.TT 
IdTT  
)TT  !
{UU 
returnVV 

BadRequestVV !
(VV! "
)VV" #
;VV# $
}WW 
awaitYY 
MediatorYY 
.YY 
SendYY 
(YY  
commandYY  '
)YY' (
;YY( )
return[[ 
	NoContent[[ 
([[ 
)[[ 
;[[ 
}\\ 	
[cc 	

HttpDeletecc	 
(cc 
$strcc 
)cc 
]cc 
[dd 	 
ProducesResponseTypedd	 
(dd 
StatusCodesdd )
.dd) *
Status204NoContentdd* <
)dd< =
]dd= >
[ee 	 
ProducesResponseTypeee	 
(ee 
StatusCodesee )
.ee) *
Status404NotFoundee* ;
)ee; <
]ee< =
publicff 
asyncff 
Taskff 
<ff 
ActionResultff &
>ff& '
Deleteff( .
(ff. /
intff/ 2
idff3 5
)ff5 6
{gg 	
awaithh 
Mediatorhh 
.hh 
Sendhh 
(hh  
newhh  # 
DeleteProjectCommandhh$ 8
{hh9 :
Idhh; =
=hh> ?
idhh@ B
}hhC D
)hhD E
;hhE F
returnjj 
	NoContentjj 
(jj 
)jj 
;jj 
}kk 	
}ll 
}mm 