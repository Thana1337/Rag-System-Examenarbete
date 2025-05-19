import { useState, useRef, useEffect } from 'react';
import ChatBubble from './ChatBubble';
import InputBox from './InputBox';

export default function Chat() {
  const [messages, setMessages] = useState([
    { id: 1, text: 'Hello, how can I help you today?', author: 'bot' }
  ]);
  const containerRef = useRef();

  useEffect(() => {
    containerRef.current.scrollTop = containerRef.current.scrollHeight;
  }, [messages]);

  const sendMessage = (text) => {
    if (!text) return;
    setMessages((msgs) => [
      ...msgs,
      { id: Date.now(), text, author: 'user' }
    ]);
    // simulate bot reply
    setTimeout(() => {
      setMessages((msgs) => [
        ...msgs,
        { id: Date.now()+1, text: `💡 Bot says: ${text}`, author: 'bot' }
      ]);
    }, 600);
  };

  return (
    <div className="flex-1 flex flex-col bg-darkbg">
      <div
        ref={containerRef}
        className="flex-1 overflow-y-auto p-8 space-y-6"
      >
        {messages.map((msg) => (
          <ChatBubble
            key={msg.id}
            text={msg.text}
            isUser={msg.author === 'user'}
          />
        ))}
      </div>
      <InputBox onSend={sendMessage} />
    </div>
  );
}