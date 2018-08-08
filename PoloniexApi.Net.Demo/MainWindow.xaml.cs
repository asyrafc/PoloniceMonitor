using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Jojatekok.PoloniexAPI.Demo
{
    public sealed partial class MainWindow
    {
        private PoloniexClient PoloniexClient { get; set; }

        public MainWindow()
        {
            // Set icon from the assembly
            Icon = System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location).ToImageSource();
            
            InitializeComponent();

            PoloniexClient = new PoloniexClient(ApiKeys.PublicKey, ApiKeys.PrivateKey);

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        private async void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            var markets = await PoloniexClient.Markets.GetSummaryAsync();
            DataGrid1.Items.Clear();
            listView1.Items.Clear();

            foreach (var market in markets)
            {
                if (market.Key.ToString() == "USDT_XMR" || market.Key.ToString() == "BTC_XMR" || market.Key.ToString() == "BTC_SC")
                {
                    //MessageBox.Show(market.Key.QuoteCurrency + " - " + market.Value.OrderTopBuy);
                    DataGrid1.Items.Add(market);
                    listView1.Items.Add(new { Mkt = market.Key, LstPaid = market.Value.PriceLast, TpBid = market.Value.OrderTopBuy, TpAsk = market.Value.OrderTopSell, Sprd = market.Value.OrderSpreadPercentage });
                }
            }
            DataGrid1.Items.Refresh();

            if(lastSelectedIndex > -1)
            {
                listView1.SelectedItem = listView1.Items[lastSelectedIndex];
                listView1.UpdateLayout(); // Pre-generates item containers 

                var listBoxItem = (ListBoxItem)listView1
                    .ItemContainerGenerator
                    .ContainerFromItem(listView1.SelectedItem);

                listBoxItem.Focus();
            }
        }

        int lastSelectedIndex;

        private void listView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lastSelectedIndex = listView1.SelectedIndex;

            if (listView1.SelectedIndex > -1)
            {
                dynamic selectedItm = listView1.SelectedItem;
                textBoxCoinExchange.Text = (selectedItm.Mkt).ToString();
                textBoxLastPaid.Text = (selectedItm.LstPaid).ToString();
                textBoxHighestBid.Text = (selectedItm.TpBid).ToString();
                textBoxHIghestAsk.Text = (selectedItm.TpAsk).ToString();
                textBoxDifference.Text = (selectedItm.Sprd).ToString();
            }
        }
    }
}
