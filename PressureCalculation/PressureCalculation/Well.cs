using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace PressureCalculation
{
    public class Well
    {
        private const double g = 9.08665;

        private int id;
        private double depth;
        private double densityOfLiquid;
        private int numberOfSteps;
        
        List<KeyValuePair<double, double>> pressures;

        public KeyValuePair<double, double> keyValuePair;

        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }

        public double Depth
        {
            get { return depth; }
            set
            {
                depth = value;
                OnPropertyChanged("Depth");
            }
        }
        public double DensityOfLiquid
        {
            get
            {
                return densityOfLiquid;
            }
            set
            {
                densityOfLiquid = value;
                OnPropertyChanged("DensityOfLiquid");
            }
        }
        public int NumberOfSteps
        {
            get { return numberOfSteps; }
            set
            {
                numberOfSteps = value;
                OnPropertyChanged("NumberOfSteps");
            }
        }

        public List<KeyValuePair<double, double>> Pressures
        {
            get { return pressures; }
            set
            {
                pressures = value;
                OnPropertyChanged("Pressures");
            }
        }


        public  void CalculatePressure()
        {
            pressures = new List<KeyValuePair<double, double>>();
            keyValuePair = new KeyValuePair<double, double>();
            double interval;
            double pressure;

            for (int i = 0; i < numberOfSteps; i++)
            {
                interval = (depth / numberOfSteps) * (i + 1);
                pressure = densityOfLiquid * g * interval;
                pressures.Add(new KeyValuePair<double, double>(interval, pressure));
                Thread.Sleep(100);

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
