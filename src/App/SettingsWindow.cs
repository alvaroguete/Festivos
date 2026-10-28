using System.Windows;
using System.Windows.Controls;
namespace Festivos.Desktop;
public sealed class SettingsWindow:Window
{
 public Preferences Result{get;private set;}
 public SettingsWindow(Preferences original){Title="Configuración · Festivos de Colombia";Width=350;Height=460;ResizeMode=ResizeMode.NoResize;WindowStartupLocation=WindowStartupLocation.CenterOwner;FontFamily=new("Segoe UI");Background=System.Windows.Media.Brushes.FloralWhite;
 Result=new(){Color=original.Color,Opacity=original.Opacity,AutoStart=original.AutoStart,Topmost=original.Topmost};var panel=new StackPanel{Margin=new Thickness(24)};Content=panel;
 panel.Children.Add(new TextBlock{Text="Hazlo tuyo",FontSize=24,FontWeight=FontWeights.SemiBold,Margin=new Thickness(0,0,0,20)});panel.Children.Add(new TextBlock{Text="Color de fondo"});
 var colors=new ComboBox{Margin=new Thickness(0,6,0,16)};var palette=new[]{("Amarillo Post-it","#FFF2AD"),("Verde salvia","#DDF1CE"),("Azul cielo","#DCECF9"),("Rosa suave","#F5DCEA"),("Arena","#F2E8DA")};foreach(var (name,color)in palette)colors.Items.Add(new ComboBoxItem{Content=name,Tag=color});colors.SelectedIndex=Array.FindIndex(palette,x=>x.Item2==original.Color);if(colors.SelectedIndex<0)colors.SelectedIndex=0;panel.Children.Add(colors);
 var label=new TextBlock();panel.Children.Add(label);var opacity=new Slider{Minimum=.55,Maximum=1,Value=original.Opacity,TickFrequency=.05,IsSnapToTickEnabled=true,Margin=new Thickness(0,10,0,16)};void Label()=>label.Text=$"Opacidad: {opacity.Value:P0}";Label();opacity.ValueChanged+=(_,_)=>Label();panel.Children.Add(opacity);
 var startup=new CheckBox{Content="Iniciar con Windows",IsChecked=original.AutoStart,Margin=new Thickness(0,4,0,8)};var top=new CheckBox{Content="Mantener siempre encima",IsChecked=original.Topmost,Margin=new Thickness(0,4,0,16)};panel.Children.Add(startup);panel.Children.Add(top);
 var reset=new Button{Content="Restaurar valores predeterminados"};reset.Click+=(_,_)=>{colors.SelectedIndex=0;opacity.Value=1;startup.IsChecked=false;top.IsChecked=false;};panel.Children.Add(reset);
 var save=new Button{Content="Guardar",Margin=new Thickness(0,14,0,0),IsDefault=true};save.Click+=(_,_)=>{Result.Color=(string)((ComboBoxItem)colors.SelectedItem).Tag;Result.Opacity=opacity.Value;Result.AutoStart=startup.IsChecked==true;Result.Topmost=top.IsChecked==true;DialogResult=true;};panel.Children.Add(save);
 var current=new Button{Content="Regresar al mes actual"};current.Click+=(_,_)=>{if(Owner is MainWindow main)main.GoToday();DialogResult=false;};panel.Children.Add(current);
 }
}
