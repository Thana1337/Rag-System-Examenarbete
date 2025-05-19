import React from 'react';
import { motion } from 'framer-motion';

export default function MessageBubble({ sender, text, index }) {
  return (
    <motion.div
      initial={{ opacity: 0, y: 10 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ delay: index * 0.05, duration: 0.2 }}
      className={`
        max-w-[70%] px-4 py-2 rounded-4xl break-words text-left
        ${sender === 'user'
          ? 'self-end bg-black/30 text-white'
          : 'self-start bg-black/30 text-white'}
      `}
    >
      {text}
    </motion.div>
  );
}
