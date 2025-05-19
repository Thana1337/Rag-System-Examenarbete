import React, { useState, useRef } from 'react';
import axios from 'axios';
import { motion } from 'framer-motion';


import MessageList    from './MessageList';
import ChatInput      from './ChatInput';
import BackButton from '../BackButton';

export default function ChatView() {
  const [messages, setMessages] = useState([
    { sender: 'bot', text: 'Hello! What’s on your mind?' }
  ]);
  const [input,    setInput]    = useState('');
  const [loading,  setLoading]  = useState(false);
  const bottomRef              = useRef();

  const QUERY_URL = import.meta.env.VITE_QUERY_URL;

  const sendMessage = async () => {
    const text = input.trim();
    if (!text) return;

    setMessages(ms => [...ms, { sender: 'user', text }]);
    setInput('');
    setLoading(true);

    try {
      const resp = await axios.post(
        QUERY_URL,
        { question: text },
        { headers: { 'Content-Type': 'application/json' } }
      );
      const answer = resp.data.answer ?? 'No answer returned.';
      setMessages(ms => [...ms, { sender: 'bot', text: answer }]);
    } catch (err) {
      console.error('Query error', err.response || err);
      setMessages(ms => [
        ...ms,
        { sender: 'bot', text: 'Sorry, something went wrong. Please try again.' }
      ]);
    } finally {
      setLoading(false);
    }
  };

  const onKeyDown = e => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      sendMessage();
    }
  };

  return (
    <motion.div className="flex flex-col items-center justify-center mt-20 overflow-hidden ">
      <BackButton/>

      <div className="flex container flex-col w-full max-w-6xl h-[65vh] bg-white/5 rounded-2xl shadow-xl">
        <MessageList
          messages={messages}
          loading={loading}
          bottomRef={bottomRef}
        />

        <ChatInput
          input={input}
          onChange={e => setInput(e.target.value)}
          onKeyDown={onKeyDown}
          sendMessage={sendMessage}
          loading={loading}
        />
      </div>
    </motion.div>
  );
}
