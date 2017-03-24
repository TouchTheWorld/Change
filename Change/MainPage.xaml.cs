using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace Change
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static SQLiteAsyncConnection sql { get; set; }
        ObservableCollection<MyIems> list = new ObservableCollection<MyIems>();
        public MainPage()
        {
            this.InitializeComponent();
            this.SizeChanged += (s, e) =>
            {
                var state = "VisuilStateOne";
                if(e.NewSize.Width < 550)
                {
                    if (text_block.Text == "")
                        state = "VisuilStateThree";
                    else
                        state = "VisuilStateTwo";
                }
                VisualStateManager.GoToState(this, state, true);
            };
        }

        private async void save_bon_Click(object sender, RoutedEventArgs e)
        {
            if(TextBox.Text != "")
            {
                sql = new SQLiteAsyncConnection("items.db");
                await sql.CreateTableAsync<MyIems>();
                MyIems ite = new MyIems { Conut = TextBox.Text };
                await sql.InsertAsync(ite);
                var query = await (sql.Table<MyIems>().Where(v => v._Id >= 1)).ToListAsync();
                list = new ObservableCollection<MyIems>(query);
                ListView.ItemsSource = list;
            }
        }

        private async void remove_bon_Click(object sender, RoutedEventArgs e)
        {
            var query = await (sql.Table<MyIems>().Where(v => v.Conut == TextBox.Text)).ToListAsync();
            await sql.DeleteAsync(query[0]);
            query = await (sql.Table<MyIems>().Where(v => v._Id >= 1)).ToListAsync();
            list = new ObservableCollection<MyIems>(query);
            ListView.ItemsSource = list;
            text_block.Text = "";
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListView.SelectedItem == null)
                return;
            text_block.Text = ListView.SelectedItem.ToString();
        }
    }
    public class MyIems
    {
        [PrimaryKey, AutoIncrement]
        public int _Id { get; set; }
        public string Conut{ get; set; }
    }
}
