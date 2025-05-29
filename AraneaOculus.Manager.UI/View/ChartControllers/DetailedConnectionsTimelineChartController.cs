using AraneaOculus.Core.Models.Data.Statistics;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Forms.DataVisualization.Charting;

namespace AraneaOculus.Manager.UI.View.ChartControllers
{
    internal class DetailedConnectionsTimelineChartController
    {
        public static void Display(Chart chart, ObservableCollection<ConnectionsHistoryEntry> entries)
        {
            Debug.WriteLine("[DetailedConnectionsTimelineChartController] START");

            chart.Series.Clear();
            chart.ChartAreas.Clear();
            chart.Legends.Clear();

            var area = new ChartArea("TimelineArea");
            area.CursorX.IsUserEnabled = true;
            area.CursorX.IsUserSelectionEnabled = true;
            area.AxisX.Title = "Time";
            area.AxisY.Title = "Condition";
            area.AxisX.LabelStyle.Format = "HH:mm:ss";
            area.AxisX.IntervalType = DateTimeIntervalType.Auto;
            area.AxisX.MajorGrid.LineColor = Color.LightGray;
            area.AxisY.MajorGrid.LineColor = Color.LightGray;
            area.AxisY.Minimum = 0;
            area.AxisY.Maximum = 1.1;
            area.AxisY.Interval = 1;
            area.AxisY.LabelStyle.Enabled = false;
            chart.ChartAreas.Add(area);

            var legend = new Legend("MainLegend")
            {
                Docking = Docking.Top,
                Alignment = StringAlignment.Center
            };
            chart.Legends.Add(legend);

            // Сортируем события по времени
            var events = new List<(DateTime time, bool isConnect)>();
            foreach (var entry in entries)
            {
                if (entry.ConnectedAt.HasValue)
                    events.Add((entry.ConnectedAt.Value, true));
                if (entry.DisconnectedAt.HasValue)
                    events.Add((entry.DisconnectedAt.Value, false));
            }
            events = events.OrderBy(e => e.time).ToList();

            var series = new Series("Online")
            {
                ChartType = SeriesChartType.StepLine,
                XValueType = ChartValueType.DateTime,
                BorderWidth = 4,
                Color = Color.MediumSeaGreen,
                IsVisibleInLegend = true
            };

            // Формируем точки: 1 - в сети, 0 - не в сети
            int state = 0; // 0 - offline, 1 - online
            DateTime? lastTime = null;

            foreach (var evt in events)
            {
                // Если есть разрыв между событиями, рисуем горизонтальную линию до следующего события
                if (lastTime.HasValue && lastTime.Value != evt.time)
                {
                    series.Points.AddXY(evt.time, state);
                }

                state = evt.isConnect ? 1 : 0;
                series.Points.AddXY(evt.time, state);
                lastTime = evt.time;
            }

            // Если нет событий, показываем "нет данных"
            if (series.Points.Count == 0)
            {
                series.Points.AddXY(DateTime.Now, 0);
            }

            chart.Series.Add(series);
            chart.ChartAreas[0].RecalculateAxesScale();

            Debug.WriteLine("[DetailedConnectionsTimelineChartController] STOP");
        }
    }
}
