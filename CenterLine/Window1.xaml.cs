using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;





namespace CenterLine
{

    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {

        public bool FlatCurve;
        public bool VerticalCurve;
        public bool Done;


        public Window1()
        {
            InitializeComponent();
            
        }


        private void FlatCurveSelection(object sender, RoutedEventArgs e)
        {
            FlatCurve = true;
            this.window.Hide();
        }

        private void VerticalCurveSelection(object sender, RoutedEventArgs e)
        {
            VerticalCurve = true;
            this.window.Hide();
        }

        private void DoneClick(object sender, RoutedEventArgs e)
        {
            Done = true;
            DialogResult = true;
        }


    }
}
