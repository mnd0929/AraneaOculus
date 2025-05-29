using AraneaOculus.Core.Models.Data.Statistics;
using AraneaOculus.Core.Utilities;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Forms.DataVisualization.Charting;

namespace AraneaOculus.Manager.UI.View.ChartControllers
{
    internal class ConnectionNumberChartController
    {
        public static void Display(Chart chart, ObservableDictionary<Guid, ObservableCollection<ConnectionsHistoryEntry>> entries)
        {
            Debug.WriteLine("[ConnectionNumberChartController] START");

            chart.Series.Clear();
            chart.ChartAreas.Clear();
            chart.Legends.Clear();

            var area = new ChartArea("ConnectionsArea");
            area.CursorX.IsUserEnabled = true;
            area.CursorX.IsUserSelectionEnabled = true;
            area.AxisX.Title = "Time";
            area.AxisY.Title = "Count";
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

            var events = new List<(DateTime time, int delta)>();
            foreach (var deviceHistory in entries.Values)
            {
                foreach (var entry in deviceHistory)
                {
                    if (entry.ConnectedAt.HasValue)
                        events.Add((entry.ConnectedAt.Value, +1));
                    if (entry.DisconnectedAt.HasValue)
                        events.Add((entry.DisconnectedAt.Value, -1));
                }
            }

            events = events.OrderBy(e => e.time).ToList();
            
            var series = new Series("Connected Devices")
            {
                ChartType = SeriesChartType.StepLine,
                XValueType = ChartValueType.DateTime,
                BorderWidth = 2,
                Color = Color.DodgerBlue,
                IsVisibleInLegend = true
            };

            int currentCount = 0;
            DateTime? lastTime = null;
            foreach (var evt in events)
            {
                if (lastTime.HasValue && lastTime.Value != evt.time)
                    series.Points.AddXY(evt.time, currentCount);

                currentCount += evt.delta;
                series.Points.AddXY(evt.time, currentCount);
                lastTime = evt.time;
            }

            if (series.Points.Count == 0)
                series.Points.AddXY(DateTime.Now, 0);

            chart.Series.Add(series);

            chart.ChartAreas[0].RecalculateAxesScale();

            Debug.WriteLine("[ConnectionNumberChartController] STOP");
        }
    }
}
