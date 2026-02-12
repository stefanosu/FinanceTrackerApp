"use client";

import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
  Cell,
} from "recharts";
import { useMonthComparison, MonthComparison } from "../../hooks/useAnalytics";

interface MonthComparisonChartProps {
  enabled?: boolean;
}

export default function MonthComparisonChart({
  enabled = true,
}: MonthComparisonChartProps) {
  const { data: comparison, isLoading, error } = useMonthComparison(enabled);

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="w-8 h-8 border-2 border-teal-400/30 rounded-full animate-spin border-t-teal-400"></div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex justify-center items-center h-64 text-rose-400">
        Failed to load comparison data
      </div>
    );
  }

  if (!comparison || comparison.length === 0) {
    return (
      <div className="flex flex-col justify-center items-center h-64">
        <div className="w-16 h-16 mb-4 rounded-full bg-slate-700 flex items-center justify-center">
          <svg
            className="w-8 h-8 text-white/30"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"
            />
          </svg>
        </div>
        <p className="text-white/40">No comparison data available</p>
      </div>
    );
  }

  const chartData = comparison.map((item: MonthComparison) => ({
    name:
      item.category.length > 10
        ? item.category.substring(0, 10) + "..."
        : item.category,
    fullName: item.category,
    "This Month": item.thisMonth,
    "Last Month": item.lastMonth,
    change: item.change,
    changePercent: item.changePercent,
  }));

  const CustomTooltip = ({
    active,
    payload,
  }: {
    active?: boolean;
    payload?: Array<{
      name: string;
      value: number;
      dataKey: string;
      payload: (typeof chartData)[0];
    }>;
  }) => {
    if (active && payload && payload.length) {
      const data = payload[0].payload;
      const change = data.change;
      const changePercent = data.changePercent;
      const isIncrease = change > 0;

      return (
        <div className="bg-slate-800/90 backdrop-blur-sm p-4 shadow-xl rounded-xl border border-white/10">
          <p className="font-medium text-white mb-2">{data.fullName}</p>
          <div className="space-y-1">
            <div className="flex items-center gap-2">
              <div className="w-2 h-2 rounded-full bg-teal-400" />
              <span className="text-white/60 text-sm">This Month:</span>
              <span className="text-white font-medium">
                $
                {data["This Month"].toLocaleString("en-US", {
                  minimumFractionDigits: 2,
                })}
              </span>
            </div>
            <div className="flex items-center gap-2">
              <div className="w-2 h-2 rounded-full bg-white/30" />
              <span className="text-white/60 text-sm">Last Month:</span>
              <span className="text-white font-medium">
                $
                {data["Last Month"].toLocaleString("en-US", {
                  minimumFractionDigits: 2,
                })}
              </span>
            </div>
          </div>
          <div
            className={`mt-2 pt-2 border-t border-white/10 text-sm font-medium ${isIncrease ? "text-rose-400" : "text-emerald-400"}`}
          >
            {isIncrease ? "↑" : "↓"} {Math.abs(changePercent)}% (
            {isIncrease ? "+" : ""}${change.toFixed(2)})
          </div>
        </div>
      );
    }
    return null;
  };

  return (
    <div className="h-64">
      <ResponsiveContainer width="100%" height="100%">
        <BarChart
          data={chartData}
          margin={{ top: 10, right: 10, left: -10, bottom: 5 }}
        >
          <CartesianGrid
            strokeDasharray="3 3"
            stroke="rgba(255,255,255,0.05)"
            vertical={false}
          />
          <XAxis
            dataKey="name"
            tick={{ fill: "rgba(255,255,255,0.5)", fontSize: 11 }}
            tickLine={false}
            axisLine={{ stroke: "rgba(255,255,255,0.1)" }}
            interval={0}
            angle={-15}
            textAnchor="end"
            height={40}
          />
          <YAxis
            tick={{ fill: "rgba(255,255,255,0.5)", fontSize: 11 }}
            tickLine={false}
            axisLine={false}
            tickFormatter={(value) => `$${value}`}
          />
          <Tooltip
            content={<CustomTooltip />}
            cursor={{ fill: "rgba(255,255,255,0.05)" }}
          />
          <Bar
            dataKey="Last Month"
            fill="rgba(255,255,255,0.2)"
            radius={[4, 4, 0, 0]}
            animationBegin={0}
            animationDuration={800}
          />
          <Bar
            dataKey="This Month"
            radius={[4, 4, 0, 0]}
            animationBegin={200}
            animationDuration={800}
          >
            {chartData.map((entry, index) => (
              <Cell
                key={`cell-${index}`}
                fill={entry.change > 0 ? "#f43f5e" : "#22c55e"}
              />
            ))}
          </Bar>
        </BarChart>
      </ResponsiveContainer>

      {/* Legend */}
      <div className="flex justify-center gap-6 mt-2">
        <div className="flex items-center gap-2">
          <div className="w-3 h-3 rounded bg-white/20" />
          <span className="text-xs text-white/50">Last Month</span>
        </div>
        <div className="flex items-center gap-2">
          <div className="w-3 h-3 rounded bg-emerald-500" />
          <span className="text-xs text-white/50">Decreased</span>
        </div>
        <div className="flex items-center gap-2">
          <div className="w-3 h-3 rounded bg-rose-500" />
          <span className="text-xs text-white/50">Increased</span>
        </div>
      </div>
    </div>
  );
}
