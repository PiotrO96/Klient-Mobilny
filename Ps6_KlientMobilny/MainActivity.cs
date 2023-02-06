using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;
using NetMQ;
using NetMQ.Sockets;
using SkiaSharp;
using System.Collections.Generic;
using Microcharts;
//using Entry = Microcharts.Entry;  
using Microcharts.Droid;
using System.ComponentModel;
using System.Net.Sockets;
using Android.Widget;
using System.Reflection.Emit;
using System.Threading.Tasks;
using System.Threading;
using Java.Net;
using System.Net;

namespace Ps6_KlientMobilny
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
      

        bool subUzycieRamu = false;
        bool subUzycieCpu = false;
        bool subUzycieDysk = false;
        bool isRequestRunning = false;
        int count = 1;
        bool isRunningSubscriber = false;
        string topic = "";
        bool reqRam = false;
        bool reqCpu = false;
        bool reqDisk = false;

        public static IList<string> allowableCommandLineArgs
              = new[] { "RAM", "CPU", "DISK" };
        float wartosc;
        int wartosc2;
       /* ChartEntry[] entries = new[]
        {
        new ChartEntry(212)
        {
            Label = "CPU",
                ValueLabel = "212",
                Color = SKColor.Parse("#266489")
        },
       new ChartEntry(248)
        {
            Label = "RAM",
                ValueLabel = "248",
                Color = SKColor.Parse("#266489")
        },
       new ChartEntry(128)
        {
            Label = "DISK",
                ValueLabel = "128",
                Color = SKColor.Parse("#266489")
        }
        };*/
        SubscriberSocket sub_s = new SubscriberSocket();
        ProgressBar CpuProgresBar;
        ProgressBar RamProgresBar;
        ProgressBar DyskProgresBar;
        TextView DyskLabelView;
        TextView RAMLabelView;
        TextView CpuLabelView;
        CheckBox DYSKcheckBox;
        CheckBox CPUcheckBox;
        CheckBox RAMcheckBox;
        Button STOPbutton;
        Button DYSKbutton;
        Button CPUbutton;
        Button RAMbutton;
        Button Poloczbutton;
        ChartView cpuView;
        ChartView ramView;
        ChartView dyskView;
        BarChart chart;
        ChartEntry [] entries;
        TextView respText;
        Button ReqButton;
        CheckBox RAM_REQUEST_checkBox;
        CheckBox CPU_REQUEST_checkBox;
        CheckBox DYSK_REQUEST_checkBox;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
   
            SetContentView(Resource.Layout.activity_main);
             cpuView = FindViewById<ChartView>(Resource.Id.CpuChart);
            ramView = FindViewById<ChartView>(Resource.Id.RamChart);
            dyskView = FindViewById<ChartView>(Resource.Id.dyskChart);
            //chart = new BarChart() { Entries = entries };
            // cpuView.Chart = chart;
            //  ramView.Chart = chart;

            // cpuView.Chart = chart;

            Poloczbutton = FindViewById<Button>(Resource.Id.poloczButton);
             RAMbutton = FindViewById<Button>(Resource.Id.RamButton);
             CPUbutton = FindViewById<Button>(Resource.Id.CpuButton);
             DYSKbutton = FindViewById<Button>(Resource.Id.DyskButton);
             STOPbutton = FindViewById<Button>(Resource.Id.StopButton);
             RAMcheckBox = FindViewById<CheckBox>(Resource.Id.ramCheckBox);
             CPUcheckBox = FindViewById<CheckBox>(Resource.Id.cpuCheckBox);
             DYSKcheckBox = FindViewById<CheckBox>(Resource.Id.dyskCheckBox);
            RAM_REQUEST_checkBox = FindViewById<CheckBox>(Resource.Id.RamReqCheckBox);
            CPU_REQUEST_checkBox = FindViewById<CheckBox>(Resource.Id.CpuReqCheckBox);
            DYSK_REQUEST_checkBox = FindViewById<CheckBox>(Resource.Id.DyskReqCheckBox);
            CPUcheckBox = FindViewById<CheckBox>(Resource.Id.cpuCheckBox);
            DYSKcheckBox = FindViewById<CheckBox>(Resource.Id.dyskCheckBox);
            CpuLabelView = FindViewById<TextView>(Resource.Id.cpuView);
             RAMLabelView = FindViewById<TextView>(Resource.Id.ramView);
           DyskLabelView = FindViewById<TextView>(Resource.Id.dyskView);
            CpuProgresBar = FindViewById<ProgressBar>(Resource.Id.cpuProgresBar);
            RamProgresBar = FindViewById<ProgressBar>(Resource.Id.ramProgresBar);
            DyskProgresBar = FindViewById<ProgressBar>(Resource.Id.dyskProgresBar);
            respText = FindViewById<TextView>(Resource.Id.RespText);
            ReqButton = FindViewById<Button>(Resource.Id.RequestButton);


                RAMbutton.Click += delegate
                {
                    if (RAMcheckBox.Checked)
                    {
                        Task.Run(() =>
                    {
                        ThreadPool.QueueUserWorkItem(o => StartSubscriberRAM());
                    // nie wyswietla wykresu
                    /*  chart = new BarChart() { Entries = entries };
                        ramView.Chart = chart;*/
                        ////////////////////////////
                        ///
                     //   RunOnUiThread(() => ramView.Chart = chart);
                         ChartEntry[] entries = new[]
              {
         new ChartEntry(212)
         {
             Label = "RAM",
                 ValueLabel = wartosc2.ToString(),
                 Color = SKColor.Parse("#266489")
         }
         };

                         chart = new BarChart() { Entries = entries };
                         ramView.Chart = chart;
                    });
                    }

                };
            CPUbutton.Click += delegate
            {
                if (CPUcheckBox.Checked)
                {
                    Task.Run(() =>
                    {
                        ThreadPool.QueueUserWorkItem(o => StartSubscriberCPU());
                     
                        // wyswietla tylko jedną wartosc wykresu
                        ChartEntry[] entries = new[]
             {
        new ChartEntry(200)
        {
            Label = "CPU",
                ValueLabel = wartosc2.ToString(),
                Color = SKColor.Parse("#266489")
        }
        };

                        chart = new BarChart() { Entries = entries };
                        cpuView.Chart = chart;
                    });
                }

            };
            DYSKbutton.Click += delegate
            {
                if (DYSKcheckBox.Checked)
                {
                    Task.Run(() =>
                    {
                        ThreadPool.QueueUserWorkItem(o => StartSubscriberDysk());

                        ChartEntry[] entries = new[]
             {
        new ChartEntry(122)
        {
            Label = "DYSK",
                ValueLabel = wartosc2.ToString(),
                Color = SKColor.Parse("#266489")
        }
        };

                        chart = new BarChart() { Entries = entries };
                        dyskView.Chart = chart;
                    });
                }

            };
            STOPbutton.Click += delegate
            {

                sub_s.Close();
            };
            Poloczbutton.Click += delegate {
                if (!RAMcheckBox.Checked && !CPUcheckBox.Checked && !DYSKcheckBox.Checked)
                {
                    Console.WriteLine("Error");
                    return;

                }
            };
            ReqButton.Click += delegate {
                Task.Run(() =>
                {
                    ThreadPool.QueueUserWorkItem(o => Reques());
                });
            };
        }

        private void StartSubscriberRAM()
        {
            
            isRunningSubscriber = true;
            using (SubscriberSocket sub_s = new SubscriberSocket())
            {
                sub_s.Options.ReceiveHighWatermark = 1000;
                sub_s.Connect("tcp://192.168.50.75:12345");
                sub_s.Subscribe("RAM");
                subUzycieRamu = true;
                while (subUzycieRamu)
                {
                   string wartoscTekst= sub_s.ReceiveFrameString().ToString();
             
                    wartosc = float.Parse(sub_s.ReceiveFrameString());
                  
                    string progrestxt = wartoscTekst;
                 
              string napis = wartosc.ToString();
                    RunOnUiThread(() => RAMLabelView.Text = progrestxt + ":  " + napis );
                   
                     wartosc2 = Convert.ToInt32(wartosc);
                    
                    RamProgresBar.Progress = wartosc2 % 100;
                
                 /*   entries = new[]
             {
      
                        new ChartEntry(212)
        {
            Label = "RAM",
                ValueLabel = wartosc2.ToString(),
                Color = SKColor.Parse("#266489")
        }
        };
                    chart = new BarChart() { Entries = entries };
                    ramView.Chart = chart;*/
                    //   chart = new BarChart() { Entries = entries };
                    //      ramView.Chart = chart;

                    //     RunOnUiThread(() => ramView.Chart = chart);
                    // ramView.Chart = chart;
                }


            }

        }
        private void StartSubscriberCPU()
        {

            isRunningSubscriber = true;
            using (SubscriberSocket sub_s = new SubscriberSocket())
            {
                sub_s.Options.ReceiveHighWatermark = 1000;
                
                sub_s.Connect("tcp://10.0.2.2:12345");
                sub_s.Subscribe("CPU");
                subUzycieCpu = true;
                while (subUzycieCpu)//true
                {
                    string wartoscTekst = sub_s.ReceiveFrameString();
                   
                      wartosc = float.Parse(sub_s.ReceiveFrameString());
                   
                    string progrestxt = wartoscTekst;
                    string napis = wartosc.ToString();
                    RunOnUiThread(() => CpuLabelView.Text = progrestxt + ":  " + napis);

                     wartosc2 = Convert.ToInt32(wartosc);
                    CpuProgresBar.Progress = wartosc2 % 100;
                    //   cpuView.Chart = chart;
                   
               //     RunOnUiThread(() => cpuView.Chart = chart);
                    //cpuView.Chart = chart;
                }
                

            }

        }
        private void StartSubscriberDysk()
        {

            isRunningSubscriber = true;
            using (SubscriberSocket sub_s = new SubscriberSocket())
            {
                sub_s.Options.ReceiveHighWatermark = 1000;
                
                sub_s.Connect("tcp://10.0.2.2:12345");
                sub_s.Subscribe("DISK");
                subUzycieDysk = true;
                while (subUzycieDysk)//true
                {
                    string wartoscTekst = sub_s.ReceiveFrameString();
                   
                    wartosc = float.Parse(sub_s.ReceiveFrameString());
                   
                    string progrestxt = wartoscTekst;
                    string napis = wartosc.ToString();
                    RunOnUiThread(() => DyskLabelView.Text = progrestxt + ":  " + napis);

                     wartosc2 = Convert.ToInt32(wartosc);
                    DyskProgresBar.Progress = wartosc2 % 100;
                    //dyskView.Chart = chart;
                }


            }

        }
        private void Reques()
        {
            isRequestRunning = true;
             try
           {
               
               using (var requestSocket = new RequestSocket(">tcp://192.168.50.75:5555"))
               {

                   while (isRequestRunning)
                   {
                       if (RAM_REQUEST_checkBox.Checked)
                       {
                            requestSocket.SendMoreFrame("RAM").SendFrame("Request");
                            var message2 = requestSocket.ReceiveFrameString();
                            RunOnUiThread(() => respText.Text = message2.ToString());
                        }
                       if (CPU_REQUEST_checkBox.Checked)
                       {
                            requestSocket.SendMoreFrame("CPU").SendFrame("Request");
                            var message3 = requestSocket.ReceiveFrameString();
                            RunOnUiThread(() => respText.Text = message3.ToString());

                        }
                       if (DYSK_REQUEST_checkBox.Checked)
                       {
                            requestSocket.SendMoreFrame("DYSK").SendFrame("Request");
                            var message4 = requestSocket.ReceiveFrameString();
                            RunOnUiThread(() => respText.Text = message4.ToString()) ;
                        }

                   }
               }
           }
           catch (Exception ex)
           {

           }
        }


    }
}
