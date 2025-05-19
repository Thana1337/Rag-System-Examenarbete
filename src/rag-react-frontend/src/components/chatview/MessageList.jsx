// src/components/MessageList.jsx
import React, { useRef, useLayoutEffect } from 'react';
import MessageBubble from './MessageBubble';
import TypingIndicator from './TypingIndicator';

export default function MessageList({ messages, loading }) {
  const containerRef = useRef(null);

  // After each render where messages or loading changes,
  // jump the scrollTop to the bottom instantly.
  useLayoutEffect(() => {
    const container = containerRef.current;
    if (container) {
      container.scrollTop = container.scrollHeight;
    }
  }, [messages, loading]);

  return (
    <div
      ref={containerRef}
      className="flex flex-col flex-1 overflow-y-auto p-6 space-y-4 chat-scrollbar"
    >
      {messages.map((m, i) => (
        <MessageBubble
          key={i}
          sender={m.sender}
          text={m.text}
          index={i}
        />
      ))}

      {loading && <TypingIndicator />}
    </div>
  );
}
