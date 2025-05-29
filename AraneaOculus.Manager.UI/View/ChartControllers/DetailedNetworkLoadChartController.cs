using AraneaOculus.Core.Models.Data.Statistics;
using System.Collections.ObjectModel;
using System.Windows.Forms.DataVisualization.Charting;

namespace AraneaOculus.Manager.UI.View.ChartControllers
{
    internal class DetailedNetworkLoadChartController
    {
        public static void Display(Chart chart, ObservableCollection<PacketHistoryEntry> entries)
        {
            System.Diagnostics.Debug.WriteLine("[DetailedNetworkLoadChartController] START");

            chart.Series.Clear();
            chart.ChartAreas.Clear();
            chart.Legends.Clear();

            var area = new ChartArea("NetworkLoadArea");
            area.CursorX.IsUserEnabled = true;
            area.CursorX.IsUserSelectionEnabled = true;
            area.AxisX.Title = "Time";
            area.AxisY.Title = "Bytes/s";
            area.AxisX.LabelStyle.Format = "HH:mm:ss";
            area.AxisX.IntervalType = DateTimeIntervalType.Auto;
            area.AxisX.MajorGrid.LineColor = Color.LightGray;
            area.AxisY.MajorGrid.LineColor = Color.LightGray;
            chart.ChartAreas.Add(area);

            var legend = new Legend("MainLegend")
            {
                Docking = Docking.Top,
                Alignment = StringAlignment.Center
            };
            chart.Legends.Add(legend);

            var ordered = entries.OrderBy(e => e.Timestamp).ToList();
            if (ordered.Count < 2)
            {
                var emptySeries = new Series("No data")
                {
                    ChartType = SeriesChartType.Line,
                    XValueType = ChartValueType.DateTime,
                    BorderWidth = 2,
                    Color = Color.Gray
                };
                emptySeries.Points.AddXY(DateTime.Now, 0);
                chart.Series.Add(emptySeries);
                chart.ChartAreas[0].RecalculateAxesScale();
                System.Diagnostics.Debug.WriteLine("[DetailedNetworkLoadChartController] STOP");
                return;
            }

            var sentPerSecond = new SortedDictionary<DateTime, long>();
            var receivedPerSecond = new SortedDictionary<DateTime, long>();

            for (int i = 1; i < ordered.Count; i++)
            {
                var prev = ordered[i - 1];
                var curr = ordered[i];
                var seconds = (curr.Timestamp - prev.Timestamp).TotalSeconds;
                if (seconds <= 0) continue;

                var timeKey = new DateTime(curr.Timestamp.Year, curr.Timestamp.Month, curr.Timestamp.Day,
                                           curr.Timestamp.Hour, curr.Timestamp.Minute, curr.Timestamp.Second);

                var sentDelta = curr.Sent - prev.Sent;
                var receivedDelta = curr.Received - prev.Received;

                var sentRate = (long)(sentDelta / seconds);
                var receivedRate = (long)(receivedDelta / seconds);

                if (!sentPerSecond.ContainsKey(timeKey))
                    sentPerSecond[timeKey] = 0;
                if (!receivedPerSecond.ContainsKey(timeKey))
                    receivedPerSecond[timeKey] = 0;

                sentPerSecond[timeKey] += sentRate;
                receivedPerSecond[timeKey] += receivedRate;
            }

            var sentSeries = new Series("Sent")
            {
                ChartType = SeriesChartType.Line,
                XValueType = ChartValueType.DateTime,
                BorderWidth = 2,
                Color = Color.OrangeRed,
                IsVisibleInLegend = true
            };
            var receivedSeries = new Series("Received")
            {
                ChartType = SeriesChartType.Line,
                XValueType = ChartValueType.DateTime,
                BorderWidth = 2,
                Color = Color.DodgerBlue,
                IsVisibleInLegend = true
            };

            foreach (var kv in sentPerSecond)
                sentSeries.Points.AddXY(kv.Key, kv.Value);

            foreach (var kv in receivedPerSecond)
                receivedSeries.Points.AddXY(kv.Key, kv.Value);

            if (sentSeries.Points.Count == 0)
                sentSeries.Points.AddXY(DateTime.Now, 0);
            if (receivedSeries.Points.Count == 0)
                receivedSeries.Points.AddXY(DateTime.Now, 0);

            chart.Series.Add(sentSeries);
            chart.Series.Add(receivedSeries);

            chart.ChartAreas[0].RecalculateAxesScale();

            System.Diagnostics.Debug.WriteLine("[DetailedNetworkLoadChartController] STOP");
        }
    }
}
