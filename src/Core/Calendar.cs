using System.Text.Json;
namespace Festivos;
public record Holiday(DateOnly Date,string Name,string Description);
public static class Calendar
{
 public const int MinYear=1583,MaxYear=9998;
 public static DateOnly Easter(int year){if(year<MinYear||year>9999)throw new ArgumentOutOfRangeException(nameof(year));int a=year%19,b=year/100,c=year%100,d=b/4,e=b%4,f=(b+8)/25,g=(b-f+1)/3,h=(19*a+b-d-g+15)%30,i=c/4,k=c%4,l=(32+2*e+2*i-h-k)%7,m=(a+11*h+22*l)/451,n=h+l-7*m+114;return new(year,n/31,n%31+1);}
 public static DateOnly Monday(DateOnly d)=>d.AddDays((8-(int)d.DayOfWeek)%7);
 public static IReadOnlyList<Holiday> Holidays(int year){var list=new List<Holiday>();var easter=Easter(year);
 void Add(DateOnly d,string name,bool moved=false){var legal=moved?Monday(d):d;list.Add(new(legal,name,moved?$"{name}. Celebración: {d:dd/MM}. Descanso legal: {legal:dd/MM}, conforme a la Ley 51 de 1983.":$"{name}. Festivo nacional de fecha fija: {legal:dd/MM}."));}
 void Fixed(int m,int d,string n,bool moved=false)=>Add(new(year,m,d),n,moved);
 Fixed(1,1,"Año Nuevo");Fixed(1,6,"Reyes Magos",true);Fixed(3,19,"San José",true);Add(easter.AddDays(-3),"Jueves Santo");Add(easter.AddDays(-2),"Viernes Santo");Fixed(5,1,"Día del Trabajo");Add(easter.AddDays(39),"Ascensión del Señor",true);Add(easter.AddDays(60),"Corpus Christi",true);Add(easter.AddDays(68),"Sagrado Corazón de Jesús",true);Fixed(6,29,"San Pedro y San Pablo",true);Fixed(7,20,"Día de la Independencia");Fixed(8,7,"Batalla de Boyacá");Fixed(8,15,"Asunción de la Virgen",true);Fixed(10,12,"Diversidad Étnica y Cultural",true);Fixed(11,1,"Todos los Santos",true);Fixed(11,11,"Independencia de Cartagena",true);Fixed(12,8,"Inmaculada Concepción");Fixed(12,25,"Navidad");return list.OrderBy(h=>h.Date).ToArray();}
 public static DateOnly Move(DateOnly date,int months){long n=Math.Clamp((long)date.Year*12+date.Month-1+months,MinYear*12L,MaxYear*12L+11);return new((int)(n/12),(int)(n%12)+1,1);}
}
public sealed class Preferences
{
 public double Left{get;set;}=100;public double Top{get;set;}=100;public double Width{get;set;}=390;public double Height{get;set;}=690;public double Opacity{get;set;}=1;
 public string Color{get;set;}="#FFF2AD";public bool Topmost{get;set;}public bool AutoStart{get;set;}public int Year{get;set;}=DateTime.Today.Year;public int Month{get;set;}=DateTime.Today.Month;
 public void Normalize(){Left=double.IsFinite(Left)?Left:100;Top=double.IsFinite(Top)?Top:100;Width=double.IsFinite(Width)?Math.Clamp(Width,320,1600):390;Height=double.IsFinite(Height)?Math.Clamp(Height,380,1600):690;Opacity=double.IsFinite(Opacity)?Math.Clamp(Opacity,.55,1):1;Year=Math.Clamp(Year,Calendar.MinYear,Calendar.MaxYear);Month=Math.Clamp(Month,1,12);if(!System.Text.RegularExpressions.Regex.IsMatch(Color??"","^#[0-9a-fA-F]{6}$"))Color="#FFF2AD";}
}
public static class PreferenceStore
{
 public static Preferences Load(string path){try{var p=JsonSerializer.Deserialize<Preferences>(File.ReadAllText(path))??new();p.Normalize();return p;}catch(Exception e)when(e is IOException or UnauthorizedAccessException or JsonException){return new();}}
 public static void Save(string path,Preferences p){p.Normalize();Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);string temp=path+".tmp";File.WriteAllText(temp,JsonSerializer.Serialize(p,new JsonSerializerOptions{WriteIndented=true}));File.Move(temp,path,true);}
}
