using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Proj190922
{
    public class SelectOptions
    {
        public Options EnumProperty { get; set; }
        public bool BooleanProperty { get; set; }
    }
    public enum Options
    {
        BillNo,
        BillDt,
        Amt
    }

    public class RadioButtonCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
                              System.Globalization.CultureInfo culture)
        {
            return value.Equals(parameter);
        }
        public object ConvertBack(object value, Type targetType, object parameter,
                                  System.Globalization.CultureInfo culture)
        {
            return value.Equals(true) ? parameter : Binding.DoNothing;
        }
    }

    public class SelectedItemToItemsSource : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;
            return new List<object>() { value };
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IndexToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((int)value >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(DateTime), typeof(String))]
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!String.IsNullOrEmpty(value.ToString()))
            {
                return DateTime.ParseExact(value.ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!String.IsNullOrEmpty(value.ToString()))
            {
                return DateTime.ParseExact(value.ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString();
            }
            return null;
        }
    }

    public class StringToDecimal : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double ret = 0;
            return double.TryParse((string)value, out ret) ? ret : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //throw new NotImplementedException();
            return value.ToString();
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new BillsViewModel();
        }

        public void DoSelectedRow(object sender, MouseButtonEventArgs e)
        {
            DataGridCell cell = sender as DataGridCell;
            if (cell != null && !cell.IsEditing)
            {
                DataGridRow row = FindVisualParent<DataGridRow>(cell);
                if (row != null)
                {
                    row.IsSelected = !row.IsSelected;

                    e.Handled = true;
                }
            }
        }

        public static Parent FindVisualParent<Parent>(DependencyObject child) where Parent : DependencyObject
        {
            DependencyObject parentObject = child;

            while (!((parentObject is System.Windows.Media.Visual)
                     || (parentObject is System.Windows.Media.Media3D.Visual3D)))
            {
                if (parentObject is Parent || parentObject == null)
                {
                    return parentObject as Parent;
                }
                else
                {
                    parentObject = (parentObject as FrameworkContentElement).Parent;
                }
            }
            parentObject = VisualTreeHelper.GetParent(parentObject);
            if (parentObject is Parent || parentObject == null)
            {
                return parentObject as Parent;
            }
            else
            {
                return FindVisualParent<Parent>(parentObject);
            }
        }

        void window1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void minimize_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void maximize_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
                WindowState = WindowState.Maximized;
        }

        private void quit_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
    }

    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        protected virtual void SetValue<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            field = value;
            OnPropertyChanged(propertyName);
        }
    }

    public class Bills : ViewModelBase
    {

        private int _id;
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                SetValue(ref _id, value);
            }
        }

        private string _party;
        public string Party
        {
            get
            {
                return _party;
            }
            set
            {
                SetValue(ref _party, value);
            }
        }

        private string _billNo;
        public string BillNo
        {
            get
            {
                return _billNo;
            }
            set
            {
                SetValue(ref _billNo, value);
            }
        }


        private string _billDt;
        public string BillDt
        {
            get
            {
                return  _billDt;
            }

            set
            {
                SetValue(ref _billDt, value);
            }
        }

        private string _amt;
        public string Amt
        {
            get
            {
                return _amt;
            }
            set
            {
                SetValue(ref _amt, value);
            }
        }

        private string _dueDt;
        public string DueDt
        {
            get
            {
                return _dueDt;
            }

            set
            {
                SetValue(ref _dueDt, value);
            }
        }

        private string _remarks;
        public string Remarks
        {
            get
            {
                return _remarks;
            }
            set
            {
                SetValue(ref _remarks, value);
            }
        }

        private string _paidOn;
        public string PaidOn
        {
            get
            {
                return _paidOn;
            }

            set
            {
                SetValue(ref _paidOn, value);
            }
        }

    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }


    public class BillsViewModel : ViewModelBase
    {
        public IList SelectedItems { get; set; }


        public int Cnt
        {
            get
            {
                return AllBills.OfType<Bills>().ToList().Count(x => String.IsNullOrEmpty(x.PaidOn));
            }

        }

        public int Cnt2
        {
            get
            {
                return AllBills.OfType<Bills>().ToList().Where(a => !String.IsNullOrEmpty(a.Remarks)).Count(x => String.IsNullOrEmpty(x.PaidOn));
            }

        }

        private ObservableCollection<Bills> _allBills;
        public ObservableCollection<Bills> AllBillss
        {
            get { return _allBills; }
            set
            {

                _allBills = value;
                SetValue(ref _allBills, value);
            }

        }

        private ICollectionView _allBillsCollection;
        public ICollectionView AllBills
        {
            get { return _allBillsCollection; }
            set
            {

                _allBillsCollection = value;
                OnPropertyChanged("AllBills");
            }
        }

        private ObservableCollection<string> comboItems;
        public ObservableCollection<string> ComboItems
        {
            get { return comboItems; }
            set
            {
                comboItems = value;
                OnPropertyChanged("ComboItems");
            }
        }


        private string _SelectedCBItem2;
        public string SelectedCBItem
        {
            get { return _SelectedCBItem2; }
            set
            {
                FilterString = null;
                SetValue(ref _SelectedCBItem2, value);
                AllBills.Refresh();
                OnPropertyChanged("Cnt");
                OnPropertyChanged("Cnt2");
            }
        }

        private string _filterString;
        public string FilterString
        {
            get { return _filterString; }
            set
            {
                _filterString = value;
                SetValue(ref _filterString, value);
            }
        }

        BillsBusinessObject bills;
        private ObservableCollection<Bills> _Bill;
        public ObservableCollection<Bills> Bill
        {
            get
            {
                _Bill = new ObservableCollection<Bills>(bills.GetBills());
                return _Bill;

            }
        }


        public int SelectedIndex { get; set; }
        object _SelectedInv;
        public object SelectedInv
        {
            get
            {
                return _SelectedInv;
            }
            set
            {
                if (_SelectedInv != value)
                {
                    _SelectedInv = value;
                    OnPropertyChanged("SelectedInv");
                }
            }
        }

        private BindingGroup _UpdateBindingGroup;
        public BindingGroup UpdateBindingGroup
        {
            get
            {
                return _UpdateBindingGroup;
            }
            set
            {
                if (_UpdateBindingGroup != value)
                {
                    _UpdateBindingGroup = value;
                    OnPropertyChanged("UpdateBindingGroup");
                }
            }
        }

        private string _myInfo;
        public string myInfo
        {
            get { return _myInfo; }
            set
            {
                SetValue(ref _myInfo, value);
            }
        }

        private string _CityName;
        public string CityName
        {
            get { return _CityName; }
            set
            {
                SetValue(ref _CityName, value);
            }
        }

        public RelayCommand GoButtonClicked { get; set; }

        public SelectOptions SOptions { get; set; }


        public BillsViewModel()
        {
            CultureInfo culture = new CultureInfo("en-IN");

            SOptions = new SelectOptions();

            AllBillss = DatabaseLayer.GetAllBillsFromDB();

            comboItems = new ObservableCollection<string>(AllBillss.Select(b => b.Party).Distinct().OrderBy(b => b).ToList());

            AllBills = new ListCollectionView(AllBillss)
            {
                Filter = o => ((Bills)o).Party == SelectedCBItem
            };
            GoButtonClicked = new RelayCommand(GoFilterData);

            bills = new BillsBusinessObject();
            bills.BillChanged += new EventHandler(bills_BillChanged);


            UpdateBindingGroup = new BindingGroup { Name = "Group1" };
            CancelCommand = new RelayCommand(DoCancel);
            //SaveCommand = new RelayCommand(DoSave, CanDoSave);
            SaveCommand = new RelayCommand(DoSave);
            AddCommand = new RelayCommand(AddUser);
            DeleteUserCommand = new RelayCommand(DeleteUser);

        }


        public void GoFilterData(object param)
        {
            AllBills.Filter=FilterTask;
            OnPropertyChanged("Cnt");
            OnPropertyChanged("Cnt2");
        }

        public bool FilterTask(object value)
        {
            bool f;
            var entry = value as Bills;

            if (entry != null)
            {
                if (!string.IsNullOrEmpty(FilterString))
                {

                    switch (SOptions.EnumProperty)
                    {
                        case Options.BillNo:
                            f = entry.Party == SelectedCBItem && entry.BillNo.Contains(FilterString);
                            break;
                        case Options.BillDt:
                            f = entry.Party == SelectedCBItem && entry.BillDt == FilterString;
                            break;
                        case Options.Amt:
                            f = entry.Party == SelectedCBItem && entry.Amt == FilterString;
                            break;
                        default:
                            f = entry.Party == SelectedCBItem;

                            break;
                    }


                    return f;
                }
                else
                {
                    f = entry.Party == SelectedCBItem;
                    return f;
                }

            }

            return false;
        }

        void bills_BillChanged(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                OnPropertyChanged("Bill");
                OnPropertyChanged("AllBillss");
            }));
        }


        public RelayCommand CancelCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand AddCommand { get; set; }
        public RelayCommand DeleteUserCommand { get; set; }


        void DoCancel(object param)
        {
            UpdateBindingGroup.CancelEdit();
            if (SelectedIndex == -1)    //This only closes if new
                SelectedInv = null;
        }

        void DoSave(object param)
        {
            UpdateBindingGroup.CommitEdit();
            var bill = SelectedInv as Bills;

            if (SelectedIndex == -1)
            {
                bills.AddBill(bill);
                AllBillss.Add(bill);
                if (!ComboItems.Select(a => a).Distinct().ToList().Contains(bill.Party))
                {
                    ComboItems.Add(bill.Party.ToString());

                    ComboItems = new ObservableCollection<string>(ComboItems.OrderBy(i => i));

                    OnPropertyChanged("ComboItems");

                }
            }
            else
            {
                bills.UpdateBill(bill);
            }

            SelectedInv = null;
            OnPropertyChanged("Bill");
            OnPropertyChanged("AllBillss");
            OnPropertyChanged("Cnt");
            OnPropertyChanged("Cnt2");

            AllBills.Refresh();
        }

        bool CanDoSave(object param)
        {

            var bill = SelectedInv as Bills;
            //var bill = param as Bills;
            float f = 0;
            //bool success = float.TryParse(bill.Amt, out f);

            if (string.IsNullOrEmpty(bill.Party) || string.IsNullOrEmpty(bill.BillNo)|| string.IsNullOrEmpty(bill.BillDt) || string.IsNullOrEmpty(bill.DueDt)||!float.TryParse(bill.Amt, out f))
            {
                return false;
            }

            //			else if(!DateTime.TryParseExact(bill.BillDt, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)
            //			        || !DateTime.TryParseExact(bill.DueDt, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            //			{
            //				return false;
            //			}


            else
                return true;
        }

        void AddUser(object param)
        {
            SelectedInv = null;
            var bill = new Bills();
            SelectedInv = bill;

        }
        void DeleteUser(object parameter)
        {
            var bill = SelectedInv as Bills;
            if (SelectedIndex != -1)
            {
                bills.DeleteBill(bill);

                AllBillss.Remove(bill);

                if (!AllBillss.Select(a => a.Party).Distinct().ToList().Contains(bill.Party))
                {
                    ComboItems.Remove(bill.Party.ToString());
                    OnPropertyChanged("ComboItems");
                }
            }
            else
            {
                SelectedInv = null;
            }
            OnPropertyChanged("Bill");
            OnPropertyChanged("AllBillss");

            AllBills.Refresh();
        }
    }

    public static class DatabaseLayer
    {

        public static ObservableCollection<Bills> GetAllBillsFromDB()
        {
            try
            {
                SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=test.db");
                SQLiteCommand sqlCom = new SQLiteCommand("Select * From billdata", m_dbConnection);
                SQLiteDataAdapter sda = new SQLiteDataAdapter(sqlCom);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                var Bill = new ObservableCollection<Bills>();

                foreach (DataRow row in dt.Rows)
                {

                    var p = (row["PaidOn"] == DBNull.Value) ? String.Empty : (string)(row["PaidOn"]);
                    var q = (row["Remarks"] == DBNull.Value) ? String.Empty : (string)(row["Remarks"]);

                    var obj = new Bills()
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Party = (string)row["Party"],
                        BillNo = (string)row["BillNo"],
                        BillDt = (string)(row["BillDt"]),
                        Amt = (string)(row["Amt"]),
                        DueDt = (string)(row["DueDt"]),
                        PaidOn = p,
                        Remarks =q

                    };
                    Bill.Add(obj);
                    m_dbConnection.Close();

                }

                return Bill;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        internal static int InsertBill(Bills bill)
        {
            try
            {
                const string query = "INSERT INTO billdata(Party,BillNo, BillDt,Amt,DueDt,PaidOn,Remarks) VALUES(@Party, @BillNo,@BillDt,@Amt,@DueDt,@PaidOn,@Remarks)";
                var args = new Dictionary<string, object>
                {
                    {"@Party", bill.Party},
                    {"@BillNo", bill.BillNo},
                    {"@BillDt", bill.BillDt},
                    {"@Amt", bill.Amt},
                    {"@DueDt", bill.DueDt},
                    {"@PaidOn", bill.PaidOn},
                    {"@Remarks", bill.Remarks},
                };
                return ExecuteWrite(query, args);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        internal static int UpdateBill(Bills bill)
        {

            try
            {
                const string query = "UPDATE billdata SET Party = @Party, BillNo = @BillNo, BillDt=@BillDt, Amt=@Amt, DueDt=@DueDt , PaidOn=@PaidOn, Remarks=@Remarks WHERE Id = @Id";

                var args = new Dictionary<string, object>
                {
                    {"@Id", bill.Id},
                    {"@Party", bill.Party},
                    {"@BillNo", bill.BillNo},
                    {"@BillDt", bill.BillDt},
                    {"@Amt", bill.Amt},
                    {"@DueDt", bill.DueDt},
                    {"@PaidOn", bill.PaidOn},
                    {"@Remarks", bill.Remarks},
                };

                return ExecuteWrite(query, args);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        internal static int DeleteBill(Bills bill)
        {
            try
            {
                const string query = "Delete from billdata WHERE Id = @id";
                var args = new Dictionary<string, object>
                {
                    {"@id", bill.Id}
                };
                return ExecuteWrite(query, args);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private static int ExecuteWrite(string query, Dictionary<string, object> args)
        {
            int numberOfRowsAffected;

            using (var con = new SQLiteConnection("Data Source=test.db"))
            {
                con.Open();
                using (var cmd = new SQLiteCommand(query, con))
                {
                    foreach (var pair in args)
                    {
                        cmd.Parameters.AddWithValue(pair.Key, pair.Value);
                    }
                    numberOfRowsAffected = cmd.ExecuteNonQuery();
                }
                return numberOfRowsAffected;
            }
        }
    }

    public class BillsBusinessObject
    {
        internal EventHandler BillChanged;

        ObservableCollection<Bills> Bill { get; set; }

        public BillsBusinessObject()
        {
            Bill = DatabaseLayer.GetAllBillsFromDB();
        }

        public ObservableCollection<Bills> GetBills()
        {
            return Bill = DatabaseLayer.GetAllBillsFromDB();
        }

        public void AddBill(Bills bill)
        {
            DatabaseLayer.InsertBill(bill);
            OnBillChanged();
        }

        public void UpdateBill(Bills bill)
        {
            DatabaseLayer.UpdateBill(bill);
            OnBillChanged();
        }

        public void DeleteBill(Bills bill)
        {
            DatabaseLayer.DeleteBill(bill);
            OnBillChanged();
        }

        void OnBillChanged()
        {
            if (BillChanged != null)
                BillChanged(this, null);
        }

    }
}
