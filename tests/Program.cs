using Festivos;
using Cal=Festivos.Calendar;
int passed=0;
void Check(bool condition,string name){if(!condition)throw new Exception("FALLO: "+name);passed++;Console.WriteLine("OK: "+name);}
var expected=new Dictionary<int,string>{
[2025]="01-01 01-06 03-24 04-17 04-18 05-01 06-02 06-23 06-30 06-30 07-20 08-07 08-18 10-13 11-03 11-17 12-08 12-25",
[2026]="01-01 01-12 03-23 04-02 04-03 05-01 05-18 06-08 06-15 06-29 07-20 08-07 08-17 10-12 11-02 11-16 12-08 12-25",
[2027]="01-01 01-11 03-22 03-25 03-26 05-01 05-10 05-31 06-07 07-05 07-20 08-07 08-16 10-18 11-01 11-15 12-08 12-25",
[2030]="01-01 01-07 03-25 04-18 04-19 05-01 06-03 06-24 07-01 07-01 07-20 08-07 08-19 10-14 11-04 11-11 12-08 12-25",
[2040]="01-01 01-09 03-19 03-29 03-30 05-01 05-14 06-04 06-11 07-02 07-20 08-07 08-20 10-15 11-05 11-12 12-08 12-25"};
foreach(var(year,dates)in expected)Check(string.Join(" ",Cal.Holidays(year).Select(h=>h.Date.ToString("MM-dd")))==dates,$"18 celebraciones legales {year}");
foreach(var(year,date)in new[]{(2025,"2025-04-20"),(2026,"2026-04-05"),(2027,"2027-03-28"),(2030,"2030-04-21"),(2040,"2040-04-01")})Check(Cal.Easter(year)==DateOnly.Parse(date),$"Pascua {year}");
for(int i=0;i<7;i++){var d=new DateOnly(2026,1,5).AddDays(i);Check(Cal.Monday(d).DayOfWeek==DayOfWeek.Monday&&Cal.Monday(d)>=d&&Cal.Monday(d).DayNumber-d.DayNumber<7,$"Traslado al lunes, día {i}");}
Check(Cal.Move(new(2026,12,1),1)==new DateOnly(2027,1,1),"Diciembre a enero");Check(Cal.Move(new(2026,1,1),-1)==new DateOnly(2025,12,1),"Enero a diciembre");Check(Cal.Move(new(2026,6,1),12)==new DateOnly(2027,6,1),"Cambio de año");
Check(DateTime.DaysInMonth(2040,2)==29&&Cal.Holidays(2040).Count==18,"Año bisiesto");Check(Cal.Holidays(2025).Any(h=>h.Date==new DateOnly(2025,7,20)&&h.Date.DayOfWeek==DayOfWeek.Sunday),"Festivo fijo en domingo permanece");
Check(Cal.Move(new(1583,1,1),-1)==new DateOnly(1583,1,1),"Límite inferior");Check(Cal.Move(new(9998,12,1),1)==new DateOnly(9998,12,1),"Límite superior");
var path=Path.Combine(Path.GetTempPath(),Guid.NewGuid()+".json");
try{var p=new Preferences{Year=2030,Month=12,Width=420,Height=700,Left=-100,Top=30,Topmost=true,AutoStart=true,Color="#DDF1CE",Opacity=.75};PreferenceStore.Save(path,p);var q=PreferenceStore.Load(path);Check(q.Year==2030&&q.Month==12&&q.Width==420&&q.Height==700&&q.Left==-100&&q.Top==30&&q.Topmost&&q.AutoStart&&q.Color==p.Color&&q.Opacity==.75,"Persistencia completa");PreferenceStore.Save(path,q);Check(PreferenceStore.Load(path).Month==12,"Guardado repetible");File.WriteAllText(path,"invalid");Check(PreferenceStore.Load(path).Color=="#FFF2AD","Recuperación JSON corrupto");File.WriteAllText(path,"{\"Month\":99,\"Year\":0,\"Color\":null}");q=PreferenceStore.Load(path);Check(q.Month==12&&q.Year==1583&&q.Color=="#FFF2AD","Normalización de preferencias");}finally{File.Delete(path);}
Console.WriteLine($"{passed} pruebas superadas.");
