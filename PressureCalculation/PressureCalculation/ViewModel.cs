using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Controls.DataVisualization.Charting;

namespace PressureCalculation
{
    public class ViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Well> Wells { get; set; }
        private Well selectedWell;
        private string pauseText;

        public double depth { get; set; }
        public double density  {get; set; }
        public int numOfSteps { get; set; }

        private bool IsPause = false;
        private bool IsCalcStart = false;
        int index;
        int id;

        
        ManualResetEvent me = new ManualResetEvent(false);
        ManualResetEvent me1 = new ManualResetEvent(false);

        public ViewModel()
        {
            Wells = new ObservableCollection<Well>();
            id = 1;
        }

        public Well SelectedWell
        {
            get { return selectedWell; }
            set
            {
                selectedWell = value;
                OnPropertyChanged("SelectedWell");
            }
        }

        public string PauseText
        {
            get { return pauseText; }
            set
            {
                pauseText = value;
                OnPropertyChanged("PauseText");
            }
        }

        private ICommand _add;
        public ICommand AddCommand
        {

            get { 
                return _add ?? (_add = new RelayCommand(o =>
                {
                    Wells.Add(new Well {Id = id, Depth = depth, DensityOfLiquid = density, NumberOfSteps = numOfSteps });
                    id++;
                }));
            }
        }

        private ICommand _calculate;
        public ICommand CalculateCommand
        {
            get 
            {
                return _calculate ?? (_calculate = new RelayCommand(o =>
                {
                    CalculatePessure();
                }));
            }
        }

        private async void CalculatePessure()
        {
            await Task.Run(() =>
            {
                IsCalcStart = true;
                foreach (var w in Wells)
                    if (!IsPause)
                    {
                        w.CalculatePressure();
                        index = w.Id;
                        if (IsPause)
                            me1.Set();
                    }
                    else
                    {
                        me.WaitOne();
                        w.CalculatePressure();
                        index = w.Id;
                    }
                IsCalcStart = false;
            });
        }

        private ICommand _pause;
        public ICommand PauseCommand
        {
            get
            {
                return _pause ?? (_pause = new RelayCommand(o =>
                {
                    if (IsCalcStart)
                    {
                        if (!IsPause)
                        {
                            IsPause = true;
                            me1.WaitOne();
                            PauseText = "Расчет остановлен\nСкважина " + index + ", \nГлубина " + Wells[index - 1].Pressures.Last().Key + ", \nДавление " + Wells[index - 1].Pressures.Last().Value;
                        }
                        else
                        {
                            IsPause = false;
                            me.Set();
                            PauseText = "Расчет возобновлен";
                        }
                    }
                }));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

    }
}
