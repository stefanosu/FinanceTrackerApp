"use client";

import { PieChart, Pie, Cell, ResponsiveContainer, Tooltip } from "recharts";
import { useFinancialSummary } from "../../hooks/useAnalytics";

interface SavingsDonutChartProps {
  startDate?: string;
  endDate?: string;
  enabled?: boolean;
}

export default function SavingsDonutChart({
  startDate,
  endDate,
  enabled = true,
}: SavingsDonutChartProps) {
  const {
    data: summary,
    isLoading,
    error,
  } = useFinancialSummary(startDate, endDate, enabled);

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-8 w-8 border-t-2 border-b-2 border-indigo-600"></div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex justify-center items-center h-64 text-red-500">
        Failed to load financial data
      </div>
    );
  }

  if (!summary || summary.totalIncome === 0) {
    return (
      <div className="flex justify-center items-center h-64 text-gray-500">
        No income data available
      </div>
    );
  }

  const spent = summary.totalExpenses;
  const saved = Math.max(0, summary.netSavings);
  const total = spent + saved;

  const chartData = [
    { name: "Spent", value: spent, color: "#ef4444" },
    { name: "Saved", value: saved, color: "#22c55e" },
  ];

  const CustomTooltip = ({
    active,
    payload,
  }: {
    active?: boolean;
    payload?: Array<{
      payload: { name: string; value: number; color: string };
    }>;
  }) => {
    if (active && payload && payload.length) {
      const data = payload[0].payload;
      const percentage =
        total > 0 ? ((data.value / total) * 100).toFixed(1) : "0";
      return (
        <div className="bg-white p-3 shadow-lg rounded-lg border">
          <p className="font-medium" style={{ color: data.color }}>
            {data.name}
          </p>
          <p className="text-gray-600">
            ${data.value.toLocaleString("en-US", { minimumFractionDigits: 2 })}
          </p>
          <p className="text-gray-500 text-sm">{percentage}%</p>
        </div>
      );
    }
    return null;
  };

  const savingsRate = summary.savingsRate;

  return (
    <div className="h-64 relative">
      <ResponsiveContainer width="100%" height="100%">
        <PieChart>
          <Pie
            data={chartData}
            cx="50%"
            cy="50%"
            innerRadius={60}
            outerRadius={80}
            paddingAngle={2}
            dataKey="value"
          >
            {chartData.map((entry, index) => (
              <Cell key={`cell-${index}`} fill={entry.color} />
            ))}
          </Pie>
          <Tooltip content={<CustomTooltip />} />
        </PieChart>
      </ResponsiveContainer>
      <div className="absolute inset-0 flex flex-col items-center justify-center pointer-events-none">
        <span
          className={`text-2xl font-bold ${savingsRate >= 0 ? "text-green-600" : "text-red-600"}`}
        >
          {savingsRate.toFixed(1)}%
        </span>
        <span className="text-sm text-gray-500">Savings Rate</span>
      </div>
      <div className="flex justify-center gap-6 mt-2">
        <div className="flex items-center gap-2">
          <div className="w-3 h-3 rounded-full bg-red-500"></div>
          <span className="text-sm text-gray-600">
            Spent: $
            {spent.toLocaleString("en-US", { minimumFractionDigits: 2 })}
          </span>
        </div>
        <div className="flex items-center gap-2">
          <div className="w-3 h-3 rounded-full bg-green-500"></div>
          <span className="text-sm text-gray-600">
            Saved: $
            {saved.toLocaleString("en-US", { minimumFractionDigits: 2 })}
          </span>
        </div>
      </div>
    </div>
  );
}
