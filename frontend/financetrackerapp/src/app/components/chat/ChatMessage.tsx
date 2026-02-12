"use client";

interface ChatMessageProps {
  role: "user" | "assistant";
  content: string;
  timestamp?: Date;
}

export default function ChatMessage({
  role,
  content,
  timestamp,
}: ChatMessageProps) {
  const isUser = role === "user";

  return (
    <div className={`flex ${isUser ? "justify-end" : "justify-start"}`}>
      <div
        className={`max-w-[85%] rounded-2xl px-4 py-3 ${
          isUser
            ? "bg-gradient-to-br from-teal-500 to-emerald-500 text-white rounded-br-md"
            : "bg-slate-700 text-white rounded-bl-md border border-slate-600"
        }`}
      >
        <p className="text-sm whitespace-pre-wrap leading-relaxed">{content}</p>
        {timestamp && (
          <p
            className={`text-xs mt-1.5 ${
              isUser ? "text-white/60" : "text-white/40"
            }`}
          >
            {timestamp.toLocaleTimeString([], {
              hour: "2-digit",
              minute: "2-digit",
            })}
          </p>
        )}
      </div>
    </div>
  );
}
